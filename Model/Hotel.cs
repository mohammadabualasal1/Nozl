using System.ComponentModel.DataAnnotations.Schema;

namespace Nozl.Model
{
    public class Hotel
    {
        public long Id { get; set; }

        [ForeignKey("Owner")]
        public long OwnerId { get; set; }
        public User? Owner { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Status { get; set; } 
    }
}
