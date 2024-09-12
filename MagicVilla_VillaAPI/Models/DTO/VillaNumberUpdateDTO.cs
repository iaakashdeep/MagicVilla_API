using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_VillaAPI.Models.DTO
{
    /// <summary>
    /// Creating seperate DTO for Updating because it may be possible that we have different validations for create and update
    /// Like; in Updating we need Id because updating will be done on the basis of that only
    /// </summary>
    public class VillaNumberUpdateDTO
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillaNo { get; set; }
        [Required]
        public int VillaId { get; set; }
        public string SpecialDetails { get; set; }

    }
}
