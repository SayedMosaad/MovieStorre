using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MovieStorre.Models
{
    public class Genre
    {
        //to make the primary key 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
