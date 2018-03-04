using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HotelReservationSystem1.Models;
using HotelReservationSystem1.Models.Hotel;
using HotelReservationSystem1.ViewModels;

namespace HotelReservationSystem1.Controllers
{
    public class BookingsController : Controller
    {
        ApplicationDbContext _db = new ApplicationDbContext();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HandleError(View = "Error")]
        public async Task<ActionResult> Index()
        {
            ViewBag.TotalAmount = 0;
            PopulateTitlesDropDownList();

            var reservationsVM = await PopulateReservationsVM();

            return View(reservationsVM);
        }

        [HttpGet]
        [HandleError(View = "Error")]
        public async Task<ActionResult> RetrieveBooking(int? reservationId, string lastName)
        {
            if (reservationId == null || string.IsNullOrWhiteSpace(lastName))
            {
                logger.Error("reservationId and lastname not provided.");
                TempData["Message"] = "Reservation # and Last Name both must be provided.";
                return RedirectToAction("Index");
            }

            var booking = await _db.RoomReservations.SingleOrDefaultAsync(r => r.Id == reservationId);

            if (booking == null)
            {
                logger.ErrorFormat("RoomReservations object for id {0} is not found.", reservationId);
                TempData["Message"] = "Reservation # provided is not valid.";
                return RedirectToAction("Index");
            }

            if (!(booking.Guest.LastName.ToUpper().Trim() == lastName.ToUpper().Trim()))
            {
                logger.ErrorFormat("RoomReservation # {0} retrieval for last name {1} mismatched.", reservationId, lastName);
                TempData["Message"] = string.Format("Last Name: {0} provided for Reservation #: {1} is not valid.",
                    lastName, booking.Id);
                return RedirectToAction("Index");
            }

            PopulateTitlesDropDownList(booking.Guest.TitleId);
            ViewBag.TotalAmount = booking.TotalAmount;

            var reservationsVM = await PopulateReservationsVM(booking.GuestId, booking.Id);

            logger.InfoFormat("Request for booking retrieval for reservation # {0} by {1} {2} succeeded.",
                 reservationId, booking.Guest.FirstName, booking.Guest.LastName);

            return View("Index", reservationsVM);
        }

