using System;
using System.Linq;
using System.Reflection;
using UnboundLib.Cards;

namespace BrawlRounds.Runtime
{
    internal static class CardRegistry
    {
        public static void RegisterAll()
        {
            var candidates = typeof(CustomCard).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "BuildCard" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1)
                .OrderBy(m => m.GetParameters().Length)
                .ToArray();
            var build = candidates.FirstOrDefault(m => m.GetParameters().Length == 0) ?? candidates.FirstOrDefault();
            if (build == null) throw new MissingMethodException("UnboundLib CustomCard.BuildCard<T> was not found.");

            foreach (var t in typeof(CardRegistry).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(CustomCard).IsAssignableFrom(t) && t.Namespace == "BrawlRounds.Generated.Cards"))
            {
                var method = build.MakeGenericMethod(t);
                var args = method.GetParameters().Select(p => p.HasDefaultValue ? p.DefaultValue : Type.Missing).ToArray();
                method.Invoke(null, args);
            }
        }
    }
}
