namespace HollowKnightItems.Common.UIs.Basic
{
    internal class ItemImage : UIElement
    {
        private int[] ItemID;
        private Texture2D Image;
        private string Text;

        private int Timer;
        private int Index;

        public ItemImage(int[] ID)
        {
            ItemID = ID;
        }

        public override void OnInitialize()
        {
            foreach (int i in ItemID)
            {
                if (i != 0)
                {
                    Main.instance.LoadItem(i);
                }
            }
            Timer = 0;
            Index = 0;
        }

        public void ChangeItem(int[] NewID)
        {
            if (NewID != ItemID)
            {
                ItemID = NewID;
                OnInitialize();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ItemID[Index] != 0)
            {
                Image = TextureAssets.Item[ItemID[Index]].Value;
                Text = new Item(ItemID[Index]).Name;
            }
            else
            {
                Image = null;
            }

            Timer++;
            if (Timer >= 60)
            {
                Timer = 0;
                Index = (Index + 1) % ItemID.Length;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (Image != null)
            {
                Vector2 position = GetDimensions().Position();
                Rectangle? sourceRect = Image.Height > Image.Width * 3 ? Image.ToSquare() : null;
                spriteBatch.Draw(Image, position, sourceRect, Color.White, 0, Vector2.Zero, 1f, 0, 0f);

                if (IsMouseHovering)
                {
                    Main.hoverItemName = Text;
                }
            }
        }
    }
}
