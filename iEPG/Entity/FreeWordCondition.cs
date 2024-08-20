using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iEPG.Entity
{
    public class FreeWordConditon
    {
        [Key]
        public Guid Id { get; set; }
        public string FreeWord { get; set; }
    }
}
