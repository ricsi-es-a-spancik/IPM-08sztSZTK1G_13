using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class EmployeeConfiguration : EntityTypeConfiguration<Employee>
    {
        public EmployeeConfiguration()
        {
            ToTable("Employees");
            HasKey(e => new { e.UserId, e.OrganizationId });
            Property(e => e.UserId).HasColumnOrder(0).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(e => e.OrganizationId).HasColumnOrder(1).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}