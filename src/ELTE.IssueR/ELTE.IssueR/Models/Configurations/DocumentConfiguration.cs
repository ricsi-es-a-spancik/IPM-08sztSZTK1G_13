using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class DocumentConfiguration : EntityTypeConfiguration<Document>
    {
        public DocumentConfiguration()
        {
            ToTable("Documents");
            Property(d => d.Name).IsRequired().HasMaxLength(50);
            Property(d => d.Content).IsRequired();
        }
    }
}