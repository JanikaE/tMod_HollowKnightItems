using HollowKnightItems.Common.UIs;
using HollowKnightItems.Content.Buffs;
using HollowKnightItems.Content.Projectiles.Grimmchild;
using HollowKnightItems.Content.Rarities;

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

            Item.consumable = false;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.value = 100;
        }

        public override bool CanUseItem(Player player)
        {
            GrimmchildUI.Visible = !GrimmchildUI.Visible;
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Summon(player);
            player.GetModPlayer<GrimmchidPlayer>().GrimmchildType = true;
        }

        public override void UpdateVanity(Player player)
        {
            Summon(player);
            player.GetModPlayer<GrimmchidPlayer>().GrimmchildType = false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.GetModPlayer<GrimmchidPlayer>().HasGrimmchild)
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
            player.GetModPlayer<GrimmchidPlayer>().HasGrimmchild = true;
        }
    }
}