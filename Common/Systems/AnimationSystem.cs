using HollowKnightItems.Assets;
using HollowKnightItems.Content.Dusts;

namespace HollowKnightItems.Common.Systems
{
    internal class AnimationSystem : ModSystem
    {
        public static Dictionary<int, Animation> animations = new();

        public override void OnWorldLoad()
        {
            animations.Add((int)MyAnimationID.GrimmDeath, new Animation(GrimmDeathAction));
        }

        public override void PostUpdateEverything()
        {
            foreach (Animation animation in animations.Values)
            {
                if (animation.isPlay)
                {
                    Play(animation);
                }
            }
        }

        public static void StartPlay(MyAnimationID id, int timer, Vector2 position)
        {
            Animation animation = animations.GetValueOrDefault((int)id);
            animation.timer = timer;
            animation.position = position;
            animation.isPlay = true;
        }

        private static void Play(Animation animation)
        {
            animation.action(animation.timer, animation.position);

            animation.timer--;
            if (animation.timer == 0)
            {
                animation.isPlay = false;
            }
        }

        private static void GrimmDeathAction(int timer, Vector2 position)
        {
            int num = timer == 1 ? 200 : 10;
            for (int i = 0; i < num; i++)
            {
                float rotation = (float)(random.Next(360) * Math.PI / 180);
                Vector2 dir = rotation.ToRotationVector2() * 20;
                Dust.NewDust(position, 0, 0, ModContent.DustType<Explosion>(), dir.X, dir.Y, newColor: new Color(255, 150, 150));
            }
            if (timer == 1)
            {
                SoundEngine.PlaySound(SoundLoader.Boss_Explode, position);
            }
        }
    }

    public class Animation
    {
        public bool isPlay;
        public int timer;
        public Vector2 position;
        public Action<int, Vector2> action;

        public Animation(Action<int, Vector2> action)
        {
            this.action = action;
        }
    }

    public enum MyAnimationID
    {
        GrimmDeath
    };
}
