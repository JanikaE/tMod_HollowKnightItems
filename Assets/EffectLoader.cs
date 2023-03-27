namespace HollowKnightItems.Assets
{
    internal class EffectLoader : ModSystem
    {
        public static Effect Fireball;
        public static Effect Text;

        public override void Load()
        {
            // Screen Shader

            // Other Shader
            Fireball = GetEffect("Fireball").Value;
            Text = GetEffect("Text").Value;
        }
    }
}
