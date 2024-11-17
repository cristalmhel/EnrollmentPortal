using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnrollmentPortal.Data;
using EnrollmentPortal.Models.Entities;
using EnrollmentPortal.Helper;
using System.Drawing.Printing;
using System.Security.Claims;
using EnrollmentPortal.Models.ViewModel;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EnrollmentPortal.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5; // Number of items per page

        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Enrollment
        public async Task<IActionResult> Index(string? searchString, int? pageNumber)
        {
            // Store the current filter in ViewData to preserve it across requests
            ViewData["CurrentFilterEnroll"] = searchString;

            var enrolled = _context.EnrollmentHeaderFiles.Include(e => e.StudentFile).AsQueryable().AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                enrolled = enrolled.Where(e => e.StudentFile.StudId.ToString().Contains(searchString));
            }

            // Use the PageSize and PageNumber to get paginated data
            int currentPageNumber = pageNumber ?? 1;
            var paginatedSubjects = await PaginatedList<EnrollmentHeaderFile>.CreateAsync(enrolled, currentPageNumber, PageSize);

            return View(paginatedSubjects);
        }

        // GET: Enrollment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollmentHeaderFile = await _context.EnrollmentHeaderFiles
                .Include(s => s.StudentFile)
                .ThenInclude(student => student.Course)
                .Include(e => e.EnrollmentDetailFiles)
                .SingleOrDefaultAsync(s => s.Id == id);
            if (enrollmentHeaderFile == null)
            {
                return NotFound();
            }

            if (enrollmentHeaderFile.EnrollmentDetailFiles != null)
            {
                List<ScheduleViewModel> enrolledSchedules = new List<ScheduleViewModel>();

                foreach (var detail in enrollmentHeaderFile.EnrollmentDetailFiles)
                {
                    var sched = await _context.SubjectSchedFiles.Include(s => s.SubjectFile).FirstOrDefaultAsync(sf => sf.Id == detail.SubjectSchedFileId);

                    enrolledSchedules.Add(new ScheduleViewModel
                    {
                        scheduleId = sched.Id,
                        subjectId = sched.SubjectFile.Id,
                        edpCode = sched.SSFEDPCODE,
                        subjectCode = sched.SubjectFile.SFSUBJCODE,
                        subjectUnits = sched.SubjectFile.SFSUBJUNITS,
                        startTime = sched.SSFSTARTTIME,
                        endTime = sched.SSFENDTIME,
                        ampm = sched.SSFXM,
                        days = sched.SSFDAYS,
                        room = sched.SSFROOM,
                        maxSize = sched.SSFMAXSIZE,
                        classSize = sched.SSFCLASSSIZE,
                        status = detail.ENRDFSTUDSTATUS
                    });
                }

                // Configure JSON serialization options to handle references
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };

                var schedulesJson = JsonSerializer.Serialize(enrolledSchedules, options);

                // Store serialized JSON in ViewData
                ViewData["EnrolledSchedulesJson"] = schedulesJson;
            }
            return View(enrollmentHeaderFile);
        }

        // GET: Enrollment/Create
        public IActionResult Create()
        {
            var semesterOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Semester", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "1st Sem", Value = "1st Sem" },
                new SelectListItem { Text = "2nd Sem", Value = "2nd Sem" },
                new SelectListItem { Text = "Summer", Value = "Summer" }
            };
            ViewData["SemesterOptions"] = semesterOptions;

            var schoolYearOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select School Year", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "2024-2025", Value = "2024-2025" },
                new SelectListItem { Text = "2025-2026", Value = "2025-2026"},
                new SelectListItem { Text = "2026-2027", Value = "2026-2027" },
                new SelectListItem { Text = "2027-2028", Value = "2027-2028" }
            };
            ViewData["SchoolYearOptions"] = schoolYearOptions;

            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Status", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "Enrolled", Value = "Enrolled" },
                new SelectListItem { Text = "Unenrolled", Value = "Unenrolled"}
            };
            ViewData["StatusOptions"] = statusOptions;

            var students = new SelectList(_context.StudentFiles.Where(s => s.STFSTUDSTATUS == "Active").AsNoTracking(), "StudId", "StudId");
            var studentsList = students.ToList();
            studentsList.Insert(0, new SelectListItem
            {
                Text = "Select student Id",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["StudentFileId"] = studentsList;

            var edps = new SelectList(_context.SubjectSchedFiles.Where(s => s.SSFSTATUS == "Active").AsNoTracking(), "Id", "SSFEDPCODE");
            var edpsList = edps.ToList();
            edpsList.Insert(0, new SelectListItem
            {
                Text = "Select EDP",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["edpsList"] = edpsList;
            return View();
        }

        // POST: Enrollment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ENRHFSTUDDATEENROLL,ENRHFSTUDSCHLYR,ENRHFSTUDSEM,ENRHFSTUDENCODER,ENRHFSTUDTOTALUNITS,ENRHFSTUDSTATUS,StudentFileId")] EnrollmentHeaderFile enrollmentHeaderFile, int[] subjectScheduleIds, int[] subjectIds)
        {
            // Check if the students if not yet enrolled base in the semester and school year
            if (_context.EnrollmentHeaderFiles.Any(
                c => c.StudentFileId == enrollmentHeaderFile.StudentFileId &&
                c.ENRHFSTUDSCHLYR == enrollmentHeaderFile.ENRHFSTUDSCHLYR &&
                c.ENRHFSTUDSEM == enrollmentHeaderFile.ENRHFSTUDSEM
            ))
            {
                ModelState.AddModelError("StudentFileId", "This student is already enrolled this semester!");
            }

            if (ModelState.IsValid && (subjectScheduleIds != null && subjectScheduleIds.Length > 0))
            {
                enrollmentHeaderFile.ENRHFSTUDENCODER = User.FindFirst(ClaimTypes.Name)?.Value;
                _context.Add(enrollmentHeaderFile);
                await _context.SaveChangesAsync();

                enrollmentHeaderFile.EnrollmentDetailFiles = new List<EnrollmentDetailFile>();

                for (int i = 0; i < subjectScheduleIds.Length; i++)
                {
                    var schedule = new EnrollmentDetailFile
                    {
                        EnrollmentHeaderFileId = enrollmentHeaderFile.Id,
                        SubjectFileId = subjectIds[i],
                        SubjectSchedFileId = subjectScheduleIds[i],
                        ENRDFSTUDSTATUS = "Active"
                    };
                    enrollmentHeaderFile.EnrollmentDetailFiles.Add(schedule);

                    // Retrieve the schedule record from the database
                    var scheduleToUpdate = await _context.SubjectSchedFiles
                                                         .SingleOrDefaultAsync(s => s.Id == subjectScheduleIds[i]);

                    if (scheduleToUpdate != null)
                    {
                        // Increment the ClassSize by 1
                        scheduleToUpdate.SSFCLASSSIZE += 1;

                        // Save the updated ClassSize back to the database
                        _context.SubjectSchedFiles.Update(scheduleToUpdate); // Optional: EF will track changes automatically.
                    }
                }

                // Add and save the EnrollmentDetailFiles (if any)
                _context.EnrollmentDetailFiles.AddRange(enrollmentHeaderFile.EnrollmentDetailFiles);
                await _context.SaveChangesAsync();  // Second save
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("StudentFileId", "Enrollment Failed: No schedule added.");
            }

            var semesterOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Semester", Value = "", Disabled = true, Selected = (enrollmentHeaderFile.ENRHFSTUDSEM == null) },
                new SelectListItem { Text = "First Semester", Value = "1st Sem", Selected = (enrollmentHeaderFile.ENRHFSTUDSEM == "1st Sem") },
                new SelectListItem { Text = "Second Semester", Value = "2nd Sem", Selected = (enrollmentHeaderFile.ENRHFSTUDSEM == "2nd Sem") },
                new SelectListItem { Text = "Summer", Value = "Summer", Selected = (enrollmentHeaderFile.ENRHFSTUDSEM == "Summer") }
            };
            ViewData["SemesterOptions"] = semesterOptions;

            var schoolYearOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select School Year", Value = "", Disabled = true, Selected = (enrollmentHeaderFile.ENRHFSTUDSCHLYR == null) },
                new SelectListItem { Text = "2024-2025", Value = "2024-2025", Selected = (enrollmentHeaderFile.ENRHFSTUDSCHLYR == "2024-2025") },
                new SelectListItem { Text = "2025-2026", Value = "2025-2026", Selected = (enrollmentHeaderFile.ENRHFSTUDSCHLYR == "2025-2026")},
                new SelectListItem { Text = "2026-2027", Value = "2026-2027", Selected = (enrollmentHeaderFile.ENRHFSTUDSCHLYR == "2026-2027") },
                new SelectListItem { Text = "2027-2028", Value = "2027-2028", Selected = (enrollmentHeaderFile.ENRHFSTUDSCHLYR == "2027-2028") }
            };
            ViewData["SchoolYearOptions"] = schoolYearOptions;

            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Status", Value = "", Disabled = true, Selected = (enrollmentHeaderFile.ENRHFSTUDSTATUS == null) },
                new SelectListItem { Text = "Enrolled", Value = "Enrolled", Selected = (enrollmentHeaderFile.ENRHFSTUDSTATUS == "Enrolled") },
                new SelectListItem { Text = "Unenrolled", Value = "Unenrolled", Selected = (enrollmentHeaderFile.ENRHFSTUDSTATUS == "Unenrolled")}
            };
            ViewData["StatusOptions"] = statusOptions;

            var students = new SelectList(_context.StudentFiles.Where(s => s.STFSTUDSTATUS == "Active").AsNoTracking(), "StudId", "StudId", enrollmentHeaderFile.StudentFileId);
            var studentsList = students.ToList();
            studentsList.Insert(0, new SelectListItem
            {
                Text = "Select student Id",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["StudentFileId"] = studentsList;

            var edps = new SelectList(_context.SubjectSchedFiles.Where(s => s.SSFSTATUS == "Active").AsNoTracking(), "Id", "SSFEDPCODE");
            var edpsList = edps.ToList();
            edpsList.Insert(0, new SelectListItem
            {
                Text = "Select EDP",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["edpsList"] = edpsList;

            return View(enrollmentHeaderFile);
        }

        // GET: Enrollment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var semesterOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Semester", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "1st Sem", Value = "1st Sem" },
                new SelectListItem { Text = "2nd Sem", Value = "2nd Sem" },
                new SelectListItem { Text = "Summer", Value = "Summer" }
            };
            ViewData["SemesterOptions"] = semesterOptions;

            var schoolYearOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select School Year", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "2024-2025", Value = "2024-2025" },
                new SelectListItem { Text = "2025-2026", Value = "2025-2026"},
                new SelectListItem { Text = "2026-2027", Value = "2026-2027" },
                new SelectListItem { Text = "2027-2028", Value = "2027-2028" }
            };
            ViewData["SchoolYearOptions"] = schoolYearOptions;

            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Status", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "Enrolled", Value = "Enrolled" },
                new SelectListItem { Text = "Unenrolled", Value = "Unenrolled"}
            };
            ViewData["StatusOptions"] = statusOptions;

            var edps = new SelectList(_context.SubjectSchedFiles.Where(s => s.SSFSTATUS == "Active").AsNoTracking(), "Id", "SSFEDPCODE");
            var edpsList = edps.ToList();
            edpsList.Insert(0, new SelectListItem
            {
                Text = "Select EDP",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["edpsList"] = edpsList;

            var enrollmentHeaderFile = await _context.EnrollmentHeaderFiles
                .Include(s => s.StudentFile)
                .ThenInclude(student => student.Course)
                .Include(e => e.EnrollmentDetailFiles)
                .SingleOrDefaultAsync(s => s.Id == id);
            if (enrollmentHeaderFile == null)
            {
                return NotFound();
            }
            
            if (enrollmentHeaderFile.EnrollmentDetailFiles != null)
            {
                List<ScheduleViewModel> enrolledSchedules = new List<ScheduleViewModel>();

                foreach (var detail in enrollmentHeaderFile.EnrollmentDetailFiles)
                {
                    var sched = await _context.SubjectSchedFiles.Include(s => s.SubjectFile).FirstOrDefaultAsync(sf => sf.Id == detail.SubjectSchedFileId);

                    enrolledSchedules.Add(new ScheduleViewModel
                    {
                        scheduleId = sched.Id,
                        subjectId = sched.SubjectFile.Id,
                        edpCode  = sched.SSFEDPCODE,
                        subjectCode  = sched.SubjectFile.SFSUBJCODE,
                        subjectUnits  = sched.SubjectFile.SFSUBJUNITS,
                        startTime = sched.SSFSTARTTIME,
                        endTime = sched.SSFENDTIME,
                        ampm = sched.SSFXM,
                        days = sched.SSFDAYS,
                        room  = sched.SSFROOM,
                        maxSize = sched.SSFMAXSIZE,
                        classSize = sched.SSFCLASSSIZE,
                        status = detail.ENRDFSTUDSTATUS
                    });
                }

                // Configure JSON serialization options to handle references
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };

                var schedulesJson = JsonSerializer.Serialize(enrolledSchedules, options);

                // Store serialized JSON in ViewData
                ViewData["EnrolledSchedulesJson"] = schedulesJson;
            }
            return View(enrollmentHeaderFile);
        }

        // POST: Enrollment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ENRHFSTUDDATEENROLL,ENRHFSTUDSCHLYR,ENRHFSTUDSEM,ENRHFSTUDENCODER,ENRHFSTUDTOTALUNITS,ENRHFSTUDSTATUS,StudentFileId")] EnrollmentHeaderFile enrollmentHeaderFile, int[] subjectScheduleIds, int[] subjectIds, string[] statuses, int[] origScheduleIds, string[] origStatuses)
        {
            if (id != enrollmentHeaderFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid && (subjectScheduleIds != null && subjectScheduleIds.Length > 0))
            {
                try
                {
                    enrollmentHeaderFile.ENRHFSTUDENCODER = User.FindFirst(ClaimTypes.Name)?.Value;
                    _context.Update(enrollmentHeaderFile);
                    await _context.SaveChangesAsync();

                    var detailsToDelete = await _context.EnrollmentDetailFiles
                       .Where(e => e.EnrollmentHeaderFileId == id)
                       .ToListAsync();

                    if (detailsToDelete.Any())
                    {
                        // Remove each requisite entry from the context
                        _context.EnrollmentDetailFiles.RemoveRange(detailsToDelete);
                        await _context.SaveChangesAsync(); // Save the deletion
                    }

                    enrollmentHeaderFile.EnrollmentDetailFiles = new List<EnrollmentDetailFile>();

                    for (int i = 0; i < subjectScheduleIds.Length; i++)
                    {
                        var schedule = new EnrollmentDetailFile
                        {
                            EnrollmentHeaderFileId = enrollmentHeaderFile.Id,
                            SubjectFileId = subjectIds[i],
                            SubjectSchedFileId = subjectScheduleIds[i],
                            ENRDFSTUDSTATUS = statuses[i],
                        };
                        enrollmentHeaderFile.EnrollmentDetailFiles.Add(schedule);
                    }

                    // Add and save the EnrollmentDetailFiles (if any)
                    _context.EnrollmentDetailFiles.AddRange(enrollmentHeaderFile.EnrollmentDetailFiles);

                    // Update the Class size of the Subject Schedule / EDP
                    for (int i = 0; i < subjectScheduleIds.Length; i++)
                    {
                        for (int j = 0; j < origScheduleIds.Length; j++)
                        {
                            if (subjectScheduleIds[i] == origScheduleIds[j] && statuses[i] != origStatuses[j])
                            {
                                var scheduleToUpdate = await _context.SubjectSchedFiles
                                    .SingleOrDefaultAsync(s => s.Id == subjectScheduleIds[i]);
                                if (scheduleToUpdate != null)
                                {
                                    if (statuses[i] == "Active" && (origStatuses[i] == "Cancelled" || origStatuses[i] == "Withdrawn"))
                                    {
                                        scheduleToUpdate.SSFCLASSSIZE += 1;
                                    }
                                    else if (origStatuses[i] == "Active" && (statuses[i] == "Cancelled" || statuses[i] == "Withdrawn"))
                                    {
                                        if (scheduleToUpdate.SSFCLASSSIZE <= 0)
                                        {
                                            scheduleToUpdate.SSFCLASSSIZE = 0;
                                        }
                                        else
                                        {
                                            scheduleToUpdate.SSFCLASSSIZE -= 1;
                                        }
                                    }
                                    Console.WriteLine($"Value: {statuses[i]}");
                                    // Save the updated ClassSize back to the database
                                    _context.SubjectSchedFiles.Update(scheduleToUpdate); // Optional: EF will track changes automatically.
                                }
                            }
                        }
                    }

                    var newSchedIds = subjectScheduleIds
                        .Select((value, index) => new { Id = value, Index = index })
                        .Where(x => !origScheduleIds.Contains(x.Id));

                    if (newSchedIds.Any())
                    {
                        foreach (var sched in newSchedIds)
                        {
                            var scheduleToUpdate = await _context.SubjectSchedFiles
                                .SingleOrDefaultAsync(s => s.Id == sched.Id);

                            int i = sched.Index;

                            if (scheduleToUpdate != null)
                            {
                                if (statuses[i] == "Active")
                                {
                                    scheduleToUpdate.SSFCLASSSIZE += 1;
                                }
                                
                                // Save the updated ClassSize back to the database
                                _context.SubjectSchedFiles.Update(scheduleToUpdate); // Optional: EF will track changes automatically.
                            }
                        }
                    }

                    await _context.SaveChangesAsync();  // Second save
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentHeaderFileExists(enrollmentHeaderFile.Id))
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

            var semesterOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Semester", Value = "", Disabled = true, Selected = (enrollmentHeaderFile.ENRHFSTUDSEM == null) },
                new SelectListItem { Text = "First Semester", Value = "1st Sem", Selected = (enrollmentHeaderFile.ENRHFSTUDSEM == "1st Sem") },
                new SelectListItem { Text = "Second Semester", Value = "2nd Sem", Selected = (enrollmentHeaderFile.ENRHFSTUDSEM == "2nd Sem") },
                new SelectListItem { Text = "Summer", Value = "Summer", Selected = (enrollmentHeaderFile.ENRHFSTUDSEM == "Summer") }
            };
            ViewData["SemesterOptions"] = semesterOptions;

            var schoolYearOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select School Year", Value = "", Disabled = true, Selected = (enrollmentHeaderFile.ENRHFSTUDSCHLYR == null) },
                new SelectListItem { Text = "2024-2025", Value = "2024-2025", Selected = (enrollmentHeaderFile.ENRHFSTUDSCHLYR == "2024-2025") },
                new SelectListItem { Text = "2025-2026", Value = "2025-2026", Selected = (enrollmentHeaderFile.ENRHFSTUDSCHLYR == "2025-2026")},
                new SelectListItem { Text = "2026-2027", Value = "2026-2027", Selected = (enrollmentHeaderFile.ENRHFSTUDSCHLYR == "2026-2027") },
                new SelectListItem { Text = "2027-2028", Value = "2027-2028", Selected = (enrollmentHeaderFile.ENRHFSTUDSCHLYR == "2027-2028") }
            };
            ViewData["SchoolYearOptions"] = schoolYearOptions;

            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Status", Value = "", Disabled = true, Selected = (enrollmentHeaderFile.ENRHFSTUDSTATUS == null) },
                new SelectListItem { Text = "Enrolled", Value = "Enrolled", Selected = (enrollmentHeaderFile.ENRHFSTUDSTATUS == "Enrolled") },
                new SelectListItem { Text = "Unenrolled", Value = "Unenrolled", Selected = (enrollmentHeaderFile.ENRHFSTUDSTATUS == "Unenrolled")}
            };
            ViewData["StatusOptions"] = statusOptions;

            var edps = new SelectList(_context.SubjectSchedFiles.Where(s => s.SSFSTATUS == "Active").AsNoTracking(), "Id", "SSFEDPCODE");
            var edpsList = edps.ToList();
            edpsList.Insert(0, new SelectListItem
            {
                Text = "Select EDP",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["edpsList"] = edpsList;

            return View(enrollmentHeaderFile);
        }

        // GET: Enrollment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollmentHeaderFile = await _context.EnrollmentHeaderFiles
                .Include(s => s.StudentFile)
                .ThenInclude(student => student.Course)
                .Include(e => e.EnrollmentDetailFiles)
                .SingleOrDefaultAsync(s => s.Id == id);
            if (enrollmentHeaderFile == null)
            {
                return NotFound();
            }

            if (enrollmentHeaderFile.EnrollmentDetailFiles != null)
            {
                List<ScheduleViewModel> enrolledSchedules = new List<ScheduleViewModel>();

                foreach (var detail in enrollmentHeaderFile.EnrollmentDetailFiles)
                {
                    var sched = await _context.SubjectSchedFiles.Include(s => s.SubjectFile).FirstOrDefaultAsync(sf => sf.Id == detail.SubjectSchedFileId);

                    enrolledSchedules.Add(new ScheduleViewModel
                    {
                        scheduleId = sched.Id,
                        subjectId = sched.SubjectFile.Id,
                        edpCode = sched.SSFEDPCODE,
                        subjectCode = sched.SubjectFile.SFSUBJCODE,
                        subjectUnits = sched.SubjectFile.SFSUBJUNITS,
                        startTime = sched.SSFSTARTTIME,
                        endTime = sched.SSFENDTIME,
                        ampm = sched.SSFXM,
                        days = sched.SSFDAYS,
                        room = sched.SSFROOM,
                        maxSize = sched.SSFMAXSIZE,
                        classSize = sched.SSFCLASSSIZE
                    });
                }

                // Configure JSON serialization options to handle references
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };

                var schedulesJson = JsonSerializer.Serialize(enrolledSchedules, options);

                // Store serialized JSON in ViewData
                ViewData["EnrolledSchedulesJson"] = schedulesJson;
            }

            return View(enrollmentHeaderFile);
        }

        // POST: Enrollment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollmentHeaderFile = await _context.EnrollmentHeaderFiles.FindAsync(id);
            if (enrollmentHeaderFile != null)
            {
                _context.EnrollmentHeaderFiles.Remove(enrollmentHeaderFile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // This will be called by AJAX to get the student details
        [HttpGet]
        public IActionResult GetStudent(long Id)
        {
            var student = _context.StudentFiles.Include(c => c.Course).FirstOrDefault(s => s.StudId == Id);
            if (student == null)
            {
                return NotFound();
            }
            return Json(student);
        }

        // Search schedule that will be called via AJAX
        [HttpGet]
        public JsonResult SearchEDP(string searchTerm)
        {
            // Check if searchTerm is provided
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Json(new { success = false, message = "Search term cannot be empty." });
            }

            // Perform search based on the search term (case-insensitive)
            var schedules = _context.SubjectSchedFiles
                .Include(s => s.SubjectFile)
                .Where(s => s.Id.ToString().ToLower().Equals(searchTerm.ToLower()) && s.SSFSTATUS == "Active")
                .Select(s => new {
                    s.Id,
                    s.SSFEDPCODE,
                    subjectId = s.SubjectFile.Id,
                    subjectUnit = s.SubjectFile.SFSUBJUNITS,
                    subjectCode = s.SubjectFile.SFSUBJCODE,
                    SSFSTARTTIME = s.SSFSTARTTIME.ToString("hh:mm tt"),
                    SSFENDTIME = s.SSFENDTIME.ToString("hh:mm tt"),
                    s.SSFDAYS,
                    s.SSFXM,
                    s.SSFCLASSSIZE,
                    s.SSFMAXSIZE,
                    s.SSFROOM
                })
                .ToList();

            return Json(new { success = true, schedules });
        }

        private bool EnrollmentHeaderFileExists(int id)
        {
            return _context.EnrollmentHeaderFiles.Any(e => e.Id == id);
        }
    }
}
