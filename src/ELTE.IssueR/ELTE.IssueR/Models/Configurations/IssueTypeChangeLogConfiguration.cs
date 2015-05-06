using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class IssueTypeChangeLogConfiguration : EntityTypeConfiguration<IssueTypeChangeLog>
    {
        public IssueTypeChangeLogConfiguration()
        {
            ToTable("IssueTypeChangeLogs");
            Property(i => i.OldValue).IsOptional();
            Property(i => i.NewValue).IsRequired();
            Property(i => i.ModifiedAt).IsRequired();
        }
    }
}