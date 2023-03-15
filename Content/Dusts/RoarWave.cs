namespace HollowKnightItems.Content.Dusts
{
    internal class RoarWave : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation = dust.velocity.ToRotation();
            dust.scale *= 1.2f;

            if (dust.scale > 10)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
