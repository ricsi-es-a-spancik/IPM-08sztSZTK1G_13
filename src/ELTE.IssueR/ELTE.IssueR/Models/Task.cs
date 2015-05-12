namespace ELTE.IssueR.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class Task
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public string Resource { get; set; }

        public int DependentTaskId { get; set; }
    }
}