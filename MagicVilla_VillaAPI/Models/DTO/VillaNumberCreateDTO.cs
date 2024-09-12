using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_VillaAPI.Models.DTO
{
    /// <summary>
    /// Creating seperate DTO for Creating and Updation because it may be possible in future the logic for creation will be changed
    /// </summary>
    public class VillaNumberCreateDTO
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillaNo { get; set; }
        [Required]
        public int VillaId { get; set; }
        public string SpecialDetails { get; set; }

    }
}
