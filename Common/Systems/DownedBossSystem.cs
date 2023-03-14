namespace HollowKnightItems.Common.Systems
{
    public class DownedBossSystem : ModSystem
    {
        public static bool downedGrimm = false;

        public override void OnWorldLoad()
        {
            downedGrimm = false;
        }

        public override void OnWorldUnload()
        {
            downedGrimm = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedGrimm)
            {
                tag["downedGrimm"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedGrimm = tag.ContainsKey("downedGrimm");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedGrimm;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedGrimm = flags[0];
        }
    }
}