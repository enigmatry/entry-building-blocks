using JetBrains.Annotations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Enigmatry.BuildingBlocks.Core.Times
{
    public sealed class Period : IEquatable<Period>
    {
        [UsedImplicitly]
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

        public Period(Period period)
        {
#pragma warning disable CS8625, CS8604 // Cannot convert null literal to non-nullable reference type.
            if (period == null)
            {
                throw new ArgumentNullException(nameof(period));
            }

            StartDate = period.StartDate;
            EndDate = period.EndDate;
        }

        public override bool Equals(object obj) => Equals(obj as Period);

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
        public Period? CalculateOverlap(Period period)
        {
            if (period == null)
#pragma warning restore CS8625, CS8604 // Cannot convert null literal to non-nullable reference type.
            {
                return null;
            }

            if (EndDate < period.StartDate || StartDate > period.EndDate)
            {
                // no overlap
                return null;
            }
            var startDateOfOverlap = Max(StartDate, period.StartDate);
            var endDateOfOverlap = Min(EndDate, period.EndDate);

            return new Period(startDateOfOverlap, endDateOfOverlap);
        }

        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local",
            Justification = "It is needed for tools like NHibernate to set the properties. If it is deleted, date would be DateTime.MinValue.")]
        public DateTime StartDate { get; private set; }

        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local",
            Justification = "It is needed for tools like NHibernate to set the properties. If it is deleted, date would be DateTime.MinValue.")]
        public DateTime EndDate { get; private set; }

        public TimeSpan TimeSpan => EndDate - StartDate;

        public bool InFuture => StartDate > DateTime.Now;

        public bool InPast => EndDate < DateTime.Now;

        public bool InPresent => Contains(DateTime.Now);

        public bool Equals(Period other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            return StartDate == other.StartDate && EndDate == other.EndDate;
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
        [SuppressMessage("ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract")]
        public static bool operator ==(Period p1, Period p2) => p1?.Equals(p2) ?? p2 is null;

        public static bool operator !=(Period p1, Period p2) => !(p1 == p2);

        public bool Contains(DateTime date) => date >= StartDate && date <= EndDate;

        public bool Contains(DateTime start, DateTime end) => Contains(start) && Contains(end);

        public bool Contains(Period period) => Contains(period.StartDate, period.EndDate);

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode",
            Justification = "False positive since it's private setter and nobody changes values.")]
        public override int GetHashCode() => StartDate.GetHashCode() ^ EndDate.GetHashCode();

        private static DateTime Min(DateTime d1, DateTime d2) => d1 < d2 ? d1 : d2;

        private static DateTime Max(DateTime d1, DateTime d2) => d1 > d2 ? d1 : d2;

        public override string ToString() => $"[{StartDate:d}, {EndDate:d}]";
    }
}
