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
    public class StudentFilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5; // Number of items per page

        public StudentFilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudentFiles
        public async Task<IActionResult> Index(string? searchString, int? pageNumber)
        {
            // Store the current filter in ViewData to preserve it across requests
            ViewData["CurrentFilterStudent"] = searchString;

            var students = _context.StudentFiles.Include(s => s.Course).AsQueryable().AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.StudId.ToString().Contains(searchString)
                                          || s.STFSTUDFNAME.Contains(searchString)
                                          || s.STFSTUDLNAME.Contains(searchString));
            }

            // Use the PageSize and PageNumber to get paginated data
            int currentPageNumber = pageNumber ?? 1;
            var paginatedStudents = await PaginatedList<StudentFile>.CreateAsync(students, currentPageNumber, PageSize);

            return View(paginatedStudents);
        }


        // GET: StudentFiles/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFile = await _context.StudentFiles
                .Include(s => s.Course)
                .FirstOrDefaultAsync(m => m.StudId == id);
            if (studentFile == null)
            {
                return NotFound();
            }

            return View(studentFile);
        }

        // GET: StudentFiles/Create
        public IActionResult Create()
        {
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

        // POST: StudentFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,STFSTUDLNAME,STFSTUDFNAME,STFSTUDMNAME,STFSTUDYEAR,STFSTUDREMARKS,STFSTUDSTATUS,CourseId")] StudentFile studentFile)
        {
            if (ModelState.IsValid)
            {
                // Generate the unique ID as a string
                string datePart = DateTime.Now.ToString("yyyyMMdd");
                int currentCount = await _context.StudentFiles.CountAsync();
                string newIdString = $"{datePart}{currentCount + 1:D4}"; // Format to ensure 4 digits for the number part

                // Convert the ID string to a long
                if (long.TryParse(newIdString, out long newId))
                {
                    // Assign the converted ID to the studentFile object
                    studentFile.StudId = newId;

                    _context.Add(studentFile);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Generated ID could not be converted to long.");
                }
            }

            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select status", Value = "", Disabled = true, Selected = (studentFile.STFSTUDSTATUS == null) },
                new SelectListItem { Text = "Active", Value = "Active", Selected = (studentFile.STFSTUDSTATUS == "Active") },
                new SelectListItem { Text = "Inactive", Value = "Inactive", Selected = (studentFile.STFSTUDSTATUS == "Inactive") }
            };

            ViewData["StatusOptions"] = statusOptions;

            var courses = new SelectList(_context.Courses, "Id", "Code", studentFile.CourseId);
            var courseList = courses.ToList();
            courseList.Insert(0, new SelectListItem
            {
                Text = "Select a course",
                Value = "",
                Disabled = true,
                Selected = true
            });
            ViewData["CourseId"] = courseList;
            return View(studentFile);
        }

        // GET: StudentFiles/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFile = await _context.StudentFiles.FindAsync(id);
            if (studentFile == null)
            {
                return NotFound();
            }
            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select status", Value = "", Disabled = true, Selected = (studentFile.STFSTUDSTATUS == null) },
                new SelectListItem { Text = "Active", Value = "Active", Selected = (studentFile.STFSTUDSTATUS == "Active") },
                new SelectListItem { Text = "Inactive", Value = "Inactive", Selected = (studentFile.STFSTUDSTATUS == "Inactive") }
            };

            ViewData["StatusOptions"] = statusOptions;

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Code", studentFile.CourseId);
            return View(studentFile);
        }

        // POST: StudentFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("StudId,STFSTUDLNAME,STFSTUDFNAME,STFSTUDMNAME,STFSTUDYEAR,STFSTUDREMARKS,STFSTUDSTATUS,CourseId")] StudentFile studentFile)
        {
            if (id != studentFile.StudId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentFileExists(studentFile.StudId))
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
                new SelectListItem { Text = "Select status", Value = "", Disabled = true, Selected = (studentFile.STFSTUDSTATUS == null) },
                new SelectListItem { Text = "Active", Value = "Active", Selected = (studentFile.STFSTUDSTATUS == "Active") },
                new SelectListItem { Text = "Inactive", Value = "Inactive", Selected = (studentFile.STFSTUDSTATUS == "Inactive") }
            };

            ViewData["StatusOptions"] = statusOptions;

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Code", studentFile.CourseId);
            return View(studentFile);
        }

        // GET: StudentFiles/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentFile = await _context.StudentFiles
                .Include(s => s.Course)
                .FirstOrDefaultAsync(m => m.StudId == id);
            if (studentFile == null)
            {
                return NotFound();
            }

            return View(studentFile);
        }

        // POST: StudentFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var studentFile = await _context.StudentFiles.FindAsync(id);
            if (studentFile != null)
            {
                _context.StudentFiles.Remove(studentFile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentFileExists(long id)
        {
            return _context.StudentFiles.Any(e => e.StudId == id);
        }
    }
}
