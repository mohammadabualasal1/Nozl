using Nozl.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nozl.DTOs.HotelDtos
{
    public class AddHotelDto
    {
      
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Status { get; set; }

    }
}
