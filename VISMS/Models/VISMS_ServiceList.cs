using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VISMS.Models
{
    public class VISMS_ServiceList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SRL { get; set; }

        [Required]
        public string ServiceCode { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required] [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RegDate { get; set; }
    }
}