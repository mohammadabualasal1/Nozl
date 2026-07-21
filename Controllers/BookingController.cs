using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nozl.DTOs.BookingDtos;
using Nozl.Model;
using System.Security.Claims;

namespace Nozl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {

        private NozlDbContext _dbContext;

        public BookingController(NozlDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet("GetMyBooking")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> GetMyBooking()
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var book = await (from b in _dbContext.Bookings
                              from r in _dbContext.Rooms
                              where r.Id == UserId
                              select new
                              {
                                  r.RoomNumber,
                                  b.CheckIn,
                                  b.CheckOut,
                                  b.CreatedAt,
                                  b.TotalPrice,
                                  b.Status,
                              }).ToListAsync();
            return Ok(book);
        }

        [HttpGet("GetAllBookingsForHotel")]
        [Authorize(Roles = "HotelOwner")]
        public async Task<IActionResult> GetAllBookingsForHotel(long id)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var book = await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            var booK = await (from b in _dbContext.Bookings
                              from h in _dbContext.Hotels
                              from r in _dbContext.Rooms
                              where h.Id == UserId && r.HotelId == h.Id && b.RoomId == r.Id
                              select new
                              {
                                  h.Name,
                                  r.RoomNumber,
                                  b.Status,
                              }).ToListAsync();

            return Ok(booK);
        }

        [HttpPost("BookRoom")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> BookRoom(BookRoomDto dto)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var bookId = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == dto.RoomId);
            if (bookId == null)
            {
                return NotFound("");
            }
            bool overlapping = await _dbContext.Bookings.AnyAsync(b =>
               b.RoomId == dto.RoomId &&             // 5 , 10   11 , 13
               b.CheckIn < dto.CheckOut &&             // 5 < 11 
               b.CheckOut > dto.CheckIn);             //  13 > 10

            if (overlapping)
            {
                return BadRequest("Room is not available for the selected dates");
            }

            var book = new Booking
            {
                RoomId = dto.RoomId,
                CheckIn = DateTime.SpecifyKind(dto.CheckIn, DateTimeKind.Utc),
                CheckOut = DateTime.SpecifyKind(dto.CheckOut, DateTimeKind.Utc),
                TotalPrice = (dto.CheckOut - dto.CheckIn).Days * bookId.PricePerNight,
                Status = "Active"
            };

            _dbContext.Bookings.Add(book);
            await _dbContext.SaveChangesAsync();
            return Ok(book);

        }

        [HttpPut("CancelBooking")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> CancelBooking(long id)
        {

            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var book = await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound(); 
            }

            if (book.GuestId != UserId)
            {
                return Forbid();
            }
            book.Status = "Cancelled";
            await _dbContext.SaveChangesAsync();
            return Ok(book);
        }
        [HttpGet("GetAllBookingInSystem")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBookingInSystem()
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var book = await (from b in _dbContext.Bookings 
                              from r in _dbContext.Rooms
                              from h in _dbContext.Hotels
                              where r.Id == UserId && h.Id == UserId
                       select new
                       { 
                           h.Name,
                           r.RoomNumber,
                           b.Status,
                           b.CheckIn,
                           b.CheckOut
                       }).ToListAsync();
            return Ok(book);
        }
    }
}
