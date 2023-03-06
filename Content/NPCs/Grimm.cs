using HollowKnightItems.Common.Systems;
using HollowKnightItems.Content.NPCs.StateMachine;
using HollowKnightItems.Content.Projectiles.Grimm;
using Microsoft.Xna.Framework;
using System;
using System.Drawing.Imaging;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static HollowKnightItems.Content.NPCs.Grimm;
using static HollowKnightItems.Content.NPCs.Utils;

namespace HollowKnightItems.Content.NPCs
{
    [AutoloadBossHead]
    internal class Grimm : SMNPC
    {        
        public const int NormalDefence = 20;

        public const int AttackDistance = 500;
        public const int SprintSpeed = 10;

        public override void SetStaticDefaults()
        {
            // 这里以后写到Localization里去
            DisplayName.SetDefault("Grimm");
            DisplayName.AddTranslation(7, "格林团长");
            //Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;  // 这两个代表这个NPC的碰撞箱宽高，以及tr会从你的贴图里扣多大的图
            NPC.aiStyle = -1;
            NPC.damage = 100;
            NPC.lifeMax = 8000;
            NPC.defense = 0;  // 在每个状态里再分别写防御
            NPC.scale = 1f;  // npc的贴图和碰撞箱的放缩倍率
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.value = Item.buyPrice(0, 4, 50, 0);  // NPC的爆出来的MONEY的数量，四个空从左到右是铂金，金，银，铜
            NPC.lavaImmune = true;  // 对岩浆免疫
            NPC.noGravity = true;  // 不受重力影响
            NPC.noTileCollide = true;  // 可穿墙
            NPC.npcSlots = 20;  // NPC所占用的NPC数量
            NPC.boss = true;  // 将npc设为boss。会掉弱治药水和心，会显示xxx已被击败，会有血条
            NPC.dontTakeDamage = false;  // 为true则为无敌。弹幕不会打到npc，并且不显示npc的血条
            NPC.netAlways = true;
            Music = MusicLoader.GetMusicSlot("HollowKnightItems/Assets/Audio/TheGrimmTroupe");
        }

        public override void Initialize()
        {
            RegisterState(new StartState());
            RegisterState(new AngryState());
            RegisterState(new TeleportState());
            RegisterState(new BlowfishState());
            RegisterState(new FlyState());
            RegisterState(new FirebirdState());
            RegisterState(new ThornState());
            RegisterState(new SwoopState());
            RegisterState(new ShoryukenState());
        }

        public override bool CheckDead()
        {
            return false;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedGrimm, -1);
            base.OnKill();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            // 使用BOSS免疫冷却时间计数器，防止通过从其他来源受到伤害来忽略BOSS攻击
            cooldownSlot = ImmunityCooldownID.Bosses;  
            return true;
        }

        public override void AIBefore()
        {
            // 索敌
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            Player player = Main.player[NPC.target];
            if (player.dead)
            {
                // 如果玩家寄了就让Boss消失
                NPC.EncourageDespawn(10);
                return;
            }
        }

        public class StartState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.defense = NormalDefence;
                n.Stage = 0;

                // 受到攻击时变为Angry状态
                if (npc.life != npc.lifeMax)
                {                    
                    n.SetState<AngryState>();
                    n.Timer = 0;
                    npc.netUpdate = true;
                }
                // 正常的状态
                else
                {
                    // 根据计时器切换帧图
                    switch (n.Timer)
                    {
                        case 1:
                        case 60:
                            // frame
                            break;
                        case 30:
                            // frame
                            break;
                        case 90:
                            // 切换至Teleport状态
                            n.SetState<TeleportState>();
                            n.Timer = 0;
                            npc.netUpdate = true;
                            break;
                    }
                }

