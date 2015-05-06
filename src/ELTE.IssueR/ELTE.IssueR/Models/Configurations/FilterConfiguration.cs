using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class FilterConfiguration : EntityTypeConfiguration<Filter>
    {
        public FilterConfiguration()
        {
            ToTable("Filters");
            Property(f => f.Name).IsRequired().HasMaxLength(15);
            Ignore(f => f.IsActive);
        }
    }
}