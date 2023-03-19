using HollowKnightItems.Content.Dusts;

namespace HollowKnightItems.Common.Utils
{
    /// <summary>
    /// 与NPC/Projectile的AI相关的工具
    /// </summary>
    internal static class AIUtils
    {
        /// <summary>
        /// 友方单位索敌
        /// </summary>
        /// <param name="position">索敌中心</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="predicate">额外条件</param>
        public static NPC FindClosestEnemy(Vector2 position, float maxDistance, Func<NPC, bool> predicate)
        {
            NPC res = null;
            foreach (var npc in Main.npc.Where(n => n.active && !n.friendly && predicate(n)))
            {
                float dis = Vector2.Distance(position, npc.Center);
                if (dis < maxDistance)
                {
                    maxDistance = dis;
                    res = npc;
                }
            }
            return res;
        }

        /// <summary>
        /// 敌方单位索敌
        /// </summary>
        /// <param name="position">索敌中心</param>
        /// <param name="maxDistance">最大距离</param>
        public static Player FindClosestPlayer(Vector2 position, float maxDistance)
        {
            Player res = null;
            foreach (var player in Main.player)
            {
                float dis = Vector2.Distance(position, player.Center);
                if (dis < maxDistance)
                {
                    maxDistance = dis;
                    res = player;
                }
            }
            return res;
        }

        /// <summary>
        /// 限定在一个正方形内随机运动
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="center">正方形的中心</param>
        /// <param name="distance">边界与中心的距离</param>
        public static void MoveBetween(Entity entity, Vector2 center, int distance)
        {
            Vector2 newDistance = new(distance, distance);
            MoveBetween(entity, center, newDistance);
        }

        /// <summary>
        /// 限定在一个矩形内随机运动
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="center">矩形的中心</param>
        /// <param name="distance">边界与中心的距离</param>
        public static void MoveBetween(Entity entity, Vector2 center, Vector2 distance)
        {
            // x轴
            float tarX = center.X;
            if (random.Next(30) == 0)
            {
                entity.velocity.X *= -1;
            }
            if (entity.Center.X - tarX > distance.X && entity.velocity.X > 0 ||
                entity.Center.X - tarX < -distance.X && entity.velocity.X < 0)
            {
                entity.velocity.X *= -1;
            }
            // y轴
            float tarY = center.Y;
            if (random.Next(30) == 0)
            {
                entity.velocity.Y *= -1;
            }
            if (entity.Center.Y - tarY > distance.Y && entity.velocity.Y > 0 ||
                entity.Center.Y - tarY < -distance.Y && entity.velocity.Y < 0)
            {
                entity.velocity.Y *= -1;
            }
        }

        /// <summary>
        /// 弹幕拖尾的Dust
        /// </summary>
        /// <param name="projectile">弹幕实体</param>
        /// <param name="type">Dust类型</param>
        /// <param name="num">Dust数量</param>
        public static void TailDust(Projectile projectile, int type, int num)
        {
            for (int i = 0; i < num; i++)
            {
                Dust.NewDust(projectile.position - projectile.velocity, projectile.width, projectile.height, type, newColor: new Color(255, 89, 89));
            }
        }

        /// <summary>
        /// Boss开局吼的Dust
        /// </summary>
        public static void RoarDust(Vector2 center)
        {
            for (int i = 0; i < 36; i++)
            {
                float r = (float)(i * Math.PI / 18);
                Vector2 vel = r.ToRotationVector2() * 40;
                Dust.NewDust(center, 0, 0, ModContent.DustType<RoarWave>(), vel.X, vel.Y);
            }
        }

        /// <summary>
        /// 给碰撞箱描框的Dust
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="color"></param>
        public static void RoundHitboxDust(Entity entity, Color color) 
        {
            int type = ModContent.DustType<StaticPoint>();
            Dust.NewDust(entity.TopLeft, 0, 0, type, 10, 0, newColor: color);
            Dust.NewDust(entity.TopRight, 0, 0, type, 0, 10, newColor: color);
            Dust.NewDust(entity.BottomRight, 0, 0, type, -10, 0, newColor: color);
            Dust.NewDust(entity.BottomLeft, 0, 0, type, 0, -10, newColor: color);
        }
    }
}
