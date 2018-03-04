using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HotelReservationSystem1.Models.Hotel
{
    public enum Title
    {
        Mr = 1, Mrs, Miss, Rev
    }

    public class Guest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Select title.")]
        [Display(Name = "Title")]
        public Title TitleId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name should be between {2} and {1} characters long.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name should be between {2} and {1} characters long.")]
        public string LastName { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Address should be between {2} and {1} characters long.")]
        public string Address { get; set; }

        [Display(Name = "Contact No.")]
        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Enter a valid contact number.")]
        [RegularExpression(@"^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{2})[-. ]*(\d{4})(?: *x(\d+))?\s*$"
            , ErrorMessage = "Enter a valid contact number.")]
        public string ContactNumber { get; set; }

        // TODO: Future enhancement. Guest can register which is optional.
        //public string UserId { get; set; }
        //public virtual ApplicationUser User { get; set; }
    }
}