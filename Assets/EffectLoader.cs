namespace HollowKnightItems.Assets
{
    internal class EffectLoader : ModSystem
    {
        public static Effect Fireball;

        public override void Load()
        {
            // Screen Shader

            // Projectile Shader
            Fireball = GetEffect("Fireball").Value;
        }
    }
}
