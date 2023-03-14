namespace HollowKnightItems.Assets
{
    internal class EffectsLoader : ModSystem
    {
        public static Effect Fireball;

        public static Effect RoarScreen;

        public override void Load()
        {
            // Screen Shader
            // 注意设置正确的Pass名字，Scene的名字可以随便填，不和别的Mod以及原版冲突即可
            //Filters.Scene["HollowKnightItems:Roar"] = new Filter(
            //    new ScreenShader(new Ref<Effect>(GetEffect("RoarScreen").Value), "Test"),
            //    EffectPriority.Medium);
            //Filters.Scene["HollowKnightItems:Roar"].Load();

            // Projectile Shader
            Fireball = GetEffect("Fireball").Value;
        }
    }
}
