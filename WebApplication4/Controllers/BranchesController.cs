using WebApplication4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Controllers
{
    public class BranchesController : Controller
    {
      
        private ApplicationDbContext _context;

        public BranchesController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Branches
        
        public ActionResult Index()
        {
            return View(new Branch());
        }

        [HttpPost]
        public ActionResult Create(Branch branch)
        {
            if (!ModelState.IsValid)
            {
                return View(branch);
            }
            //            var branchInDb = _context.Branches.LastOrDefault();
            //            var id = branchInDb.Id + 1;
            //            branch.Id = id;
            // branch.Id = 0;
            _context.Branches.Add(branch);
            _context.SaveChanges();
            return RedirectToAction("Index", "Branches");

        }
    
}
}