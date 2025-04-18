using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("storage_movement")]
    public class StorageMovement
    {
        [Key]
        [Column("movement_id")]
        public int MovementId { get; set; }

        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("movement_type")]
        public string MovementType { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("movement_date")]
        public DateTime MovementDate { get; set; }

        [Column("related_task_id")]
        public int? RelatedTaskId { get; set; }

        [ForeignKey("ItemId")]
        public StorageItem Item { get; set; }

        [ForeignKey("RelatedTaskId")]
        public Task Task { get; set; }
    }
}