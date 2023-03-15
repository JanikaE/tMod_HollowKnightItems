namespace HollowKnightItems.Assets
{
    internal class EffectsLoader : ModSystem
    {
        public static Effect Fireball;

        public static Effect RoarScreen;

        public override void Load()
        {
            // Screen Shader

            // Projectile Shader
            Fireball = GetEffect("Fireball").Value;
        }
    }
}
