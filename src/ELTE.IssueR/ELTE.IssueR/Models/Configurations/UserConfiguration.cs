using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            ToTable("Users");
            Property(u => u.Name).IsRequired().HasMaxLength(50);
            Property(u => u.PasswordHash).IsFixedLength();

            HasMany(e => e.Employees)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            HasMany(e => e.Issues)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            HasMany(e => e.Messages)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.FromId)
                .WillCascadeOnDelete(false);

            HasMany(e => e.ProjectMembers)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            HasMany(e => e.UserImages)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
        }
    }
}