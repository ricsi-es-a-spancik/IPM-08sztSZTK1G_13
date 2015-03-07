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
        public Nullable<TypeEnum> Type { get; set; }

        public StatusEnum Status { get; set; }

        [DataType(DataType.Date)]
        public Nullable<System.DateTime> Deadline { get; set; }

        [Required(ErrorMessage = "A projektazonosító megadása kötelező")]
        public Nullable<int> ProjectId { get; set; }

        [Required(ErrorMessage = "Foglalkoztatott megadása kötelező")]
        public Nullable<int> UserId { get; set; }

        public List<User> Users { get; set; }

        public enum StatusEnum { ToDo=1, InProgress=2, NeedsReview=3, Done=4 }

        public enum TypeEnum { Feature=1, Bug=2, Refactor=3}

        public List<TypeEnum> TypeEnumList 
        {
            get { return Enum.GetValues(typeof(TypeEnum)).Cast<TypeEnum>().ToList(); }
            private set { } 
        }

        public List<StatusEnum> StatusEnumList
        {
            get { return Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>().ToList(); }
            private set { }
        }

        public IssueViewModel()
        {
            Type = TypeEnum.Feature;
        }

        public IssueViewModel(Issue issue)
        {
            Id = issue.Id;
            Name = issue.Name;
            Type = (TypeEnum)issue.Type;
            Status = (StatusEnum)issue.Status;
            ProjectId = issue.ProjectId;
            UserId = issue.UserId;
            Deadline = issue.Deadline;
        }
    }
}