using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FEClient.Database
{
    [Table("PubKey")]
    internal class PubKey
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Pem { get; set; }
    }
}