        [HandleError(View = "Error")]
        public async Task<ActionResult> RemoveRoom(int? id)
        {
            if (id == null)
            {
                logger.Error("ReservationDetailId not provided.");
                TempData["Message"] = "ReservationDetailId must be provided.";
                return RedirectToAction("Index");
            }

            var removedRoomDetails = await _db.ReservationDetails.SingleOrDefaultAsync(rd => rd.Id == id);

            if (removedRoomDetails == null)
            {
                logger.Error("Invalid ReservationDetailId.");
                TempData["Message"] = "ReservationDetailId is not valid.";
                return RedirectToAction("Index");
            }

            var roomNo = removedRoomDetails.Room.Number;
            var lastName = removedRoomDetails.RoomReservation.Guest.LastName;

            _db.ReservationDetails.Remove(removedRoomDetails);
            await _db.SaveChangesAsync();

            var reservationDetails = await _db.ReservationDetails.Where(r => r.RoomReservationId == removedRoomDetails.RoomReservationId).ToListAsync();
            var total = reservationDetails != null ? reservationDetails.Sum(r => r.TotalRate) : 0;

            await UpdateReservationTotalAmount(removedRoomDetails.RoomReservationId, total);

            TempData["Message"] =
                string.Format("Reservation of Room # {0} for the period {1: dd-MMM-yyyy} - {2: dd-MMM-yyyy} removed successfully.",
                roomNo, removedRoomDetails.CheckInDate, removedRoomDetails.CheckOutDate);

            return RedirectToAction("RetrieveBooking",
                new
                {
                    reservationId = removedRoomDetails.RoomReservationId,
                    lastName = lastName
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError(View = "Error")]
        public async Task<ActionResult> AddBooking(AddBookingVM vm)
        {
            if (vm == null)
            {
                return RedirectToAction("Index");
            }

            if (vm.NumOfDays == 0)
            {
                ModelState.AddModelError("", "Failed to add booking. CheckOut Date should be greater than CheckIn Date.");
                TempData["Message"] = "Failed to add booking. CheckOut Date should be greater than CheckIn Date.";

                if (vm.RoomReservationId == 0)
                {
                    return RedirectToAction("Index");
                }

                return RedirectToAction("RetrieveBooking", new { reservationId = vm.RoomReservationId, lastName = vm.LastName });
            }

            var roomsAlreadyBooked = _db.ReservationDetails.Where(r => r.RoomId == vm.RoomId).ToList();

            if (roomsAlreadyBooked != null && roomsAlreadyBooked.Count() > 0)
            {
                var validRooms = roomsAlreadyBooked.Where(r => (vm.CheckInDate < r.CheckInDate && vm.CheckOutDate <= r.CheckInDate)
                || (vm.CheckInDate > r.CheckOutDate)).ToList();

                if (validRooms != null && validRooms.Count != roomsAlreadyBooked.Count())
                {
                    string msg = string.Format(@"Sorry, selected room already booked or there is an overlap with other bookings 
                                for the period {0: dd-MMM-yyyy} - {1: dd-MMM-yyyy}. Please revice your booking dates and try again.",
                                vm.CheckInDate, vm.CheckOutDate);
                    TempData["Message"] = msg;
                    ModelState.AddModelError("", msg);

                    if (vm.RoomReservationId == 0)
                    {
                        return RedirectToAction("Index");
                    }

                    return RedirectToAction("RetrieveBooking", new
                    {
                        reservationId = vm.RoomReservationId,
                        lastName = vm.LastName
                    });
                }

            }

            if (ModelState.IsValid)
            {
                if (vm.RoomReservationId == 0) // First time registration. No reservation yet.
                {
                    var newGuest = await RegisterGuest(vm); // Register Guest.
                    vm.GuestId = newGuest.Id;

                    var reservation = await CreateReservationForTheRegisteredGuest(newGuest.Id); // Create Reservation.
                    vm.RoomReservationId = reservation.Id;
                }

                await AddReservationDetails(vm); // Add room reservation details.
                await UpdateGuestDetails(vm.GuestId); // Update guest details if changed.

                PopulateTitlesDropDownList(vm.TitleId);

                int guestId = vm.GuestId;
                int roomReservationId = vm.RoomReservationId;

                var reservationsVM = await PopulateReservationsVM(guestId, roomReservationId);

                await UpdateReservationTotalAmount(roomReservationId, reservationsVM.TotalAmount);
                ViewBag.TotalAmount = reservationsVM.TotalAmount;

                TempData["Message"] = "Booking details saved successfully.";
                return View("Index", reservationsVM);
            }

            TempData["Message"] = string.Format("Failed to save the booking. Check booking details validity. Try again.");
            return RedirectToAction("Index");
        }

        [HandleError(View = "Error")]
        public async Task<ActionResult> SaveReservation(int? id)
        {
            if (id == null)
            {
                TempData["Message"] = "Reservation Id not provided.";
                return RedirectToAction("Index");
            }

            var booking = await _db.RoomReservations.FindAsync(id);

            if (booking == null)
            {
                TempData["Message"] = "Invalid Reservation.";
                return RedirectToAction("Index");
            }

            booking.BookingCreatedOn = DateTime.Now;
            booking.BookingConfirmed = true;
            _db.Entry(booking).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            TempData["Message"] = string.Format(@"Thank you. Your Reservation has been confirmed & saved successfully. Following
                are your reservation details. Please keep them to retrieve the booking in the future.
                Reservation #: {0} , Last Name: {1}", booking.Id, booking.Guest.LastName.ToUpper());

            //return RedirectToAction("RetrieveBooking", new { reservationId = booking.Id, lastName = booking.Guest.LastName });
            return RedirectToAction("Index");
        }

        [HandleError(View = "Error")]
        public async Task<ActionResult> DeleteReservation(int? id)
        {
            if (id == null)
            {
                TempData["Message"] = "Reservation Id not provided.";
                return RedirectToAction("Index");
            }

            var booking = await _db.RoomReservations.FindAsync(id);

            if (booking == null)
            {
                TempData["Message"] = "Invalid Reservation.";
                return RedirectToAction("Index");
            }

            _db.Guests.Remove(booking.Guest);
            await _db.SaveChangesAsync();

            TempData["Message"] = "Reservation removed successfully.";

            return RedirectToAction("Index");
        }

        [NonAction]
        private async Task<ReservationsVM> PopulateReservationsVM(int? guestId = -1, int? roomReservationId = -1)
        {
            var guest = await _db.Guests.SingleOrDefaultAsync(g => g.Id == guestId);
            var reservationDetails = await _db.ReservationDetails.Where(rd => rd.RoomReservationId == roomReservationId)
                .OrderBy(r => r.CheckInDate).ToListAsync();
            var roomReservations = await _db.RoomReservations.SingleOrDefaultAsync(r => r.Id == roomReservationId);
            var vacantRooms = await GetVacantRooms();

            var reservationsVM = new ReservationsVM
            {
                Rooms = vacantRooms != null ? vacantRooms : new List<Room>(),
                Guest = guest != null ? guest : new Guest(),
                ReservationDetails = reservationDetails != null ? reservationDetails : new List<ReservationDetail>(),
                RoomReservations = roomReservations != null ? roomReservations : new RoomReservation()
            };

            return reservationsVM;
        }

        [NonAction]
        private async Task UpdateGuestDetails(int? guestId)
        {
            var guestEntry = await _db.Guests.SingleOrDefaultAsync(g => g.Id == guestId);
            if (guestEntry != null &&
                TryUpdateModel(guestEntry, new string[] { "TitleId", "FirstName", "LastName", "Address", "ContactNumber" }))
            {
                _db.Entry(guestEntry).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
        }

        [NonAction]
        private async Task UpdateReservationTotalAmount(int? roomReservationId = -1, decimal totalAmount = 0)
        {
            var booking = await _db.RoomReservations.SingleOrDefaultAsync(r => r.Id == roomReservationId);

            if (booking != null)
            {
                booking.TotalAmount = totalAmount;
                _db.Entry(booking).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
        }

        [NonAction]
        private async Task AddReservationDetails(AddBookingVM vm)
        {
            var reservationDetails = new ReservationDetail
            {
                RoomReservationId = vm.RoomReservationId,
                RoomId = vm.RoomId,
                CheckInDate = vm.CheckInDate,
                CheckOutDate = vm.CheckOutDate,
                Rate = _db.Rooms.Single(r => r.Id == vm.RoomId).Rate
            };

            _db.ReservationDetails.Add(reservationDetails);
            await _db.SaveChangesAsync();
        }

        [NonAction]
        private async Task<RoomReservation> CreateReservationForTheRegisteredGuest(int guestId)
        {
            var reservation = new RoomReservation
            {
                GuestId = guestId,
                TotalAmount = 0,
                BookingCreatedOn = DateTime.Now
            };

            _db.RoomReservations.Add(reservation);
            await _db.SaveChangesAsync();

            return reservation;
        }

        [NonAction]
        private async Task<Guest> RegisterGuest(AddBookingVM vm)
        {
            var guest = new Guest
            {
                TitleId = vm.TitleId,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Address = vm.Address,
                ContactNumber = vm.ContactNumber
            };

            _db.Guests.Add(guest);
            await _db.SaveChangesAsync();

            return guest;
        }

        [NonAction]
        void PopulateTitlesDropDownList(object selectedTitle = null)
        {
            ViewBag.TitleId = new SelectList(Enum.GetValues(typeof(Title)), selectedTitle);
        }

        [NonAction]
        private async Task<List<Room>> GetVacantRooms()
        {
            var vacantRooms = await _db.Rooms.ToListAsync();

            return vacantRooms;
        }

        [NonAction]
        private void HandleError(Exception ex)
        {
            logger.ErrorFormat("Unhandled exception {0}: {1} in {2} for {3}.", ex.Message,
                       ex.InnerException.InnerException.Message, Request.ApplicationPath, Request.RawUrl);

            Response.Redirect(string.Format("~/Error?returnUrl={0}", Request.UrlReferrer));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
