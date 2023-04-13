namespace HollowKnightItems.Common.Utils
{
    /// <summary>
    /// 与Items/Tile相关的一些工具
    /// </summary>
    internal static class ObjUtils
    {
        public static readonly int[] Chairs =
        {
            15
        };

        /// <summary>
        /// 获取最近的物块位置
        /// </summary>
        /// <param name="position">中心位置</param>
        /// <param name="tileType">目标物块</param>
        public static Vector2? FindClosestTile(Vector2 position, int[] tileType)
        {
            Vector2? target = null;
            float distance = float.MaxValue;

            // 遍历地图中所有物块
            for (int i = 0; i < Main.mapMaxX; i++) 
            {
                for (int j = 0; j < Main.mapMaxY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tileType.Contains(tile.TileType))
                    {
                        Vector2 newTarget = new Point(i, j).ToWorldCoordinates();
                        float newDistance = position.Distance(newTarget); 
                        if (newDistance < distance)
                        {
                            distance = newDistance;
                            target = newTarget;
                        }
                    }
                }
            }

            return target;
        }
    }
}
