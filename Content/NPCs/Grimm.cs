using HollowKnightItems.Common.Systems;
using HollowKnightItems.Content.NPCs.StateMachine;
using HollowKnightItems.Content.Projectiles.Grimm;
using Microsoft.Xna.Framework;
using System;
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

        public const int AttackDistance = 400;
        public const int DashSpeed = 10;

        public override void SetStaticDefaults()
        {
            // 这里以后写到Localization里去
            DisplayName.SetDefault("Grimm");
            DisplayName.AddTranslation(7, "格林团长");
            Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;  // 这两个代表这个NPC的碰撞箱宽高，以及tr会从你的贴图里扣多大的图
            //DrawOffsetY = 240;

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

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            // 不显示小血条
            return false;
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
                npc.velocity = Vector2.Zero;

                // 受到攻击时切换至Angry状态
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
                            GetFrame(n, 0);
                            break;
                        case 30:
                            GetFrame(n, 0);
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
                        GetFrame(n, 0);
                        break;
                    case 60:
                        GetFrame(n, 1);
                        break;
                    case 70:
                        GetFrame(n, 2);
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
                npc.velocity = Vector2.Zero;
                Player player = Main.player[npc.target];

                switch (n.Timer) 
                {
                    case 1:
                        GetFrame(n, 1);
                        // Dust
                        
                        // 在血量下降到一定程度时，切换至Blowfish状态
                        // 用n.Stage的个位标记Blowfish次数
                        if (npc.life < npc.lifeMax * 0.8 && n.Stage % 10 == 0 ||
                            npc.life < npc.lifeMax * 0.5 && n.Stage % 10 == 1 ||
                            npc.life < npc.lifeMax * 0.2 && n.Stage % 10 == 2)
                        {
                            n.Any = 0;
                        }
                        // 反之则在4个attack状态中随机
                        else
                        {
                            int State = new Random().Next(1, 5);
                            // 避免连续切换至同一状态
                            while (State == n.Any)
                            {
                                State = new Random().Next(1, 5);
                            }                            
                            n.Any = State;
                        }                        
                        break;
                    case 10:
                        GetFrame(n, 2);
                        break;
                    case 60:
                        GetFrame(n, 1);
                        // Dust

                        // 瞬移
                        float x, y;
                        switch (n.Any)
                        {
                            case 0:
                                // 传送至目标玩家上方
                                npc.position = player.Center + new Vector2(-npc.width / 2, (float)(-AttackDistance * 1.5));
                                break;
                            case 1:
                            case 2:
                            case 4:
                                // 传送至目标玩家面朝的方向
                                x = player.direction > 0 ? player.Center.X + AttackDistance : player.Center.X - AttackDistance - npc.width;
                                // 让玩家与NPC底部对齐
                                y = player.Bottom.Y - npc.height;
                                npc.position = new Vector2(x, y);
                                break;
                            case 3:
                                // 传送至目标玩家面朝的方向的斜上方
                                x = player.direction > 0 ? player.Center.X + AttackDistance : player.Center.X - AttackDistance - npc.width;
                                // 让(NPC在俯冲之后)玩家与NPC底部对齐
                                y = player.Bottom.Y - npc.height - AttackDistance;
                                npc.position = new Vector2(x, y);
                                break;
                        }
                        break;
                    case 70:
                        // 切换状态
                        switch (n.Any) 
                        {
                            case 0:
                                n.SetState<BlowfishState>();
                                n.Stage++;
                                break;
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
                        n.Timer = 0;
                        npc.netUpdate = true;
                        break;
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
                npc.velocity = Vector2.Zero;
                GetFrame(n, 0);
                
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
                Player player = Main.player[npc.target];
                GetFrame(n, 0);

                // 乱飞
                // 把移动目标定在玩家头顶
                Vector2 tar = player.Center + new Vector2(-npc.width / 2, -AttackDistance / 2);
                float Vx = npc.position.X - tar.X > 0 ? -20 : 20;
                float Vy = npc.position.Y - tar.Y > 0 ? -10 : 10;
                npc.velocity += new Vector2(Vx, Vy);

                switch (n.Timer) 
                {
                    case 1:
                        // 召唤分身
                        for (int i = 0; i < 10; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                    npc.position + new Vector2(i % 5 * 30 - 60, i / 5 * 30),
                                                    new Vector2(0, 0),
                                                    ModContent.ProjectileType<GrimmFly>(),
                                                    0,
                                                    0,
                                                    npc.whoAmI);
                        }
                        break;
                    case 300:
                        // 切换至Teleport状态
                        n.SetState<TeleportState>();
                        n.Timer = 0;
                        npc.netUpdate = true;
                        break;
                }

                // npc.direction貌似有点问题，先这么写着
                npc.spriteDirection = npc.velocity.X > 0 ? -1 : 1;
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
                        GetFrame(n, 0);
                        break;
                    case 20:
                    case 50:
                    case 80:
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
                        GetFrame(n, 0);
                        break;
                    case 10:
                        GetFrame(n, 0);
                        // 地刺预备
                        break;
                    case 50:
                        GetFrame(n, 0);
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
                        GetFrame(n, 0);
                        // 给一个小的初速度
                        npc.velocity = player.direction > 0 ? new Vector2(-1, 1) : new Vector2(1, 1);
                        break;
                    case 10:
                        // 俯冲
                        npc.velocity *= DashSpeed;
                        break;                    
                    case 50:                        
                        // 停顿
                        npc.velocity = player.position.X - npc.position.X > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                        break;
                    case 55:
                        GetFrame(n, 0);
                        break;
                    case 60:
                        // 横冲
                        npc.velocity *= DashSpeed;
                        break;
                    case 100:
                        GetFrame(n, 0);
                        // 停顿
                        npc.velocity *= 1 / DashSpeed;
                        break;
                    case 110:
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
                        GetFrame(n, 0);
                        // 给一个小的初速度
                        npc.velocity = player.direction > 0 ? new Vector2(-1, 0) : new Vector2(1, 0);
                        break;
                    case 10:
                        GetFrame(n, 0);
                        // 横冲
                        npc.velocity *= DashSpeed;
                        break;
                    case 50:
                        // 停顿
                        npc.velocity = npc.velocity.X > 0 ? new Vector2(1, -1) : new Vector2(-1, -1);
                        break;
                    case 55:
                        GetFrame(n, 0);
                        break;
                    case 60:                        
                        // 升龙拳
                        npc.velocity *= DashSpeed;
                        break;
                    case 100:
                        // 停顿
                        npc.velocity *= 1 / DashSpeed;
                        // 弹幕
                        break;
                    case 110:
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
            // 获取n.Stage的十位
            int stage = n.Stage / 10;
            // 在Attack状态中，受到攻击使血量下降到一定程度时，切换至Fly状态
            // 用n.Stage的十位标记Fly次数
            if (npc.life < npc.lifeMax * 0.65 && stage == 0 ||
                npc.life < npc.lifeMax * 0.35 && stage == 1)
            {
                n.SetState<FlyState>();
                n.Stage += 10;
                n.Timer = 0;
                npc.netUpdate = true;
            }
        }

        public static void GetFrame(SMNPC n, int y)
        {
            NPC npc = n.NPC;
            npc.frame = new Rectangle(0, npc.height * y, npc.width, npc.height);
        }
    }
}
