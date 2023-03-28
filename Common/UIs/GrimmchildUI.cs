using HollowKnightItems.Assets;
using HollowKnightItems.Common.UIs.Basic;

namespace HollowKnightItems.Common.UIs
{
    internal class GrimmchildUI : UIState
    {
        public static bool Visible = false;

        private readonly DragablePanel panel = new();
        private readonly ItemImage target = new(121);
        private readonly HoverImageButton close = new(TextureLoader.UIDelete, "close");
        private readonly HoverTextBox upgrade = new("upgrade", Color.Blue, Color.LightBlue);

        public override void OnInitialize()
        {
            panel.Width.Set(150f, 0f);
            panel.Height.Set(150f, 0f);
            panel.HAlign = 0.5f;
            panel.VAlign = 0.5f;
            Append(panel);

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

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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
