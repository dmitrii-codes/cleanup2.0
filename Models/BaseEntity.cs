using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cleanup.Models
{
    public abstract class BaseEntity {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt{get;set;}
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt{get;set;}
    }
}