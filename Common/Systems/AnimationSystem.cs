using HollowKnightItems.Assets;
using HollowKnightItems.Content.Dusts;

namespace HollowKnightItems.Common.Systems
{
    internal class AnimationSystem : ModSystem
    { 
        public static Animation[] Animations = new Animation[1];
        public enum MyAnimationID{
            GrimmDeath
        };

        public override void OnWorldLoad()
        {
            for (int i = 0; i < Animations.Length; i++)
            {
                Animations[i] = new Animation();
            }
        }

        public override void PostUpdateEverything()
        {
            for (int i = 0; i < Animations.Length; i++)
            {
                if (Animations[i].IsPlay)
                {
                    Play(i);
                }
            }
        }

        public static void StartPlay(int id, int time, Vector2 position)
        {            
            Animations[id].IsPlay = true;
            Animations[id].Timer = time;
            Animations[id].Position = position;
        }

        private static void Play(int id)
        {
            switch (id)
            {
                case (int)MyAnimationID.GrimmDeath:
                    PlayGrimmDeath(id);
                    break;
            }

            Animations[id].Timer--;
            if (Animations[id].Timer == 0)
            {
                Animations[id].IsPlay = false;
            }
        }

        private static void PlayGrimmDeath(int id)
        {
            int num = Animations[id].Timer == 1 ? 200 : 10;
            for (int i = 0; i < num; i++)
            {
                float rotation = (float)(random.Next(360) * Math.PI / 180);
                Vector2 dir = rotation.ToRotationVector2() * 20;
                Dust.NewDust(Animations[id].Position, 0, 0, ModContent.DustType<Explosion>(), dir.X, dir.Y, newColor: new Color(255, 150, 150));
            }
            if (Animations[id].Timer == 1)
            {
                SoundEngine.PlaySound(SoundLoader.Boss_Explode, Animations[id].Position);
            }
        }
    }

    public class Animation 
    {
        public bool IsPlay;
        public float Timer;
        public Vector2 Position;

        public Animation()
        {
            IsPlay = false;
            Timer = 0;
            Position = Vector2.Zero;
        }

        public Animation(bool isPlay, float timer, Vector2 position)
        {
            IsPlay = isPlay;
            Timer = timer;
            Position = position;
        }
    }
}
