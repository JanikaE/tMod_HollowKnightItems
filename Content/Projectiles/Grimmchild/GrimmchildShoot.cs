using HollowKnightItems.Assets;
using HollowKnightItems.Content.Dusts;

namespace HollowKnightItems.Content.Projectiles.Grimmchild
{
    [Autoload(false)]
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

            TailDust(Projectile, ModContent.DustType<TailingFlame>(), (int)(Projectile.width * Projectile.scale / 4));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate == 0)
            {
                Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

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
            }

            HitDust(Projectile.position, DustID.TintableDustLighted, new Color(255, 0, 0));
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            HitDust(Projectile.position, DustID.TintableDustLighted, new Color(255, 0, 0));
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
            EffectLoader.Fireball.Parameters["uColorCenter"].SetValue(new Vector4(1, (float)0.6, (float)0.6, 1));  // 设置中心颜色
            EffectLoader.Fireball.Parameters["uColorEdge"].SetValue(new Vector4(1, (float)0.35, (float)0.35, 1));  // 设置边缘颜色
            EffectLoader.Fireball.CurrentTechnique.Passes["Test"].Apply();
            return true;
        }
    }

    [Autoload(true)]
    internal class GrimmchildShoot_Big : GrimmchildShoot 
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 17;
            Projectile.height = 17;

            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            base.AI();

            // 让弹幕在自然消失时也分裂
            if (Projectile.timeLeft == 1)
            {
                Split(Projectile.velocity);
            }

            // 将格林之子的阶段反映在弹幕行为上
            switch (Projectile.ai[0])
            {
                case 1:
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Split(oldVelocity);
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Split(Projectile.velocity);
            base.OnHitNPC(target, damage, knockback, crit);
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
                for (int i = 0; i < 360; i += 360 / num)
                {
                    float vel = velocity.ToRotation() + (i * MathHelper.Pi / 180);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                                            Projectile.position,
                                            velocity + vel.ToRotationVector2() * 2f,
                                            ModContent.ProjectileType<GrimmchildShoot_Small>(),
                                            GetGrimmchildAttack() / 2,
                                            Projectile.knockBack + 1,
                                            Projectile.owner);
                }
            }
        }
    }

    [Autoload(true)]
    internal class GrimmchildShoot_Small : GrimmchildShoot 
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 17;
            Projectile.height = 17;

            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();

            if (Projectile.timeLeft == 1)
            {
                Projectile.timeLeft = 2;
                Projectile.scale *= 0.95f;
            }
            if (Projectile.scale < 0.1f)
            {
                Projectile.Kill();
            }
        }
    }
}
