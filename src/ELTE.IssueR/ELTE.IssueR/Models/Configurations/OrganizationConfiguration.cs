using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class OrganizationConfiguration : EntityTypeConfiguration<Organization>
    {
        public OrganizationConfiguration()
        {
            ToTable("Organizations");
            
            Property(o => o.Name).IsRequired().HasMaxLength(50);
            Property(o => o.Country).HasMaxLength(50);
            Property(o => o.City).HasMaxLength(50);
            Property(o => o.Activity).HasMaxLength(100);

            HasMany(e => e.CoverImages)
                .WithRequired(e => e.Organization)
                .WillCascadeOnDelete(false);

            HasMany(e => e.Employees)
                .WithRequired(e => e.Organization)
                .WillCascadeOnDelete(false);

            HasMany(e => e.Projects)
                .WithRequired(e => e.Organization)
                .WillCascadeOnDelete(false);
        }
    }
}