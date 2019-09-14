using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BDONotiBot.Code
{
    public static class Converter
    {
        public static string IntToWd(int dw)
        {
            string result = null;

            switch (dw)
            {
                case 1:
                    result = "Понедельник";
                    break;
                case 2:
                    result = "Вторник";
                    break;
                case 3:
                    result = "Среда";
                    break;
                case 4:
                    result = "Четверг";
                    break;
                case 5:
                    result = "Пятница";
                    break;
                case 6:
                    result = "Суббота";
                    break;
                case 7:
                    result = "Воскресенье";
                    break;
            }

            return result;
        }
    }
}
