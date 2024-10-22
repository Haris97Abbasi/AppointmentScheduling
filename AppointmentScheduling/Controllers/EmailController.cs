using AppointmentScheduling.Models;
using AppointmentScheduling.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class EmailController : Controller
{
    private readonly EmailService _emailService;

    // The EmailService is injected by ASP.NET Core's Dependency Injection
    public EmailController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpGet]
    public ActionResult Send()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Send(EmailModel emailModel)
    {
        if (ModelState.IsValid)
        {
            await _emailService.SendEmailAsync(emailModel);  // Call the async method
            ViewBag.Message = "Email sent successfully!";
        }
        return View(emailModel);
    }
}
