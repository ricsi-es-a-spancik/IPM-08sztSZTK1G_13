using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class ProjectMemberConfiguration : EntityTypeConfiguration<ProjectMember>
    {
        public ProjectMemberConfiguration()
        {
            ToTable("ProjectMembers");
            HasKey(pm => new { pm.UserId, pm.ProjectId });
            Property(pm => pm.UserId).HasColumnOrder(0).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(pm => pm.ProjectId).HasColumnOrder(1).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}