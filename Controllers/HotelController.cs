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
     }
}
