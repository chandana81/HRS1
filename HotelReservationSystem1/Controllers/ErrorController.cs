using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelReservationSystem1.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = message;
            }         
            
            return View("Error");
        }
    }
}