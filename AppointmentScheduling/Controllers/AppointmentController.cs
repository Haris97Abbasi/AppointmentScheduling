using AppointmentScheduling.Services;
using AppointmentScheduling.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduling.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        //[Authorize]
        //[Authorize(Roles =Helper.Admin)] //the role (e.g.Admin) must be of type const
        public IActionResult Index()
        {
            ViewBag.DoctorList = _appointmentService.GetDoctorList();
            ViewBag.PatientList = _appointmentService.GetPatientList();
            ViewBag.Duration = Helper.GetTimeDropDown();
            // Session variable (a global variable accessable throughout the app)
            HttpContext.Session.SetString("UserName", "Haris");
            return View();
        }
    }
}
