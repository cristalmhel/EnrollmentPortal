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

namespace EnrollmentPortal.Controllers
{
    public class SubjectFilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5; // Number of items per page

        public SubjectFilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubjectFiles
        public async Task<IActionResult> Index(string? searchString, int? pageNumber)
        {
            // Store the current filter in ViewData to preserve it across requests
            ViewData["CurrentFilterSubject"] = searchString;

            var subjects = _context.SubjectFiles.Include(s => s.Course).AsQueryable().AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                subjects = subjects.Where(s => s.SFSUBJCODE.ToString().Contains(searchString));
            }

            // Use the PageSize and PageNumber to get paginated data
            int currentPageNumber = pageNumber ?? 1;
            var paginatedSubjects = await PaginatedList<SubjectFile>.CreateAsync(subjects, currentPageNumber, PageSize);

            return View(paginatedSubjects);
        }

        // GET: SubjectFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjectFile = await _context.SubjectFiles
                .Include(s => s.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectFile == null)
            {
                return NotFound();
            }

            return View(subjectFile);
        }

        // GET: SubjectFiles/Create
        public IActionResult Create()
        {
            var offeringOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select offering", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "First Semester", Value = "First Semester" },
                new SelectListItem { Text = "Second Semester", Value = "Second Semester" },
                new SelectListItem { Text = "Summer", Value = "Summer" }
            };

            ViewData["OfferingOptions"] = offeringOptions;

            var categoryOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select category", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "Lecture", Value = "Lecture" },
                new SelectListItem { Text = "Laboratory", Value = "Laboratory" }
            };

            ViewData["CategoryOptions"] = categoryOptions;

            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select status", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "Active", Value = "Active" },
                new SelectListItem { Text = "Inactive", Value = "Inactive" }
            };

            ViewData["StatusOptions"] = statusOptions;

            var courses = new SelectList(_context.Courses.AsNoTracking(), "Id", "Code");
            var courseList = courses.ToList();
            courseList.Insert(0, new SelectListItem
            {
                Text = "Select a course",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["CourseId"] = courseList;
            return View();
        }

        // POST: SubjectFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SFSUBJCODE,SFSUBJDESC,SFSUBJUNITS,SFSUBJREGOFRNG,SFSUBJSCHLYR,SFSUBJCATEGORY,SFSUBJSTATUS,SFSUBJCURRCODE,CourseId,SubjectPreqFile")] SubjectFile subjectFile)
        {
            // Check if a Subject with the same Code already exists
            if (_context.SubjectFiles.Any(s => s.SFSUBJCODE == subjectFile.SFSUBJCODE))
            {
                ModelState.AddModelError("SFSUBJCODE", "A subject with this code already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(subjectFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var offeringOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select offering", Value = "", Disabled = true, Selected = (subjectFile.SFSUBJREGOFRNG == null) },
                new SelectListItem { Text = "First Semester", Value = "First Semester", Selected = (subjectFile.SFSUBJREGOFRNG == "First Semester") },
                new SelectListItem { Text = "Second Semester", Value = "Second Semester", Selected = (subjectFile.SFSUBJREGOFRNG == "Second Semester") },
                new SelectListItem { Text = "Summer", Value = "Summer", Selected = (subjectFile.SFSUBJREGOFRNG == "Summer") }
            };

            ViewData["OfferingOptions"] = offeringOptions;

            var categoryOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select category", Value = "", Disabled = true, Selected = (subjectFile.SFSUBJCATEGORY == null) },
                new SelectListItem { Text = "Lecture", Value = "Lecture", Selected = (subjectFile.SFSUBJCATEGORY == "Lecture") },
                new SelectListItem { Text = "Laboratory", Value = "Laboratory", Selected = (subjectFile.SFSUBJCATEGORY == "Laboratory") }
            };

            ViewData["CategoryOptions"] = categoryOptions;

            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select status", Value = "", Disabled = true, Selected = (subjectFile.SFSUBJSTATUS == null) },
                new SelectListItem { Text = "Active", Value = "Active", Selected = (subjectFile.SFSUBJSTATUS == "Active") },
                new SelectListItem { Text = "Inactive", Value = "Inactive", Selected = (subjectFile.SFSUBJSTATUS == "Inactive") }
            };

            ViewData["StatusOptions"] = statusOptions;

            var courses = new SelectList(_context.Courses, "Id", "Code", subjectFile.CourseId);
            var courseList = courses.ToList();
            courseList.Insert(0, new SelectListItem
            {
                Text = "Select a course",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["CourseId"] = courseList;
            return View(subjectFile);
        }

        // GET: SubjectFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offeringOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select offering", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "First Semester", Value = "First Semester" },
                new SelectListItem { Text = "Second Semester", Value = "Second Semester" },
                new SelectListItem { Text = "Summer", Value = "Summer" }
            };

            ViewData["OfferingOptions"] = offeringOptions;

            var categoryOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select category", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "Lecture", Value = "Lecture" },
                new SelectListItem { Text = "Laboratory", Value = "Laboratory" }
            };

            ViewData["CategoryOptions"] = categoryOptions;

            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select status", Value = "", Disabled = true, Selected = true },
                new SelectListItem { Text = "Active", Value = "Active" },
                new SelectListItem { Text = "Inactive", Value = "Inactive" }
            };

            ViewData["StatusOptions"] = statusOptions;

            var courses = new SelectList(_context.Courses.AsNoTracking(), "Id", "Code");
            var courseList = courses.ToList();
            courseList.Insert(0, new SelectListItem
            {
                Text = "Select a course",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["CourseId"] = courseList;

            var subjectFile = await _context.SubjectFiles.FindAsync(id);
            if (subjectFile == null)
            {
                return NotFound();
            }

            return View(subjectFile);
        }

        // POST: SubjectFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SFSUBJCODE,SFSUBJDESC,SFSUBJUNITS,SFSUBJREGOFRNG,SFSUBJSCHLYR,SFSUBJCATEGORY,SFSUBJSTATUS,SFSUBJCURRCODE,CourseId")] SubjectFile subjectFile)
        {
            if (id != subjectFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subjectFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectFileExists(subjectFile.Id))
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
            var offeringOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select offering", Value = "", Disabled = true, Selected = (subjectFile.SFSUBJREGOFRNG == null) },
                new SelectListItem { Text = "First Semester", Value = "First Semester", Selected = (subjectFile.SFSUBJREGOFRNG == "First Semester") },
                new SelectListItem { Text = "Second Semester", Value = "Second Semester", Selected = (subjectFile.SFSUBJREGOFRNG == "Second Semester") },
                new SelectListItem { Text = "Summer", Value = "Summer", Selected = (subjectFile.SFSUBJREGOFRNG == "Summer") }
            };

            ViewData["OfferingOptions"] = offeringOptions;

            var categoryOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select category", Value = "", Disabled = true, Selected = (subjectFile.SFSUBJCATEGORY == null) },
                new SelectListItem { Text = "Lecture", Value = "Lecture", Selected = (subjectFile.SFSUBJCATEGORY == "Lecture") },
                new SelectListItem { Text = "Laboratory", Value = "Laboratory", Selected = (subjectFile.SFSUBJCATEGORY == "Laboratory") }
            };

            ViewData["CategoryOptions"] = categoryOptions;

            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select status", Value = "", Disabled = true, Selected = (subjectFile.SFSUBJSTATUS == null) },
                new SelectListItem { Text = "Active", Value = "Active", Selected = (subjectFile.SFSUBJSTATUS == "Active") },
                new SelectListItem { Text = "Inactive", Value = "Inactive", Selected = (subjectFile.SFSUBJSTATUS == "Inactive") }
            };

            ViewData["StatusOptions"] = statusOptions;

            var courses = new SelectList(_context.Courses, "Id", "Code", subjectFile.CourseId);
            var courseList = courses.ToList();
            courseList.Insert(0, new SelectListItem
            {
                Text = "Select a course",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["CourseId"] = courseList;
            
            return View(subjectFile);
        }

        // GET: SubjectFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjectFile = await _context.SubjectFiles
                .Include(s => s.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectFile == null)
            {
                return NotFound();
            }

            return View(subjectFile);
        }

        // POST: SubjectFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subjectFile = await _context.SubjectFiles.FindAsync(id);
            if (subjectFile != null)
            {
                _context.SubjectFiles.Remove(subjectFile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Search subjects that will be called via AJAX
        [HttpGet]
        public JsonResult SearchSubjects(string searchTerm)
        {
            // Check if searchTerm is provided
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Json(new { success = false, message = "Search term cannot be empty." });
            }

            // Perform search based on the search term (case-insensitive)
            var subjects = _context.SubjectFiles
                .Where(s => s.SFSUBJCODE.ToLower().Equals(searchTerm.ToLower()))
                .Select(s => new { 
                    s.Id,
                    s.SFSUBJCODE,
                    s.SFSUBJDESC,
                    s.SFSUBJUNITS,
                    s.SFSUBJCATEGORY,
                    s.SFSUBJREGOFRNG,
                    s.SFSUBJSTATUS
                })
                .ToList();

            return Json(new { success = true, subjects });
        }

        private bool SubjectFileExists(int id)
        {
            return _context.SubjectFiles.Any(e => e.Id == id);
        }
    }
}
