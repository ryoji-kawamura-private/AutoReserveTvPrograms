using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace iEPG.Entity
{
    public class SearchCondition
    {
        [Key]
        public Guid Id { get; set; }
    }
}
