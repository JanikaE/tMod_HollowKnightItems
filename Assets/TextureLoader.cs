namespace HollowKnightItems.Assets
{
    internal class TextureLoader : ModSystem
    {
        public static Asset<Texture2D> GrimmDeath;
        public static Asset<Texture2D> UIDelete;

        public override void Load()
        {
            GrimmDeath = GetTexture("GrimmDeath");
            UIDelete = GetTexture("UIDelete");
        }
    }
}
