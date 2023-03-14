namespace HollowKnightItems.Content.Buffs
{
    internal class GrimmchildBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grimmchild");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "格林之子");
            Description.SetDefault("Meow~");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "喵~");

            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            Main.lightPet[Type] = false;
            Main.vanityPet[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }

    internal class GrimmchidPlayer : ModPlayer 
    {
        public bool HasGrimmchild;
        public bool GrimmchildType;  // 格林之子类型，true为饰品栏，false为时装栏

        public override void ResetEffects()
        {
            HasGrimmchild = false;
            GrimmchildType = false;
        }
    }
}