                n.Timer++;
            }
        }

        public class AngryState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.defense = 9999;

                switch (n.Timer) 
                {
                    case 1:
                        // angry frame
                        break;
                    case 60:
                        // teleport frame
                        break;
                    case 120:
                        // 切换至Blowfish状态
                        n.SetState<BlowfishState>();
                        n.Stage++;
                        n.Timer = 0;
                        npc.netUpdate = true;
                        break;
                }

                n.Timer++;
            }
        }

        public class TeleportState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;                
                npc.defense = 9999;

                // frame

                if (n.Timer == 60)
                {
                    // 在血量下降到一定程度时切换至Blowfish状态
                    if (npc.life < npc.lifeMax * 0.8 && n.Stage % 10 == 0 ||
                        npc.life < npc.lifeMax * 0.5 && n.Stage % 10 == 1 ||
                        npc.life < npc.lifeMax * 0.2 && n.Stage % 10 == 2)
                    {
                        n.SetState<BlowfishState>();
                        n.Stage++;
                        n.Timer = 0;
                        npc.netUpdate = true;
                    }
                    // 一般情况下在4个attack状态中随机选一个
                    else
                    {
                        SwitchStateToAttack(n);
                    }
                    n.Timer = 0;
                    npc.netUpdate = true;
                }

                n.Timer++;
            }
        }

        public class BlowfishState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.defense = 9999;

                Player player = Main.player[npc.target];

                if (n.Timer == 1)
                {
                    // 传送至目标玩家上方
                    npc.position = new Vector2(player.position.X, player.position.Y - AttackDistance);
                }

                // frame

                // 弹幕
                //

                if (n.Timer == 600)
                {
                    // 切换至Teleport状态
                    n.SetState<TeleportState>();
                    n.Timer = 0;
                    npc.netUpdate = true;
                }

                n.Timer++;
            }
        }

        public class FlyState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.defense = 0;

                // frame

                // 乱飞
                // 将当前速度方向转换为角度
                float dir = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);
                // 随机转向
                int turn = new Random().Next(-60, 60);
                // 新速度方向的弧度
                float dir_new = (float)((dir + turn) * Math.PI / 180);
                npc.velocity = 60 * dir_new.ToRotationVector2();
                                
                if (n.Timer == 300)
                {
                    // 切换至Teleport状态
                    n.SetState<TeleportState>();
                    n.Timer = 0;
                    npc.netUpdate = true;
                }

                npc.spriteDirection = npc.direction;
                n.Timer++;
            }
        }

        public class FirebirdState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.defense = NormalDefence;
                Player player = Main.player[npc.target];

                // 根据计时器切换子状态
                switch (n.Timer)
                {
                    case 1:
                        // 传送至目标玩家面朝的方向
                        Vector2 place = player.direction > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                        npc.position = player.position + place * AttackDistance;
                        break;
                    case 20:
                    case 50:
                    case 80:
                        // frame
                        // 弹幕                        
                        Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                npc.position + new Vector2(-npc.spriteDirection * 60, 0),
                                                new Vector2(-npc.spriteDirection * 20, 0),
                                                ModContent.ProjectileType<GrimmFirebird>(),
                                                npc.damage,
                                                0.2f,
                                                npc.whoAmI);
                        Main.NewText($"弹幕呢弹幕?");
                        break;
                    case 110:
                        n.SetState<TeleportState>();
                        n.Timer = 0;
                        npc.netUpdate = true;
                        break;
                }

                // 让npc面朝玩家
                // spriteDirection = 1意为翻转
                npc.spriteDirection = player.position.X - npc.position.X > 0 ? -1 : 1;
                SwitchStateToFly(n);
                n.Timer++;
            }
        }

        public class ThornState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.defense = NormalDefence;
                Player player = Main.player[npc.target];

                // 根据计时器切换子状态
                switch (n.Timer)
                {
                    case 1:
                        // 传送至目标玩家面朝的方向
                        Vector2 place = player.direction > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                        npc.position = player.position + place * AttackDistance;                        
                        break;
                    case 10:
                        // frame
                        // 地刺预备
                        break;
                    case 50:
                        // frame
                        // 地刺
                        break;
                    case 120:
                        // 切换至Teleport状态
                        n.SetState<TeleportState>();
                        n.Timer = 0;
                        npc.netUpdate = true;
                        break;
                }

                // 让npc面朝玩家
                npc.spriteDirection = player.position.X - npc.position.X > 0 ? -1 : 1;
                SwitchStateToFly(n);
                n.Timer++;
            }
        }

        public class SwoopState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.defense = NormalDefence;
                Player player = Main.player[npc.target];

                // 根据计时器切换子状态
                switch (n.Timer) 
                {
                    case 1:
                        // 传送至目标玩家面前斜上方一段距离
                        Vector2 place = player.direction > 0 ? new Vector2(1, -1) : new Vector2(-1, -1);
                        npc.position = player.position + place * AttackDistance;
                        npc.velocity = -place;
                        break;
                    case 10:                    
                        // frame
                        // 俯冲
                        npc.velocity *= SprintSpeed;
                        break;                    
                    case 60:                    
                        // frame
                        // 停顿
                        Vector2 tar = player.position - npc.position;
                        npc.velocity = tar.X > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                        break;
                    case 70:
                        // frame
                        // 横冲
                        npc.velocity *= SprintSpeed;
                        break;
                    case 120:
                        // frame
                        // 停顿
                        npc.velocity *= 1 / SprintSpeed;
                        break;
                    case 130:
                        // 切换至Teleport状态
                        n.SetState<TeleportState>();
                        n.Timer = 0;
                        npc.netUpdate = true;
                        break;
                }

                // npc.direction貌似有点问题，先这么写着
                npc.spriteDirection = npc.velocity.X > 0 ? -1 : 1;
                SwitchStateToFly(n);
                n.Timer++;
            }
        }

        public class ShoryukenState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.defense = NormalDefence;
                Player player = Main.player[npc.target];

                // 根据计时器切换子状态
                switch (n.Timer)
                {
                    case 1:
                        // 传送至目标玩家面前一段距离
                        Vector2 place = player.direction > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                        npc.position = player.position + place * AttackDistance;
                        npc.velocity = -place;
                        break;
                    case 10:
                        // frame
                        // 横冲
                        npc.velocity *= SprintSpeed;
                        break;
                    case 60:
                        // frame
                        // 停顿
                        npc.velocity = npc.velocity.X > 0 ? new Vector2(1, -1) : new Vector2(-1, -1);
                        break;
                    case 70:
                        // frame
                        // 升龙拳
                        npc.velocity *= SprintSpeed;
                        break;
                    case 120:
                        // frame
                        // 停顿
                        npc.velocity *= 1 / SprintSpeed;
                        // 弹幕
                        break;
                    case 130:
                        // 切换至Teleport状态
                        n.SetState<TeleportState>();
                        n.Timer = 0;
                        npc.netUpdate = true;
                        break;
                }

                // npc.direction貌似有点问题，先这么写着
                npc.spriteDirection = npc.velocity.X > 0 ? -1 : 1;
                SwitchStateToFly(n);
                n.Timer++;
            }
        }
    }

    internal static class Utils 
    {
        public static void SwitchStateToFly(SMNPC n)
        {
            NPC npc = n.NPC;
            int stage = n.Stage / 10;
            if (npc.life < npc.lifeMax * 0.65 && stage == 0 ||
                npc.life < npc.lifeMax * 0.35 && stage == 1)
            {
                n.SetState<FlyState>();
                n.Stage += 10;
                n.Timer = 0;
                npc.velocity = new Vector2(npc.spriteDirection * 60, 0);
                npc.netUpdate = true;
            }
        }

        public static void SwitchStateToAttack(SMNPC n)
        {
            int State = new Random().Next(1, 5);
            // 避免连续切换至同一状态
            while(State == n.Any)
            {
                State = new Random().Next(1, 5);
            }

            switch (State)
            {
                case 1:
                    n.SetState<FirebirdState>();
                    break;
                case 2:
                    n.SetState<ThornState>();
                    break;
                case 3:
                    n.SetState<SwoopState>();
                    break;
                case 4:
                    n.SetState<ShoryukenState>();
                    break;
            }
            n.Any = State;
        }
    }
}
