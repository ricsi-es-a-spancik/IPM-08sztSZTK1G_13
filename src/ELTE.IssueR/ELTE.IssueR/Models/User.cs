using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class User : IdentityUser
    {
        public User()
            : base()
        {
            Employees = new HashSet<Employee>();
            Issues = new HashSet<Issue>();
            Messages = new HashSet<Message>();
            ProjectMembers = new HashSet<ProjectMember>();
            UserImages = new HashSet<UserImage>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        // IssueR fields
        public string Name { get; set; }

        public DateTime? RegisterDate { get; set; }

        public DateTime? LastLogin { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public virtual ICollection<Issue> Issues { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<ProjectMember> ProjectMembers { get; set; }

        public virtual ICollection<UserImage> UserImages { get; set; }
    }
}