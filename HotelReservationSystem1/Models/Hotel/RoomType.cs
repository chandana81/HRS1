using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelReservationSystem1.Models.Hotel
{
    public class RoomType
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}