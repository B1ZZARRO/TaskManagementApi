using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace AssistantApi.Data.Models
{
    [Table("history")]
    [Index(nameof(UserId), Name = "userID")]
    public partial class History
    {
        [Key]
        [Column("historyID", TypeName = "int(11)")]
        public int HistoryId { get; set; }
        [Required]
        [Column("query")]
        [StringLength(250)]
        public string Query { get; set; }
        [Required]
        [Column("response")]
        [StringLength(250)]
        public string Response { get; set; }
        [Column("userID", TypeName = "int(11)")]
        public int UserId { get; set; }
        [Required]
        [Column("date")]
        [StringLength(100)]
        public string Date { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("Histories")]
        public virtual User User { get; set; }
    }
}
