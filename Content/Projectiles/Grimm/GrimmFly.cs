using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HollowKnightItems.Content.Projectiles.Grimm
{
    internal class GrimmFly : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 40;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.friendly = false;
            Projectile.hostile = true;            
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
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
                if (n.type == ModContent.NPCType<NPCs.GrimmBoss>())
                {
                    npc = n;
                }
            }
            
            if (npc == null) 
            {
                Projectile.timeLeft = 1;
                return;
            }
            Vector2 dir = npc.Center - Projectile.Center;

            Projectile.velocity = (Projectile.velocity * 20f + dir * 5) / 21f;  // 渐进方式运动
            Projectile.spriteDirection = Projectile.direction;
        }    
    }
}
