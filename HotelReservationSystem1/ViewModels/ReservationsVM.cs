using HotelReservationSystem1.Models.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelReservationSystem1.ViewModels
{
    public class ReservationsVM
    {
        public Guest Guest { get; set; }
        public RoomReservation RoomReservations { get; set; }
        public IEnumerable<ReservationDetail> ReservationDetails { get; set; }
        public IEnumerable<Room> Rooms { get; set; }
        public decimal TotalAmount
        {
            get
            {
                return ReservationDetails != null ? ReservationDetails.Sum(r => r.TotalRate) : 0;
            }
        }
    }
}