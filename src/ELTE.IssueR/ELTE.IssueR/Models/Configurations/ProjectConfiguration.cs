using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class ProjectConfiguration : EntityTypeConfiguration<Project>
    {
        public ProjectConfiguration()
        {
            ToTable("Projects");
            Property(p => p.Name).IsRequired().HasMaxLength(50);

            HasMany(e => e.Issues)
                .WithRequired(e => e.Project)
                .WillCascadeOnDelete(false);

            HasMany(e => e.ProjectMembers)
                .WithRequired(e => e.Project)
                .WillCascadeOnDelete(false);

            HasMany(e => e.Documents)
                .WithRequired(e => e.Project)
                .WillCascadeOnDelete(false);
        }
    }
}