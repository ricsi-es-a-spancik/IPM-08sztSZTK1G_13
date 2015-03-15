namespace ELTE.IssueR.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Comment
    {
        public int IssueId { get; set; }

        public string UserName { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Érvénytelen hozzászólás!")]
        public string Text { get; set; }

        public DateTime SentAt { get; set; }

        public virtual Issue Issue { get; set; }

        public virtual User User { get; set; }
    }
}