using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VISMS.Models
{
    public class VISMS_ProductList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SRL { get; set; }

        [Required]
        public string ServiceCode { get; set; }

        [Required]
        public int ProductNo { get; set; }

        [Required]
        public string ProductName { get; set; }

        public int RelationProductNo { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RegDate { get; set; }

        [Required]
        public int ProductExpire { get; set; }

        [Required]
        public int ProductPieces { get; set; }

        [Required]
        public int PaymentType { get; set; }

        [Required]
        public int SalePrice { get; set; }

        [Required]
        public int CategoryNo { get; set;}

        [Required]
        public int BonusProductCount { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductID { get; set; }

        [Required]
        [StringLength(32)]
        public string ProductGUID { get; set; }

        [Required]
        [StringLength(2)]
        public string ProductType { get; set; }


    }
}
