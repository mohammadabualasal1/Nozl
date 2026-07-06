using System.ComponentModel.DataAnnotations.Schema;

namespace Nozl.Model
{
    public class Room
    {
        public long Id { get; set; }

        [ForeignKey("Hotel")]
        public long HotelId { get; set; }
        public Hotel? Hotel { get; set; }
        public string RoomNumber { get; set; }
        public string Type { get; set; } 
        public int PricePerNight { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
