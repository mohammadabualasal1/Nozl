using Nozl.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nozl.DTOs.RoomDto
{
    public class AddRoomDto
    {
        public long HotelId { get; set; }
        public string RoomNumber { get; set; }
        public string Type { get; set; }
        public long PricePerNight { get; set; }
        public long Capacity { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
