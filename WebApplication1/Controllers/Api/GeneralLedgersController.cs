using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Dtos;
using AutoMapper;
using System.Data.Entity;

namespace WebApplication1.Controllers.Api
{
    public class GeneralLedgersController : ApiController
    {
        private ApplicationDbContext _context;

        public GeneralLedgersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: /api/generalledgers
        public IHttpActionResult GetCategories()
        {

            var categoriesDto = _context.GlCategories.Include(c => c.Categories).ToList();

            return Ok(categoriesDto);
            
        }

        [Route("api/GeneralLedgers/GetAccount")]
        public IHttpActionResult GetAccount()
        {
            var accountDto = _context.GlAccounts.Include(c => c.GlCategories).Include(b => b.Branch).ToList();

            return Ok(accountDto);
        }



    }
}