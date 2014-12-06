using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Account
{
    public class NewMessageViewModel
    {
        [Required(ErrorMessage = "A téma megadása kötelező")]
        [StringLength(50, ErrorMessage = "A téma maximum 50 karakter hosszú lehet.")]
        public String Subject { get; set; }

        [Required(ErrorMessage = "Az üzenet megadása kötelező")]
        public String Content { get; set; }

        public Int32 ToId { get; set; }

        public NewMessageViewModel()
        {

        }

        public NewMessageViewModel(Int32 p_id)
        {
            ToId = p_id;
        }

    }
}