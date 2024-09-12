using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    /// <summary>
    /// Creating seperate DTO for Creating because it may be possible that we have different validations for create and update
    /// Like; in Creating we don't need Id because EF Core will automatically insert that
    /// </summary>
    public class VillaCreateDTO
    {
        //public int Id { get; set; }       
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public string Details { get; set; }
        [Required]
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }
        
    }

    //[ApiController]-> this helps to let api know about data annotations, as this has built in support for that
}
