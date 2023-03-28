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
            foreach(int i in ItemID)
            {
                Main.instance.LoadItem(i);
            }
            Timer = 0;
            Index = 0;
        }

        public void ChangeItem(int[] NewID)
        {
            ItemID = NewID;
            OnInitialize();
        }

        public override void Update(GameTime gameTime)
        {
            Image = TextureAssets.Item[ItemID[Index]].Value;
            Text = new Item(ItemID[Index]).Name;
            Timer++;
            if (Timer == 180)
            {
                Timer = 0;
                Index = (Index + 1) % ItemID.Length;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {         
            base.Draw(spriteBatch);
            Vector2 position = GetDimensions().Position();
            spriteBatch.Draw(Image, position, null, Color.White, 0, Image.Size() / this.GetSize(), 1f, 0, 0f);

            if (IsMouseHovering)
            {
                Main.hoverItemName = Text;
            }
        }
    }
}
