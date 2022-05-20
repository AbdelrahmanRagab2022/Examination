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
using System.Dynamic;
using System.Collections;

namespace Examination.Controllers
{
    [Authorize]
    public class studentController : Controller
    {
        private readonly ExamSystemContext _context;
         public student std;
   
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
            std = _context.students.FirstOrDefault(i=>i.stud_Username== stud_Username);
            if(std == null || std.stud_pw!=stud_PW)
            {
                return RedirectToAction("Login");
            }
            Claim c1 = new Claim("stud_Username", stud_Username);
            Claim c2 = new Claim("stud_Pw", stud_PW);
            Claim c4 = new Claim("stud_Id", std.stud_ID.ToString());
            Claim c3 = new Claim(ClaimTypes.Role, "Student");
            ClaimsIdentity identity = new ClaimsIdentity("Cookies");
            identity.AddClaim(c1);
            identity.AddClaim(c2);
            identity.AddClaim(c4);
            identity.AddClaim(c3);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("Cookies", claimsPrincipal);
            return RedirectToAction("stud_details");
        }
        public IActionResult stud_details()
        {
           
            int st_id= int.Parse(HttpContext.User.Claims.FirstOrDefault(a=>a.Type=="stud_Id").Value);
            dynamic model = new ExpandoObject();
            
            model.student = _context.students.Include(i=>i.dept).FirstOrDefault(i => i.stud_ID == st_id);
           

            //var x= _context.students.FirstOrDefault(
            //    a => a.dept_ID == std.dept_ID);
            //model.departments= _context.departments.FirstOrDefault(
            //    a => a.dept_ID == department.dept_ID);

            model.courses = _context.courses.ToList();
            var crs = _context.courses.ToList();
            return View(model);

        }
        public IActionResult stud_details2(student stud)
        {

           //student std = _context.students.FirstOrDefault(i => i.stud_Username == studentName);
           // if (std == null || std.stud_pw != studentpassword)
           // {
           //     return RedirectToAction("Login");
           // }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        // New part
        //Get : student/StartExam

        public async Task<IActionResult> StartExam()
        {
            
            int StdId = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Stud_Id").Value);

            // int ExamID = await _context.Procedures.generateExam2Async("C#", 3, 7);

            int ExamID = await _context.Procedures.generateExamAsync("C#", StdId, 3, 7);
            await _context.Procedures.AssignExamStudentAsync(ExamID, StdId);
            List<question> eq = _context.exams_questions.Include(i => i.q_IDNavigation).ThenInclude(i => i.choices).Where(i => i.exam_ID == ExamID).Select(i => i.q_IDNavigation).ToList();
            int cnt = 0;

            Dictionary<int, ArrayList> Qs = new Dictionary<int, ArrayList>();

            // Dictionary<int, List<choice>> Dic= new Dictionary<int, List<choice>>();

            foreach (var question in eq)
            {
                cnt++;
                ArrayList QStr = new ArrayList();
                //List<choice> ListChoice = new List<choice>();
                QStr.Add(question.q_desc);
                foreach (var ch in question.choices)
                {
                    QStr.Add(ch);
                }
                Qs.Add(cnt, QStr);


            }

            ViewBag.Qs = Qs;
            ViewBag.ExamID = ExamID;
            return View();
        }

        // Post student / EndExam
        [HttpPost]
        public async Task<IActionResult> EndExam(string ExamId, string q1, string q2, string q3, string q4, string q5, string q6, string q7, string q8, string q9, string q10)
        {
            
            int StdId = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Stud_Id").Value);
            await _context.Procedures.examAnsAsync(int.Parse(ExamId), StdId, int.Parse(q1), int.Parse(q2), int.Parse(q3), int.Parse(q4), int.Parse(q5)
                 , int.Parse(q6), int.Parse(q7), int.Parse(q8), int.Parse(q9), int.Parse(q10));

            await _context.Procedures.correctExamAsync(int.Parse(ExamId), StdId);
            _context
            return View();
        }

     
        



    }
}
