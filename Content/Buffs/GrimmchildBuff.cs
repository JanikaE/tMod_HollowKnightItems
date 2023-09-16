namespace HollowKnightItems.Content.Buffs
{
    internal class GrimmchildBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            Main.lightPet[Type] = false;
            Main.vanityPet[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}
