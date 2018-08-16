using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class EditProfileViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<Branch> Branches { get; set; }
        public string Password { get; set; }
    }
}