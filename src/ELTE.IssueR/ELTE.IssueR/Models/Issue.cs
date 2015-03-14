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

        [Required(ErrorMessage = "A feladat megnevez�s�nek megad�sa k�telez�!")]
        [StringLength(30, ErrorMessage = "A feladat le�r�sa maximum 30 karakter lehet!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A t�pus megad�sa k�telez�!")]
        public IssueType Type { get; set; }

        [Required(ErrorMessage = "Az �llapot megad�sa k�telez�!")]
        public IssueStatus Status { get; set; }

        public string Description { get; set; }

        public DateTime? Deadline { get; set; }

        [Required(ErrorMessage = "A foglalkoztatott megad�sa k�telez�!")]
        public string UserId { get; set; }

        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public virtual User User { get; set; }
    }
}
