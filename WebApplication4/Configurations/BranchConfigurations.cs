using WebApplication4.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace WebApplication4.Configurations
{
    public class BranchConfigurations : EntityTypeConfiguration<Branch>
    {
        public BranchConfigurations()
        {
            HasKey(a => a.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(a => a.Name).IsRequired().HasMaxLength(10000);

        }
    
    }
}