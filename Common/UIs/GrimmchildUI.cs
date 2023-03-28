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
        private readonly ItemImage target = new(ItemDefault);
        private readonly HoverImageButton close = new(TextureLoader.UIClose, GetText("Common.Close"));
        private readonly HoverTextBox upgrade = new(GetText("Common.Upgrade"), Color.Blue, Color.LightBlue);        

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

            target.Width.Set(60f, 0f);
            target.Height.Set(60f, 0f);
            target.HAlign = 0.5f;
            target.VAlign = 0.2f;
            panel.Append(target);

            close.Width.Set(20f, 0f);
            close.Height.Set(20f, 0f);
            close.HAlign = 1f;
            close.VAlign = 0f;
            close.OnClick += Close_OnClick;
            panel.Append(close);

            upgrade.Width.Set(30f, 0f);
            upgrade.Height.Set(30f, 0f);
            upgrade.HAlign = 0.5f;
            upgrade.VAlign = 1f;
            upgrade.ShowInputTicker = false;
            upgrade.OnClick += Upgrade_OnClick;
            panel.Append(upgrade);
        }

        private void Close_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Visible = false;
        }

        private void Upgrade_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Player player = Main.LocalPlayer;
            Stage = player.GetModPlayer<GrimmchidPlayer>().Stage;
            int[] material;
            switch (Stage)
            {
                case 1:
                    material = Item1;
                    break;
                case 2:
                    material = Item2;
                    break;
                case 3:
                    material = Item3;
                    break;
                default:
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
            if (Visible)
            {
                Player player = Main.LocalPlayer;
                Stage = player.GetModPlayer<GrimmchidPlayer>().Stage;
                string s = Stage == 4 ? "MAX" : Stage.ToString(); 
                currentStage.SetText(GetText("Common.Stage") + s);
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
            }
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
