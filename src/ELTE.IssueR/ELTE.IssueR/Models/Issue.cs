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

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "A feladat megnevezésének megadása kötelezõ!")]
        [StringLength(30, ErrorMessage = "A feladat leírása maximum 30 karakter lehet!")]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "A típus megadása kötelezõ!")]
        public IssueType Type { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Az állapot megadása kötelezõ!")]
        public IssueStatus Status { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "A foglalkoztatott megadása kötelezõ!")]
        public string UserId { get; set; }

        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
