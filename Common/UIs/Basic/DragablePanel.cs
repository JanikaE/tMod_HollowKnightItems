namespace HollowKnightItems.Common.UIs.Basic
{
    // 允许拖动面板
    // 确保面板在被拖到外面或屏幕大小调整时会反弹回边界
    // UIPanel不会阻止玩家在单击鼠标时使用项目
    public class DragablePanel : UIPanel
    {
        private Vector2 offset;  // 在拖动时存储与UIPanel左上角的偏移量        
        private bool dragging;  // 检查当前是否正在拖动面板的标志

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            DragStart(evt);
        }

        public override void MouseUp(UIMouseEvent evt)
        {
            base.MouseUp(evt);
            DragEnd(evt);
        }

        private void DragStart(UIMouseEvent evt)
        {
            // 偏移变量有助于记住面板相对于鼠标位置的位置
            // 因此，无论您从何处开始拖动面板，它都会流畅地移动
            offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt)
        {
            Vector2 endMousePosition = evt.MousePosition;
            dragging = false;

            Left.Set(endMousePosition.X - offset.X, 0f);
            Top.Set(endMousePosition.Y - offset.Y, 0f);

            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // 检查ContainsPoint然后将鼠标接口设置为true是很常见的
            // 这会导致单击此UIElement不会导致玩家使用当前项目
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f); // Main.MouseScreen.X和Main.mouseX是相同的
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            // 检查可拖动UIPanel是否在父UIElement矩形之外
            // 通过这样做和一些简单的数学运算，如果用户调整窗口大小或以其他方式更改分辨率，我们可以将面板重新贴靠在屏幕上
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Terraria.Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Terraria.Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                // 强制UI系统再次执行定位数学运算
                Recalculate();
            }
        }
    }
}