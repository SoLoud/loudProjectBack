using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SoLoud.Models
{
    public class SoLoudContext : IdentityDbContext<ApplicationUser>
    {
        private string schema = "SoLoud";
        private static string defaultConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        public SoLoudContext()
            : base(defaultConnectionString, throwIfV1Schema: false)
        {
            this.Database.CommandTimeout = 360;
        }
        public static SoLoudContext Create()
        {
            return new SoLoudContext();
        }

        #region ModelCreation not needed so frequently!

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Add(new OneToManyCascadeDeleteConvention());

            //// Configure Asp Net Identity Tables
            //modelBuilder.Entity<User>().ToTable("User");
            //modelBuilder.Entity<User>().Property(u => u.PasswordHash).HasMaxLength(500);
            //modelBuilder.Entity<User>().Property(u => u.Stamp).HasMaxLength(500);
            //modelBuilder.Entity<User>().Property(u => u.PhoneNumber).HasMaxLength(50);

            //modelBuilder.Entity<Role>().ToTable("Role");
            //modelBuilder.Entity<UserRole>().ToTable("UserRole");
            //modelBuilder.Entity<UserLogin>().ToTable("UserLogin");
            //modelBuilder.Entity<UserClaim>().ToTable("UserClaim");
            //modelBuilder.Entity<UserClaim>().Property(u => u.ClaimType).HasMaxLength(150);
            //modelBuilder.Entity<UserClaim>().Property(u => u.ClaimValue).HasMaxLength(500);
        }

        #endregion

        #region DataSets
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Contest> Contests { get; set; }

        #endregion

    }
}
