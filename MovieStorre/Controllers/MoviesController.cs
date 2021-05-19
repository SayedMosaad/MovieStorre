using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieStorre.Models;
using MovieStorre.ViewModels;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStorre.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IToastNotification _ToastNotification;
        public MoviesController(ApplicationDBContext context, IToastNotification toastNotification)
        {
            _context = context;
            _ToastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            var Movies = await _context.Movies.OrderByDescending(x=>x.Rate).ToListAsync();
            return View(Movies);
        }

        public async Task<IActionResult> Create()
        {
            var model = new MovieViewModel
            {
                Genres=await _context.Genres.OrderBy(m=>m.Name).ToListAsync()
            };
            return View("MovieForm",model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(MovieViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm", model);
            }
            var files = Request.Form.Files;
            if(!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please select movie poster!");
                return View("MovieForm", model);
            }
 
            var poster = files["Poster"];
            var allowedextensions = new List<string> { ".jpg", ".png" };
            if(!allowedextensions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "only jpg and png are allowed");
                return View("MovieForm", model);
            }
            if(poster.Length>1048576)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "poster cannot be more than 1 megabyte");
                return View("MovieForm", model);
            }
            using var datastream = new MemoryStream();
            await poster.CopyToAsync(datastream);

            var movie = new Movie
            {
                Title=model.Title,
                GenreId=model.GenreId,
                Year=model.Year,
                Rate=model.Rate,
                StoryLine=model.StoryLine,
                Poster=datastream.ToArray()
            };
            _context.Movies.Add(movie);
            _context.SaveChanges();

            _ToastNotification.AddSuccessToastMessage("Movie has been added successfuly");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return BadRequest();
            }
            var movie = await _context.Movies.FirstOrDefaultAsync(x => x.ID == id);
            if(movie==null)
            {
                return NotFound();
            }
            var model = new MovieViewModel
            {
                Id=movie.ID,
                Title=movie.Title,
                GenreId=movie.GenreId,
                Year=movie.Year,
                Rate=movie.Rate,
                StoryLine=movie.StoryLine,
                Poster=movie.Poster,
                Genres =await _context.Genres.OrderBy(x=>x.Name).ToListAsync()
            };

            return View("MovieForm", model);

        }


        [HttpPost]
        public async Task<IActionResult> Edit(MovieViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm", model);
            }
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.ID == model.Id);
            
            if(movie==null)
            {
                return NotFound();
            }

            // check if the end user change the poster file or not
            var files = Request.Form.Files;
            if(files.Any())
            {
                // if the client change the poster 
                var poster = files.FirstOrDefault();
                using var datastream = new MemoryStream();
                await poster.CopyToAsync(datastream);

                model.Poster = datastream.ToArray();
                var allowedextensions = new List<string> { ".jpg", ".png" };
                if (!allowedextensions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "only jpg and png are allowed");
                    return View("MovieForm", model);
                }
                if (poster.Length > 1048576)
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "poster cannot be more than 1 megabyte");
                    return View("MovieForm", model);
                }

                // here i change the poster 
                movie.Poster = datastream.ToArray();
            }


            movie.Title = model.Title;
            movie.Year = model.Year;
            movie.GenreId = model.GenreId;
            movie.Rate = model.Rate;
            movie.StoryLine = model.StoryLine;

            _context.SaveChanges();
            
            _ToastNotification.AddInfoToastMessage("Movie Updated!");
            return RedirectToAction(nameof(Index));
            
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if(id==null)
            {
                return BadRequest();
            }
            var movie = await _context.Movies.Include(x=>x.Genre).FirstOrDefaultAsync(x => x.ID == id);
            
            if(movie==null)
            {
                return NotFound();
            }
            return View(movie);

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var movie = await _context.Movies.FirstOrDefaultAsync(x => x.ID == id);

            if (movie == null)
            {
                return NotFound();
            }
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return Ok();

        }

    }
}
