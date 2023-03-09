using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HollowKnightItems.Content.Projectiles.GrimmBoss
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

            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;

            Projectile.light = 0.2f;

            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;
        }
    }
}
