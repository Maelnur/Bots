using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BDONotiBot.Models
{
    [Table("RespTimes")]
    public class RespTime
    {
        [Key]
        public int Id { get; set; }
        public int? RespId { get; set; }
        public DateTime Resp { get; set; }
    }
}
