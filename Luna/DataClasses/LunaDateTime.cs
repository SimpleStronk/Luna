using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.DataClasses
{
    internal class LunaDateTime
    {
        private DateTime dateTime;

        public DateTime DateTime
        {
            get
            {
                return dateTime;
            }
        }

        public static LunaDateTime Now
        {
            get
            {
                LunaDateTime lunaDateTime = new LunaDateTime();
                lunaDateTime.dateTime = DateTime.Now;
                return lunaDateTime;
            }
        }

        public static LunaDateTime FromDateTime(DateTime dateTime)
        {
            LunaDateTime lunaDateTime = new LunaDateTime();
            lunaDateTime.dateTime = dateTime;
            return lunaDateTime;
        }

        public string ShortDisplay
        {
            get { return dateTime.ToShortDateString(); }
        }

        public string LongDisplay
        {
            get { return dateTime.ToLongDateString(); }
        }
    }
}
