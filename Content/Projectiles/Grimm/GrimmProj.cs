using HollowKnightItems.Assets;
using HollowKnightItems.Content.Dusts;
using HollowKnightItems.Content.NPCs;

namespace HollowKnightItems.Content.Projectiles.Grimm
{
    /*  GrimmProj
     *      -GrimmThorn
     *      -GrimmFirebird
     *      -GrimmFireball
     *      -GrimmShoot
     *          -GrimmShoot_Sides
     *          -GrimmShoot_Below
     */
    [Autoload(false)]
    internal class GrimmProj : ModProjectile
    {
        public static int TailType => ModContent.DustType<TailingFlame>();

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
    }

    [Autoload(true)]
    internal class GrimmThorn : GrimmProj
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Nightmare Thorn");
            DisplayName.AddTranslation(7, "梦魇尖刺");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 120;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                int offset = Math.Abs((int)Projectile.velocity.Y / 20);
                Projectile.velocity.X = random.Next(- offset, offset + 1);
                Projectile.ai[0] = 1;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }

    [Autoload(true)]
    internal class GrimmFirebird : GrimmProj
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Nightmare Firebird");
            DisplayName.AddTranslation(7, "梦魇火鸟");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 116;
            Projectile.height = 60;
            DrawOriginOffsetY = -20;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            TailDust(Projectile, TailType, Projectile.height / 10);
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
                if (player.Center.Y > Projectile.position.Y && Projectile.velocity.Y < 4)
                {
                    Projectile.velocity.Y++;
                }
                else if(player.Center.Y < Projectile.position.Y && Projectile.velocity.Y > -4)
                {
                    Projectile.velocity.Y--;
                }
            }
        }
    }

    [Autoload(true)]
    internal class GrimmFireball : GrimmProj
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Nightmare Big Fireball");
            DisplayName.AddTranslation(7, "梦魇大火球");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            TailDust(Projectile, TailType, Projectile.height);
            // 近似抛物线的运动
            if (Projectile.ai[0] % 3 == 0) 
            {
                Projectile.velocity.Y += 1;
            }
            if (Projectile.ai[0] % 10 == 0)
            {
                Projectile.velocity.X *= 0.95f;
            }
            Projectile.ai[0]++;
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
            EffectLoader.Fireball.Parameters["uColorCenter"].SetValue(new Vector4(1, 1, 1, 1));  // 设置中心颜色
            EffectLoader.Fireball.Parameters["uColorEdge"].SetValue(new Vector4(1, (float)0.34, (float)0.37, 1));  // 设置边缘颜色
            EffectLoader.Fireball.CurrentTechnique.Passes["Test"].Apply();
            return true;
        }
    }

    [Autoload(false)]
    internal class GrimmShoot : GrimmProj
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Nightmare Fireball");
            DisplayName.AddTranslation(7, "梦魇火球");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.timeLeft = 300;
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

        public override void AI()
        {
            TailDust(Projectile, TailType, Projectile.height / 10);
        }
    }

    [Autoload(true)]
    internal class GrimmShoot_Sides : GrimmShoot 
    {
        public override void AI()
        {
            base.AI();
            // 初始速度决定了弹幕第一阶段的目标位置
            if (Projectile.ai[0] == 0)
            {
                if (Projectile.velocity.X > 0)
                {
                    Projectile.ai[0] = Projectile.position.X + 120;
                }
                else
                {
                    Projectile.ai[0] = Projectile.position.X - 120;
                }
                Projectile.ai[1] = Projectile.position.Y + Projectile.velocity.Y * 60;
            }

            Vector2 pos = new(Projectile.ai[0], Projectile.ai[1]);
            Vector2 dir = pos - Projectile.position;
            if (Projectile.velocity.X > 0)
            {
                Projectile.velocity.X = 6;
                if (Projectile.position.X < pos.X)
                {
                    // 保证同一轮的弹幕在同一时间到达目标位置
                    Projectile.velocity.Y = 12 * dir.Y / dir.X;
                }
                else
                {
                    // 然后平行运动
                    Projectile.velocity.Y = 0;
                }
                
            }
            else
            {
                Projectile.velocity.X = -6;
                if (Projectile.position.X > pos.X)
                {
                    Projectile.velocity.Y = - 12 * dir.Y / dir.X;
                }
                else
                {
                    Projectile.velocity.Y = 0;
                }
            }
        }
    }

    [Autoload(true)]
    internal class GrimmShoot_Below : GrimmShoot
    {
        public override void AI()
        {
            base.AI();
            Vector2 vel = Projectile.velocity;
            vel.Normalize();
            Projectile.velocity = vel * 6;
        }
    }
}
