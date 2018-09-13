using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Dtos
{
    public class RegisterDto
    {

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public int BranchId { get; set; }

        public string RoleName { get; set; }

        public string Username { get; set; }

    }
}