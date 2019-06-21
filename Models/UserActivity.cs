using System;
using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models
{
    public class UserActivity
    {
        [Key]
        public int UserActivityId {get;set;}
        public int ActivityId {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
        public Activity Activity {get;set;}
    }
}