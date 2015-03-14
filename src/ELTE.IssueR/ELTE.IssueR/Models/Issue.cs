namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public enum IssueStatus : short 
    { 
        To_do, 
        In_progress, 
        Needs_review, 
        Done 
    }

    public enum IssueType : short
    { 
        Feature, 
        Bug, 
        Refactor 
    }

    public partial class Issue
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A feladat megnevezésének megadása kötelezõ!")]
        [StringLength(30, ErrorMessage = "A feladat leírása maximum 30 karakter lehet!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A típus megadása kötelezõ!")]
        public IssueType Type { get; set; }

        [Required(ErrorMessage = "Az állapot megadása kötelezõ!")]
        public IssueStatus Status { get; set; }

        public string Description { get; set; }

        public DateTime? Deadline { get; set; }

        [Required(ErrorMessage = "A foglalkoztatott megadása kötelezõ!")]
        public string UserId { get; set; }

        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public virtual User User { get; set; }
    }
}
