namespace HollowKnightItems.Common.Utils
{
    /// <summary>
    /// 自用的一些工具
    /// </summary>
    internal static class MyUtils
    {
        public static Random random = new();

        /// <summary>
        /// int数组元素置0
        /// </summary>
        public static void ToDefault(this int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }
        }

        public static Vector2 GetSize(this UIElement uie)
        {
            return new(uie.Width.Pixels, uie.Height.Pixels);
        }

        public static Rectangle ToSquare(this Texture2D texture)
        {
            int r = Math.Min(texture.Width, texture.Height);
            return new Rectangle(0, 0, r, r);
        }

        /// <summary>
        /// 修正因旋转导致的位置偏移
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <param name="r">旋转半径</param>
        public static Vector2 OffsetByAngle(float angle, int r)
        {
            angle = angle * MathHelper.Pi / 180;
            return new Vector2((float)(Math.Sin(angle)), (float)((1 - Math.Cos(angle)))) * r;
        }

        /// <summary>
        /// 首尾相接的位置偏移
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <param name="length">长度</param>
        public static Vector2 OffsetToLink(float angle, int length)
        {
            angle = angle * MathHelper.Pi / 180;
            return new Vector2((float)(Math.Sin(angle)), -(float)(Math.Cos(angle))) * length;
        }

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="path">路径为Mods.HollowKnightItems.*</param>
        public static string GetText(string path)
        {
            return Language.GetTextValue($"Mods.HollowKnightItems.{path}");
        }

        /// <summary>
        /// 获取城镇NPC对话(通常)
        /// </summary>
        /// <param name="name">NPC名字</param>
        /// <param name="num">对话数量</param>
        public static WeightedRandom<string> GetNPCChat(string name, int num)
        {
            WeightedRandom<string> chats = new();
            for (int i = 0; i < num; i++)
            {
                string chat = GetText($"NPCs.{name}.Chat.{i}");
                chats.Add(chat);
            }
            return chats;
        }

        /// <summary>
        /// 获取城镇NPC对话(特殊条件)
        /// </summary>
        /// <param name="name">NPC名字</param>
        /// <param name="condistion">条件</param>
        public static string GetNPCChat(string name, string condistion)
        {
            return GetText($"NPCs.{name}.Chat.{condistion}");
        }

        /// <summary>
        /// 获取NPC图鉴描述
        /// </summary>
        /// <param name="name">NPC名字</param>
        public static string GetNPCBestiary(string name)
        {
            return GetText($"NPCs.{name}.Bestiary");
        }

        /// <summary>
        /// 获取Effect
        /// </summary>
        /// <param name="fileName">路径为Assets/Effects/Content/*</param>
        public static Asset<Effect> GetEffect(string fileName)
        {
            return ModContent.Request<Effect>($"HollowKnightItems/Assets/Effects/Content/{fileName}", AssetRequestMode.ImmediateLoad);
        }

        /// <summary>
        /// 获取Texture2D
        /// </summary>
        /// <param name="fileName">路径为Assets/Textures/*</param>
        public static Asset<Texture2D> GetTexture(string fileName)
        {
            return ModContent.Request<Texture2D>($"HollowKnightItems/Assets/Textures/{fileName}", AssetRequestMode.ImmediateLoad);
        }

        /// <summary>
        /// 获取Sound
        /// </summary>
        /// <param name="fileName">路径为Assets/Sounds/*</param>
        /// <param name="numVariants">随机选取的目标数量</param>
        /// <param name="Volume">音量</param>
        /// <param name="Pitch">音调</param>
        /// <param name="PitchVariance">音调随机浮动</param>
        /// <param name="MaxInstances">叠加播放上限</param>
        /// <param name="SoundLimitBehavior">到达上限之后的操作</param>
        /// <param name="Type">声音类型（音乐/音效/环境）</param>
        public static SoundStyle GetSoundStyle(string fileName, int numVariants, float Volume, float Pitch = 0, float PitchVariance = 0, int MaxInstances = 1, SoundLimitBehavior SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, SoundType Type = SoundType.Sound)
        {
            return new SoundStyle($"HollowKnightItems/Assets/Sounds/{fileName}", numVariants) with
            {
                Volume = Volume,
                Pitch = Pitch,
                PitchVariance = PitchVariance,
                MaxInstances = MaxInstances,
                SoundLimitBehavior = SoundLimitBehavior,
                Type = Type
            };
        }
    }
}
