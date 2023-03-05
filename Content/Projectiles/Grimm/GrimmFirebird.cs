using Terraria;
using Terraria.ModLoader;

namespace HollowKnightItems.Content.Projectiles.Grimm
{
    internal class GrimmFirebird : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 30;

            Projectile.penetrate = 10;
            Projectile.timeLeft = 200;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;

            Projectile.light = 0.2f;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;
        }
    }
}
