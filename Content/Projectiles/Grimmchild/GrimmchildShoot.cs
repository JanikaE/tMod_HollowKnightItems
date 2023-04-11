using HollowKnightItems.Assets;
using HollowKnightItems.Content.Dusts;

namespace HollowKnightItems.Content.Projectiles.Grimmchild
{
    internal class GrimmchildShoot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 200;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;

            Projectile.ArmorPenetration = 9999;
        }

        public override void AI()
        {
            //if (++Projectile.frameCounter >= 10)
            //{
            //    Projectile.frameCounter = 0;
            //    if (++Projectile.frame >= Main.projFrames[Projectile.type])
            //    {
            //        Projectile.frame = 0;
            //    }
            //}
            switch (Projectile.ai[1])
            {
                case 1:
                    // 让弹幕在自然消失时也分裂
                    if (Projectile.timeLeft == 1)
                    {
                        Split(Projectile.velocity);
                    }
                    break;
                case 2:
                    // 让弹幕在自然消失时逐渐缩小
                    if (Projectile.timeLeft == 1)
                    {
                        Projectile.timeLeft = 2;
                        Projectile.scale *= 0.95f;
                    }
                    if (Projectile.scale < 0.1f)
                    {
                        Projectile.Kill();
                    }
                    break;
            }
            TailDust(Projectile, ModContent.DustType<TailingFlame>(), (int)(9 * Projectile.scale));
        }

        public override void OnSpawn(IEntitySource source)
        {
            // 初始化弹幕，区分分裂前后
            switch (Projectile.ai[1]) 
            {
                case 1:                    
                    Projectile.scale = 1f;
                    Projectile.penetrate = 1;
                    break;
                case 2:
                    Projectile.scale = 0.5f;
                    Projectile.penetrate = -1;
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            switch (Projectile.ai[1])
            {
                case 1:
                    Split(oldVelocity);
                    Projectile.Kill();
                    break;
                case 2:
                    // Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

                    // 弹幕反弹
                    // If the projectile hits the left or right side of the tile, reverse the X velocity
                    if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                    {
                        Projectile.velocity.X = -oldVelocity.X;
                    }
                    // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
                    if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    {
                        Projectile.velocity.Y = -oldVelocity.Y;
                    }
                    break;
            }
            ProjectileHitDust(Projectile.position, DustID.TintableDustLighted, new Color(255, 0, 0));
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[1] == 1)
            {
                Split(Projectile.velocity);
            }
            ProjectileHitDust(Projectile.position, DustID.TintableDustLighted, new Color(255, 0, 0));
        }
                
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        // 图像就直接用shader来画，原图给个形状就行
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            EffectLoader.ApplyEffect_Fireball(new Color(255, 153, 153), new Color(255, 90, 90));
            return true;
        }

        /// <summary>
        /// 生成分裂的弹幕
        /// </summary>
        private void Split(Vector2 velocity)
        {
            if (Projectile.ai[0] > 1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                // 在2/3/4阶段分别分裂出3/6/9个小弹幕
                int num = ((int)Projectile.ai[0] - 1) * 3;
                ProjectileSplit(Projectile, ModContent.ProjectileType<GrimmchildShoot>(), num, velocity, 2f, GetGrimmchildAttack() / 2,ai1: 2);
            }
        }
    }
}
