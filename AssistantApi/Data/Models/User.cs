using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace AssistantApi.Data.Models
{
    [Table("user")]
    public partial class User
    {
        public User()
        {
            Histories = new HashSet<History>();
        }

        [Key]
        [Column("userID", TypeName = "int(11)")]
        public int UserId { get; set; }
        [Required]
        [Column("login")]
        [StringLength(100)]
        public string Login { get; set; }
        [Required]
        [Column("password")]
        [StringLength(100)]
        public string Password { get; set; }
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }

        [InverseProperty(nameof(History.User))]
        public virtual ICollection<History> Histories { get; set; }
    }
}
