namespace HollowKnightItems.Content.Dusts
{
    internal class Hit : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.98f;
            if (dust.velocity.Length() < 3)
            {
                dust.scale *= 0.99f;
            }

            if (dust.scale < 0.3)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
