using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Dtos
{
    public class BranchDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime DateCreated { get; set; }
    }
}