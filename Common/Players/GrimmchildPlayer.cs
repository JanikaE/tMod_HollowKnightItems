namespace HollowKnightItems.Common.Players
{
    internal class GrimmchidPlayer : ModPlayer
    {
        public bool HasGrimmchild;
        public bool Type;  // 格林之子类型，true为饰品栏，false为时装栏
        public int Stage;

        public override void Initialize()
        {
            Stage = 1;
        }

        public override void ResetEffects()
        {
            HasGrimmchild = false;
            Type = false;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["GrimmchildStage"] = Stage;
        }

        public override void LoadData(TagCompound tag)
        {
            Stage = (int)tag["GrimmchildStage"];
        }
    }
}
