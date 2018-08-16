using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Dtos
{
    public class UserAccountDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(1000)]
        public string Name { get; set; }
        public int BranchId { get; set; }
        
        public string Email { get; set; }
        
        [StringLength(1000)]
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        
        [StringLength(1000)]
        public string Password { get; set; }
    }
}