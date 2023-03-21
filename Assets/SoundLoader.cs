namespace HollowKnightItems.Assets
{
    internal class SoundLoader : ModSystem
    {
        public static SoundStyle CarefreeMelody;
        public static SoundStyle Enemy_Hit;
        public static SoundStyle Grimm_Attack;
        public static SoundStyle Grimm_Death;
        public static SoundStyle Grimm_Firebird;
        public static SoundStyle Grimm_FlyEnd;
        public static SoundStyle Grimm_Teleport;
        public static SoundStyle Grimm_Thorn;
        public static SoundStyle Grimmchild_Attack;
        public static SoundStyle Grimmchild_Routine;        

        public override void Load()
        {
            CarefreeMelody = GetSoundStyle("CarefreeMelody", 1, 0.5f);
            Enemy_Hit = GetSoundStyle("Enemy_Hit", 1, 1f);
            Grimm_Attack = GetSoundStyle("Grimm_Attack", 1, 1f);
            Grimm_Death = GetSoundStyle("Grimm_Die", 1, 1f);
            Grimm_Firebird = GetSoundStyle("Grimm_Firebird", 1, 1f);
            Grimm_FlyEnd = GetSoundStyle("Grimm_FlyEnd", 1, 1f);
            Grimm_Teleport = GetSoundStyle("Grimm_Teleport", 1, 1f);
            Grimm_Thorn = GetSoundStyle("Grimm_Thorn", 1, 1f);
            Grimmchild_Attack = GetSoundStyle("Grimmchild_Attack_", 2, 0.8f);
            Grimmchild_Routine = GetSoundStyle("Grimmchild_Routine_", 6, 1f);            
        }
    }
}
