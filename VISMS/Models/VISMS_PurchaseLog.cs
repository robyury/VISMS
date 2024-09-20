using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VISMS.Models
{
    public class VISMS_PurchaseLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SRL { get; set; }

        [Required]
        public string ServiceCode { get; set; }

        [Required]
        [StringLength(26)]
        public string OrderID { get; set; }

        [Required]
        public int ProductNo { get; set; }

        [Required]
        public int PaymentType { get; set; }

        [Required]
        public int PaymentRuleID { get; set; }

        [Required]
        public int TotalPrice { get; set; }

        [Required]
        public int OrderAmmount { get; set; }

        [Required] 
        public int oid { get; set; }

        [Required]
        [StringLength(32)]
        public string strNexonID { get; set; }

        [Required]
        [StringLength(15)]
        public string IPAddress { get; set; }

        [Required]
        public bool IsGift { get; set; }

        public int? Receiver_oid { get; set; }

        [StringLength(32)]
        public string? Receiver_strNexonID { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RegDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdDate { get; set; }


    }
}
