using HollowKnightItems.Content.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HollowKnightItems.Content.Projectiles.Grimm
{
    [Autoload(false)]
    internal class GrimmProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
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
            // 弹幕拖尾的Dust
            Dust.NewDust(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.TintableDustLighted, newColor: new Color(255, 0, 0));            
        }
    }

    [Autoload(true)]
    internal class GrimmFirebird : GrimmProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 60;
            Projectile.height = 30;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            base.AI();
            Projectile.spriteDirection = Projectile.direction;

            Player player = null;
            foreach(NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<GrimmBoss>())
                {
                    player = Main.player[n.target];
                }
            }
            if (player != null)
            {
                if (player.Center.Y > Projectile.position.Y)
                {
                    Projectile.velocity.Y = 1;
                }
                else if(player.Center.Y < Projectile.position.Y)
                {
                    Projectile.velocity.Y = -1;
                }
            }

        }
    }

    [Autoload(true)]
    internal class GrimmFireball_S : GrimmProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            base.AI();
            if (Projectile.ai[0] == 0)
            {
                if (Projectile.velocity.X > 0)
                {
                    Projectile.ai[0] = Projectile.position.X + 90;
                }
                else
                {
                    Projectile.ai[0] = Projectile.position.X - 90;
                }
                Projectile.ai[1] = Projectile.position.Y + Projectile.velocity.Y * 45;             
            }

            Vector2 pos = new(Projectile.ai[0], Projectile.ai[1]);
            Vector2 dir = pos - Projectile.position;
            dir.Normalize();
            Vector2 newVel;
            if (Projectile.velocity.X > 0)
            {
                if (Projectile.position.X < pos.X) 
                {
                    newVel = dir + Projectile.velocity * 5;
                    newVel.Normalize();
                    Projectile.velocity = newVel * 5;
                }
                else
                {
                    Projectile.velocity = new Vector2(5, 0);
                }
            }
            else
            {
                if (Projectile.position.X > pos.X)
                {
                    newVel = dir + Projectile.velocity * 5;
                    newVel.Normalize();
                    Projectile.velocity = newVel * 5;
                }
                else
                {
                    Projectile.velocity = new Vector2(-5, 0);
                }
            }
        }
    }

    [Autoload(true)]
    internal class GrimmFireball_L : GrimmProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            base.AI();

            if (Projectile.velocity.Y < 12)
            {
                Projectile.velocity.Y += 1;
            }            
        }
    }
}
