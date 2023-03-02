using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HollowKnightItems.Content.NPCs
{
    [AutoloadBossHead]
    internal class Grimm : ModNPC
    {
        public enum ProjectileState
        {
            MoveAround,
            ShootAround,
            Teleport,
            Rest,
        }
        public ProjectileState State
        {
            get { return (ProjectileState)(int)NPC.ai[1]; }
            set { NPC.ai[1] = (int)value; }
        }

        public override void SetStaticDefaults()
        {
            // 这里以后写到Localization里去
            DisplayName.SetDefault("Grimm");
            DisplayName.AddTranslation(7, "格林团长");
            //Main.npcFrameCount[NPC.type] = 3;
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,
                }
            };
        }

        public override void SetDefaults()
        {
            NPC.width = 0;
            NPC.height = 0;  // 这两个代表这个NPC的碰撞箱宽高，以及tr会从你的贴图里扣多大的图
            NPC.damage = 0;
            NPC.lifeMax = 8000;  // npc的血量上限
            NPC.defense = 20;
            NPC.scale = 1f;  // npc的贴图和碰撞箱的放缩倍率
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit5;  // 挨打时发出的声音
            NPC.DeathSound = SoundID.NPCDeath7;  // 趋势时发出的声音
            NPC.value = Item.buyPrice(0, 4, 50, 0);  // NPC的爆出来的MONEY的数量，四个空从左到右是铂金，金，银，铜
            NPC.lavaImmune = true;  // 对岩浆免疫
            NPC.noGravity = true;  // 不受重力影响
            NPC.noTileCollide = true;  // 可穿墙
            NPC.npcSlots = 20;  // NPC所占用的NPC数量
            NPC.boss = true;  // 将npc设为boss。会掉弱治药水和心，会显示xxx已被击败，会有血条
            NPC.dontTakeDamage = false;  // 为true则为无敌，这里的无敌意思是弹幕不会打到npc，并且npc的血条也不会显示了
            Music = MusicLoader.GetMusicSlot("HollowKnightItems/Assets/Music/Grimm");
        }

        public override void AI()
        {
            
        }
    }
}
