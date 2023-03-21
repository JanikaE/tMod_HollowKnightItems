namespace HollowKnightItems.Assets
{
    internal class TextureLoader : ModSystem
    {
        public static Asset<Texture2D> GrimmDeath;

        public override void Load()
        {
            GrimmDeath = GetTexture("GrimmDeath");
        }
    }
}
