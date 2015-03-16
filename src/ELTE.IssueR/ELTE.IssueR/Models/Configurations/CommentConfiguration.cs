using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class CommentConfiguration : EntityTypeConfiguration<Comment>
    {
        public CommentConfiguration()
        {
            ToTable("Comments");
            HasKey(c => new { c.UserName, c.IssueId, c.SentAt });
            Property(c => c.IssueId).HasColumnOrder(0).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(c => c.UserName).HasColumnOrder(1).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(c => c.SentAt).HasColumnOrder(2).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}