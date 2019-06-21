using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models
{
    public class Activity
    {
        [Key]
        public int ActivityId {get;set;}
        [Required]
        [Display(Name="Title")]
        public string Name {get;set;}
        [Required]
        [FutureDate]
        public DateTime Date {get;set;}
        [Required]
        public DateTime Time {get;set;}
        [Required]
        public int Duration {get;set;}
        [Required]
        public string DurationUnit {get;set;}
        [Required]
        public string Description {get;set;}
        [Required]
        public int UserId {get;set;}

        public User Coordinator {get;set;}
        public List<UserActivity> Participants {get;set;}
    }
}