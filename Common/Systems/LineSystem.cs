using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace HollowKnightItems.Common.Systems
{
    internal class LineSystem : ModSystem
    {
        public static Line[] lines = new Line[50];

        public override void OnWorldLoad()
        {
            Line.ClearLine();
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
                    "Test : Preview",
                    // 匿名方法
                    delegate
                    {
                        DrawLine();
                        return true;
                    },
                    // 绘制层的类型
                    InterfaceScaleType.Game)
                );
            }
        }

        public void DrawLine()
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != null)
                {
                    lines[i].DrawLine();
                }
            }
        }
    }

    internal class Line 
    {
        public float x;
        public Color color;

        public Line(float x, Color color)
        {
            this.x = x;
            this.color = color;
        }

        public static void NewLine(float x, Color color)
        {
            Line line = new(x, color);
            for (int i = 0; i < LineSystem.lines.Length; i++)
            {
                if (LineSystem.lines[i] == null)
                {
                    LineSystem.lines[i] = line;
                    break;
                }
            }            
        }

        public static void ClearLine()
        {
            for (int i = 0; i < LineSystem.lines.Length; i++)
            {
                LineSystem.lines[i] = null;
            }
        }

        public void DrawLine()
        {
            Texture2D texture = TextureAssets.MagicPixel.Value;
            Main.spriteBatch.Draw(texture,
                                new Vector2(x - 1, 0) - Main.screenPosition,
                                null,
                                color,
                                0f,
                                Vector2.Zero,
                                new Vector2(3, Main.screenHeight),
                                SpriteEffects.None,
                                0f);
        }
    }
}
