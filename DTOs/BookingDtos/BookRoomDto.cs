namespace Nozl.DTOs.BookingDtos
{
    public class BookRoomDto
    {
        public long RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public long TotalPrice { get; set; }
    }
}
