using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace HollowKnightItems.Assets
{
    internal class EffectsLoader : ModSystem
    {
        public static Effect Fireball;

        public override void Load()
        {
            Fireball = GetEffect("Fireball").Value;
        }
    }
}
