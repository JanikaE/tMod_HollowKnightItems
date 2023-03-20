using HollowKnightItems.Content.Dusts;
using System.Threading;

namespace HollowKnightItems.Common.Systems
{
    internal class AnimationSystem : ModSystem
    {        
        public static bool[] IsPlay = { false };
        public static int[] Timer = { 0 };
        public static Vector2[] Position = { Vector2.Zero };
        public enum MyAnimationID{
            GrimmDying
        };

        public override void OnWorldLoad()
        {
            ArrayToDefault(ref Timer);
            ArrayToDefault(ref IsPlay);
            ArrayToDefault(ref Position);
        }

        public override void PostUpdateEverything()
        {
            for (int i = 0; i < IsPlay.Length; i++)
            {
                if (IsPlay[i])
                {
                    Play(i);
                }
            }
        }

        public static void StartPlay(int id, int time, Vector2 position)
        {
            IsPlay[id] = true;
            Timer[id] = time;
            Position[id] = position;
        }

        private static void Play(int id)
        {
            switch (id)
            {
                case (int)MyAnimationID.GrimmDying:
                    PlayGrimmDying(id);
                    break;
            }

            Timer[id]--;
            if (Timer[id] == 0)
            {
                IsPlay[id] = false;
            }
        }

        private static void PlayGrimmDying(int id)
        {
            int num = Timer[id] == 1 ? 200 : 10;
            for (int i = 0; i < num; i++)
            {
                float rotation = (float)(random.Next(360) * Math.PI / 180);
                Vector2 dir = rotation.ToRotationVector2() * 20;
                Dust.NewDust(Position[id], 0, 0, ModContent.DustType<Explosion>(), dir.X, dir.Y, newColor: new Color(255, 150, 150));
            }
        }
    }
}
