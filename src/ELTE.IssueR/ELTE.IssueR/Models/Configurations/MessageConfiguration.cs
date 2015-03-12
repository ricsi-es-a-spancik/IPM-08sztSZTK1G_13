using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class MessageConfiguration : EntityTypeConfiguration<Message>
    {
        public MessageConfiguration()
        {
            ToTable("Messages");
            Property(m => m.Subject).IsRequired().HasMaxLength(50);
            Property(m => m.Content).IsRequired();
        }
    }
}