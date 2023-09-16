using HollowKnightItems.Common.Players;
using HollowKnightItems.Content.Rarities;

namespace HollowKnightItems.Content.Items.Charms
{
    internal class CarefreeMelody : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.hasVanityEffects = true;
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
                TooltipLine Line = new(Mod, "CarefreeProbability", GetText("Items.CarefreeMelody.Probability") + player.GetModPlayer<CarefreeMelodyPlayer>().CarefreeOdds.ToString() + "%");
                tooltips.Add(Line);
            }
        }
    }
}