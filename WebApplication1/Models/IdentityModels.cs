using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplication1.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        public Branch Branch { get; set; }
        public Guid ActivationCode { get; set; }

        public bool IsActive { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {

        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<GLCategory> GlCategories { get; set; }
        public DbSet<GLAccount> GlAccounts { get; set; }
        public DbSet<GLPostings> GlPostings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<LoanDetails> LoanDetails { get; set; }
//        public DbSet<SavingsAccountType> SavingsAccountTypes { get; set; }
//        public DbSet<CurrentAccountType> CurrentAccountTypes { get; set; }
//        public DbSet<LoanAccountType> LoanAccountTypes { get; set; }
//        public DbSet<CustomerAccountType> CustomerAccountTypes { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Terms> Terms { get; set; }
        public DbSet<Teller> Tellers { get; set; }
        public DbSet<TellerPosting> TellerPostings { get; set; }
        public DbSet<FinancialReport> FinancialReports { get; set; }
        public DbSet<BusinessStatus> BusinessStatus { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}