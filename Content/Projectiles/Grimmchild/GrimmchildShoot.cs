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
            Projectile.width = 34;
            Projectile.height = 34;

            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;

            Projectile.light = 0.2f;

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

            // 弹幕拖尾的Dust
            for (int i = 0; i < 9; i++)
            {
                Dust.NewDust(Projectile.position - Projectile.velocity, 30, 30, ModContent.DustType<TailingFlame>(), newColor: new Color(255, 89, 89));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }

            // 弹幕碰撞的Dust
            for (int i = 0; i < 12; i++)
            {
                Vector2 rotation = (i * MathHelper.Pi / 60).ToRotationVector2();
                Dust.NewDust(Projectile.position, 1, 1, DustID.TintableDustLighted, rotation.X, rotation.Y, newColor: new Color(255, 0, 0));
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // 弹幕碰撞的Dust
            for (int i = 0; i < 12; i++)
            {
                Vector2 rotation = (i * MathHelper.Pi / 60).ToRotationVector2();
                Dust.NewDust(Projectile.position, 1, 1, DustID.TintableDustLighted, rotation.X, rotation.Y, newColor: new Color(255, 0, 0));
            }
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
}
