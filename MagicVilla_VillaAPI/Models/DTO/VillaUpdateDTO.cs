using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    /// <summary>
    /// Creating seperate DTO for Updating because it may be possible that we have different validations for create and update
    /// Like; in Updating we need Id because updating will be done on the basis of that only
    /// </summary>
    public class VillaUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public string Details { get; set; }
        [Required]
        public double Rate { get; set; }
        [Required]
        public int Sqft { get; set; }
        [Required]
        public int Occupancy { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }
        
    }

    //[ApiController]-> this helps to let api know about data annotations, as this has built in support for that
}
