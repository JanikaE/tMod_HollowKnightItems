namespace HollowKnightItems.Common.UIs
{
    internal class ScreenTextUI : UIState
    {
        public static bool Visible = false;

        public static TimerUIText[] UITexts = new TimerUIText[10];

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // 默认UI关闭
            Visible = false;
            for (int i = 0; i< UITexts.Length; i++)
            {
                if (UITexts[i] != null)
                {
                    Append(UITexts[i]);
                    // 当存在text时，设置UI开启
                    Visible = true;

                    // 当text的计时器归零时，清除这个text
                    if (UITexts[i].timer == 0)
                    {
                        UITexts[i] = null;
                    }
                }                
            }
        }
    }

    internal class ScreenText
    {
        public string text;
        public int scale;
        public float HAlign;
        public float VAlign;

        public ScreenText(string text, int scale, float HAlign, float VAlign)
        {
            this.text = text;
            this.scale = scale;
            this.HAlign = HAlign;
            this.VAlign = VAlign;
        }

        /// <summary>
        /// 往ScreenText中插入一个新的<see cref="TimerUIText"/>
        /// </summary>
        /// <param name="text">text内容</param>
        /// <param name="scale">text大小</param>
        /// <param name="timer">text显示时间，-1表示默认无限时间</param>
        /// <param name="HAlign">横向位置</param>
        /// <param name="VAlign">纵向位置</param>
        /// <returns>生成对象在<see cref="ScreenTextUI.UITexts"/>中的下标，方便之后针对性清除。如果为-1则表示位置已满，生成失败</returns>
        public static int NewScreenText(string text, int scale, int timer, float HAlign, float VAlign)
        {
            TimerUIText uiText = new(text, scale, false, timer)
            {
                HAlign = HAlign,
                VAlign = VAlign
            };

            int index = -1;
            for (int i = 0; i < ScreenTextUI.UITexts.Length; i++)
            {
                if (ScreenTextUI.UITexts[i] == null)
                {
                    ScreenTextUI.UITexts[i] = uiText;
                    index = i;
                    break;
                }
                
            }
            return index;
        }

        /// <summary>
        /// 清除<see cref="ScreenTextUI.UITexts"/>中指定下标的对象
        /// </summary>
        public static void ClearScreenText(int index)
        {
            ScreenTextUI.UITexts[index] = null;
        }

        /// <summary>
        /// 清除<see cref="ScreenTextUI.UITexts"/>中所有的对象
        /// </summary>
        public static void ClearAllScreenText()
        {
            for (int i = 0; i < ScreenTextUI.UITexts.Length; i++)
            {
                ScreenTextUI.UITexts[i] = null;
            }
        }
    }

    internal class ScreenTextSystem : ModSystem 
    {
        public ScreenTextUI screenTextUI;
        public UserInterface userInterface;

        public override void Load()
        {
            screenTextUI = new ScreenTextUI();
            screenTextUI.Activate();
            ScreenText.ClearAllScreenText();
            userInterface = new UserInterface();
            userInterface.SetState(screenTextUI);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            userInterface?.Update(gameTime);
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
                        if (ScreenTextUI.Visible)
                        {
                            //绘制UI
                            screenTextUI.Draw(Main.spriteBatch);
                        }
                        return true;
                    },
                    // 绘制层的类型
                    InterfaceScaleType.Game)
                );
            }
        }
    }
}
