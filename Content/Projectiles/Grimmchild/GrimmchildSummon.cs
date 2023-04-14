using HollowKnightItems.Assets;
using HollowKnightItems.Common.Players;
using HollowKnightItems.Content.Buffs;
using HollowKnightItems.Content.NPCs;
using HollowKnightItems.Content.Projectiles.StateMachine;

namespace HollowKnightItems.Content.Projectiles.Grimmchild
{
    internal class GrimmchildSummon : SMProjectile
    {
        private const int FrameCount = 5;
        private const int SoundFrequency = 600;

        public override void SetStaticDefaults()
        {
            // 0-5：运动
            // 6-12：攻击
            // 13-19：瞬移
            // 20-22：休息
            Main.projFrames[Projectile.type] = 23;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 40;
            DrawOffsetX = -45;
            DrawOriginOffsetY = -40;

            Projectile.timeLeft = 9999;
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

        public override void Initialize()
        {
            RegisterState(new MoveState());
            RegisterState(new ShootState());
            RegisterState(new TeleportState());
            RegisterState(new RestState());
        }

        public override void AIBefore()
        {
            Player player = Main.player[Projectile.owner];
            // 玩家不再持有buff（卸除护符）时，让召唤物消失
            if (player.HasBuff(ModContent.BuffType<GrimmchildBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }

        public class MoveState : ProjState
        {
            public override void AI(SMProjectile proj)
            {
                var Projectile = proj.Projectile;
                Player player = Main.player[Projectile.owner];

                Vector2 offset = player.direction >= 0 ? new Vector2(-50, -50) : new Vector2(50, -50);  // 目标位置（玩家身后上方的位置）与玩家位置的偏移
                Vector2 dir = player.Center - Projectile.Center + offset;  // 目标方向
                float distance = dir.Length();  // 目标距离
                dir.Normalize();

                // 运动逻辑
                if (distance > 20)
                {
                    Projectile.velocity = (Projectile.velocity * 20f + dir * 5) / 21f;  // 渐进方式运动
                    Projectile.spriteDirection = Projectile.direction;  // sprite方向与运动方向一致
                }
                else
                {
                    Projectile.velocity *= 0.9f;  // 到达目标位置附近后减速
                    Projectile.spriteDirection = player.direction;  // sprite方向与玩家方向一致
                }

                // 动画效果
                if (Projectile.frame == 22)
                {
                    Projectile.frame = 0;
                }
                if (++Projectile.frameCounter >= FrameCount)  // 每张帧图持续帧数
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame == 6 || Projectile.frame == 13)  // 有可能在进入移动状态后处于攻击动画中
                    {
                        Projectile.frame = 0;
                    }
                }
                proj.Timer = 1;

                // 播放声音
                if (random.Next(0, SoundFrequency) == 0)
                {
                    SoundEngine.PlaySound(SoundLoader.Grimmchild_Routine, Projectile.position);
                }

                SwitchState_Grimmchild(proj);
            }
        }

        public class ShootState : ProjState
        {
            public override void AI(SMProjectile proj)
            {
                var Projectile = proj.Projectile;
                Player player = Main.player[Projectile.owner];
                Vector2 tar = proj.Target;

                Vector2 dirProj = tar - Projectile.Center;  // 攻击目标相对位置
                Vector2 offset = dirProj.X >= 0 ? new Vector2(47, 10) : new Vector2(-47, 10);  // 朝向不同时，射弹生成的位置也不同
                Vector2 vel = dirProj - offset;  // 射弹方向。由于射弹生成位置的偏移，使射弹方向也产生相应的偏移
                vel.Normalize();

                // 生成射弹
                if (Projectile.frame == 11 && proj.Timer == 0 && Main.netMode != NetmodeID.MultiplayerClient)  // 在第11张帧图时生成射弹，保证攻击与动画一致
                {
                    // 获取格林之子的阶段
                    int Stage = player.GetModPlayer<GrimmchidPlayer>().Stage;
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(),
                                                Projectile.Center + offset,
                                                vel * 13f,
                                                ModContent.ProjectileType<GrimmchildShoot>(),
                                                GetGrimmchildAttack(),
                                                Projectile.knockBack + 1,
                                                Projectile.owner,
                                                Stage,  // 初始化ai[0]为格林之子的阶段
                                                1);  // 分裂前的弹幕ai[1]标记为1
                    proj.Timer++;

                    //播放声音
                    SoundEngine.PlaySound(SoundLoader.Grimmchild_Attack, Projectile.position);
                }

                // 运动逻辑，保持位置在玩家附近
                Vector2 dirPlayer = tar - player.Center;  // 攻击目标相对玩家位置
                float distance = dirPlayer.Length();  // 攻击目标与玩家距离
                dirPlayer.Normalize();
                Vector2 moveTar = player.Center + dirPlayer * (distance > 125 ? 100 : distance * 0.8f);  // 移动目标位置
                Vector2 moveDir = moveTar - Projectile.Center;  // 移动方向
                moveDir.Normalize();
                Projectile.velocity = (Projectile.velocity * 20f + moveDir * 5) / 21f;
                Projectile.spriteDirection = dirProj.X > 0 ? 1 : -1;  // sprite方向与攻击方向一致

                // 动画效果
                if (Projectile.frame == 22)
                {
                    Projectile.frame = 0;
                }
                if (++Projectile.frameCounter >= FrameCount)  // 每张帧图持续帧数
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame == 6)
                    {
                        if (proj.Timer == 3)  // 循环两轮运动动画再进入攻击动画
                        {
                            proj.Timer = 0;
                        }
                        else
                        {
                            Projectile.frame = 0;
                            proj.Timer++;
                        }
                    }
                    if (Projectile.frame == 13)
                    {
                        Projectile.frame = 0;
                    }
                }

