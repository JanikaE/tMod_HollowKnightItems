﻿namespace HollowKnightItems.Content.Dusts
{
    internal class TailingFlame : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.scale *= 2f;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X * 0.15f;
            dust.scale *= 0.9f;

            float light = 0.35f * dust.scale;
            Lighting.AddLight(dust.position, light, light, light);

            if (dust.scale < 0.3f)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
