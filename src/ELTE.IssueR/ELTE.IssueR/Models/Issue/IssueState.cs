using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models
{
    public class IssueState
    {
        [Required(ErrorMessage = "A feladat leírásának megadása kötelező.")]
        [StringLength(30, ErrorMessage = "A feladat leírása maximum 200 karakter lehet.")]
        [RegularExpression("^[A-Za-z0-9_-]{3,30}$", ErrorMessage = "A leírás formátuma nem megfelelő.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A típus megadása kötelező")]
        public Nullable<TypeEnum> Type { get; set; }

        [DataType(DataType.Date)]
        public Nullable<System.DateTime> Deadline { get; set; }

        [Required(ErrorMessage = "A projektazonosító megadása kötelező")]
        public Nullable<int> ProjectId { get; set; }

        [Required(ErrorMessage = "A bázis megadása kötelező")]
        public Nullable<int> EpicId { get; set; }

        public Nullable<int> EmployeeId { get; set; }

        //public Nullable<int> EstimatedTime { get; set; }
        //public Nullable<int> TimeSpent { get; set; }

        //public List<Project> Projects { get; set; }

        public List<Epic> Epics { get; set; }

        public List<Employee> Employees { get; set; }

        public enum TypeEnum { Feature=1, Bug=2, Refactor=3}
    }
}