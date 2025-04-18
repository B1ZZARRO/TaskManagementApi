using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("device_component")]
    public class DeviceComponent
    {
        [Key]
        [Column("component_id")]
        public int ComponentId { get; set; }

        [Column("device_id")]
        public int DeviceId { get; set; }

        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("quantity_needed")]
        public int QuantityNeeded { get; set; }

        [ForeignKey("DeviceId")]
        public Device Device { get; set; }

        [ForeignKey("ItemId")]
        public StorageItem Item { get; set; }
    }
}