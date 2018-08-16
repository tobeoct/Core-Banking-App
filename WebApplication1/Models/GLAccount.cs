using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class GLAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "GL Account Name")]
        public string Name { get; set; }

        public string Code { get; set; }

        public Branch Branch { get; set; }

        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        public Categories Categories { get; set; }

        [Display(Name = "Main Account Category ")]
        public byte CategoriesId { get; set; }
    }
}