namespace HollowKnightItems.Common.Systems.DrawSystem
{
    internal class GraphSystem : ModSystem
    {
        public static Graph graph;

        public override void OnWorldLoad()
        {
            graph = null;
        }

        public override void PostUpdateEverything()
        {
            if (graph != null)
            {
                if (graph.timer == 0)
                {
                    graph = null;
                    return;
                }
                graph.timer--;
            }
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
                    "Test : Graph",
                    // 匿名方法
                    delegate
                    {
                        if (graph != null)
                        {
                            Main.spriteBatch.Draw(graph.texture,
                                graph.position - Main.screenPosition,
                                new Rectangle(0, 0, graph.texture.Width, graph.texture.Height),
                                Color.White,
                                0f,
                                Vector2.Zero,
                                1f,
                                SpriteEffects.None,
                                0f);
                        }
                        return true;
                    },
                    // 绘制层的类型
                    InterfaceScaleType.Game)
                );
            }
        }
    }

    internal class Graph
    {
        public Texture2D texture;
        public Vector2 position;
        public int timer;

        public Graph(Texture2D texture, Vector2 position, int timer)
        {
            this.texture = texture;
            this.position = position;
            this.timer = timer;
        }

        public static void NewGraph(Texture2D texture, Vector2 position, int timer)
        {
            Graph graph = new(texture, position, timer);
            GraphSystem.graph = graph;
        }

        public static void ClearGraph()
        {
            GraphSystem.graph = null;
        }
    }
}
