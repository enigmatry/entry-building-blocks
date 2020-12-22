using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigmatry.Blueprint.BuildingBlocks.Core.Collections
{
    public static class CollectionUpdaterExtensions
    {
        public static CollectionUpdater<T, TCollection> UpdateWith<T, TCollection>(
            this ICollection<TCollection> collection,
            ICollection<T> values,
            Func<T, TCollection, bool> matcher,
            Func<T, TCollection> creator,
            Action<T, TCollection> updater,
            Action<TCollection>? deleter = null) =>
            new CollectionUpdater<T, TCollection>(collection, true).Apply(values, matcher, creator, updater, deleter ?? NullDeleter<TCollection>());

        public static CollectionUpdater<T, TCollection> UpdateWithoutRemove<T, TCollection>(
            this ICollection<TCollection> collection,
            ICollection<T> values,
            Func<T, TCollection, bool> matcher,
            Func<T, TCollection> creator,
            Action<T, TCollection> updater) =>
            new CollectionUpdater<T, TCollection>(collection, false).Apply(values, matcher, creator, updater, NullDeleter<TCollection>());

        private static Action<TCollection> NullDeleter<TCollection>() => item => {};

        public static CollectionUpdater<T, TCollection> UpdateWith<T, TCollection>(
            this ICollection<TCollection> collection,
            ICollection<T> values,
            Func<T, TCollection, bool> matcher,
            Func<T, TCollection> creator) =>
            new CollectionUpdater<T, TCollection>(collection, true).Apply(values, matcher, creator, NullDeleter<TCollection>());

        public static TCollection AddOrUpdate<T, TCollection>(this ICollection<TCollection> collection, T value,
            Func<TCollection, bool> matcher, Func<T, TCollection> creator, Action<T, TCollection> updater)
        {
            TCollection entity = collection.SingleOrDefault(matcher);

            if (entity == null)
            {
                entity = creator(value);
                collection.Add(entity);
            }
            else
            {
                updater(value, entity);
            }

            return entity;
        }
    }
}
