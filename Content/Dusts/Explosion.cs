namespace HollowKnightItems.Content.Dusts
{
    internal class Explosion : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.scale *= 3f;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.98f;
            if (dust.velocity.Length() < 5)
            {
                dust.scale *= 0.98f;
            }

            if (dust.scale < 0.3)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
