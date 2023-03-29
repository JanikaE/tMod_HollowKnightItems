using HollowKnightItems.Common.Systems;

namespace HollowKnightItems.Common.Utils
{
    /// <summary>
    /// 自用的一些工具
    /// </summary>
    internal static class MyUtils
    {
        public static Random random = new();

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

        public static bool[] bossDown = {
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
            for (int i = 0; i < bossDown.Length; i++)
            {
                if (bossDown[i])
                {
                    damage += 4;
                }
            }
            return damage;
        }

        public static void ArrayToDefault(ref int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }
        }

        public static void ArrayToDefault(ref bool[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = false;
            }
        }

        public static void ArrayToDefault(ref Vector2[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Vector2.Zero;
            }
        }

        public static Vector2 GetSize(this UIElement uie)
        {
            return new(uie.Width.Pixels, uie.Height.Pixels);
        }

        public static Rectangle ToSquare(this Texture2D texture)
        {
            int r = Math.Min(texture.Width, texture.Height);
            return new Rectangle(0, 0, r, r);
        }

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="path">路径为Mods.HollowKnightItems.*</param>
        public static string GetText(string path)
        {
            return Language.GetTextValue($"Mods.HollowKnightItems.{path}");
        }

        /// <summary>
        /// 获取Effect
        /// </summary>
        /// <param name="fileName">路径为Assets/Effects/Content/*</param>
        public static Asset<Effect> GetEffect(string fileName)
        {
            return ModContent.Request<Effect>($"HollowKnightItems/Assets/Effects/Content/{fileName}", AssetRequestMode.ImmediateLoad);
        }

        /// <summary>
        /// 获取Texture2D
        /// </summary>
        /// <param name="fileName">路径为Assets/Textures/*</param>
        public static Asset<Texture2D> GetTexture(string fileName)
        {
            return ModContent.Request<Texture2D>($"HollowKnightItems/Assets/Textures/{fileName}", AssetRequestMode.ImmediateLoad);
        }

        /// <summary>
        /// 获取Sound
        /// </summary>
        /// <param name="fileName">路径为Assets/Sounds/*</param>
        /// <param name="numVariants">随机选取的目标数量</param>
        /// <param name="Volume">音量</param>
        /// <param name="Pitch">音调</param>
        /// <param name="PitchVariance">音调随机浮动</param>
        /// <param name="MaxInstances">叠加播放上限</param>
        /// <param name="SoundLimitBehavior">到达上限之后的操作</param>
        /// <param name="Type">声音类型（音乐/音效/环境）</param>
        public static SoundStyle GetSoundStyle(string fileName, int numVariants, float Volume, float Pitch = 0, float PitchVariance = 0, int MaxInstances = 1, SoundLimitBehavior SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, SoundType Type = SoundType.Sound)
        {
            return new SoundStyle($"HollowKnightItems/Assets/Sounds/{fileName}", numVariants) with {
                Volume = Volume,
                Pitch = Pitch,
                PitchVariance = PitchVariance,
                MaxInstances = MaxInstances,
                SoundLimitBehavior = SoundLimitBehavior,
                Type = Type
            };
        }
    }
}
