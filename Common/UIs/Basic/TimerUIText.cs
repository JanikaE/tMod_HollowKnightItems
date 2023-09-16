using HollowKnightItems.Assets;

namespace HollowKnightItems.Common.UIs.Basic
{
    /// <summary>
    /// 带计时器的UIText
    /// </summary>
    internal class TimerUIText : UIText
    {
        /// <summary>
        /// 计时器
        /// </summary>
        public int Timer;

        public TimerUIText(string text, float textScale = 1, bool large = false, int timer = -1) : base(text, textScale, large)
        {
            Timer = timer;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Timer > 0)
            {
                Timer--;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            EffectLoader.ApplyEffect_Text(0.25f);
        }
    }
}
