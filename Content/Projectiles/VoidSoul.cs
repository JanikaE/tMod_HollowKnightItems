﻿namespace HollowKnightItems.Content.Projectiles
{
    internal class VoidSoul : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shade Soul");
            DisplayName.AddTranslation(7, "暗影之魂");
        }

        public override void SetDefaults()
        {
            Projectile.width = 53;
            Projectile.height = 18;
            Projectile.timeLeft = 240;

            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;

            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
