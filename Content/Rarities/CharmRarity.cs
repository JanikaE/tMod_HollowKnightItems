using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace HollowKnightItems.Content.Rarities
{
    internal class CharmRarity : ModRarity
    {
        public override Color RarityColor => new(191, 158, 112);

        public override string Name => "CharmRarity";

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0)
            {
                return ModContent.RarityType<CharmRarity>();
            }

            return Type;
        }
    }
}
