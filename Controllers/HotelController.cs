using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nozl.DTOs.HotelDtos;
using Nozl.Model;
using System.Security.Claims;

namespace Nozl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
           private NozlDbContext _dbContext;

            public HotelController(NozlDbContext dbContext)
            {
                _dbContext = dbContext;
            }

        [HttpGet("GetAllHotels")]
        [Authorize]
        public async Task<IActionResult> GetAllHotels()
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var hotel = _dbContext.Hotels.Select(h => new
            {
                h.Name,
                h.ImageUrl,
                h.Owner,
                h.Address,
                h.City,
                h.Description,
                h.Phone
            });
          return Ok(hotel);
        }
        [HttpGet("GetById")]
        [Authorize]
        public async Task<IActionResult> GetById(long id)
        {

            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var hotel = from h in _dbContext.Hotels
                        where h.Id == id
                        select new
                        {
                            h.Name,
                            h.ImageUrl,
                            h.Owner,
                            h.Address,
                            h.City,
                            h.Description,
                            h.Phone
                        };
            var data = await hotel.ToListAsync();
            return Ok(hotel);   
        }
        [HttpPost("AddNewHotel")]
        [Authorize (Roles ="HotelOwner")]
        public async Task<IActionResult> AddNewHotel(AddHotelDto dto)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var hotel = new Hotel
            {
                OwnerId = UserId,
                Name = dto.Name,
                ImageUrl = dto.ImageUrl,
                City = dto.City,
                Description = dto.Description,
                Status ="Pending",
                Phone = dto.Phone,
                Address = dto.Address,

            };
            _dbContext.Hotels.Add(hotel);
            await _dbContext.SaveChangesAsync();
             return Ok(hotel);

        }

        [HttpPut("UpdateHotelInfo")]
        [Authorize(Roles ="HotelOwner")]
        public async Task<IActionResult> UpdateHotelInfo (long id, UpdateHotelInfoDto dto)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var hotel = await _dbContext.Hotels.FirstOrDefaultAsync(h => h.Id == id);
            if (hotel == null)
            {
                return NotFound();
            }
            hotel.Name=dto.Name;
            hotel.Status = dto.Status;
            hotel.Description = dto.Description;
            hotel.Phone = dto.Phone;
            hotel.ImageUrl = dto.ImageUrl;


            await _dbContext.SaveChangesAsync();
            return Ok(hotel);
        }

        [HttpPut("HotelStatus")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HotelStatus(long id, StatusHotelDto dto)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var hotel = await _dbContext.Hotels.FirstOrDefaultAsync(h => h.Id == id);
            if (hotel == null)
            {
                return NotFound();
            }

            hotel.Status = dto.Status;
            await _dbContext.SaveChangesAsync();
            return Ok(hotel);
        }

        [HttpPut("DeleteHotel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHotel(long id)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserId = int.Parse(UserIdString);

            var hotel = await _dbContext.Hotels.FirstOrDefaultAsync(h => h.Id == id);
            if (hotel == null) { return NotFound(); }

            _dbContext.Hotels.Remove(hotel);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        }
    }
