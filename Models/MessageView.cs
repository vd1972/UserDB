using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace userdb.Models
{
    public class UserMessages
    {
        public User user {get; set;}
        public List<Message> messages {get; set;}
        public UserMessages(int id, DBcontext _context)
        {
            messages = new List<Message>();
        }
    }
}