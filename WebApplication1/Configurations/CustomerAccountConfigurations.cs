using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Configurations
{
    public class CustomerAccountConfigurations : EntityTypeConfiguration<CustomerAccount>
    {
        public CustomerAccountConfigurations()
        {
            HasKey(a => a.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);



        }
    }

}
