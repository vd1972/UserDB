using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace userdb.Models
{
    public class Comment
    {
        [Key]
        public int comment_id {get; set;}
        public int message_id {get;set;}
        public string comment_description {get; set;}
        public string comment_added_by {get; set;}
        public DateTime created_at {get; set;}
        public DateTime updated_at {get; set;}
        public Message message {get; set;}
    }
}