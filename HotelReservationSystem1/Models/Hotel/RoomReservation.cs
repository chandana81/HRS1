using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelReservationSystem1.Models.Hotel
{
    public class RoomReservation
    {
        [Display(Name = "Reservation #")]
        public int Id { get; set; }

        [Required]
        public int GuestId { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        public bool BookingConfirmed { get; set; }

        public DateTime BookingCreatedOn { get; set; }

        public virtual Guest Guest { get; set; }
    }
}