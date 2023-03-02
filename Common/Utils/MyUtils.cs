using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HollowKnightItems.Common.Utils
{
    internal static class MyUtils
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
            NPC.downedMartians
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

        public static Asset<Effect> GetEffect(string fileName)
        {
            return ModContent.Request<Effect>($"HollowKnightItems/Assets/Effects/Content/{fileName}", AssetRequestMode.ImmediateLoad);
        }
    }
}
