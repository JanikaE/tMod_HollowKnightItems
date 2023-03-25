namespace HollowKnightItems.Content.Buffs
{
    internal class RoarDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Roar");
            DisplayName.AddTranslation(7, "战吼");
            Description.SetDefault("Can't move");
            Description.AddTranslation(7, "动弹不得");

            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<RoarDebuffPlayer>().HasRoarDebuff = true;
        }
    }

    internal class RoarDebuffPlayer : ModPlayer
    {
        public bool HasRoarDebuff;

        public override void ResetEffects()
        {
            HasRoarDebuff = false;
        }

        public override bool CanUseItem(Item item)
        {
            // debuff存在时无法使用物品
            return !HasRoarDebuff;
        }

        public override void PreUpdateMovement()
        {
            // debuff存在时无法移动
            if (HasRoarDebuff)
            {
                Player.velocity = Vector2.Zero;
            }            
        }
    }
}
