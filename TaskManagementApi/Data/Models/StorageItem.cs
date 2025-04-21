using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("storage_item")]
    public class StorageItem
    {
        [Key]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("item_name")]
        public string ItemName { get; set; }

        [Column("item_type")]
        public string ItemType { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }
        
        public ICollection<ComponentItem> ComponentItems { get; set; }
    }
}