using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Configurations
{
    public class UserImageConfiguration : EntityTypeConfiguration<UserImage>
    {
        public UserImageConfiguration()
        {
            ToTable("UserImages");
        }
    }
}