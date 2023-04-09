using HollowKnightItems.Assets;
using HollowKnightItems.Content.Dusts;
using HollowKnightItems.Content.NPCs;

namespace HollowKnightItems.Content.Projectiles.Grimm
{
    /*  GrimmProj
     *      -GrimmSpike
     *      -GrimmFirebird
     *      -GrimmFireball
     *      -GrimmShoot
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

    [Autoload(false)]
    internal class GrimmSpike : GrimmProj
    {
        public override string Texture => "HollowKnightItems/Content/Projectiles/Grimm/GrimmSpike";

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
            Projectile.height = 24;
            DrawOriginOffsetY = -48;
            Projectile.timeLeft = 240;
        }
    }

    [Autoload(true)]
    internal class GrimmSpike_Thorn : GrimmSpike 
    {
        public override void AI()
        {
            // 弹幕的轮次通过ai[1]的个位传入
            int turn = (int)(Projectile.ai[1] % 10);
            // 弹幕是否指向玩家通过ai[1]的十位传入
            bool isTar = Projectile.ai[1] >= 10;

            if (Projectile.ai[0] == 10)
            {
                Projectile.velocity = Vector2.Zero;
            }
            if (isTar)
            {
                if (Projectile.ai[0] + turn * 10 >= 30 && Projectile.ai[0] + turn * 10 < 60)
                {
                    Player player = GetNPCTarget(ModContent.NPCType<GrimmBoss>());
                    Vector2 dir;
                    if (player != null)
                    {
                        dir = player.Center - Projectile.Center;
                    }
                    else
                    {
                        dir = Vector2.Zero;
                    }
                    // 将方向指向玩家位置
                    Projectile.rotation = dir.ToRotation() + MathHelper.PiOver2;
                }
                else if (Projectile.ai[0] + turn * 10 >= 60)
                {
                    Vector2 dir = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
                    Projectile.velocity = dir * 20;
                }
            }
            else
            {
                if (Projectile.ai[0] + turn * 10 == 60)
                {
                    Vector2 dir = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
                    Projectile.velocity = dir * 20;
                }
            }            
            Projectile.ai[0]++;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // 弹幕的旋转角度通过ai[0]传入
            Projectile.rotation = Projectile.ai[0] * MathHelper.Pi / 180;
            Vector2 dir = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            Projectile.velocity = dir * 12;
            // 随后将ai[0]用作计时器
            Projectile.ai[0] = 0;
        }
    }

    [Autoload(true)]
    internal class GrimmSpike_Swoop : GrimmSpike
    {

        public override void AI()
        {
            if (Projectile.ai[0] < 20 || Projectile.ai[0] > 60)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.ai[1] = Projectile.velocity.Length();
            }
            else if (Projectile.ai[0] < 50)
            {
                Player player = GetNPCTarget(ModContent.NPCType<GrimmBoss>());
                Vector2 dir;
                if (player != null)
                {
                    dir = player.Center - Projectile.Center;                    
                }
                else
                {
                    dir = Vector2.Zero;
                }
                // 停止运动，将方向指向玩家位置
                Projectile.rotation = dir.ToRotation() + MathHelper.PiOver2;
                Projectile.velocity = Vector2.Zero;
            }
            else if (Projectile.ai[0] == 60)
            {
                Vector2 dir = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
                Projectile.velocity = dir * Projectile.ai[1];
            }
            Projectile.ai[0]++;
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

            Player player = GetNPCTarget(ModContent.NPCType<GrimmBoss>());
            if (player != null && Projectile.ai[0] > 10)
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

            Projectile.ai[0]++;
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
            Projectile.timeLeft = 125;
        }

        public override void AI()
        {
            TailDust(Projectile, TailType, 2, 1.5f);
            // 近似抛物线的运动
            //if (Projectile.ai[0] % 3 == 0) 
            //{
            //    Projectile.velocity.Y += 1;
            //}
            //if (Projectile.ai[0] % 10 == 0)
            //{
            //    Projectile.velocity.X *= 0.95f;
            //}

            // 一点曲线运动
            float rot = Projectile.velocity.ToRotation() + 0.05f;
            Vector2 oldVel = Projectile.velocity;
            Projectile.velocity = rot.ToRotationVector2() * oldVel.Length();

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
            EffectLoader.ApplyEffect_Fireball(new Color(255, 255, 255), new Color(255, 90, 90));
            return true;
        }
    }

    [Autoload(true)]
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
            EffectLoader.ApplyEffect_Fireball(new Color(255, 153, 153), new Color(255, 90, 90));
            return true;
        }

        public override void AI()
        {
            TailDust(Projectile, TailType, (int)(4 * Projectile.scale));
            
            // ai[1]等于1/2表示两种不同的分裂设计
            if (Projectile.ai[0] > 60 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Projectile.ai[1] == 1)
                {
                    ProjectileSplit(Projectile, Type, 8, Vector2.Zero, 15, Projectile.damage, ai1: -1);
                }
                else if (Projectile.ai[1] == 2)
                {
                    ProjectileSplit(Projectile, Type, 4, Projectile.velocity, 0.5f, Projectile.damage, ai1: -2);
                }
                Projectile.Kill();
            }
            // ai[1]等于3/4表示两种不同方向的圆周运动
            if (Projectile.ai[1] == 3)
            {
                float rot = Projectile.velocity.ToRotation() + 0.01f;
                Vector2 oldVel = Projectile.velocity;
                Projectile.velocity = rot.ToRotationVector2() * oldVel.Length();
            }
            if (Projectile.ai[1] == 4)
            {
                float rot = Projectile.velocity.ToRotation() - 0.01f;
                Vector2 oldVel = Projectile.velocity;
                Projectile.velocity = rot.ToRotationVector2() * oldVel.Length();
            }

            Projectile.ai[0]++;
        }

        public override void OnSpawn(IEntitySource source)
        {
            switch (Projectile.ai[1])
            {
                case 1:
                    Projectile.scale = 1.5f;
                    break;
                case -1:
                    Projectile.scale = 0.6f;
                    break;
                case -2:
                    Projectile.scale = 0.5f;
                    break;
            }
        }
    }

    // old
    [Autoload(false)]
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

    // old
    [Autoload(false)]
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
