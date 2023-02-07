using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HollowKnightItems.Buffs
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
}
