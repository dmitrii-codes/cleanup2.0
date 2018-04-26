using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class Live : BaseEntity
    {
        public int LiveId {get;set;}
        public string Messages{get;set;}
        
    }
    
}