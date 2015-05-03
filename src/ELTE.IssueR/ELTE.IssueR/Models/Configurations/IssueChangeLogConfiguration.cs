using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class IssueChangeLogConfiguration : EntityTypeConfiguration<IssueChangeLog>
    {
        public IssueChangeLogConfiguration()
        {
            ToTable("IssueChangeLogs");
            Property(i => i.OldStatus).IsOptional();
            Property(i => i.OldType).IsOptional();
            Property(i => i.ModifiedAt).IsRequired();
        }
    }
}