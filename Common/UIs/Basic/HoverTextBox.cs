namespace HollowKnightItems.Common.UIs.Basic
{
    internal class HoverTextBox : UITextBox
    {
        public Color defaultColor;
        public Color hoverColor;

        public HoverTextBox(string text, Color defaultColor, Color hoverColor, float textScale = 1, bool large = false) : base(text, textScale, large)
        {
            this.defaultColor = defaultColor;
            this.hoverColor = hoverColor;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering)
            {
                BackgroundColor = hoverColor;
            }
            else
            {
                BackgroundColor = defaultColor;
            }
        }
    }
}
