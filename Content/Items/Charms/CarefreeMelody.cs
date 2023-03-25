using HollowKnightItems.Common.Players;
using HollowKnightItems.Content.Rarities;

namespace HollowKnightItems.Content.Items.Charms
{
    internal class CarefreeMelody : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Carefree Melody");
            DisplayName.AddTranslation(7, "无忧旋律");
            Tooltip.SetDefault("Token commemorating the start of a friendship.\n" +
                                "Contains a song of protection that may defend the bearer from damage.");
            Tooltip.AddTranslation(7, "纪念一份友谊建立的信物。\n" +
                                "包含一首可能使持有者免受伤害的守护之歌。");
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.rare = ModContent.RarityType<CharmRarity>();

            Item.value = 100;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CarefreeMelodyPlayer>().HasCarefreeMelody = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.GetModPlayer<CarefreeMelodyPlayer>().HasCarefreeMelody)
            {
                TooltipLine Line = new(Mod, "CarefreeProbability", Language.GetTextValue("Mods.HollowKnightItems.Items.CarefreeMelody.Probability") +
                                                                    ":" +
                                                                    player.GetModPlayer<CarefreeMelodyPlayer>().CarefreeOdds.ToString() + 
                                                                    "%");
                tooltips.Add(Line);
            }
        }
    }
}