using HollowKnightItems.Common.Players;
using HollowKnightItems.Content.Buffs;
using HollowKnightItems.Content.Projectiles.Grimmchild;
using HollowKnightItems.Content.Rarities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HollowKnightItems.Content.Items.Charms
{
    internal class Grimmchild : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grimmchild");
            DisplayName.AddTranslation(7, "格林之子");
            Tooltip.SetDefault("Symbol of a completed ritual. Contains a living, scarlet flame.");
            Tooltip.AddTranslation(7, "一场完成的仪式的标志。包含着一团跳动的猩红之火。");
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
            Summon(player);
            player.GetModPlayer<CharmsPlayer>().GrimmchildType = true;
        }

        public override void UpdateVanity(Player player)
        {
            Summon(player);
            player.GetModPlayer<CharmsPlayer>().GrimmchildType = false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.GetModPlayer<CharmsPlayer>().HasGrimmchild)
            {
                TooltipLine Line = new(Mod, "GrimmchildDamage", Language.GetTextValue("Mods.HollowKnightItems.Items.Grimmchild.Damage") +
                                                                        ":" +
                                                                        GetGrimmchildAttack());
                tooltips.Add(Line);
            }
        }

        private void Summon(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GrimmchildSummon>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item),
                    player.Center,
                    new Vector2(0, 0),
                    ModContent.ProjectileType<GrimmchildSummon>(),
                    0,
                    0,
                    player.whoAmI);
            }
            player.AddBuff(ModContent.BuffType<GrimmchildBuff>(), 2);
            player.GetModPlayer<CharmsPlayer>().HasGrimmchild = true;
        }
    }
}