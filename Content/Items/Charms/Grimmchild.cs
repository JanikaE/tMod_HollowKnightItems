using HollowKnightItems.Common.Players;
using HollowKnightItems.Content.Buffs;
using HollowKnightItems.Content.Projectiles;
using HollowKnightItems.Content.Rarities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HollowKnightItems.Content.Items.Charms
{
    internal class Grimmchild : ModItem
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
            player.GetModPlayer<CharmsPlayer>().GrimmchildType = true;
        }

        public override void UpdateVanity(Player player)
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
            player.GetModPlayer<CharmsPlayer>().GrimmchildType = false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.GetModPlayer<CharmsPlayer>().HasGrimmchild)
            {
                TooltipLine tooltipLine = new(Mod, "GrimmchildDamage", $"{GetGrimmchildAttack()}");
                tooltips.Add(tooltipLine);

                /*bool[] bools = new GrimmchildSummon().bossDown;
                for (int i = 0; i < bools.Length; i++)
                {
                    tooltipLine = new TooltipLine(Mod, "", $"{i} {bools[i]}");
                    tooltips.Add(tooltipLine);
                }*/
            }
        }
    }
}