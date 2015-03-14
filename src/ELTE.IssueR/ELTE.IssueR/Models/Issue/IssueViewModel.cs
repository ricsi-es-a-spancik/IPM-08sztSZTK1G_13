using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class IssueViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "A feladat leírásának megadása kötelező.")]
        [StringLength(20, ErrorMessage = "A feladat leírása maximum 20 karakter lehet.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A típus megadása kötelező")]
        public Nullable<IssueType> Type { get; set; }

        public IssueStatus Status { get; set; }

        [DataType(DataType.Date)]
        public Nullable<System.DateTime> Deadline { get; set; }

        [Required(ErrorMessage = "A projektazonosító megadása kötelező")]
        public int? ProjectId { get; set; }

        [Required(ErrorMessage = "Foglalkoztatott megadása kötelező")]
        public string UserId { get; set; }

        public List<User> Users { get; set; }


        public IssueViewModel()
        {
            Type = IssueType.Feature;
        }

        public IssueViewModel(Issue issue)
        {
            Id = issue.Id;
            Name = issue.Name;
            Type = issue.Type;
            Status = issue.Status;
            ProjectId = issue.ProjectId;
            UserId = issue.UserId;
            Deadline = issue.Deadline;
        }
    }
}