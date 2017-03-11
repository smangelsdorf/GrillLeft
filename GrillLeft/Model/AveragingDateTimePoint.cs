using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrillLeft.Model
{
    internal class AveragingDateTimePoint
    {
        internal readonly double Total;
        internal readonly int Count;
        internal readonly DateTime DateTime;

        internal AveragingDateTimePoint(DateTime dateTime, double value) : this(dateTime, value, 1)
        {
        }

        internal AveragingDateTimePoint(DateTime dateTime, double total, int count)
        {
            Total = total;
            Count = count;
            DateTime = dateTime;
        }

        internal double Value
        {
            get
            {
                return Total / Count;
            }
        }

        internal AveragingDateTimePoint Concat(AveragingDateTimePoint other)
        {
            var dateTime = DateTime;
            if (other.DateTime > DateTime) dateTime = other.DateTime;

            var newTotal = Total + other.Total;
            var newCount = Count + other.Count;

            return new AveragingDateTimePoint(dateTime, newTotal, newCount);
        }
    }
}