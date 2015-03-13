using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class CoverImageConfiguration : EntityTypeConfiguration<CoverImage>
    {
        public CoverImageConfiguration()
        {
            ToTable("CoverImages");
            Property(ci => ci.Image).IsRequired();
        }
    }
}