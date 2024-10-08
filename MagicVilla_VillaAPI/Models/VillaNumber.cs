﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_VillaAPI.Models
{
    public class VillaNumber
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillaNo { get; set; }
        public string SpecialDetails { get; set; }
        public DateOnly CreatedDate { get; set; }
        public DateOnly UpdatedDate { get; set; }
        [Required]
        public int VillaId {  get; set; }
        [ForeignKey(nameof(VillaId))]
        public VillaAPI villaAPI { get; set; }
    }
}
