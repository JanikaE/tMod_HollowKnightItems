namespace HollowKnightItems.Content.Dusts
{
    internal class StaticPoint : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.scale = 3f;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation++;
            dust.scale *= 0.9f;

            if (dust.scale < 0.1)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
