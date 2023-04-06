using HollowKnightItems.Content.NPCs;

namespace HollowKnightItems.Content.Projectiles.Grimm
{
    internal class GrimmFly : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 68;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 380;
            Projectile.friendly = false;
            Projectile.hostile = true;            
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;

            Main.projPet[Type] = true;  // 召唤物必备属性，屏蔽召唤物的接触伤害
        }

        public override void AI()
        {
            NPC npc = null;
            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<GrimmBoss>())
                {
                    npc = n;
                }
            }            
            if (npc == null || npc.life <= 0) 
            {
                Projectile.active = false;
                return;
            }

            if (Projectile.ai[0] == 0)
            {
                // 区分不同的分身
                switch (Projectile.velocity.X)
                {
                    // 设定实际的初始速度
                    case 0: 
                        Projectile.velocity = new Vector2(16, 8);
                        break;
                    case 1:
                        Projectile.velocity = new Vector2(-16, 8);
                        break;
                    case 2:
                        Projectile.velocity = new Vector2(16, -8);
                        break;
                    case 3:
                        Projectile.velocity = new Vector2(-16, -8);
                        break;
                    case 4:
                        Projectile.velocity = new Vector2(8, 16);
                        break;
                    case 5:
                        Projectile.velocity = new Vector2(-8, 16);
                        break;
                    case 6:
                        Projectile.velocity = new Vector2(8, -16);
                        break;
                    case 7:
                        Projectile.velocity = new Vector2(-8, -16);
                        break;
                }
            }

            if (Projectile.ai[0] < 250)
            {
                MoveBetween(Projectile, npc.Center, 150);
            }
            else
            {
                Vector2 dir = npc.Center - Projectile.Center;
                if (dir.Length() < 20)
                {
                    Projectile.active = false;
                }
                dir.Normalize();
                Projectile.velocity = dir * 18;
            }
            Projectile.spriteDirection = Projectile.direction;
            Projectile.ai[0]++;
        }    
    }
}
