using HollowKnightItems.Common.Players;
using HollowKnightItems.Content.Buffs;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HollowKnightItems.Content.Projectiles.Grimmchild
{
    [Autoload(false)]
    internal class GrimmchildSummon_old : ModProjectile
    {
        public enum ProjectileState
        {
            MoveAround,
            ShootAround,
            Teleport,
            Rest,
        }
        // 状态控制
        public ProjectileState State
        {
            get { return (ProjectileState)(int)Projectile.ai[0]; }
            set { Projectile.ai[0] = (int)value; }
        }
        // 攻击频率控制
        public int ShootCtrl
        {
            get { return (int)Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }

        // 一些标记变量
        private int teleportState;
        private int soundTimer;

        // 设置声音
        SoundStyle attack = new SoundStyle("HollowKnightItems/Assets/Audio/GrimmchildAttack_", 2) with
        {
            Volume = 1f,  // 音量
            Pitch = 0,  // 音调
            PitchVariance = 0,  // 音调随机浮动
            MaxInstances = 1,  // 叠加播放上限
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew,  // 到达上限后的操作
            Type = SoundType.Sound,   // 声音类型（音乐/音效/环境）
        };
        SoundStyle routine = new SoundStyle("HollowKnightItems/Assets/Audio/GrimmchildRoutine_", 6) with
        {
            Volume = 1f,
            Pitch = 0,
            PitchVariance = 0,
            MaxInstances = 1,
            SoundLimitBehavior = SoundLimitBehavior.IgnoreNew,
            Type = SoundType.Sound,
        };

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

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.HasBuff(ModContent.BuffType<GrimmchildBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            ProjectileState prevState = State;
            NPC npc = FindCloestEnemy(Projectile.Center, 500f, (n) =>
            {
                return n.CanBeChasedBy() &&
                !n.dontTakeDamage && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1);
            });

            // 判断状态
            if (Vector2.Distance(Projectile.Center, player.Center) > 800)
            {
                Projectile.position = player.Center + new Vector2(-15, -20);  // 传送至与玩家重合位置，尽量防止传送后被卡住
                Projectile.frame = 13;
                State = ProjectileState.Teleport;  // 传送状态
            }
            else if (teleportState == 1)
            {
                State = ProjectileState.Teleport;  // 保持传送状态，直至传送动画完成
            }
            else if (npc != null & player.GetModPlayer<CharmsPlayer>().GrimmchildType)
            {
                State = ProjectileState.ShootAround; // 射击状态
            }
            else if (player.sitting.isSitting || player.sleeping.isSleeping)
            {
                State = ProjectileState.Rest; // 休息状态
            }
            else
            {
                State = ProjectileState.MoveAround;  // 移动状态
            }

            // 联机同步
            if (prevState != State)
            {
                Projectile.netUpdate = true;
            }

            // 选择状态
            switch (State)
            {
                case ProjectileState.MoveAround:
                    {
                        MoveAround(player);
                        break;
                    }
                case ProjectileState.ShootAround:
                    {
                        ShootAround(npc.Center, player);
                        break;
                    }
                case ProjectileState.Teleport:
                    {
                        Teleport();
                        break;
                    }
                case ProjectileState.Rest:
                    {
                        Rest(player);
                        break;
                    }
            }
        }

        public static NPC FindCloestEnemy(Vector2 position, float maxDistance, Func<NPC, bool> predicate)
        {
            NPC res = null;
            foreach (var npc in Main.npc.Where(n => n.active && !n.friendly && predicate(n)))
            {
                float dis = Vector2.Distance(position, npc.Center);
                if (dis < maxDistance)
                {
                    maxDistance = dis;
                    res = npc;
                }
            }
            return res;
        }

        public void MoveAround(Player player)
        {
            Vector2 offset = player.direction >= 0 ? new Vector2(-50, -50) : new Vector2(50, -50);  // 目标位置（玩家身后上方的位置）与玩家位置的偏移
            Vector2 dir = player.Center - Projectile.Center + offset;  // 目标方向
            float distance = dir.Length();  // 目标距离
            dir.Normalize();

            // 运动逻辑
            if (distance > 20)
            {
                Projectile.velocity = (Projectile.velocity * 20f + dir * 5) / 21f;  // 渐进方式运动
                Projectile.spriteDirection = Projectile.direction;  // 图方向与运动方向一致
            }
            else
            {
                Projectile.velocity *= 0.9f;  // 到达目标位置附近后减速
                Projectile.spriteDirection = player.direction;  // 图方向与玩家方向一致
            }

            // 动画效果
            if (Projectile.frame == 22)
            {
                Projectile.frame = 0;
            }
            if (++Projectile.frameCounter >= 5)  // 每张图持续5帧
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame == 6 || Projectile.frame == 13)  // 如果仍处在攻击动画中，让这一轮动画走完
                {
                    Projectile.frame = 0;
                }
            }
            ShootCtrl = 1;

            // 播放声音
            if (Main.LocalPlayer == player)
            {
                soundTimer++;
                if (soundTimer == 300)
                {
                    SoundEngine.PlaySound(routine);
                    soundTimer = 0;
                }
            }
        }

        public void ShootAround(Vector2 tar, Player player)
        {
            Vector2 dirProj = tar - Projectile.Center;  // NPC相对位置
            Vector2 offset = dirProj.X >= 0 ? new Vector2(47, 10) : new Vector2(-47, 10);  // 朝向不同时，射弹生成的位置也不同
            Vector2 vel = dirProj - offset;  // 射弹方向。由于射弹生成位置的偏移，使射弹方向也产生相应的偏移
            vel.Normalize();

            // 生成射弹
            if (Projectile.frame == 11 && ShootCtrl == 0)  // 在第11张图时生成射弹，保证攻击与动画一致
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(),
                    Projectile.Center + offset,  // 生成位置
                    vel * 13f,  // 速度
                    ModContent.ProjectileType<GrimmchildShoot>(),
                    Projectile.damage + GetGrimmchildAttack(),
                    Projectile.knockBack + 1,
                    Projectile.owner);
                ShootCtrl++;  // 限制在一张图的多帧内只生成一次射弹

                // 播放声音
                if (Main.LocalPlayer == player)
                {
                    SoundEngine.PlaySound(attack);
                    soundTimer = 0;
                }
            }

            // 运动逻辑，保持位置在玩家附近
            Vector2 dirPlayer = tar - player.Center;  // NPC相对玩家位置
            float distance = dirPlayer.Length();  // NPC与玩家距离
            dirPlayer.Normalize();
            Vector2 moveTar = player.Center + dirPlayer * (distance > 125 ? 100 : distance * 0.8f);  // 移动目标位置
            Vector2 moveDir = moveTar - Projectile.Center;  // 移动方向
            moveDir.Normalize();
            Projectile.velocity = (Projectile.velocity * 20f + moveDir * 5) / 21f;

            // 动画效果
            if (Projectile.frame == 22)
            {
                Projectile.frame = 0;
            }
            if (++Projectile.frameCounter >= 5)  // 每张图持续5帧
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame == 6)
                {
                    if (ShootCtrl == 3)  // 循环两轮运动动画再进入攻击动画
                    {
                        ShootCtrl = 0;
                    }
                    else
                    {
                        Projectile.frame = 0;
                        ShootCtrl++;
                    }
                }
                if (Projectile.frame == 13)
                {
                    Projectile.frame = 0;
                }
            }
            Projectile.spriteDirection = dirProj.X > 0 ? 1 : -1;  // 图方向与攻击方向一致
        }

        public void Teleport()
        {
            teleportState = 1;

            // 动画效果
            if (++Projectile.frameCounter >= 5)  // 每张图持续5帧
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame == 20)  // 播放完瞬移动画
                {
                    Projectile.frame = 0;
                    teleportState = 0;  // 结束瞬移状态
                }
            }

            Projectile.velocity = new(0, 0);  // 瞬移时速度为0

            soundTimer = 0;
        }

        public void Rest(Player player)
        {
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
                if (++Projectile.frameCounter >= 5)  // 每张图持续5帧
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
                if (++Projectile.frameCounter >= 10)  // 每张图持续10帧
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
            if (Main.LocalPlayer == player)
            {
                soundTimer++;
                if (soundTimer == 300)
                {
                    SoundEngine.PlaySound(routine);
                    soundTimer = 0;
                }
            }
        }
    }
}