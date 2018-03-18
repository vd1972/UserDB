using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace userdb.Models
{
    public class Message
    {
        [Key]
        public int message_id {get; set;}
        public int user_id {get;set;}
        public string message_added_by{get; set;}
        public string message_description {get; set;}
        public DateTime created_at {get; set;}
        public DateTime updated_at {get; set;}
        public User user {get; set;}
        public List<Comment> comments {get; set;}
    }
}