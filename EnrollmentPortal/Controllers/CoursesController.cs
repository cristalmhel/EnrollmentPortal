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
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5; // Number of items per page

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index(string? searchString, int? pageNumber)
        {
            // Store the current filter in ViewData to preserve it across requests
            ViewData["CurrentFilter"] = searchString;

            // Fetch the total count of courses to calculate the total number of pages
            var courses = _context.Courses.AsQueryable().AsNoTracking();

            // If the searchString is not empty or null, filter the courses
            if (!String.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(c => c.Name.Contains(searchString)
                                          || c.Code.Contains(searchString)
                                          || c.Description.Contains(searchString));
            }

            // Use the PageSize and PageNumber to get paginated data
            int currentPageNumber = pageNumber ?? 1;
            var paginatedCourses = await PaginatedList<Course>.CreateAsync(courses, currentPageNumber, PageSize);

            return View(paginatedCourses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Name,Description")] Course course)
        {
            // Check if a course with the same Code already exists
            if (_context.Courses.Any(c => c.Code == course.Code))
            {
                ModelState.AddModelError("Code", "A course with this code already exists.");
            }

            // Proceed only if the model state is valid and no validation errors have occurred
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If we reached here, something went wrong, so re-display the form with the error message
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Name,Description")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            /*// Check if a course with the same Code already exists
            if (_context.Courses.Any(c => c.Code == course.Code))
            {
                ModelState.AddModelError("Code", "A course with this code already exists.");
            }*/

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                    /*TempData["SuccessCourseMessage"] = "Course saved successfully!";*/
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
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
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