                SwitchState_Grimmchild(proj);
            }
        }

        public class TeleportState : ProjState
        {
            public override void AI(SMProjectile proj)
            {
                var Projectile = proj.Projectile;
                Player player = Main.player[Projectile.owner];

                if (Vector2.Distance(Projectile.Center, player.Center) > 800)
                {
                    Projectile.position = player.Center + new Vector2(-15, -20);  // 传送至与玩家重合位置，尽量防止传送后被卡住
                    Projectile.frame = 13;
                }

                if (++Projectile.frameCounter >= FrameCount)  // 每张帧图持续帧数
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame == 20)  // 播放完瞬移动画
                    {
                        Projectile.frame = 0;
                        // 在瞬移动画播放完后再考虑切换状态，保证瞬移不被打断
                        SwitchState_Grimmchild(proj);
                    }
                }
                Projectile.velocity = new(0, 0);  // 瞬移时速度保持为0
            }
        }

        public class RestState : ProjState
        {
            public override void AI(SMProjectile proj)
            {
                var Projectile = proj.Projectile;
                Player player = Main.player[Projectile.owner];

                // 把目标位置定在玩家脚下
                Vector2 dir = player.Bottom - Projectile.Bottom - new Vector2(0, 30);
                float distance = dir.Length();
                dir.Normalize();

                if (distance > 1)
                {
                    // 移动至目标位置
                    Projectile.velocity = (Projectile.velocity * 20f + dir * 5) / 21f;  // 渐进方式运动
                    Projectile.spriteDirection = Projectile.direction;  // 图方向与运动方向一致

                    // 动画效果，与运动状态一致
                    if (Projectile.frame == 22)
                    {
                        Projectile.frame = 0;
                    }
                    if (++Projectile.frameCounter >= FrameCount)  // 每张帧图持续帧数
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame == 6 || Projectile.frame == 13)
                        {
                            Projectile.frame = 0;
                        }
                    }
                }
                else
                {
                    // 到达位置后停下
                    Projectile.velocity = new(0, 0);
                    Projectile.spriteDirection = Projectile.oldDirection;

                    // 动画效果
                    if (++Projectile.frameCounter >= FrameCount * 2)  // 每张帧图持续帧数
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame == 6)  // 走完这一轮动画后，开始休息
                        {
                            Projectile.frame = 20;
                        }
                        if (Projectile.frame == 23)
                        {
                            Projectile.frame = 22;  // 保持休息姿势不变
                        }
                    }
                }

                // 播放声音                 
                if (random.Next(0, SoundFrequency) == 0)
                {
                    SoundEngine.PlaySound(SoundLoader.Grimmchild_Routine, Projectile.position);
                }

                SwitchState_Grimmchild(proj);
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public static void SwitchState_Grimmchild(SMProjectile proj)
        {
            var Projectile = proj.Projectile;
            Player player = Main.player[Projectile.owner];
            NPC npc = FindClosestEnemy(Projectile.Center, 500f, (n) =>
            {
                return n.CanBeChasedBy() &&
                !n.dontTakeDamage &&
                Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1) &&
                n.type != ModContent.NPCType<GrimmBoss>();
            });

            if (Vector2.Distance(Projectile.Center, player.Center) > 800)
            {
                proj.SetState<TeleportState>();  // 传送状态                  
            }
            else if (npc != null & player.GetModPlayer<GrimmchidPlayer>().Type)
            {
                proj.SetState<ShootState>();  // 射击状态
                proj.Target = npc.Center;
            }
            else if (player.sitting.isSitting || player.sleeping.isSleeping)
            {
                proj.SetState<RestState>();  // 休息状态
            }
            else
            {
                proj.SetState<MoveState>();  // 移动状态
            }

            // 联机同步
            Projectile.netUpdate = true;
        }
    }
}
