using HollowKnightItems.Assets;
using HollowKnightItems.Common.Systems;
using HollowKnightItems.Content.Dusts;
using HollowKnightItems.Content.Items;
using HollowKnightItems.Content.Items.Charms;
using HollowKnightItems.Content.Projectiles;

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
            NPC.Happiness.SetBiomeAffection<UndergroundBiome>(AffectionLevel.Love)
                        .SetBiomeAffection<JungleBiome>(AffectionLevel.Like)
                        .SetBiomeAffection<OceanBiome>(AffectionLevel.Dislike)
                        .SetBiomeAffection<HallowBiome>(AffectionLevel.Hate)
                        .SetNPCAffection(NPCID.Guide, AffectionLevel.Love)
                        .SetNPCAffection(NPCID.Wizard, AffectionLevel.Like)
                        .SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike);
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
            NPC.HitSound = SoundLoader.Creature_Hit;

            AnimationType = ID;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            NPCHitDust(NPC, hitDirection, Color.Black);
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 24; i++)
                {
                    float rotation = (float)(i * Math.PI / 3);
                    Vector2 dir = rotation.ToRotationVector2() * 20;
                    Dust.NewDust(NPC.Center, 0, 0, ModContent.DustType<Hit>(), dir.X, dir.Y, newColor: new Color(0, 0, 0));
                }
            }
        }

        public override bool SpecialOnKill()
        {
            Main.NewText(GetText("NPCs.Void.DeathInfo"));
            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
                new FlavorTextBestiaryInfoElement(GetNPCBestiary(Name))
            }) ;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money) => true;

        public override List<string> SetNPCNameList()
        {
            // 随机姓名
            return new List<string>() { 
                "Revek", "Milly", "Caspian", "Atra", "Chagax",
                "Garro", "Kcin", "Karina", "Warrior", "Grohac",
                "Perpetos", "Molten", "Magnus", "Waldie", "Wayner",
                "Wyatt", "Hex", "Thistlewind", "Boss"
            };
        }

        // 无性别
        public override bool CanGoToStatue(bool toKingStatue) => true;

        public override string GetChat()
        {
            WeightedRandom<string> chats = GetNPCChat(Name, 4);
            if (DownedBossSystem.downedGrimm)
            {
                chats.Add(GetNPCChat(Name, "Grimmchild"));
            }
            return chats;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            // 第一个按钮的作用
            if (firstButton)
            {
                shop = true;
            }
            // 第二个按钮的作用
            else
            {
                
            }
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
