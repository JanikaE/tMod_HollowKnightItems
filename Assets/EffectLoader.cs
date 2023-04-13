namespace HollowKnightItems.Assets
{
    internal class EffectLoader : ModSystem
    {
        public static Effect Fireball;
        public static Effect Text;

        public override void Load()
        {
            // Screen Shader

            // Pixel Shader
            Fireball = GetEffect("Fireball").Value;
            Text = GetEffect("Text").Value;
        }

        public static void ApplyEffect_Text(Vector2 imageSize)
        {
            Text.Parameters["uImageSize"].SetValue(imageSize);
            Text.CurrentTechnique.Passes["Test"].Apply();
        } 

        public static void ApplyEffect_Fireball(Color centerColor, Color edgeColor)
        {
            Vector4 center = centerColor.ToVector4();
            Vector4 edge = edgeColor.ToVector4();
            Fireball.Parameters["uColorCenter"].SetValue(center);
            Fireball.Parameters["uColorEdge"].SetValue(edge);
            Fireball.CurrentTechnique.Passes["Test"].Apply();
        }
    }
}
