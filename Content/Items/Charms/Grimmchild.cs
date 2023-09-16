using HollowKnightItems.Common.Players;
using HollowKnightItems.Common.UIs;
using HollowKnightItems.Content.Buffs;
using HollowKnightItems.Content.Projectiles.Grimmchild;
using HollowKnightItems.Content.Rarities;

namespace HollowKnightItems.Content.Items.Charms
{
    internal class Grimmchild : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.hasVanityEffects = true;
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
            player.GetModPlayer<GrimmchidPlayer>().Type = true;
        }

        public override void UpdateVanity(Player player)
        {
            Summon(player);
            player.GetModPlayer<GrimmchidPlayer>().Type = false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine Line;

            Line = new(Mod, "LeftClick", GetText("Items.Grimmchild.LeftClick"));
            tooltips.Add(Line);
            Line = new(Mod, "Intro", GetText("Items.Grimmchild.Intro"));
            tooltips.Add(Line);

            int stage = player.GetModPlayer<GrimmchidPlayer>().Stage;
            string s = stage == 4 ? "(Max)" : "";
            Line = new(Mod, "Stage", GetText("Common.Stage") + stage + s);
            tooltips.Add(Line);
            Line = new(Mod, "Damage", GetText("Common.Damage") + GetGrimmchildAttack());
            tooltips.Add(Line);
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