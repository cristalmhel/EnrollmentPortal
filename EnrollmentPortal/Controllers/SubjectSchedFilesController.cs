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

namespace EnrollmentPortal.Controllers
{
    public class SubjectSchedFilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5; // Number of items per page

        public SubjectSchedFilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubjectSchedFiles
        public async Task<IActionResult> Index(string? searchString, int? pageNumber)
        {
            // Store the current filter in ViewData to preserve it across requests
            ViewData["CurrentFilterSchedule"] = searchString;

            var schedules = _context.SubjectSchedFiles.Include(s => s.SubjectFile).AsQueryable().AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                schedules = schedules.Where(s => s.SSFEDPCODE.ToString().Contains(searchString));
            }

            // Use the PageSize and PageNumber to get paginated data
            int currentPageNumber = pageNumber ?? 1;
            var paginatedSchedules = await PaginatedList<SubjectSchedFile>.CreateAsync(schedules, currentPageNumber, PageSize);

            return View(paginatedSchedules);
        }

        // GET: SubjectSchedFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjectSchedFile = await _context.SubjectSchedFiles
                .Include(s => s.SubjectFile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectSchedFile == null)
            {
                return NotFound();
            }

            return View(subjectSchedFile);
        }

        // GET: SubjectSchedFiles/Create
        public IActionResult Create()
        {
            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select status", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "Active", Value = "Active" },
                new SelectListItem { Text = "Inactive", Value = "Inactive" },
                new SelectListItem { Text = "Dissolved", Value = "Dissolved" },
                new SelectListItem { Text = "Restricted", Value = "Restricted" },
                new SelectListItem { Text = "Closed", Value = "Closed" }
            };

            ViewData["StatusOptions"] = statusOptions;

            var ampmOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select meridiem", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "AM", Value = "AM" },
                new SelectListItem { Text = "PM", Value = "PM" }
            };

            ViewData["AmPmOptions"] = ampmOptions;

            var subjects = new SelectList(_context.SubjectFiles.AsNoTracking(), "Id", "SFSUBJCODE");
            var subjectList = subjects.ToList();
            subjectList.Insert(0, new SelectListItem
            {
                Text = "Select a subject",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["SubjectFileId"] = subjectList;
            Random random = new Random();
            int randomNumber = random.Next(1000, 10000);
            int currentCount = _context.SubjectSchedFiles.Count();
            string edpCode = $"{randomNumber}{currentCount}";
            ViewBag.EdpCodeGenerated = edpCode;
            return View();
        }

        // POST: SubjectSchedFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SSFEDPCODE,SubjectFileId,SSFSTARTTIME,SSFENDTIME,SSFDAYS,SSFROOM,SSFMAXSIZE,SSFCLASSSIZE,SSFSTATUS,SSFXM,SSFSECTION,SSFSCHOOLYEAR,SSFSSEM")] SubjectSchedFile subjectSchedFile)
        {
            // Check if a Subject with the same Code already exists
            if (_context.SubjectSchedFiles.Any(s => s.SSFEDPCODE == subjectSchedFile.SSFEDPCODE))
            {
                ModelState.AddModelError("SSFEDPCODE", "A schedule with this code already exists.");
            }

            if (subjectSchedFile.SSFSTARTTIME != null && subjectSchedFile.SSFENDTIME != null)
            {
                if (subjectSchedFile.SSFSTARTTIME >= subjectSchedFile.SSFENDTIME)
                {
                    ModelState.AddModelError("SSFENDTIME", "Start time must be earlier than end time.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(subjectSchedFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select status", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "Active", Value = "Active" },
                new SelectListItem { Text = "Inactive", Value = "Inactive" },
                new SelectListItem { Text = "Dissolved", Value = "Dissolved" },
                new SelectListItem { Text = "Restricted", Value = "Restricted" },
                new SelectListItem { Text = "Closed", Value = "Closed" }
            };

            ViewData["StatusOptions"] = statusOptions;

            var ampmOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select meridiem", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "AM", Value = "AM" },
                new SelectListItem { Text = "PM", Value = "PM" }
            };

            ViewData["AmPmOptions"] = ampmOptions;

            var subjects = new SelectList(_context.SubjectFiles.AsNoTracking(), "Id", "SFSUBJCODE");
            var subjectList = subjects.ToList();
            subjectList.Insert(0, new SelectListItem
            {
                Text = "Select a subject",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["SubjectFileId"] = subjectList;
            return View();
        }

        // GET: SubjectSchedFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select status", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "Active", Value = "Active" },
                new SelectListItem { Text = "Inactive", Value = "Inactive" },
                new SelectListItem { Text = "Dissolved", Value = "Dissolved" },
                new SelectListItem { Text = "Restricted", Value = "Restricted" },
                new SelectListItem { Text = "Closed", Value = "Closed" }
            };

            ViewData["StatusOptions"] = statusOptions;

            var ampmOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select meridiem", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "AM", Value = "AM" },
                new SelectListItem { Text = "PM", Value = "PM" }
            };

            ViewData["AmPmOptions"] = ampmOptions;

            var subjects = new SelectList(_context.SubjectFiles.AsNoTracking(), "Id", "SFSUBJCODE");
            var subjectList = subjects.ToList();
            subjectList.Insert(0, new SelectListItem
            {
                Text = "Select a subject",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["SubjectFileId"] = subjectList;

            var subjectSchedFile = await _context.SubjectSchedFiles
                .Include(s => s.SubjectFile)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (subjectSchedFile == null)
            {
                return NotFound();
            }
            return View(subjectSchedFile);
        }

        // POST: SubjectSchedFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SSFEDPCODE,SubjectFileId,SSFSTARTTIME,SSFENDTIME,SSFDAYS,SSFROOM,SSFMAXSIZE,SSFCLASSSIZE,SSFSTATUS,SSFXM,SSFSECTION,SSFSCHOOLYEAR,SSFSSEM")] SubjectSchedFile subjectSchedFile)
        {
            if (id != subjectSchedFile.Id)
            {
                return NotFound();
            }

            if (subjectSchedFile.SSFSTARTTIME != null && subjectSchedFile.SSFENDTIME != null)
            {
                if (subjectSchedFile.SSFSTARTTIME >= subjectSchedFile.SSFENDTIME)
                {
                    ModelState.AddModelError("SSFENDTIME", "Start time must be earlier than end time.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subjectSchedFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectSchedFileExists(subjectSchedFile.Id))
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
            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select status", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "Active", Value = "Active" },
                new SelectListItem { Text = "Inactive", Value = "Inactive" },
                new SelectListItem { Text = "Dissolved", Value = "Dissolved" },
                new SelectListItem { Text = "Restricted", Value = "Restricted" },
                new SelectListItem { Text = "Closed", Value = "Closed" }
            };

            ViewData["StatusOptions"] = statusOptions;

            var ampmOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select meridiem", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "AM", Value = "AM" },
                new SelectListItem { Text = "PM", Value = "PM" }
            };

            ViewData["AmPmOptions"] = ampmOptions;

            var subjects = new SelectList(_context.SubjectFiles.AsNoTracking(), "Id", "SFSUBJCODE");
            var subjectList = subjects.ToList();
            subjectList.Insert(0, new SelectListItem
            {
                Text = "Select a subject",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["SubjectFileId"] = subjectList;

            var subjectFile = await _context.SubjectFiles
                .FirstOrDefaultAsync(s => s.Id == subjectSchedFile.SubjectFileId);
            subjectSchedFile.SubjectFile = subjectFile;

            return View(subjectSchedFile);
        }

        // GET: SubjectSchedFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjectSchedFile = await _context.SubjectSchedFiles
                .Include(s => s.SubjectFile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectSchedFile == null)
            {
                return NotFound();
            }

            return View(subjectSchedFile);
        }

        // POST: SubjectSchedFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subjectSchedFile = await _context.SubjectSchedFiles.FindAsync(id);
            if (subjectSchedFile != null)
            {
                _context.SubjectSchedFiles.Remove(subjectSchedFile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // This will be called by AJAX to get the subject details
        [HttpGet]
        public IActionResult GetSubject(int Id)
        {
            var subject = _context.SubjectFiles.FirstOrDefault(s => s.Id == Id);
            if (subject == null)
            {
                return NotFound();
            }
            return Json(subject);
        }

        private bool SubjectSchedFileExists(int id)
        {
            return _context.SubjectSchedFiles.Any(e => e.Id == id);
        }
    }
}
