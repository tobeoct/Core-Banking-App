using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api
{
    public class TSSController : ApiController
    {
        private ApplicationDbContext _context;
        public TSSController()
        {
            _context = new ApplicationDbContext();
        }
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/RemoteOnUs/UpdateAccount
        [AcceptVerbs("GET", "POST", "PUT")]
        [HttpPost]
        [Route("api/TSS/UpdateGLAccount")]
        public HttpResponseMessage UpdateGLAccount(Indexes index)
        {
            //  var id = index.Id.ToString().PadLeft(9, '0');
            var id = index.Id;
            var accountInDb = _context.TSSAccountConfig.FirstOrDefault();
            if (accountInDb != null)
            {

                accountInDb.GLAccountId = id;

            }
            else
            {
                var config = new TSSAccount();
                config.GLAccountId = id;

                _context.TSSAccountConfig.Add(config);
            }

            _context.SaveChanges();

            const string message = "TSS GL Account Updated Successfully";
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }


        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}