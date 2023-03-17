using HollowKnightItems.Common.UIs;
using HollowKnightItems.Content.NPCs;
using HollowKnightItems.Content.Rarities;

namespace HollowKnightItems.Content.Items
{
    [Autoload(false)]
    internal class NightmareLantern : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nightmare Lantern");
            DisplayName.AddTranslation(7, "梦魇之灯");
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 100;
            Item.maxStack = 1;
            Item.value = 100;
            Item.rare = ModContent.RarityType<CharmRarity>();
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new(Mod, "NightmareLantern", Language.GetTextValue("Mods.HollowKnightItems.Items.NightmareLantern.Default"));
            tooltips.Add(line);
        }
    }

    [Autoload(true)]
    internal class NightmareLantern_OFF : NightmareLantern
    {
        public override bool CanUseItem(Player player)
        {
            // If you decide to use the below UseItem code, you have to include !NPC.AnyNPCs(id), as this is also the check the server does when receiving MessageID.SpawnBoss.
            return !NPC.AnyNPCs(ModContent.NPCType<GrimmBoss>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                int type = ModContent.NPCType<GrimmBoss>();
                Vector2 position = player.Bottom + new Vector2(300, 0);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // If the player is not in multiplayer, spawn directly
                    NPC.SpawnBoss((int)position.X, (int)position.Y, type, player.whoAmI);
                }
                else
                {
                    // If the player is in multiplayer, request a spawn
                    NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: type);
                }

                ScreenText.NewScreenText(Language.GetTextValue("Mods.HollowKnightItems.NPCs.Grimm.SpawnInfo1"), 2, 200, 0.1f, 0.8f);
                ScreenText.NewScreenText(Language.GetTextValue("Mods.HollowKnightItems.NPCs.Grimm.SpawnInfo2"), 5, 200, 0.1f, 0.9f);
            }
            return true;
        }

        public override void UpdateInventory(Player player)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<GrimmBoss>()))
            {
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    if (player.inventory[i].type == Item.type)
                    {
                        bool flag = player.inventory[i].favorited;
                        player.inventory[i].SetDefaults(ModContent.ItemType<NightmareLantern_ON>());
                        player.inventory[i].favorited = flag;
                    }
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            TooltipLine line = new(Mod, "NightmareLanternOFF", Language.GetTextValue("Mods.HollowKnightItems.Items.NightmareLantern.OFF"));
            tooltips.Add(line);
        }
    }

    [Autoload(true)]
    internal class NightmareLantern_ON : NightmareLantern
    {
        public override void UpdateInventory(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<GrimmBoss>()))
            {
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    if (player.inventory[i].type == Item.type)
                    {
                        bool flag = player.inventory[i].favorited;
                        player.inventory[i].SetDefaults(ModContent.ItemType<NightmareLantern_OFF>());
                        player.inventory[i].favorited = flag;
                    }
                }
            }
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            TooltipLine line = new(Mod, "NightmareLanternON", Language.GetTextValue("Mods.HollowKnightItems.Items.NightmareLantern.ON"));
            tooltips.Add(line);
        }
    }
}
