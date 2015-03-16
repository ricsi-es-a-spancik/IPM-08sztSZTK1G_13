using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class IssueConfiguration : EntityTypeConfiguration<Issue>
    {
        public IssueConfiguration()
        {
            ToTable("Issues");
            Property(i => i.Name).IsRequired().HasMaxLength(30);

            HasMany(i => i.Comments)
                .WithRequired(i => i.Issue)
                .WillCascadeOnDelete(false);
        }
    }
}