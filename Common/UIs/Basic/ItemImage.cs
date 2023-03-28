using System.Runtime.CompilerServices;

namespace HollowKnightItems.Common.UIs.Basic
{
    internal class ItemImage : UIElement
    {
        public int ItemID;
        public Texture2D Image;
        public string Text;

        public ItemImage(int ID)
        {
            ItemID = ID;
        }

        public override void OnInitialize()
        {
            Main.instance.LoadItem(ItemID);
            Image = TextureAssets.Item[ItemID].Value;
            Text = new Item(ItemID).Name;
        }

        public void ChangeItem(int NewID)
        {
            ItemID = NewID;
            Main.instance.LoadItem(ItemID);
            Image = TextureAssets.Item[ItemID].Value;
            Text = new Item(ItemID).Name;
        }

        public override void Update(GameTime gameTime)
        {
            Image = TextureAssets.Item[ItemID].Value;
            Text = new Item(ItemID).Name;            
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
