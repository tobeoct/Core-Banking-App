using AutoMapper;

using WebApplication4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication4.Dtos;

namespace WebApplication4.Controllers.Api
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


        // POST: api/<controller>
        [HttpPost]
        public IHttpActionResult CreateBranch(BranchDto branchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var branch = Mapper.Map<BranchDto, Branch>(branchDto);
            _context.Branches.Add(branch);
            _context.SaveChanges();
            branchDto.Id = branch.Id;
            return Created(new Uri(Request.RequestUri + "/" + branch.Id), branchDto);
        }

        // PUT: /api/branches/1
        [HttpPut]
        public IHttpActionResult UpdateBranch(int id, BranchDto branchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var branchInDb = _context.Branches.SingleOrDefault(b => b.Id == id);
            if (branchInDb == null)
            {
                return NotFound();
            }

            Mapper.Map(branchDto, branchInDb);
            _context.SaveChanges();
            return Ok();

        }

        // DELETE: /api/branches/1
        [HttpDelete]
        public void DeleteBranch(int id)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var branchInDb = _context.Branches.SingleOrDefault(b => b.Id == id);
            if (branchInDb == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _context.Branches.Remove(branchInDb);



            _context.SaveChanges();

        }


    }
}