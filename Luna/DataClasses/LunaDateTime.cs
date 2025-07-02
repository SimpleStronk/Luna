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

        /// <summary>
        /// This object's value as a conventional DateTime object
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                return dateTime;
            }
        }

        /// <summary>
        /// The current time as a LunaDateTime object
        /// </summary>
        public static LunaDateTime Now
        {
            get
            {
                LunaDateTime lunaDateTime = new LunaDateTime();
                lunaDateTime.dateTime = DateTime.Now;
                return lunaDateTime;
            }
        }

        /// <summary>
        /// Creates a LunaDateTime from the given DateTime object
        /// </summary>
        public static LunaDateTime FromDateTime(DateTime dateTime)
        {
            LunaDateTime lunaDateTime = new LunaDateTime();
            lunaDateTime.dateTime = dateTime;
            return lunaDateTime;
        }

        /// <summary>
        /// This object's value in the form "dd/mm/yyyy hh:mm:ss"
        /// </summary>
        public string ShortDisplay
        {
            get { return $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}:{dateTime.Second}"; }
        }

        /// <summary>
        /// This objects value in the form "dd-mm-yyyy hh-mm-ss"
        /// </summary>
        public string ShortDisplayAlt
        {
            get { return ShortDisplay.Replace('/', '-').Replace(':', '-'); }
        }

        /// <summary>
        /// This object's value in the form "dd <Month> yyyy, hh:mm:ss"
        /// </summary>
        public string LongDisplay
        {
            get { return $"{dateTime.ToLongDateString()}, {dateTime.ToShortTimeString()}:{dateTime.Second}"; }
        }
    }
}
