using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HotelReservationSystem1.Models.Hotel
{
    public class ReservationDetail
    {
        public int Id { get; set; }

        [Required]
        public int RoomReservationId { get; set; }

        [Required(ErrorMessage = "Select room number.")]
        public int RoomId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Rate { get; set; }

        [Required(ErrorMessage = "Choose a check-in date.")]
        [Display(Name = "Check-In Date")]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Choose a check-out date.")]
        [Display(Name = "Check-Out Date")]
        public DateTime CheckOutDate { get; set; }

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

        [DataType(DataType.Currency)]
        public decimal TotalRate
        {
            get
            {
                return Rate * NumOfDays;
            }
        }

        public virtual RoomReservation RoomReservation { get; set; }
        public virtual Room Room { get; set; }
    }
}