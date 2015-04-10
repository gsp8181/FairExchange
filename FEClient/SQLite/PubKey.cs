using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FEClient.SQLite
{
    [Table("PubKey")]
    internal class PubKey
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column("Id")]
        public int Id { get; set; }

        //[Column("Email")]
        [Required]
        public string Email { get; set; }

        //[Column("Pem")]
        [Required]
        public string Pem { get; set; }
    }
}