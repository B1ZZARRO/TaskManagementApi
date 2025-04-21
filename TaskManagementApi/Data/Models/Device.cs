using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("device")]
    public class Device
    {
        [Key]
        [Column("device_id")]
        public int DeviceId { get; set; }

        [Column("device_name")]
        public string DeviceName { get; set; }

        [Column("device_model")]
        public string DeviceModel { get; set; }
        
        public ICollection<DeviceComponent> Components { get; set; }
    }
}