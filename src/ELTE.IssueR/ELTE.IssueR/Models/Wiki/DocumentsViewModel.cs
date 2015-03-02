using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Wiki
{
    public class DocumentsViewModel
    {
        [Required]
        public String Title { get; set; }

        [Required]
        public String Author { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Modified { get; set; }

        [Required]
        public String Content { get; set; }

    }
}