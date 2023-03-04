using Terraria.Audio;

namespace HollowKnightItems.Common.Utils
{
    internal static class SoundUtils
    {
        public static SoundStyle GrimmchildSound_Attack = new SoundStyle("HollowKnightItems/Assets/Audio/GrimmchildAttack_", 2) with
        {
            Volume = 0.8f,  // 音量
            Pitch = 0,  // 音调
            PitchVariance = 0,  // 音调随机浮动
            MaxInstances = 1,  // 叠加播放上限
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew,  // 到达上限后的操作
            Type = SoundType.Sound,   // 声音类型（音乐/音效/环境）
        };

        public static SoundStyle GrimmchildSound_Routine = new SoundStyle("HollowKnightItems/Assets/Audio/GrimmchildRoutine_", 6) with
        {
            Volume = 1f,
            Pitch = 0,
            PitchVariance = 0,
            MaxInstances = 1,
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew,
            Type = SoundType.Sound,
        };

        public static SoundStyle CarefreeMelodySound = new SoundStyle("HollowKnightItems/Assets/Audio/CarefreeMelodySound") with
        {
            Volume = 0.8f,
            Pitch = 0,
            PitchVariance = 0,
            MaxInstances = 1,
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew,
            Type = SoundType.Sound,
        };
    }
}
