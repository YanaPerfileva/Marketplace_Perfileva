using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace.Data.Entities
{
    [Table("PromotionProducts")]
    public class PromotionProduct
    {
        public int PromotionId { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("PromotionId")]
        public virtual Promotion Promotion { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
