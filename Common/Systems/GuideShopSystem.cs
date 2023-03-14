using HollowKnightItems.Content.Items.Charms;

namespace HollowKnightItems.Common.Systems
{
    internal class GuideShopSystem : ModSystem
    {
        public override void Load()
        {
            On.Terraria.Main.GUIChatDrawInner += Main_GUIChatDrawInner;
        }

        public override void Unload()
        {
            On.Terraria.Main.GUIChatDrawInner -= Main_GUIChatDrawInner;
        }

        public static object TextDisplayCache => typeof(Main).GetField("_textDisplayCache",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Main.instance);
        public bool hover = false;
        public bool MouseLeft = false;

        public static float GetButtonPosition(int num, bool happy = false)
        {
            int i = 180;
            if (happy) i += 10;
            float dis = ChatManager.GetStringSize(FontAssets.MouseText.Value, Language.GetTextValue("LegacyInterface.64"), new(0.9f)).X;
            float postion = i + (Main.screenWidth - 800) / 2 + (num - 1) * (dis + 30);
            return postion;
        }

        public void Main_GUIChatDrawInner(On.Terraria.Main.orig_GUIChatDrawInner orig, Main self)
        {
            orig(self);
            NPC npc = Main.npc[Main.player[Main.myPlayer].talkNPC];
            if (npc.type == NPCID.Guide)
            {
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                string focusText = Language.GetTextValue("LegacyInterface.28");
                int numLines = (int)TextDisplayCache.GetType().GetProperty("AmountOfLines",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).GetValue(TextDisplayCache) + 1;
                Vector2 scale = new(0.9f);
                Vector2 stringSize = ChatManager.GetStringSize(font, Language.GetTextValue("LegacyInterface.51"), scale);
                Vector2 vector = new(1f);
                if (stringSize.X > 260f) vector.X *= 260f / stringSize.X;

                float button = GetButtonPosition(5, false);
                Vector2 position = new(button, 100 + numLines * 30);
                stringSize = ChatManager.GetStringSize(font, focusText, scale);
                if (Main.MouseScreen.Between(position, position + stringSize * scale * vector.X) && !PlayerInput.IgnoreMouseInterface)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.LocalPlayer.releaseUseItem = false;
                    scale *= 1.2f;
                    if (!hover) SoundEngine.PlaySound(SoundID.MenuTick);
                    hover = true;
                }
                else
                {
                    if (hover) SoundEngine.PlaySound(SoundID.MenuTick);
                    hover = false;
                }

                int MTC = Main.mouseTextColor;
                Color unhoverColor = new(MTC, MTC, MTC / 2, MTC);
                Color hoveringColor = new(228, 206, 114, Main.mouseTextColor / 2);
                ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, font, focusText, position + stringSize * vector * 0.5f,
                    !hover ? Color.Black : Color.Brown, 0f, stringSize * 0.5f, scale * vector);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, focusText, position + stringSize * vector * 0.5f,
                    !hover ? unhoverColor : hoveringColor, 0f, stringSize * 0.5f, scale * vector);

                if (Main.MouseScreen.Between(position, position + stringSize * scale * vector.X) && !PlayerInput.IgnoreMouseInterface)
                {
                    if (Main.mouseLeft) MouseLeft = true;
                    if (MouseLeft && !Main.mouseLeft)
                    {
                        Main.playerInventory = true;
                        Main.npcChatText = "";
                        SoundEngine.PlaySound(SoundID.MenuTick);
                        Player player = Main.player[Main.myPlayer];
                        Chest shop = Main.instance.shop[98];
                        if (npc.type == NPCID.Guide)
                        {
                            Main.SetNPCShopIndex(98);
                            int nextSlot = 0;
                            bool sell = true;

                            if (sell)
                            {
                                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Grimmchild>());
                                nextSlot++;
                                if (NPC.downedBoss2)
                                {
                                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<CarefreeMelody>());
                                    nextSlot++;
                                }
                            }
                        }
                        MouseLeft = false;
                    }
                }
            }
        }
    }
}
