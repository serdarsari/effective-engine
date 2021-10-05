using System.ComponentModel.DataAnnotations;

namespace GalaxyExplorer.DTO
{
    public class VoyagerRequest
    {
        [Required]
        public int VoyagerId { get; set; }
    }
}