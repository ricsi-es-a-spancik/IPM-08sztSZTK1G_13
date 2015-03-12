using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Account
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Teljes név")]
        public string Name { get; set; }

        public string PictureUrl { get; set; }
    }
}