using AutoMapper;

using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Dtos;

namespace WebApplication1.Controllers.Api
{
    public class BranchesController : ApiController
    {
        private ApplicationDbContext _context;

        public BranchesController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: /api/branches
        public IHttpActionResult GetBranches()
        {
            return Ok(_context.Branches.ToList());
        }
//        [AcceptVerbs("GET", "POST")]
//        [HttpPost]
//        [Route("api/Branches/CreateBranch")]
//        public IHttpActionResult CreateBranch(BranchDto branchDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest();
//            }
//
//            var branch = Mapper.Map<BranchDto, Branch>(branchDto);
//            _context.Branches.Add(branch);
//            _context.SaveChanges();
//            branchDto.Id = branch.Id;
//            return Created(new Uri(Request.RequestUri + "/" + branch.Id), branchDto);
//        }

        public class Index
        {
            public int Id { get; set; }
        }
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/Branches/Edit")]
        public HttpResponseMessage EditBranch(Index index)
        {
            var branches = _context.Branches.SingleOrDefault(c => c.Id == index.Id);
            var branchDto = new BranchDto()
            {
                Address = branches.Address,
                Name = branches.Name
            };

            return Request.CreateResponse(HttpStatusCode.OK, branchDto);
        }



        
        // PUT: /api/branches/1
       
        [AcceptVerbs("GET", "POST","PUT")]
        [HttpPut]
        [Route("api/Branches/Update")]
        public HttpResponseMessage UpdateBranch(BranchDto branchDto)
        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest();
//            }

            var branchInDb = _context.Branches.SingleOrDefault(b => b.Id == branchDto.Id);
            if (branchInDb == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,"No such branch exists");
            }

            branchInDb.Address = branchDto.Address;
            branchInDb.Name = branchDto.Name;
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, "Branch updated successfully") ;

        }
        //
        //        // DELETE: /api/branches/1
        //        [HttpDelete]
        //        public void DeleteBranch(int id)
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                throw new HttpResponseException(HttpStatusCode.BadRequest);
        //            }
        //
        //            var branchInDb = _context.Branches.SingleOrDefault(b => b.Id == id);
        //            if (branchInDb == null)
        //            {
        //                throw new HttpResponseException(HttpStatusCode.NotFound);
        //            }
        //
        //            _context.Branches.Remove(branchInDb);
        //
        //
        //
        //            _context.SaveChanges();
        //
        //        }


    }
}