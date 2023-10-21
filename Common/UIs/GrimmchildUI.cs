using HollowKnightItems.Assets;
using HollowKnightItems.Common.Players;
using HollowKnightItems.Common.UIs.Basic;

namespace HollowKnightItems.Common.UIs
{
    internal class GrimmchildUI : UIState
    {
        public static bool Visible = false;
        public static int Stage;

        private static readonly int[] ItemDefault = { 0 };
        private static readonly int[] Item1 = { 86, 1329 };
        private static readonly int[] Item2 = { 520, 521 };
        private static readonly int[] Item3 = { 3456, 3457, 3458, 3459 };

        private readonly DragablePanel panel = new();
        private readonly UIText currentStage = new(GetText("Common.Stage"));
        private readonly UIPanel itemPanel = new();
        private readonly ItemImage target = new(ItemDefault);
        private readonly HoverImageButton close = new(TextureLoader.UIClose, GetText("Common.Close"));
        private readonly HoverTextBox upgrade = new(GetText("Common.Upgrade"), Color.Blue, Color.LightBlue, "");

        public override void OnInitialize()
        {
            panel.Width.Set(150f, 0f);
            panel.Height.Set(150f, 0f);
            panel.HAlign = 0.5f;
            panel.VAlign = 0.5f;
            Append(panel);

            currentStage.Width.Set(60f, 0f);
            currentStage.Height.Set(30f, 0f);
            currentStage.HAlign = 0f;
            currentStage.VAlign = 0f;
            panel.Append(currentStage);

            itemPanel.Width.Set(50f, 0f);
            itemPanel.Height.Set(50f, 0f);
            itemPanel.HAlign = 0.5f;
            itemPanel.VAlign = 0.3f;
            panel.Append(itemPanel);

            target.Width.Set(50f, 0f);
            target.Height.Set(50f, 0f);
            itemPanel.Append(target);

            close.Width.Set(20f, 0f);
            close.Height.Set(20f, 0f);
            close.HAlign = 1f;
            close.VAlign = 0f;
            close.OnLeftClick += Close_OnLeftClick;
            panel.Append(close);

            upgrade.Width.Set(30f, 0f);
            upgrade.Height.Set(30f, 0f);
            upgrade.HAlign = 0.5f;
            upgrade.VAlign = 1f;
            upgrade.ShowInputTicker = false;
            upgrade.OnLeftClick += Upgrade_OnLeftClick;
            panel.Append(upgrade);
        }

        private void Close_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Visible = false;
        }

        private void Upgrade_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Player player = Main.LocalPlayer;
            Stage = player.GetModPlayer<GrimmchidPlayer>().Stage;
            int[] material = GetMaterial();
            if (material == null)
            {
                return;
            }

            foreach (Item item in player.inventory)
            {
                if (material.Contains(item.type) && item.stack >= 10)
                {
                    item.stack -= 10;
                    if (item.stack == 0)
                    {
                        item.TurnToAir();
                    }
                    Stage++;
                    player.GetModPlayer<GrimmchidPlayer>().Stage = Stage;
                    return;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            close.hoverText = GetText("Common.Close");
            upgrade.SetText(GetText("Common.Upgrade"));
            if (Visible)
            {
                Player player = Main.LocalPlayer;
                Stage = player.GetModPlayer<GrimmchidPlayer>().Stage;
                currentStage.SetText(GetText("Common.Stage") + Stage);
                switch (Stage)
                {
                    case 1:
                        target.ChangeItem(Item1);
                        break;
                    case 2:
                        target.ChangeItem(Item2);
                        break;
                    case 3:
                        target.ChangeItem(Item3);
                        break;
                    default:
                        target.ChangeItem(ItemDefault);
                        break;
                }

                if (Stage == 4)
                {
                    upgrade.SetHoverText(GetText("Common.Max"));
                    return;
                }
                int[] material = GetMaterial();
                if (material == null)
                {
                    upgrade.SetHoverText("");
                    return;
                }
                foreach (Item item in player.inventory)
                {
                    if (material.Contains(item.type) && item.stack >= 10)
                    {
                        upgrade.SetHoverText(item.Name + GetText("Common.Consume") + "(10)");
                        return;
                    }
                }
                upgrade.SetHoverText(GetText("Common.Lack"));
            }
        }

        private static int[] GetMaterial()
        {
            int[] material = Stage switch
            {
                1 => Item1,
                2 => Item2,
                3 => Item3,
                _ => null,
            };
            return material;
        }
    }

    internal class GrimmchildUISystem : ModSystem
    {
        public GrimmchildUI GrimmchildUI;
        public UserInterface UserInterface;

        public override void Load()
        {
            GrimmchildUI = new GrimmchildUI();
            GrimmchildUI.Activate();
            UserInterface = new UserInterface();
            UserInterface.SetState(GrimmchildUI);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (GrimmchildUI.Visible)
            {
                UserInterface?.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // 寻找绘制层，并且返回那一层的索引
            int Index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (Index != -1)
            {
                // 往绘制层集合插入一个成员，第一个参数是插入的地方的索引，第二个参数是绘制层
                layers.Insert(Index, new LegacyGameInterfaceLayer(
                    // 绘制层的名字
                    "Test : Setting",
                    // 是匿名方法
                    delegate
                    {
                        //当UI开启时
                        if (GrimmchildUI.Visible)
                        {
                            //绘制UI
                            GrimmchildUI.Draw(Main.spriteBatch);
                        }
                        return true;
                    },
                    // 绘制层的类型
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
