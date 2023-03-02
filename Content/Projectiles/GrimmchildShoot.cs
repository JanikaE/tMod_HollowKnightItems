using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HollowKnightItems.Content.Projectiles
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

            Projectile.maxPenetrate = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;

            Projectile.light = 0.5f;

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
            for (int i = 0; i< 5; i++)
            {
                Dust.NewDust(Projectile.position - Projectile.velocity, 30, 30, DustID.TintableDustLighted, newColor: new Color(255, 0, 0));
            }            
        }

        // 图像就直接用shader来画，原图给个形状就行
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            HollowKnightItems.Fireball.CurrentTechnique.Passes["Test"].Apply();
            return true;
        }
    }
}
