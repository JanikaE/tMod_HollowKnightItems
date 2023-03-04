using HollowKnightItems.Content.NPCs.StateMachine;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static HollowKnightItems.Content.NPCs.Grimm;
using static HollowKnightItems.Content.NPCs.Utils;

namespace HollowKnightItems.Content.NPCs
{
    //[AutoloadBossHead]
    [Autoload(false)]
    internal class Grimm : SMNPC
    {
        // 各状态持续的时间
        public const int StartTime = 120;
        public const int AngryTime = 120;
        public const int TeleportTime = 60;
        public const int BlowfishTime = 600;
        public const int FlyTime = 300;
        public const int FirebirdTime = 180;
        public const int ThornTime = 180;
        public const int SwoopTime = 180;
        public const int ShoryukenTime = 180;

        public const int NormalDefence = 20;

        public const int AttackDistance = 600;

        public override void SetStaticDefaults()
        {
            // 这里以后写到Localization里去
            DisplayName.SetDefault("Grimm");
            DisplayName.AddTranslation(7, "格林团长");
            //Main.npcFrameCount[NPC.type] = 3;

            // 免疫的buff
            //NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            //{
            //    SpecificallyImmuneTo = new int[] {
            //        BuffID.Poisoned,
            //    }
            //};
        }

        public override void SetDefaults()
        {
            NPC.width = 0;
            NPC.height = 0;  // 这两个代表这个NPC的碰撞箱宽高，以及tr会从你的贴图里扣多大的图
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
                    if (n.Timer < StartTime * 0.25)
                    {
                        // frame
                    }
                    else if (n.Timer < StartTime * 0.75)
                    {
                        // frame
                    }
                    else if (n.Timer < StartTime)
                    {
                        // frame
                    }
                    else if (n.Timer == StartTime)
                    {
                        n.SetState<TeleportState>();                        
                        n.Timer = 0;
                        npc.netUpdate = true;
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

                if (n.Timer < AngryTime)
                {
                    // frame
                }
                // 把传送的动画也一并放这里了
                else if (n.Timer < AngryTime + TeleportTime)
                {
                    // frame
                }
                else if (n.Timer == AngryTime + TeleportTime)
                {
                    n.SetState<BlowfishState>();
                    n.Stage++;
                    n.Timer = 0;
                    npc.netUpdate = true;
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

                if (n.Timer == TeleportTime)
                {
                    // 在血量下降到一定程度时变为Blowfish状态
                    if (npc.life < npc.lifeMax * 0.8 && n.Stage == 0 ||
                        npc.life < npc.lifeMax * 0.5 && n.Stage == 1 ||
                        npc.life < npc.lifeMax * 0.2 && n.Stage == 2)
                    {
                        n.SetState<BlowfishState>();
                        n.Stage++;
                        n.Timer = 0;
                        npc.netUpdate = true;
                    }
                    // 一般情况下在4个attack状态中随机选一个
                    else
                    {
                        switch (new Random().Next(0, 4)) 
                        {
                            case 0:
                                n.SetState<FirebirdState>();
                                break;
                            case 1:
                                n.SetState<ThornState>();
                                break;
                            case 2:
                                n.SetState<SwoopState>();
                                break;
                            case 3:
                                n.SetState<ShoryukenState>();
                                break;
                        }                        
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

                Player player = Main.player[npc.FindClosestPlayer()];

                if (n.Timer == 1)
                {
                    // 传送至目标玩家上方
                    npc.Teleport(new Vector2(player.position.X, player.position.Y - AttackDistance));
                }

                // frame

                // 弹幕
                //

                if (n.Timer == BlowfishTime)
                {
                    // 切换状态
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

                // 行为
                // 乱飞

                if (n.Timer < FlyTime)
                {
                    // frame
                }
                if (n.Timer == FlyTime)
                {
                    // 切换状态
                    n.SetState<TeleportState>();
                    n.Timer = 0;
                    npc.netUpdate = true;
                }

                n.Timer++;
            }
        }

        public class FirebirdState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.defense = NormalDefence;

                Player player = Main.player[npc.FindClosestPlayer()];
                if (n.Timer == 1)
                {
                    // 传送至目标玩家面朝的方向
                    Vector2 place = player.direction > 0 ? new Vector2(AttackDistance, 0) : new Vector2(-AttackDistance, 0);                    
                    npc.Teleport(player.position + place);
                    // 让npc面朝玩家
                    npc.spriteDirection = player.direction * (-1);
                }

                // frame

                // 弹幕

                SwitchStateToFly(n);
                if (n.Timer == FirebirdTime)
                {
                    n.SetState<TeleportState>();
                    n.Timer = 0;
                    npc.netUpdate = true;
                }

                n.Timer++;
            }
        }

        public class ThornState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.defense = NormalDefence;

                Player player = Main.player[npc.FindClosestPlayer()];
                if (n.Timer == 1)
                {
                    // 传送至目标玩家面前一段距离
                    Vector2 place = player.direction > 0 ? new Vector2(AttackDistance, 0) : new Vector2(-AttackDistance, 0);
                    npc.Teleport(player.position + place);
                    // 让npc面朝玩家
                    npc.spriteDirection = player.direction * (-1);
                }

                // frame

                // 地刺

                SwitchStateToFly(n);
                if (n.Timer == ThornTime)
                {
                    n.SetState<TeleportState>();
                    n.Timer = 0;
                    npc.netUpdate = true;
                }

                n.Timer++;
            }
        }

        public class SwoopState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.defense = NormalDefence;

                Player player = Main.player[npc.FindClosestPlayer()];
                if (n.Timer == 1)
                {
                    // 传送至目标玩家面前斜上方一段距离
                    Vector2 place = player.direction > 0 ? new Vector2(AttackDistance, -AttackDistance) : new Vector2(-AttackDistance, -AttackDistance);
                    npc.Teleport(player.position + place);
                    // 让npc面朝玩家
                    npc.spriteDirection = player.direction * (-1);
                }

                if (n.Timer < SwoopTime * 0.5)
                {
                    // frame
                    // 俯冲
                }
                else
                {
                    // frame
                    // 横冲
                }

                SwitchStateToFly(n);
                if (n.Timer == SwoopTime)
                {
                    n.SetState<TeleportState>();
                    n.Timer = 0;
                    npc.netUpdate = true;
                }

                n.Timer++;
            }
        }

        public class ShoryukenState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.defense = NormalDefence;

                Player player = Main.player[npc.FindClosestPlayer()];
                if (n.Timer == 1)
                {
                    // 传送至目标玩家面前一段距离
                    Vector2 place = player.direction > 0 ? new Vector2(AttackDistance, 0) : new Vector2(-AttackDistance, 0);
                    npc.Teleport(player.position + place);
                    // 让npc面朝玩家
                    npc.spriteDirection = player.direction * (-1);
                }

                if (n.Timer < ShoryukenTime * 0.5)
                {
                    // frame
                    // 横冲
                }
                else
                {
                    // frame
                    // 升龙拳
                }

                SwitchStateToFly(n);
                if (n.Timer == ShoryukenTime)
                {
                    n.SetState<TeleportState>();
                    n.Timer = 0;
                    npc.netUpdate = true;
                }

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
                npc.netUpdate = true;
            }
        }
    }

}
