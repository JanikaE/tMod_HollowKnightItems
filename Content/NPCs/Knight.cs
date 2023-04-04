using HollowKnightItems.Common.Systems;
using HollowKnightItems.Content.Items;
using HollowKnightItems.Content.Items.Charms;

namespace HollowKnightItems.Content.NPCs
{
    [Autoload(false)]
    internal class Knight : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
            DisplayName.AddTranslation(7, "");
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.aiStyle = 7;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return true;
        }

        public override string GetChat()
        {
            // 挖坑
            WeightedRandom<string> chat = new();
            chat.Add("");
            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            // 如果按下第一个按钮，则开启商店
            if (firstButton)
            {
                shop = true;
            }
            // 在if之后可以写第二个按钮的作用
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[0].SetDefaults(ModContent.ItemType<NightmareLantern_OFF>());
            if (DownedBossSystem.downedGrimm)
            {
                shop.item[10].SetDefaults(ModContent.ItemType<Grimmchild>());
                shop.item[11].SetDefaults(ModContent.ItemType<CarefreeMelody>());
            }
        }
    }
}
