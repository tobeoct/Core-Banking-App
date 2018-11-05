using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api
{
    public class OnUsController : ApiController
    {
        private ApplicationDbContext _context;
        private string errorMessage = "";
        public OnUsController()
        {
            _context = new ApplicationDbContext();
        }
        // GET api/<controller>
        // POST api/OnUs/CreateAtmTerminal
        [AcceptVerbs("GET", "POST")]
        [HttpGet]
        [Route("api/OnUs/GetATMTerminals")]
        public HttpResponseMessage GetATMTerminals()
        {
            var terminals = _context.ATMTerminals.ToList();
            return Request.CreateResponse(HttpStatusCode.OK, terminals);
        }

        // POST api/OnUs/CreateAtmTerminal
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/OnUs/CreateAtmTerminal")]
        public HttpResponseMessage CreateAtmTerminal(ATMTerminal aTMTerminal)
        {
           
            if (ValidateEntry(aTMTerminal) == true)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }

            var ATMTerminal = new ATMTerminal();
            ATMTerminal = aTMTerminal;


            _context.ATMTerminals.Add(ATMTerminal);
            _context.SaveChanges();


            var message = "ATM Terminal Added Successfully";
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [NonAction]
        public bool ValidateEntry(ATMTerminal aTMTerminal)
        {

            bool error = false;

            if (aTMTerminal.Name == null)
            {
                errorMessage = errorMessage + "Please Enter Account Name. ";
                error = true;

            }
            if (aTMTerminal.Location == null)
            {
                errorMessage = errorMessage + "Please enter a Location.";
                error = true;
            }
            if (aTMTerminal.TerminalID == null)
            {
                errorMessage = errorMessage + "Please enter a Terminal ID.";
                error = true;
            }
           
            return error;

        }
        // POST api/OnUs/EditAtmTerminal
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/OnUs/EditAtmTerminal")]
        public HttpResponseMessage EditAtmTerminal(Indexes index)
        {
            //  var id = index.Id.ToString().PadLeft(9, '0');
            var id = index.Id;
            var atmTerminalInDb = _context.ATMTerminals.SingleOrDefault(c => c.Id.Equals(id));
          
            if (atmTerminalInDb == null)
            {
            
                const string errorMsg = "No such ATM Terminal Exists";
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMsg);
            }

            return Request.CreateResponse(HttpStatusCode.OK, atmTerminalInDb);
        }
        // POST api/OnUs/UpdateAtmTerminal
        [AcceptVerbs("GET", "POST", "PUT")]
        [HttpPut]
        [Route("api/OnUs/UpdateAtmTerminal")]
        public HttpResponseMessage UpdateAtmTerminal(ATMTerminal aTMTerminal)
        {
            //  var id = index.Id.ToString().PadLeft(9, '0');
            var id = aTMTerminal.Id;
            var atmTerminalInDb = _context.ATMTerminals.SingleOrDefault(c => c.Id == id);
            if (atmTerminalInDb != null)
            {
                              
                atmTerminalInDb.Name = aTMTerminal.Name;
                atmTerminalInDb.TerminalID = aTMTerminal.TerminalID;
                atmTerminalInDb.Location = aTMTerminal.Location;
            }
            else
            {
                string errorMsg = "No such ATM Terminal Exists : " + id;
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, errorMsg);
            }

            _context.SaveChanges();

            const string message = "ATM Terminal Info Updated Successfully";
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}