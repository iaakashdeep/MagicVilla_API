using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class VillaDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
    }

    //[ApiController]-> this helps to let api know about data annotations, as this has built in support for that
}
