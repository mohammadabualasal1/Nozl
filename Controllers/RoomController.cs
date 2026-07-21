using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nozl.DTOs.RoomDto;
using Nozl.Model;
using System.Data;
using System.Security.Claims;

namespace Nozl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private NozlDbContext _dbContext;

        public RoomController(NozlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("GetAllRooms")]
        [Authorize]
        public async Task<IActionResult> GetAllRooms(long id)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var room = await (from r in _dbContext.Rooms
                              from h in _dbContext.Hotels
                              where h.Id == id
                              select new
                              {
                                  h.Name,
                                  r.RoomNumber,
                                  r.Type,
                                  r.PricePerNight,
                                  r.Capacity,
                                  r.ImageUrl,
                                  r.IsAvailable
                              }).ToListAsync();

            return Ok(room);
        }

        [HttpGet("GetAvailableRooms")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAvailableRooms(DateTime checkIn, DateTime checkOut)
        {
            checkIn = DateTime.SpecifyKind(checkIn, DateTimeKind.Utc);
            checkOut = DateTime.SpecifyKind(checkOut, DateTimeKind.Utc);

            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var room = await (from r in _dbContext.Rooms
                              where !_dbContext.Bookings.Any(b =>
                                  b.RoomId == r.Id &&
                                  b.CheckIn > checkOut &&
                                   b.CheckOut < checkIn )
                              select new
                              {
                                  r.RoomNumber,
                                  r.Type,
                                  r.PricePerNight,
                                  r.Capacity,
                                  r.ImageUrl

                              }).ToListAsync();

            return Ok(room);

        }

        [HttpPost("AddRoom")]
        [Authorize(Roles = "HotelOwner")]
        public async Task<IActionResult> AddRoom(AddRoomDto dto)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var room = new Room
            {
                HotelId = dto.HotelId,
                RoomNumber = dto.RoomNumber,
                Type = dto.Type,
                PricePerNight = dto.PricePerNight,
                Capacity = dto.Capacity,
                ImageUrl = dto.ImageUrl,
                IsAvailable = dto.IsAvailable

            };

            _dbContext.Rooms.Add(room);
            await _dbContext.SaveChangesAsync();
            return Ok(room);

        }

        [HttpPut("UpdateRoomInfo")]
        [Authorize(Roles = "HotelOwner")]
        public async Task<IActionResult> UpdateRoomInfo(long id,UpdateRoomInfoDto dto)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var room =  await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return NotFound();
            }
            room.RoomNumber = dto.RoomNumber;
            room.PricePerNight = dto.PricePerNight;
            room.Capacity = dto.Capacity;
            room.ImageUrl = dto.ImageUrl;
            room.Type = dto.Type;
            room.IsAvailable= dto.IsAvailable;

            await _dbContext.SaveChangesAsync();
            return Ok(room);
        }
        [HttpDelete("DeleteRoom")]
        [Authorize(Roles = "HotelOwner")]
        public async Task<IActionResult> DeleteRoom(long id)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null) { return NotFound(); }

            _dbContext.Rooms.Remove(room);
            await _dbContext.SaveChangesAsync();
            return NoContent();

        }
        }
}
