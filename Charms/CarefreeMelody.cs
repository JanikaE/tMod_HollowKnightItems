using HollowKnightItems.Players;
using HollowKnightItems.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HollowKnightItems.Charms
{
    internal class CarefreeMelody : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.rare = ModContent.RarityType<CharmRarity>();

            Item.value = 100;
        }

        public override void AddRecipes()
        {

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CharmsPlayer>().HasCarefreeMelody = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.GetModPlayer<CharmsPlayer>().HasCarefreeMelody)
            {
                TooltipLine tooltipLine = new(Mod, "CarefreeOdds", player.GetModPlayer<CharmsPlayer>().CarefreeOdds.ToString() + "%");
                tooltips.Add(tooltipLine);
            }
        }
    }
}