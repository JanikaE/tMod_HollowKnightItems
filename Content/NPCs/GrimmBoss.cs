﻿using HollowKnightItems.Common.Systems;
using HollowKnightItems.Content.NPCs.StateMachine;
using HollowKnightItems.Content.Projectiles.Grimm;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static HollowKnightItems.Content.NPCs.GrimmBoss;
using static HollowKnightItems.Content.NPCs.Utils;

namespace HollowKnightItems.Content.NPCs
{
    [AutoloadBossHead]
    internal class GrimmBoss : SMNPC
    {        
        public const int NormalDefence = 20;

        public override void SetStaticDefaults()
        {
            // 这里以后写到Localization里去
            DisplayName.SetDefault("Grimm");
            DisplayName.AddTranslation(7, "格林团长");
            Main.npcFrameCount[NPC.type] = 15;
            NPCID.Sets.MPAllowedEnemies[Type] = true;  // 有对应召唤物的boss
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

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/TheGrimmTroupe");
            }
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
                if (npc.life != npc.lifeMax && n.Timer < 180)
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
                            TeleportDust(npc);
                            n.GetFrame((int)Frame.Start);
                            break;
                        case 120:
                            n.GetFrame((int)Frame.Bow);
                            break;
                        case 180:
                            n.GetFrame((int)Frame.Start);
                            break;                        
                        case 210:
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
                Player player = Main.player[npc.target];

