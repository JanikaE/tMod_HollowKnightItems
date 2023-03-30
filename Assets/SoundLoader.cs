namespace HollowKnightItems.Assets
{
    internal class SoundLoader : ModSystem
    {
        public static SoundStyle Boss_Explode;
        public static SoundStyle CarefreeMelody;
        public static SoundStyle Dash;
        public static SoundStyle Enemy_Hit;
        public static SoundStyle Grimm_Attack;
        public static SoundStyle Grimm_Death;
        public static SoundStyle Grimm_Firebird;
        public static SoundStyle Grimm_FlyEnd;
        public static SoundStyle Grimm_Teleport;
        public static SoundStyle Grimm_Thorn;
        public static SoundStyle Grimmchild_Attack;
        public static SoundStyle Grimmchild_Routine;
        public static SoundStyle Metal_Hit;

        public override void Load()
        {
            Boss_Explode = GetSoundStyle("Boss_Explode", 1, 1f);
            CarefreeMelody = GetSoundStyle("CarefreeMelody", 1, 0.5f);
            Dash = GetSoundStyle("Dash", 1, 1f);
            Enemy_Hit = GetSoundStyle("Enemy_Hit", 1, 0.6f, SoundLimitBehavior: SoundLimitBehavior.ReplaceOldest);
            Grimm_Attack = GetSoundStyle("Grimm_Attack", 1, 1f);  // 不太行，先不用
            Grimm_Death = GetSoundStyle("Grimm_Death", 1, 1f);
            Grimm_Firebird = GetSoundStyle("Grimm_Firebird", 1, 0.8f, SoundLimitBehavior: SoundLimitBehavior.ReplaceOldest);
            Grimm_FlyEnd = GetSoundStyle("Grimm_FlyEnd", 1, 1f);
            Grimm_Teleport = GetSoundStyle("Grimm_Teleport", 1, 0.5f);
            Grimm_Thorn = GetSoundStyle("Grimm_Thorn", 1, 1f);
            Grimmchild_Attack = GetSoundStyle("Grimmchild_Attack_", 2, 0.8f);
            Grimmchild_Routine = GetSoundStyle("Grimmchild_Routine_", 6, 1f);
            Metal_Hit = GetSoundStyle("Metal_Hit", 1, 1f, SoundLimitBehavior: SoundLimitBehavior.ReplaceOldest);
        }
    }
}
