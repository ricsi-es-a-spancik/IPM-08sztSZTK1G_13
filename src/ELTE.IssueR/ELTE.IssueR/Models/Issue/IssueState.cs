using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class IssueState
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "A feladat leírásának megadása kötelező.")]
        [StringLength(200, ErrorMessage = "A feladat leírása maximum 200 karakter lehet.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A típus megadása kötelező")]
        public Nullable<TypeEnum> Type { get; set; }

        public StatusEnum Status { get; set; }

        [DataType(DataType.Date)]
        public Nullable<System.DateTime> Deadline { get; set; }

        [Required(ErrorMessage = "A projektazonosító megadása kötelező")]
        public Nullable<int> ProjectId { get; set; }

        [Required(ErrorMessage = "A bázis megadása kötelező")]
        public Nullable<int> EpicId { get; set; }

        //[Required(ErrorMessage = "Foglalkoztatott megadása kötelező")]
        public Nullable<int> EmployeeId { get; set; }

        public List<Epic> Epics { get; set; }

        public List<Employee> Employees { get; set; }

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

        public IssueState()
        {
            Type = TypeEnum.Feature;
        }

        public IssueState(Issue issue)
        {
            Id = issue.Id;
            Name = issue.Name;
            Type = (TypeEnum)issue.Type;
            Status = (StatusEnum)issue.Status;
            ProjectId = issue.Epic.ProjectId;
            EpicId = issue.EpicId;
            EmployeeId = issue.EmployeeId;
            Deadline = issue.Deadline;
        }
    }
}