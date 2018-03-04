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
    public class RoomsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Rooms
        public async Task<ActionResult> Index()
        {
            return View(await db.Rooms.ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // GET: Rooms/Create
        [Authorize(Roles = "Admin, Is_Save")]
        public ActionResult Create()
        {
            PopulateRoomsList();
            PopulateRoomTypesDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Is_Save")]
        public async Task<ActionResult> Create(RoomVM roomVM)
        {
            PopulateRoomsList();
            PopulateRoomTypesDropDownList(roomVM.RoomTypeId);

            if (RoomNumberExists(roomVM.Number))
            {
                ModelState.AddModelError("", "Room number you entered already exists. Enter different number.");
            }

            if (ModelState.IsValid)
            {
                var room = new Room
                {
                    Number = roomVM.Number,
                    Description = roomVM.Description,
                    RoomTypeId = roomVM.RoomTypeId,
                    Floor = roomVM.Floor,
                    Rate = roomVM.Rate
                };

                db.Rooms.Add(room);
                await db.SaveChangesAsync();

                TempData["Message"] = "Room added successfully.";

                return RedirectToAction("Create");
            }

            return View(roomVM);
        }

        // GET: Rooms/Edit/5
        [Authorize(Roles = "Admin, Is_Save, Is_Update")]
        public async Task<ActionResult> EditRoom(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }

            var roomVM = new RoomVM
            {
                Id = room.Id,
                Number = room.Number,
                Description = room.Description,
                RoomType = room.RoomType,
                RoomTypeId = room.RoomTypeId,
                Rate = room.Rate,
                Floor = room.Floor
            };

            PopulateRoomsList();
            PopulateRoomTypesDropDownList(roomVM.RoomTypeId);

            return View(roomVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Is_Save, Is_Update")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var roomToUpdate = await db.Rooms.SingleAsync(i => i.Id == id);

            if (TryUpdateModel(roomToUpdate, "", new string[] { "Description", "RoomTypeId", "Floor", "Rate" }))
            {
                try
                {
                    db.Entry(roomToUpdate).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to apply changes. Please try again.");
                }

                TempData["Message"] = "Room updated successfully.";
                return RedirectToAction("Index");
            }

            return View(roomToUpdate);
        }

        // GET: Rooms/Delete/5
        [Authorize(Roles = "Admin, Is_Save")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Is_Save")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Room room = await db.Rooms.FindAsync(id);
            db.Rooms.Remove(room);
            await db.SaveChangesAsync();

            TempData["Message"] = "Room deleted successfully.";
            return RedirectToAction("Index");
        }

        [NonAction]
        bool RoomNumberExists(int number)
        {
            return (db.Rooms.Any(r => r.Number == number));
        }

        [NonAction]
        void PopulateRoomTypesDropDownList(object selectedRoomType = null)
        {
            ViewBag.RoomTypeId = new SelectList(db.RoomTypes, "Id", "Name", selectedRoomType);
        }

        [NonAction]
        void PopulateRoomsList()
        {
            TempData["RoomsList"] = db.Rooms.OrderBy(r => r.Number).ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
