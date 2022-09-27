using JetBrains.Annotations;
using System;

namespace Enigmatry.BuildingBlocks.Core.Times
{
    [PublicAPI]
    public sealed record Period
    {
        private Period()
        {
            // By Serializers
        }

        public Period(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentOutOfRangeException(nameof(startDate), $@"{nameof(startDate)} should be < {nameof(endDate)}");
            }

            StartDate = startDate;
            EndDate = endDate;
        }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public TimeSpan TimeSpan => EndDate - StartDate;

        public bool InFuture => StartDate > DateTime.Now;

        public bool InPast => EndDate < DateTime.Now;

        public bool InPresent => Contains(DateTime.Now);

        public bool Contains(DateTime date) => date >= StartDate && date <= EndDate;

        public bool Contains(DateTime start, DateTime end) => Contains(start) && Contains(end);

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

        private static DateTime Min(DateTime d1, DateTime d2) => d1 < d2 ? d1 : d2;

        private static DateTime Max(DateTime d1, DateTime d2) => d1 > d2 ? d1 : d2;
    }
}
