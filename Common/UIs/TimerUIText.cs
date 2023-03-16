namespace HollowKnightItems.Common.UIs
{
    /// <summary>
    /// 带计时器的UIText
    /// </summary>
    internal class TimerUIText : UIText
    {
        /// <summary>
        /// 计时器
        /// </summary>
        public int timer;

        public TimerUIText(string text, float textScale = 1, bool large = false, int timer = -1) : base(text, textScale, large)
        {
            this.timer = timer;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (timer > 0)
            {
                timer--;
            }
        }
    }
}
