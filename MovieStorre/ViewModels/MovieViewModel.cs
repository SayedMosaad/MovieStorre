using MovieStorre.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStorre.ViewModels
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [Range(1,10)]
        public double Rate { get; set; }

        //string length used in viewmodel and maxlength used in models
        [Required, StringLength(2500)]
        public string StoryLine { get; set; }

        [Display(Name = "Select Poster ...")]
        public byte[] Poster { get; set; }

        [Display(Name ="Genre")]
        public byte GenreId { get; set; }
        public IEnumerable<Genre> Genres { get; set; }

    }
}
