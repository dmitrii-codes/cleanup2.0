using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class Image : BaseEntity
    {
        [Key]
        public int ImageId {get;set;}
        public string FileName{get;set;}
        [ForeignKey("CleanupEvent")]
        public int CleanupEventId{get;set;}
        public CleanupEvent CleanupEvent{get;set;}
    }
}