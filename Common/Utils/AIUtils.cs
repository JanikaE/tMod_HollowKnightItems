using HollowKnightItems.Common.Systems;
using HollowKnightItems.Content.Dusts;

namespace HollowKnightItems.Common.Utils
{
    /// <summary>
    /// 与NPC/Projectile的AI/数值相关的工具
    /// </summary>
    internal static class AIUtils
    {
        public static int[] bossList = {
            NPCID.KingSlime,  // 史莱姆王
            NPCID.EyeofCthulhu,  // 克苏鲁之眼
            NPCID.BrainofCthulhu,  // 克苏鲁之脑
            NPCID.EaterofWorldsHead,  // 世界吞噬怪
            NPCID.QueenBee,  // 蜂王
            NPCID.Skeleton,  // 骷髅王
            NPCID.Deerclops,  // 独眼巨鹿
            NPCID.WallofFlesh,  // 血肉墙

            NPCID.QueenSlimeBoss,  // 史莱姆皇后
            NPCID.Retinazer,  // 激光眼
            NPCID.Spazmatism,  // 魔焰眼
            NPCID.TheDestroyer,  // 毁灭者
            NPCID.SkeletronPrime,  // 机械骷髅王
            NPCID.Plantera,  // 世纪之花
            NPCID.Golem,  // 石巨人
            NPCID.DukeFishron,  // 猪龙鱼公爵
            NPCID.HallowBoss,  // 光之女皇
            NPCID.CultistBoss,  // 拜月教邪教徒
            NPCID.MoonLordCore,  // 月亮领主

            NPCID.DD2DarkMageT1,  // 黑暗魔法师
            NPCID.DD2OgreT2,  // 食人魔
            NPCID.DD2Betsy,  // 双足飞龙
            NPCID.PirateShip,  // 荷兰飞盗船
            NPCID.MourningWood,  // 哀木
            NPCID.Pumpking,  // 南瓜王
            NPCID.Everscream,  // 常绿尖叫怪
            NPCID.SantaNK1,  // 圣诞坦克
            NPCID.IceQueen,  // 冰雪女王
            NPCID.MartianSaucer  // 火星飞碟
        };

        public static bool[] BossDown => new bool[]{
            NPC.downedSlimeKing,
            NPC.downedBoss1,
            NPC.downedBoss2,
            NPC.downedBoss3,
            NPC.downedQueenBee,
            NPC.downedDeerclops,
            Main.hardMode,

            NPC.downedQueenSlime,
            NPC.downedMechBoss1,
            NPC.downedMechBoss2,
            NPC.downedMechBoss3,
            NPC.downedPlantBoss,
            NPC.downedGolemBoss,
            NPC.downedFishron,
            NPC.downedEmpressOfLight,
            NPC.downedAncientCultist,
            NPC.downedMoonlord,

            NPC.downedGoblins,
            NPC.downedPirates,
            NPC.downedHalloweenTree,
            NPC.downedHalloweenKing,
            NPC.downedChristmasTree,
            NPC.downedChristmasSantank,
            NPC.downedChristmasIceQueen,
            NPC.downedMartians,

            DownedBossSystem.downedGrimm
        };

        /// <summary>
        /// 获取格林之子的攻击力
        /// </summary>
        public static int GetGrimmchildAttack()
        {
            int damage = 5;
            for (int i = 0; i < BossDown.Length; i++)
            {
                if (BossDown[i])
                {
                    damage += 4;
                }
            }
            return damage;
        }

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
        /// 获取某个NPC(一般是Boss)的目标玩家
        /// </summary>
        /// <param name="type">NPC类型</param>
        public static Player GetNPCTarget(int type)
        {
            Player player = null;
            foreach (NPC n in Main.npc)
            {
                if (n.type == type)
                {
                    player = Main.player[n.target];
                }
            }
            return player;
        }

        /// <summary>
        /// 限定在一个正方形内随机运动
        /// </summary>
        /// <param name="entity">实体对象</param>
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
        /// <param name="entity">实体对象</param>
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
        /// 弹幕分裂
        /// </summary>
        /// <param name="projectile">分裂前的弹幕对象</param>
        /// <param name="type">分裂产生的弹幕类型</param>
        /// <param name="num">弹幕数量</param>
        /// <param name="oldVelocity">分裂前的弹幕速度, 若为0则忽略旧的速度</param>
        /// <param name="velocity">分裂后的弹幕相对速率</param>
        /// <param name="damage">弹幕伤害</param>
        /// <param name="randomOffset">是否增加随机的角度偏移</param>
        /// <param name="ai0">弹幕ai[0]初值</param>
        /// <param name="ai1">弹幕ai[1]初值</param>
        public static void ProjectileSplit(Projectile projectile, int type, int num, Vector2 oldVelocity, float velocity, int damage, bool randomOffset = true, float ai0 = 0, float ai1 = 0)
        {
            int offset = randomOffset ? random.Next(360) : 0;
            for (int i = 0; i < 360; i += 360 / num)
            {
                float dir = projectile.velocity.ToRotation() + ((i + offset) * MathHelper.Pi / 180);
                Vector2 vel = dir.ToRotationVector2() * velocity + oldVelocity;
                Projectile.NewProjectile(projectile.GetSource_FromAI(),
                                        projectile.position,
                                        vel,
                                        type,
                                        damage,
                                        projectile.knockBack,
                                        projectile.owner,
                                        ai0,
                                        ai1);
            }
        }

        /// <summary>
        /// 弹幕拖尾的Dust
        /// </summary>
        /// <param name="projectile">弹幕对象</param>
        /// <param name="type">Dust类型</param>
        /// <param name="num">Dust数量</param>
        /// <param name="scale">Dust大小</param>
        public static void TailDust(Projectile projectile, int type, int num, float scale = 1)
        {
            for (int i = 0; i < num; i++)
            {
                Dust.NewDust(projectile.position - projectile.velocity, projectile.width, projectile.height, type, newColor: new Color(255, 89, 89), Scale: scale);
            }
        }

        /// <summary>
        /// 弹幕碰撞的Dust
        /// </summary>
        /// <param name="position">碰撞位置</param>
        /// <param name="type">Dust类型</param>
        public static void HitDust(Vector2 position, int type, Color color = default)
        {
            for (int i = 0; i < 12; i++)
            {
                Vector2 rotation = (i * MathHelper.Pi / 60).ToRotationVector2();
                Dust.NewDust(position, 1, 1, type, rotation.X, rotation.Y, newColor: color);
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
        /// <param name="entity">实体对象</param>
        /// <param name="color">Dust颜色</param>
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
