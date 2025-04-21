using System.Collections.Generic;
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
        
        [Column("component_name")]
        public string ComponentName { get; set; }

        [ForeignKey("DeviceId")]
        public Device Device { get; set; }

        public ICollection<ComponentItem> ComponentItems { get; set; }
    }
}