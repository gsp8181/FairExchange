using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEClient.SQLite
{
    [Table("PubKey")]
    class PubKey
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
