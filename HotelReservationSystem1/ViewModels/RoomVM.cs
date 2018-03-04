using HotelReservationSystem1.Models.Hotel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelReservationSystem1.ViewModels
{
    public class RoomVM
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Room Number")]
        public int Number { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description has to be minimum 10 characters long.")]
        public string Description { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Floor number between {1} - {2} .")]
        public int Floor { get; set; }

        [Required]
        [Range(100, 20000, ErrorMessage = "Enter Rate between {1} - {2} .")]
        [DataType(DataType.Currency)]
        public decimal Rate { get; set; }

        [Required]
        [Display(Name = "Room Type")]
        public int RoomTypeId { get; set; }

        public virtual RoomType RoomType { get; set; }
    }
}