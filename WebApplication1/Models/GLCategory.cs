using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class GLCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Categories Categories { get; set; }
        [Display(Name = "Main Account Category")]
        public byte CategoriesId { get; set; }
        
        [Required]
        [StringLength(1000)]
        [Display(Name = "GL Category Name")]
        public string MainAccountCategory { get; set; }

        [Required]
        public string Description { get; set; }
    }
}