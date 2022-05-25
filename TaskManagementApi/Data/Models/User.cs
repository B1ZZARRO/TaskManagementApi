using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("last_name")]
        [Required]
        public string LastName { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; }

        [Column("surname")]
        public string Surname { get; set; }

        [Column("login")]
        [Required]
        public string Login { get; set; }

        [Column("password")]
        [Required]
        public string Password { get; set; }

        [Column("role_id")]
        public int? RoleId { get; set; }

        [Column("group_id")]
        public int? GroupId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }

        [ForeignKey(nameof(GroupId))]
        public UserGroup UserGroup { get; set; }
    }
}