                switch (n.Timer) 
                {
                    case 1:
                        n.GetFrame((int)Frame.Angry);
                        break;
                    case 90:
                        n.GetFrame((int)Frame.Teleport);
                        break;
                    case 110:
                        n.GetFrame((int)Frame.None);
                        break;
                    case 140:
                        n.GetFrame((int)Frame.Teleport);
                        break;
                    case 160:
                        // 传送至目标玩家上方                        
                        npc.position = player.Center + new Vector2(-npc.width / 2, -Distance.Blowfish);
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
                    case 20:
                        n.GetFrame((int)Frame.None);
                        TeleportDust(npc);
                        break;
                    case 70:
                        n.GetFrame((int)Frame.Teleport);                        

                        // 瞬移
                        float x, y;
                        switch (n.Any)
                        {                            
                            // Blowfish
                            case 0:
                                // 传送至目标玩家上方
                                npc.position = player.Center + new Vector2(-npc.width / 2, -Distance.Blowfish);
                                break;
                            // Firebird
                            case 1:
                                TeleportToFront(n, player, Distance.Firebird);
                                break;
                            // Thorn
                            case 2:
                                TeleportToFront(n, player, Distance.Thorn);
                                break;
                            // Shoryuken
                            case 4:
                                TeleportToFront(n, player, Distance.Sho);
                                break;
                            // Swoop
                            case 3:
                                // 传送至目标玩家面朝的方向的斜上方
                                x = player.direction > 0 ? player.Center.X + Distance.Swoop : player.Center.X - Distance.Swoop - npc.width;
                                // 让(NPC在俯冲之后)玩家与NPC底部对齐
                                y = player.Bottom.Y - npc.height - Distance.Swoop;
                                npc.position = new Vector2(x, y);
                                break;
                        }

                        TeleportDust(npc);
                        break;
                    case 90:
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
                n.GetFrame((int)Frame.Blowfish);
                
                // 从第50帧到第490帧，每隔40帧发射一轮弹幕，共计12轮
                if ((n.Timer - 50) % 40 == 0 && n.Timer < 500)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -4; i < 6; i++)
                        {
                            if (new Random().Next(2) > 0)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                        npc.Center,
                                                        new Vector2(1, i),
                                                        ModContent.ProjectileType<GrimmShoot_Sides>(),
                                                        npc.damage,
                                                        0.2f,
                                                        Main.myPlayer);
                            }
                            if (new Random().Next(2) > 0)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                        npc.Center,
                                                        new Vector2(-1, i),
                                                        ModContent.ProjectileType<GrimmShoot_Sides>(),
                                                        npc.damage,
                                                        0.2f,
                                                        Main.myPlayer);
                            }                                
                        }
                    }
                }

                if (n.Timer == 550)
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
                n.GetFrame((int)Frame.Fly);

                // 乱飞，x与y用不同的逻辑
                // x轴
                float tarX = player.Center.X;
                int Vx = new Random().Next(5) > 0 ? 1 : -1;
                npc.velocity.X = npc.velocity.X > 0 ? npc.velocity.X + Vx : npc.velocity.X - Vx;
                if (Math.Abs(npc.Center.X - tarX) > Distance.Fly)
                {
                    npc.velocity.X *= -1;
                }
                // y轴
                float tarY = player.position.Y - Distance.Fly;
                float Vy = npc.position.Y - tarY > 0 ? -1 : 1;
                npc.velocity.Y += Vy;

                switch (n.Timer) 
                {
                    case 1:
                        // 召唤分身
                        for (int i = 0; i < 10; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                    npc.position,
                                                    new Vector2(0, 0),
                                                    ModContent.ProjectileType<GrimmFly>(),
                                                    0,
                                                    0,
                                                    Main.myPlayer);
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
                        n.GetFrame((int)Frame.Flybird); // ready
                        break;
                    case 12:
                        n.GetFrame((int)Frame.Flybird);
                        break;
                    case 32:
                    case 68:
                    case 86:
                        // 弹幕
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                    npc.Center,
                                                    new Vector2(-npc.spriteDirection * 20, 0),
                                                    ModContent.ProjectileType<GrimmFirebird>(),
                                                    npc.damage,
                                                    0.2f,
                                                    Main.myPlayer);
                        }                        
                        break;
                    case 126:
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
                        n.GetFrame((int)Frame.Thorn1);
                        // 地刺预备
                        break;
                    case 20:
                        n.GetFrame((int)Frame.Thorn2);
                        // 地刺动作

                        // 预警线
                        for (int i = -12; i < 13; i++)
                        {
                            Line.NewLine(npc.Center.X + i * 90, new Color(255, 150, 150));
                        }                        
                        break;
                    case 60:
                        // 地刺

                        // 清除预警线
                        Line.ClearLine();
                        break;
                    case 90:
                        // 收刺，后摇
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
                        n.GetFrame((int)Frame.Swoop1);
                        // 给一个小的初速度
                        npc.velocity = player.Center.X - npc.Center.X > 0 ? new Vector2(1, 1) : new Vector2(-1, 1);
                        break;
                    case 15:
                        // 俯冲
                        npc.velocity *= Speed.Swoop;
                        break;                    
                    case 25:                        
                        // 停顿
                        npc.velocity = player.Center.X - npc.Center.X > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                        n.GetFrame((int)Frame.Swoop2);  // ready
                        break;
                    case 55:
                        n.GetFrame((int)Frame.Swoop2);
                        // 横冲
                        npc.velocity *= Speed.Swoop;
                        break;
                    case 65:
                        // 停顿
                        npc.velocity = npc.velocity.X > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                        break;
                    case 85:
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
                        n.GetFrame((int)Frame.Sho1);
                        // 给一个小的初速度
                        npc.velocity = player.Center.X - npc.Center.X > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                        break;
                    case 24:
                        n.GetFrame((int)Frame.Sho2);
                        // 横冲
                        npc.velocity *= Speed.Scratch;
                        break;
                    case 36:
                        // 停顿
                        npc.velocity = npc.velocity.X > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                        break;
                    case 56:
                        n.GetFrame((int)Frame.Sho3);                   
                        // 升龙拳
                        npc.velocity = new Vector2(Speed.Sho / 2, Speed.Sho) * -1;
                        break;
                    case 68:
                        n.GetFrame((int)Frame.Start);
                        npc.velocity = Vector2.Zero;
                        // 弹幕
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = -2; i < 3; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                        npc.Center,
                                                        new Vector2(i * 8, 0),
                                                        ModContent.ProjectileType<GrimmFireball>(),
                                                        npc.damage,
                                                        0.2f,
                                                        Main.myPlayer);
                            }                            
                        }
                        break;
                    case 120:
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
        internal static class Distance
        {
            public const int Blowfish = 300;
            public const int Firebird = 400;
            public const int Thorn = 300;
            public const int Sho = 200;
            public const int Swoop = 300;
            public const int Fly = 200;
        }

        internal static class Speed
        {
            public const int Swoop = 30;
            public const int Scratch = 15;
            public const int Sho = 25;
        }


        // 用枚举给每张帧图命名
        public enum Frame {
            Start,
            Bow,
            Teleport,
            None,
            Angry,
            Blowfish,
            Fly,
            Flybird,
            Thorn1,
            Thorn2,
            Swoop1,
            Swoop2,
            Sho1,
            Sho2,
            Sho3
        }

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

        public static void TeleportToFront(SMNPC n, Player player, int distance) 
        {
            float x, y;
            NPC npc = n.NPC;
            // 传送至目标玩家面朝的方向
            x = player.direction > 0 ? player.Center.X + distance : player.Center.X - distance - npc.width;
            // 让玩家与NPC底部对齐
            y = player.Bottom.Y - npc.height;
            npc.position = new Vector2(x, y);
        }

        public static void TeleportDust(NPC npc)
        {
            Vector2 pos = npc.position;
            for (int i = 0; i < 300; i++)
            {
                Dust.NewDust(pos, npc.width, npc.height, DustID.TintableDustLighted, SpeedY: -4, newColor: new Color(255, 0, 0));
            }
        }
    }    
}