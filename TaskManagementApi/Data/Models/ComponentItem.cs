using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("component_item")]
    public class ComponentItem
    {
        [Key]
        [Column("component_item_id")]
        public int ComponentItemId { get; set; }

        [Column("component_id")]
        public int ComponentId { get; set; }

        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("quantity_needed")]
        public int QuantityNeeded { get; set; }

        [ForeignKey(nameof(ComponentId))]
        public DeviceComponent Component { get; set; }

        [ForeignKey(nameof(ItemId))]
        public StorageItem Item { get; set; }
    }
}