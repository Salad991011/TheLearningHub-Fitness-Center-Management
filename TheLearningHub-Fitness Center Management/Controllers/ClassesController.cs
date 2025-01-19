using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheLearningHub_Fitness_Center_Management.Models;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    [Authorize(Roles = "Trainer")]
    public class ClassesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClassesController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            // Retrieve classes associated with the logged-in trainer
            var trainerId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
            var modelContext = _context.Classes
                .Include(c => c.User)
                .Where(c => c.Userid == trainerId);
            return View(await modelContext.ToListAsync());
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Classid == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Classes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Classid,Classname,Classtime,Classdate,Classdesc,ClassImageFile")] Class @class)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (@class.ClassImageFile != null)
                {
                    var fileName = $"{Guid.NewGuid()}_{@class.ClassImageFile.FileName}";
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await @class.ClassImageFile.CopyToAsync(stream);
                    }

                    @class.Imagepath = fileName;
                }

                // Assign the logged-in trainer's UserId
                var trainerId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
                @class.Userid = trainerId;

                // Set initial status to "Pending"
                @class.APPROVALSTATUS = "Pending";
                @class.ISAPPROVED = false;

                _context.Add(@class);
                await _context.SaveChangesAsync();

                // Send email notification to the trainer
                var trainerEmail = User.Claims.First(c => c.Type == ClaimTypes.Name).Value; // Assuming the email is stored in the Name claim
                await SendEmailAsync(
                    trainerEmail,
                    "Class Submission Confirmation",
                    $"Dear Trainer,\n\nYour class '{@class.Classname}' is pending approval by the Admin. You will be notified once it is reviewed."
                );

                TempData["InfoMessage"] = "Your class request has been submitted for approval. A confirmation email has been sent to your registered email address.";
                return RedirectToAction(nameof(Index));
            }

            return View(@class);
        }


        private async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                using (var smtpClient = new SmtpClient("smtp.office365.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("Saladforyra@Saladsc.onmicrosoft.com", "Salad1234");
                    smtpClient.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("Saladforyra@Saladsc.onmicrosoft.com", "Admin"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(recipientEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // POST: Classes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Classid,Classname,Classtime,Classdate,Classdesc,ClassImageFile,Imagepath")] Class @class)
        {
            if (id != @class.Classid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image upload
                    if (@class.ClassImageFile != null)
                    {
                        var fileName = $"{Guid.NewGuid()}_{@class.ClassImageFile.FileName}";
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await @class.ClassImageFile.CopyToAsync(stream);
                        }

                        @class.Imagepath = fileName;
                    }

                    // Ensure the logged-in trainer can only edit their own classes
                    var trainerId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
                    @class.Userid = trainerId;

                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Classid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(@class);
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Classid == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Classes == null)
            {
                return Problem("Entity set 'ModelContext.Classes' is null.");
            }
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(decimal id)
        {
            return (_context.Classes?.Any(e => e.Classid == id)).GetValueOrDefault();
        }
    }
}
