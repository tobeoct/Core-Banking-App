using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Dtos
{
    public class GLCategoryDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public byte CategoriesId { get; set; }
        [Required]
        [StringLength(255)]
        public string MainAccountCategory { get; set; }
        public string Description { get; set; }
    }
}