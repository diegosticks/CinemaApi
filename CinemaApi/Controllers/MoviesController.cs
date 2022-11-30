using CinemaApi.Data;
using CinemaApi.Models;
using CinemaApi.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public MoviesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetMovies(string? sort, int? pageNumber, int? pageSize)
        {
            var currentPageNumber = pageNumber?? 1;
            var currentPageSize = pageSize ?? 1; 

            var movies = from movie in _db.Movies
                         select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             Duration = movie.Duration,
                             Language = movie.Language,
                             Rating = movie.Rating,
                             Genre = movie.Genre,
                             ImageUrl = movie.ImageUrl
                         };
            switch (sort)
            {
                case "desc":
                    return Ok(_db.Movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderByDescending(m => m.Rating));
                case "asc":
                    return Ok(_db.Movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderBy(m => m.Rating));
                default:
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));
            }
        }

        [Authorize]
        [HttpGet("{id}", Name ="GetMovie")]
        public ActionResult GetMovie(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var movie = _db.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }

        [Authorize]
        [HttpGet("[action]")]
        public ActionResult FindMovie(String movieName)
        {
            if (movieName== null)
            {
                return NotFound();
            }

            var movies = from movie in _db.Movies
                         where movie.Name.StartsWith(movieName)
                         select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             ImageUrl = movie.ImageUrl
                         };
            return Ok(movies);
        }


        [Authorize(Roles ="Admin")]
        [HttpPost]
        public ActionResult<MovieDto> Post([FromForm] MovieDto movieDto)
        {
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");

            if (movieDto.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieDto.Image.CopyTo(fileStream);
            }

            if (_db.Movies.FirstOrDefault(m => m.Name.ToLower() == movieDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Movie Already Exist.");
                return BadRequest(ModelState);
            }

            if (movieDto == null)
            {
                return BadRequest(movieDto);
            }

            if (movieDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            //movieDto.ImageUrl = filePath.Remove(0, 7);

            Movie model = new()
            {
                Id = movieDto.Id,
                Name = movieDto.Name,
                Language = movieDto.Language,
                Rating = movieDto.Rating,
                Description= movieDto.Description,
                Duration= movieDto.Duration,
                Genre= movieDto.Genre,
                Image = movieDto.Image,
                PlayingDate= movieDto.PlayingDate,
                PlayingTime= movieDto.PlayingTime,
                TicketPrice= movieDto.TicketPrice,
                TrailUrl= movieDto.TrailUrl,
                ImageUrl = movieDto.ImageUrl = filePath.Remove(0, 7)
            };
            _db.Movies.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetMovie", new { id = movieDto.Id }, movieDto);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}", Name ="UpdateMovie")]
        public IActionResult Put(int id, [FromForm] MovieDto movieDto)
        {
            if (movieDto == null || id != movieDto.Id)
            {
                return BadRequest();
            }

            Movie model = new()
            {
                Id = movieDto.Id,
                Name = movieDto.Name,
                Language = movieDto.Language,
                Rating = movieDto.Rating,
                Description = movieDto.Description,
                Duration = movieDto.Duration,
                Genre = movieDto.Genre,
                Image = movieDto.Image,
                PlayingDate = movieDto.PlayingDate,
                PlayingTime = movieDto.PlayingTime,
                TicketPrice = movieDto.TicketPrice,
                TrailUrl = movieDto.TrailUrl

            };

            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");
            if (movieDto.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieDto.Image.CopyTo(fileStream);
                model.ImageUrl = filePath.Remove(0, 7);
            }

            _db.Movies.Update(model);
            _db.SaveChanges();

            return NoContent();
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var movie = _db.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            _db.Movies.Remove(movie);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
