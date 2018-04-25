using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class CleanupEvent : BaseEntity
    {
        [Key]
        public int CleanupId {get;set;}
        public string Title{get;set;}
        public double Latitude{get;set;}
        public double Longitude{get;set;}
        public string DescriptionOfArea{get;set;}
        public string DescriptionOfTrash{get;set;}
        public int Value{get;set;}
        public bool Pending{get;set;}
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User {get;set;}
        public List<Image> Images{get;set;}
        public List<BoardMessage> BoardMessages {get;set;}
        [InverseProperty("CleanupEvent")]
        public List<User> CleaningUsers{get;set;}
        public CleanupEvent()
        {
            BoardMessages = new List<BoardMessage>();
            Images = new List<Image>();
            CleaningUsers = new List<User>();
        }
    }
}