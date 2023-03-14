namespace HollowKnightItems.Common.Systems
{
    internal class RectSystem : ModSystem
    {
        public static Rect[] rects = new Rect[50];

        public override void OnWorldLoad()
        {
            Rect.ClearRect();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // 寻找绘制层，并且返回那一层的索引
            int Index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
            if (Index != -1)
            {
                // 往绘制层集合插入一个成员，第一个参数是插入的地方的索引，第二个参数是绘制层
                layers.Insert(Index, new LegacyGameInterfaceLayer(
                    // 绘制层的名字
                    "Test : Boss",
                    // 匿名方法
                    delegate
                    {
                        DrawRect();
                        return true;
                    },
                    // 绘制层的类型
                    InterfaceScaleType.Game)
                );
            }
        }

        public static void DrawRect()
        {
            for (int i = 0; i < rects.Length; i++)
            {
                rects[i]?.DrawRect();             
            }
        }
    }

    internal class Rect 
    {
        public Rectangle rectangle;
        public Color color;

        public Rect(Rectangle rectangle, Color color)
        {
            this.rectangle = rectangle;
            this.color = color;
        }

        public static void NewRect(int x, int y, int width, int height, Color color)
        {
            Rectangle rectangle = new(x, y, width, height);
            NewRect(rectangle, color);
        }

        public static void NewRect(Vector2 center, int halfWidth, int halfHeight, Color color)
        {
            Rectangle rectangle = new((int)center.X - halfWidth, (int)center.Y - halfHeight, halfWidth * 2 + 1, halfHeight * 2 + 1);
            NewRect(rectangle, color);
        }

        public static void NewRect(Rectangle rectangle, Color color)
        {
            Rect rect = new(rectangle, color);
            for (int i = 0; i < RectSystem.rects.Length; i++)
            {
                if (RectSystem.rects[i] == null)
                {
                    RectSystem.rects[i] = rect;
                    break;
                }
            }
        }

        public static void ClearRect()
        {
            for (int i = 0; i < RectSystem.rects.Length; i++)
            {
                RectSystem.rects[i] = null;
            }
        }

        public void DrawRect()
        {
            Texture2D texture = TextureAssets.MagicPixel.Value;
            Main.spriteBatch.Draw(texture,
                                rectangle.TopLeft() - Main.screenPosition,
                                new(0, 0, 1, 1),  // 填null会出问题
                                color * 0.5f,
                                0f,
                                Vector2.Zero,
                                rectangle.Size(),
                                SpriteEffects.None,
                                0f);
        }
    }
}
