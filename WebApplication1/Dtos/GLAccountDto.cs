using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Dtos
{
    public class GLAccountDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte Id { get; set; }
        [Required]
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Main Account Category ")]
        public byte CategoriesId { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
    }
}