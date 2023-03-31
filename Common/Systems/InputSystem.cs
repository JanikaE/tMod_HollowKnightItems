using HollowKnightItems.Common.Players;

namespace HollowKnightItems.Common.Systems
{
    internal class InputSystem : ModSystem
    {
        string text;

        public override void PostUpdateEverything()
        {
            // 简单的获取聊天框字符串的功能
            if (Main.chatText != "")
            {
                text = Main.chatText;
            }            
            if (Main.inputText.IsKeyDown(Keys.Enter) && text != null)
            {
                // 开个后门
                // 直接修改格林之子的阶段
                if (text.StartsWith("/grimmchild"))
                {
                    char[] chars = text.ToCharArray();
                    int stage = chars[^1] - 48;
                    if (stage >= 1 && stage <= 4)
                    {
                        Player player = Main.LocalPlayer;
                        player.GetModPlayer<GrimmchidPlayer>().Stage = stage;
                        Main.NewText("Set Grimmchild stage to " + stage);
                    }
                }

                text = null;
            }
        }
    }
}
