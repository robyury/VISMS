using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VISMS.Models
{
    public class VISMS_UserList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SRL { get; set; }

        [Required]
        public string ServiceCode { get; set; }

        [Required]
        public int oid { get; set; }

        [Required]
        [StringLength(32)]
        public string strNexonID { get; set; }

        [Required]
        [StringLength(32)]
        public string strLNexonID { get; set; }

        [Required]
        public int RealBalance { get; set; }

        [Required]
        public int BonusBalance { get; set; }

        [Required]
        public long TotalBalance { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RegDate { get; set; }

        public DateTime UpdDate { get; set; }
    }
}