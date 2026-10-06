namespace BrawlRounds.Runtime
{
    internal static class CardRuntime
    {
        public static void Add(Player player, CardEffect effect)
        {
            if (player == null || effect == null) return;
            BrawlRuntime.State(player).SetCard(effect);
        }
        public static void Remove(Player player, string id)
        {
            if (player == null) return;
            BrawlRuntime.State(player).RemoveCard(id);
        }
    }
}
