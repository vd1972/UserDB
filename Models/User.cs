using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace userdb.Models
{
    public class User
    {
       [Key]
        public int user_id {get;set;}
        public string first_name {get; set;}
        public string last_name {get; set;}
        public string email {get; set;}
        public string password {get; set;}
        public int user_level {get; set;}
        public DateTime created_at {get; set;}
        public DateTime updated_at {get; set;}
        public string user_level_desc {
            get
            {
                return user_level == 1 ? "Admin" : "Normal";
            }
        }
        public bool is_admin {
            get
            {
                return user_level == 1 ? true : false;
            }
        }
        public List<Message> messages {get; set;}

    }
}