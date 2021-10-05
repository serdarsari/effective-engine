using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GalaxyExplorer.DTO
{
    public class CreateVoyagerRequest
    {
        [Required]
        [MinLength(10)]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string Grade { get; set; }
    }
}
