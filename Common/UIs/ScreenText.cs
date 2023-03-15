namespace HollowKnightItems.Common.UIs
{
    internal class ScreenText : UIState
    {
        public static bool Visible = false;

        public static UIText[] UITexts = new UIText[10];

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
                }                
            }
        }
    }

    internal class Text
    {
        public string text;
        public int scale;
        public float HAlign;
        public float VAlign;

        public Text(string text, int scale, float HAlign, float VAlign)
        {
            this.text = text;
            this.scale = scale;
            this.HAlign = HAlign;
            this.VAlign = VAlign;
        }

        /// <summary>
        /// 往ScreenText中插入一个新的<see cref="UIText"/>
        /// </summary>
        /// <param name="text">text内容</param>
        /// <param name="scale">text大小</param>
        /// <param name="HAlign">横向位置</param>
        /// <param name="VAlign">纵向位置</param>
        /// <returns>生成对象在<see cref="ScreenText.UITexts"/>中的下标，方便之后针对性清除。如果为-1则表示位置已满，生成失败</returns>
        public static int NewUIText(string text, int scale, float HAlign, float VAlign)
        {
            UIText uiText = new(text, scale)
            {
                HAlign = HAlign,
                VAlign = VAlign
            };

            int index = -1;
            for (int i = 0; i < ScreenText.UITexts.Length; i++)
            {
                if (ScreenText.UITexts[i] == null)
                {
                    ScreenText.UITexts[i] = uiText;
                    index = i;
                    break;
                }
                
            }
            return index;
        }

        /// <summary>
        /// 清除<see cref="ScreenText.UITexts"/>中指定下标的对象
        /// </summary>
        public static void ClearUIText(int index)
        {
            ScreenText.UITexts[index] = null;
        }

        /// <summary>
        /// 清除<see cref="ScreenText.UITexts"/>中所有的对象
        /// </summary>
        public static void ClaerAllUIText()
        {
            for (int i = 0; i < ScreenText.UITexts.Length; i++)
            {
                ScreenText.UITexts[i] = null;
            }
        }
    }

    internal class ScreenTextSystem : ModSystem 
    {
        public ScreenText screenText;
        public UserInterface userInterface;

        public override void Load()
        {
            screenText = new ScreenText();
            screenText.Activate();
            Text.ClaerAllUIText();
            userInterface = new UserInterface();
            userInterface.SetState(screenText);
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
                        if (ScreenText.Visible)
                        {
                            //绘制UI
                            screenText.Draw(Main.spriteBatch);
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
