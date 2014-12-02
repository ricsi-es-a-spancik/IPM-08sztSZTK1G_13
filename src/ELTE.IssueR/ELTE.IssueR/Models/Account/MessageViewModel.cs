using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Account
{
    public class MessageViewModel
    {
        public List<Message> Messages;

        public MessageViewModel()
        {
            Messages = new List<Message>();
        }
    }
}