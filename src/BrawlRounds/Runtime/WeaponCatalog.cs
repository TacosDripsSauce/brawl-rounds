using System;
using System.Collections.Generic;
using System.Linq;
using BrawlRounds.Generated;

namespace BrawlRounds.Runtime
{
    internal static class WeaponCatalog
    {
        private static readonly Dictionary<string, WeaponDefinition> Items = typeof(WeaponCatalog).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && typeof(WeaponDefinition).IsAssignableFrom(t))
            .Select(t => (WeaponDefinition)Activator.CreateInstance(t))
            .ToDictionary(x => x.Id, x => x);
        public static IEnumerable<WeaponDefinition> All { get { return Items.Values; } }
        public static WeaponDefinition Get(string id) { WeaponDefinition v; return Items.TryGetValue(id, out v) ? v : null; }
    }
}
