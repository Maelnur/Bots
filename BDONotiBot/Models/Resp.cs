using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BDONotiBot.Models
{
    [Table("Resps")]
    public class Resp
    {
        [Key]
        public int Id { get; set; }
        public int? BossId { get; set; }
        public int DayOfTheWeek { get; set; }
        public virtual List<RespTime> RespTime { get; set; }

        public Resp()
        {
            RespTime = new List<RespTime>();
        }
    }
}
