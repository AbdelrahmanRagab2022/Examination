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
    public class instructorController : Controller
    {
        private readonly ExamSystemContext _context;
        public instructor ins;

        public instructorController(ExamSystemContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnurl, string ins_Username, string ins_PW)
        {

            returnurl = returnurl ?? "/";
            ins = _context.instructors.FirstOrDefault(i => i.inst_name == ins_Username);
            if (ins == null || ins.inst_pw != ins_PW)
            {
                return RedirectToAction("Login");
            }
            Claim c1 = new Claim("ins_Username", ins_Username);
            Claim c2 = new Claim("ins_Pw", ins_PW);
            Claim c4 = new Claim("ins_Id", ins.inst_ID.ToString());
            Claim c3 = new Claim(ClaimTypes.Role, "Instructor");
            ClaimsIdentity identity = new ClaimsIdentity("Cookies");
            identity.AddClaim(c1);
            identity.AddClaim(c2);
            identity.AddClaim(c4);
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
