using CinemaApi.Data;
using CinemaApi.Models;
using CinemaApi.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ReservationsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public ActionResult GetReservations()
        {
            var reservations = from reserves in _db.Reservations
                               join customer in _db.Users on reserves.UserId equals customer.Id
                               join movie in _db.Movies on reserves.MovieId equals movie.Id
                               select new
                               {
                                   Id = reserves.Id,
                                   ReserationTime = reserves.ReservationTime,
                                   CustomerName = customer.Name,
                                   MovieName = movie.Name,
                               };
            return Ok(reservations);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("{id:int}", Name ="GetReservation")]
        public ActionResult GetReservation(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var reservation = (from reserves in _db.Reservations
                              join customer in _db.Users on reserves.UserId equals customer.Id
                              join movie in _db.Movies on reserves.MovieId equals movie.Id
                              where reserves.Id == id
                               select new
                               {
                                   Id = reserves.Id,
                                   ReserationTime = reserves.ReservationTime,
                                   CustomerName = customer.Name,
                                   MovieName = movie.Name,
                                   Email = customer.Email,
                                   Qty = reserves.Qty,
                                   Price = reserves.Price,
                                   Phone = reserves.Phone,
                                   PlayingDate = movie.PlayingDate,
                                   PlayingTime = movie.PlayingDate
                               }).FirstOrDefault();

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateReservation([FromBody] ReservationDto reserve)
        {
            if (reserve == null)
            {
                return BadRequest(reserve);
            }
            if (reserve.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Reservation model = new()
            {
                Id= reserve.Id,
                Qty= reserve.Qty,
                MovieId=reserve.MovieId,
                Phone=reserve.Phone,
                Price=reserve.Price,
                UserId = reserve.UserId
            };
            _db.Reservations.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetReservation", new { id = reserve.Id }, reserve);
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete]
        public IActionResult DeleteReservation(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var reservation = _db.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }
            _db.Reservations.Remove(reservation);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
