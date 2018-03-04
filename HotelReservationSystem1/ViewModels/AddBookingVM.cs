using HotelReservationSystem1.Models.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelReservationSystem1.ViewModels
{
    public class AddBookingVM
    {
        public Title TitleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int RoomId { get; set; }
        public int GuestId { get; set; }
        public int RoomReservationId { get; set; }
        public IEnumerable<ReservationDetail> ReservationDetails { get; set; }
        public int NumOfDays
        {
            get
            {
                if (CheckInDate < DateTime.Today)
                {
                    return 0;
                }

                return (CheckOutDate > CheckInDate) ? (CheckOutDate - CheckInDate).Days : 0;
            }

        }
    }
}