namespace HollowKnightItems.Assets
{
    internal class TextureLoader : ModSystem
    {
        public static Asset<Texture2D> GrimmDeath;
        public static Asset<Texture2D> UIClose;

        public override void Load()
        {
            GrimmDeath = GetTexture("GrimmDeath");
            UIClose = GetTexture("UIClose");
        }
    }
}
