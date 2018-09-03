using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Dtos
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}