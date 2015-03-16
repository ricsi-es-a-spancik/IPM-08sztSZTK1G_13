using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class ProjectDocumentConfiguration : EntityTypeConfiguration<ProjectDocument>
    {
        public ProjectDocumentConfiguration()
        {
            ToTable("ProjectDocuments");
            Property(d => d.Name).IsRequired().HasMaxLength(50);
            Property(d => d.Content).IsRequired();
        }
    }
}