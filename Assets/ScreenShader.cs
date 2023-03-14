namespace HollowKnightItems.Assets
{  
    internal class ScreenShader : ScreenShaderData
    {
        public ScreenShader(string passName) : base(passName)
        {
        }

        public ScreenShader(Ref<Effect> shader, string passName) : base(shader, passName)
        {
        }

        public override void Apply()
        {
            base.Apply();
        }
    }
}
