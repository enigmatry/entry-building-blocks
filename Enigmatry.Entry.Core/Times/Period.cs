using JetBrains.Annotations;
using System;

namespace Enigmatry.Entry.Core.Times
{
    [PublicAPI]
    public sealed record Period
    {
        private Period()
        {
            // By Serializers
        }

        public Period(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentOutOfRangeException(nameof(startDate), $@"{nameof(startDate)} should be < {nameof(endDate)}");
            }

            StartDate = startDate;
            EndDate = endDate;
        }

        public DateTimeOffset StartDate { get; private set; }

        public DateTimeOffset EndDate { get; private set; }

        public TimeSpan TimeSpan => EndDate - StartDate;

        public bool Contains(DateTimeOffset date) => date >= StartDate && date <= EndDate;

        public bool Contains(DateTimeOffset start, DateTimeOffset end) => Contains(start) && Contains(end);

        public bool Contains(Period period) => Contains(period.StartDate, period.EndDate);

        public override string ToString() => $"[{StartDate:d}, {EndDate:d}]";

        public Period Copy() => new(StartDate, EndDate);

        public Period? CalculateOverlap(Period period)
        {
            if (period == null!)
            {
                return null;
            }

            if (EndDate < period.StartDate || StartDate > period.EndDate)
            {
                // No overlap.
                return null;
            }
            var startDateOfOverlap = Max(StartDate, period.StartDate);
            var endDateOfOverlap = Min(EndDate, period.EndDate);

            return new Period(startDateOfOverlap, endDateOfOverlap);
        }

        private static DateTimeOffset Min(DateTimeOffset d1, DateTimeOffset d2) => d1 < d2 ? d1 : d2;

        private static DateTimeOffset Max(DateTimeOffset d1, DateTimeOffset d2) => d1 > d2 ? d1 : d2;
    }
}
