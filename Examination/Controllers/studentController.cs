using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Examination.Data;
using Examination.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Examination.Controllers
{
    [Authorize]
    public class studentController : Controller
    {
        private readonly ExamSystemContext _context;

        public studentController(ExamSystemContext context)
        {
            _context = context;
        }

        // GET: student
        public async Task<IActionResult> Index()
        {
            var examSystemContext = _context.students.Include(s => s.dept);
            return View(await examSystemContext.ToListAsync());
        }

        // GET: student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.User.IsInRole("Student"))
            {
                return NotFound();
            }
            if (id == null || _context.students == null)
            {
                return NotFound();
            }

            var student = await _context.students
                .Include(s => s.dept)
                .FirstOrDefaultAsync(m => m.stud_ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: student/Create
        public IActionResult Create()
        {
            if (HttpContext.User.IsInRole("Student"))
            {
                return NotFound();
            }
            ViewData["dept_ID"] = new SelectList(_context.departments, "dept_ID", "dept_name");
            return View();
        }

        // POST: student/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("stud_ID,stud_Fname,stud_Lname,dept_ID,stud_Username,stud_pw")] student student)
        {
            if(HttpContext.User.IsInRole("Student"))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["dept_ID"] = new SelectList(_context.departments, "dept_ID", "dept_name", student.dept_ID);
            return View(student);
        }

        // GET: student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.User.IsInRole("Student"))
            {
                return NotFound();
            }
            if (id == null || _context.students == null)
            {
                return NotFound();
            }

            var student = await _context.students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["dept_ID"] = new SelectList(_context.departments, "dept_ID", "dept_name", student.dept_ID);
            return View(student);
        }

        // POST: student/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("stud_ID,stud_Fname,stud_Lname,dept_ID,stud_Username,stud_pw")] student student)
        {
            if (id != student.stud_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!studentExists(student.stud_ID))
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
            ViewData["dept_ID"] = new SelectList(_context.departments, "dept_ID", "dept_name", student.dept_ID);
            return View(student);
        }

        // GET: student/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.User.IsInRole("Student"))
            {
                return NotFound();
            }
            if (id == null || _context.students == null)
            {
                return NotFound();
            }

            var student = await _context.students
                .Include(s => s.dept)
                .FirstOrDefaultAsync(m => m.stud_ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.students == null)
            {
                return Problem("Entity set 'ExamSystemContext.students'  is null.");
            }
            var student = await _context.students.FindAsync(id);
            if (student != null)
            {
                _context.students.Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool studentExists(int id)
        {
          return (_context.students?.Any(e => e.stud_ID == id)).GetValueOrDefault();
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return  View();
        }
        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Login(string returnurl, string stud_Username, string stud_PW)
        {
            returnurl = returnurl ?? "/";
            student std = _context.students.FirstOrDefault(i=>i.stud_Username== stud_Username);
            if(std == null || std.stud_pw!=stud_PW)
            {
                return RedirectToAction("Login");
            }
            Claim c1 = new Claim("stud_Username", stud_Username);
            Claim c2 = new Claim("stud_Pw", stud_PW);
            Claim c3 = new Claim(ClaimTypes.Role, "Student");
            ClaimsIdentity identity = new ClaimsIdentity("Cookies");
            identity.AddClaim(c1);
             identity.AddClaim(c2);
            identity.AddClaim(c3);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("Cookies", claimsPrincipal);
            return Redirect(returnurl);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }


    }
}
