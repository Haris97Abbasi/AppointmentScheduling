using AppointmentScheduling.Data;
using AppointmentScheduling.Models;
using AppointmentScheduling.Models.ViewModels;
using AppointmentScheduling.Utility;
using AppointmentScheduling.Services;

namespace AppointmentScheduling.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _db;
        private readonly EmailService _emailService;
        public AppointmentService(ApplicationDbContext db, EmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task<int> AddUpdate(AppointmentVM model)
        {
            var startDate = DateTime.Parse(model.StartDate);
            var endDate = DateTime.Parse(model.StartDate).AddMinutes(Convert.ToDouble(model.Duration));
            var patient = _db.Users.FirstOrDefault(u => u.Id == model.PatientId);
            var doctor = _db.Users.FirstOrDefault(u => u.Id == model.DoctorId);
            if (model != null && model.Id > 0)
            {
                //Id greater than 0 means that the record has already been created i.e., this is for update
                //update
                var appointment = _db.Appointments.FirstOrDefault(x => x.Id == model.Id);
                if (appointment != null)
                {
                    appointment.Title = model.Title;
                    appointment.Description = model.Description;
                    appointment.StartDate = startDate;
                    appointment.EndDate = endDate;
                    appointment.Duration = model.Duration;
                    appointment.DoctorId = model.DoctorId;
                    appointment.PatientId = model.PatientId;
                    appointment.IsDoctorApproved = false;
                    appointment.AdminId = model.AdminId;
                    await _db.SaveChangesAsync();
                }
                return 1;
            }
            else
            {
                //This is for creating a new data record
                Appointment appointment = new Appointment()
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = startDate,
                    EndDate = endDate,
                    Duration = model.Duration,
                    DoctorId = model.DoctorId,
                    PatientId = model.PatientId,
                    AdminId = model.AdminId,
                    IsDoctorApproved = model.IsDoctorApproved
                };
                _db.Appointments.Add(appointment);
                await _db.SaveChangesAsync();
                
                EmailModel emailModelPatient = new EmailModel();
                emailModelPatient.From = "abbasiharis1997@gmail.com";
                emailModelPatient.To = patient.Email;
                emailModelPatient.Subject = "Appointment Created";
                emailModelPatient.Body = $"Your appointment with {doctor.Name} is created and in pending status";
                //await _emailService.SendEmailAsync(emailModelPatient);

                EmailModel emailModelDoctor = new EmailModel();
                emailModelDoctor.From = "abbasiharis1997@gmail.com";
                emailModelDoctor.To = doctor.Email;
                emailModelDoctor.Subject = "Appointment Created";
                emailModelDoctor.Body = $"Your appointment with {patient.Name} is created and in pending status";
                //await _emailService.SendEmailAsync(emailModelDoctor);

                return 2;
            }
        }

        public async Task<int> ConfirmEvent(int id)
        {
            var appointment = _db.Appointments.FirstOrDefault(x => x.Id == id);
            if (appointment != null)
            {
                appointment.IsDoctorApproved = true;
                return await _db.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<int> Delete(int id)
        {
            var appointment = _db.Appointments.FirstOrDefault(x =>x.Id == id);
            if (appointment != null)
            {
                _db.Appointments.Remove(appointment);
                return await _db.SaveChangesAsync();
            }
            return 0;
        }

        public List<AppointmentVM> DoctorsEventsById(string doctorId)
        {
            return _db.Appointments.Where(x => x.DoctorId == doctorId).ToList().Select(c => new AppointmentVM()
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                IsDoctorApproved= c.IsDoctorApproved,
                Duration = c.Duration,
                StartDate = c.StartDate.HasValue ? c.StartDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                EndDate = c.EndDate.HasValue ? c.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
            }).ToList();
        }

        public AppointmentVM? GetById(int id)
        {
            return _db.Appointments.Where(x => x.Id == id).ToList().Select(c => new AppointmentVM()
            {
                Id = c.Id,
                Description = c.Description,
                StartDate = c.StartDate.HasValue ? c.StartDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                EndDate = c.EndDate.HasValue ? c.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                Title = c.Title,
                Duration = c.Duration,
                IsDoctorApproved = c.IsDoctorApproved,
                PatientId = c.PatientId,
                DoctorId = c.DoctorId,
                PatientName = _db.Users.Where(x => x.Id == c.PatientId).Select(x => x.Name).FirstOrDefault(),
                DoctorName = _db.Users.Where(x => x.Id == c.DoctorId).Select(x => x.Name).FirstOrDefault(),
            }).SingleOrDefault();
        }

        public List<DoctorVM> GetDoctorList()
        {
            var doctors = (from users in _db.Users
                           join userRoles in _db.UserRoles on users.Id equals userRoles.UserId
                           join roles in _db.Roles.Where(x=>x.Name == Helper.Doctor) on userRoles.RoleId equals roles.Id
                           select new DoctorVM
                           {
                               Id = users.Id,
                               Name = users.Name,
                           }).ToList();

            return doctors;
        }

        public List<PatientVM> GetPatientList()
        {
            var patients = (from users in _db.Users
                           join userRoles in _db.UserRoles on users.Id equals userRoles.UserId
                           join roles in _db.Roles.Where(x => x.Name == Helper.Patient) on userRoles.RoleId equals roles.Id
                           select new PatientVM
                           {
                               Id = users.Id,
                               Name = users.Name,
                           }).ToList();

            return patients;
        }

        public List<AppointmentVM> PatientsEventsById(string patientId)
        {
            return _db.Appointments.Where(x => x.PatientId == patientId).ToList().Select(c => new AppointmentVM()
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                IsDoctorApproved = c.IsDoctorApproved,
                Duration = c.Duration,
                StartDate = c.StartDate.HasValue ? c.StartDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                EndDate = c.EndDate.HasValue ? c.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
            }).ToList();
        }


    }
}
