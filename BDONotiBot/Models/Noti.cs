using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BDONotiBot.Models
{
    [Table("Notis")]
    public class Noti
    {
        [Key]
        public int Id { get; set; }
        public int NotiTime { get; set; }
    }
}
