using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_VillaAPI.Models
{
    public class VillaAPI
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]       //To make it as Identity
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }

        public DateOnly CreatedDate { get; set; }       //Not exposing these 2 property so not added in Villa DTO
        public DateOnly UpdatedDate { get; set; }

    }
}
