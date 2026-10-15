using System;
using System.Collections.Generic;

namespace InventoryManagementSystemDataAccess.Mappers.Inventory
{
    internal static class CollectionMapper
    {
        public static List<T> ToDomainModelList<TDb, T>(this IEnumerable<TDb> dbItems, Func<TDb, T?> map) where T : class
        {
            var items = new List<T>();
            foreach (var dbItem in dbItems)
            {
                var item = map(dbItem);
                if (item != null)
                {
                    items.Add(item);
                }
            }
            return items;
        }
    }
}
