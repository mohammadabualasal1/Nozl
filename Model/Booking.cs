using System.ComponentModel.DataAnnotations.Schema;

namespace Nozl.Model
{
    public class Booking
    {
        public long Id { get; set; }

        [ForeignKey("Guest")]
        public long? GuestId { get; set; }
        public User? Guest { get; set; }

        [ForeignKey("Room")]
        public long? RoomId { get; set; }
        public Room? Room { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public long TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
