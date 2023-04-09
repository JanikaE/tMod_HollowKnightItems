using HollowKnightItems.Common.Systems;
using HollowKnightItems.Content.Items;
using HollowKnightItems.Content.Items.Charms;
using HollowKnightItems.Content.Projectiles;
using Terraria.GameContent.Bestiary;

namespace HollowKnightItems.Content.NPCs
{
    internal class Void : ModNPC
    {
        private readonly int ID = NPCID.Wizard; // 动画套用巫师的数据
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void");
            DisplayName.AddTranslation(7, "虚空");

            Main.npcFrameCount[Type] = Main.npcFrameCount[ID];
            NPCID.Sets.ExtraFramesCount[Type] = NPCID.Sets.ExtraFramesCount[ID];
            NPCID.Sets.AttackFrameCount[Type] = NPCID.Sets.AttackFrameCount[ID];
            NPCID.Sets.AttackType[Type] = 2;  // 魔法型攻击
            NPCID.Sets.AttackTime[Type] = NPCID.Sets.AttackTime[ID];
            NPCID.Sets.MagicAuraColor[Type] = Color.Black;
            NPCID.Sets.DangerDetectRange[Type] = NPCID.Sets.DangerDetectRange[ID];
            NPCID.Sets.AttackAverageChance[Type] = NPCID.Sets.AttackAverageChance[ID];

            // 图鉴设置
            NPCID.Sets.TownNPCBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
            {
                //为NPC设置图鉴展示状态，赋予其Velocity即可展现出行走姿态
                Velocity = 1f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            // 幸福度设置

        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;
            NPC.damage = 20;
            NPC.defense = 15;
            NPC.lifeMax = 250;

            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.aiStyle = NPCAIStyleID.Passive;

            AnimationType = ID;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
                // 图鉴内描述
                new FlavorTextBestiaryInfoElement("")
            }) ;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return true;
        }

        public override List<string> SetNPCNameList()
        {
            // 随机姓名
            return new List<string>() { 
                "666"
            };
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            // 无性别
            return true;
        }

        public override string GetChat()
        {
            return GetNPCChat(Name, 3);
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            // 如果按下第一个按钮，则开启商店
            if (firstButton)
            {
                shop = true;
            }
            // 在if之后可以写第二个按钮的作用
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[0].SetDefaults(ModContent.ItemType<NightmareLantern_OFF>());
            if (DownedBossSystem.downedGrimm)
            {
                shop.item[10].SetDefaults(ModContent.ItemType<Grimmchild>());
                shop.item[11].SetDefaults(ModContent.ItemType<CarefreeMelody>());
            }
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = NPC.damage;
            knockback = 3f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 60;
            randExtraCooldown = 0;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<VoidSoul>();
            attackDelay = 10;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 10f;
            gravityCorrection = 0f;
            randomOffset = 0.2f;
        }
    }
}
