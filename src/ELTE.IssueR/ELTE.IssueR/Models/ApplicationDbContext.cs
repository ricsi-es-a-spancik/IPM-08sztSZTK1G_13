using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ELTE.IssueR.Models.Configurations;
using ELTE.IssueR.Migrations;

namespace ELTE.IssueR.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("IssueREntities", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>("IssueREntities"));
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        // IssueR stuff

        

        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<CoverImage> CoverImages { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<OrganizationDocument> OrganizationDocuments { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectMember> ProjectMembers { get; set; }
        public virtual DbSet<Issue> Issues { get; set; }
        public virtual DbSet<ProjectDocument> ProjectDocuments { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<UserImage> UserImages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rename ASP.NET Identity tables

            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");

            // Fluent configurations

            modelBuilder.Configurations.Add(new CoverImageConfiguration());
            modelBuilder.Configurations.Add(new EmployeeConfiguration());
            modelBuilder.Configurations.Add(new IssueConfiguration());
            modelBuilder.Configurations.Add(new MessageConfiguration());
            modelBuilder.Configurations.Add(new OrganizationConfiguration());
            modelBuilder.Configurations.Add(new OrganizationDocumentConfiguration());
            modelBuilder.Configurations.Add(new ProjectConfiguration());
            modelBuilder.Configurations.Add(new ProjectDocumentConfiguration());
            modelBuilder.Configurations.Add(new ProjectMemberConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new UserImageConfiguration());
        }
    }
}