using HollowKnightItems.Content.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HollowKnightItems.Content.Projectiles.GrimmBoss
{
    internal class GrimmFly : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 40;

            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.minionSlots = 0;  // 占用召唤物位置
            Projectile.minion = true;  // 防止召唤物离玩家太远而被刷掉
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;

            Main.projPet[Type] = true;  // 召唤物必备属性，屏蔽召唤物的接触伤害
        }

        public override void AI()
        {
            NPC npc = null;
            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<Grimm>())
                {
                    npc = n;
                }
            }
            
            if (npc == null) 
            {
                Projectile.timeLeft = 1;
                return;
            }
            Vector2 tar = npc.Center;

            float Vx = Projectile.position.X - tar.X > 0 ? -30 : 30;
            float Vy = Projectile.position.Y - tar.Y > 0 ? -10 : 10;
            Projectile.velocity += new Vector2(Vx, Vy);
        }    
    }
}
