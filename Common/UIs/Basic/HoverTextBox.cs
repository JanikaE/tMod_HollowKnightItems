namespace HollowKnightItems.Common.UIs.Basic
{
    internal class HoverTextBox : UITextBox
    {
        public Color defaultColor;
        public Color hoverColor;
        public string hoverText;

        public HoverTextBox(string text, Color defaultColor, Color hoverColor, string hoverText, float textScale = 1, bool large = false) : base(text, textScale, large)
        {
            this.defaultColor = defaultColor;
            this.hoverColor = hoverColor;
            this.hoverText = hoverText;
        }

        public void SetHoverText(string text)
        {
            hoverText = text;
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (IsMouseHovering)
            {
                Main.hoverItemName = hoverText;
            }
        }
    }
}
