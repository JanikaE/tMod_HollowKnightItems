using HollowKnightItems.Common.Systems;
using HollowKnightItems.Common.Systems.DrawSystem;
using HollowKnightItems.Content.Buffs;
using HollowKnightItems.Content.NPCs.StateMachine;
using HollowKnightItems.Content.Projectiles.Grimm;
using static HollowKnightItems.Content.NPCs.GrimmBoss;
using static HollowKnightItems.Content.NPCs.Utils;

namespace HollowKnightItems.Content.NPCs
{
    [AutoloadBossHead]
    internal class GrimmBoss : SMNPC
    {
        /// <summary>
        /// GrimmBoss AI中的一些距离常量
        /// </summary>
        internal static class Distance
        {
            public const int Blowfish = 300;
            public const int Firebird = 400;
            public const int Thorn = 300;
            public const int Sho = 200;
            public const int Swoop = 300;
            public const int Fly = 200;
        }

        /// <summary>
        /// GrimmBoss AI中的一些速率常量
        /// </summary>
        internal static class Speed
        {
            public const int Swoop = 30;
            public const int Scratch = 15;
            public const int Sho = 25;
        }

        public const int Damage = 50;
        public const int Defence = 10;

        /// <summary>
        /// 每张帧图的命名
        /// </summary>
        public enum Frame
        {
            Start,
            Bow,
            Teleport,
            Angry,
            Blowfish,
            Fly,
            Flybird,
            Thorn,
            Swoop1,
            Swoop2,
            Swoop3,
            Sho1,
            Sho2,
            Sho3,
            Die,
            None
        }

        public override void SetStaticDefaults()
        {
            // 这里以后写到Localization里去
            DisplayName.SetDefault("Troupe Master Grimm");
            DisplayName.AddTranslation(7, "剧团团长 格林");
            Main.npcFrameCount[NPC.type] = 16;
            NPCID.Sets.MPAllowedEnemies[Type] = true;  // 有对应召唤物的boss
        }

        public override void SetDefaults()
        {
            NPC.width = 240;
            NPC.height = 240;
            NPC.aiStyle = -1;
            NPC.lifeMax = 3000;            
            NPC.scale = 1f;  // npc的贴图和碰撞箱的放缩倍率
            NPC.knockBackResist = 0f;

            NPC.damage = 0;
            NPC.defense = 0;  // 在每个状态里再分别写攻击和防御

            //NPC.HitSound = SoundID.NPCHit5;
            //NPC.DeathSound = SoundID.NPCDeath7;

            NPC.value = Item.buyPrice(0, 4, 50, 0);  // NPC的爆出来的MONEY的数量，四个空从左到右是铂金，金，银，铜
            NPC.lavaImmune = true;  // 对岩浆免疫
            NPC.noGravity = true;  // 不受重力影响
            NPC.noTileCollide = true;  // 可穿墙
            NPC.npcSlots = 20;  // NPC所占用的NPC数量
            NPC.boss = true;  // 将npc设为boss。会掉弱治药水和心，会显示xxx已被击败，会有血条
            NPC.dontTakeDamage = false;  // 为true则为无敌。弹幕不会打到npc，并且不显示npc的血条
            NPC.friendly = false;
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
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedGrimm, -1);
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                Texture2D texture = GetTexture("GrimmDie").Value;
                Graph.NewGraph(texture, NPC.position, 180);
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            // 不显示小血条
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            // 使用Boss免疫冷却时间计数器，防止通过从其他来源受到伤害来忽略Boss攻击
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

            // 标记碰撞箱的四个角
            RoundHitboxDust(NPC, new Color(255, 150, 150));
        }

        public class StartState : NPCState
        {
            public override void AI(SMNPC n)
            {
                NPC npc = n.NPC;
                npc.damage = 0;
                npc.defense = Defence;
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
                npc.damage = Damage;
                npc.defense = 9999;
                Player player = Main.player[npc.target];

                // 开局吼的声浪
                // 想来想去还是用Dust简单（5毛特效）
                if (n.Timer < 90 && n.Timer % 10 == 0)
                {
                    RoarDust(npc.Center);
                }

                switch (n.Timer) 
                {
                    case 1:
                        n.GetFrame((int)Frame.Angry);
                        // 给所有玩家上debuff
                        for (int i = 0; i < Main.player.Length; i++)
                        {
                            if (Main.player[i].active)
                            {
                                Main.player[i].AddBuff(ModContent.BuffType<RoarDebuff>(), 90);
                            }                            
                        }
                        break;
                    case 90:
                        n.GetFrame((int)Frame.Teleport);
                        break;
                    case 110:
                        n.GetFrame((int)Frame.None);
                        TeleportDust(npc);
                        break;
                    case 140:
                        n.GetFrame((int)Frame.Teleport);
                        // 传送至目标玩家上方                        
                        npc.position = player.Center + new Vector2(-npc.width / 2, -Distance.Blowfish);
                        TeleportDust(npc);
                        break;
                    case 160:                        
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
                npc.damage = 0;
                npc.defense = 9999;
                npc.velocity = Vector2.Zero;
                Player player = Main.player[npc.target];

                switch (n.Timer) 
                {
                    case 1:
                        n.GetFrame((int)Frame.Teleport);
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
                            int State = random.Next(1, 5);
                            // 避免连续切换至同一状态
                            while (State == n.Any)
                            {
                                State = random.Next(1, 5);
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
                                TeleportToFront(npc, player, Distance.Firebird);
                                break;
                            // Thorn
                            case 2:
                                TeleportToFront(npc, player, Distance.Thorn);
                                break;
                            // Shoryuken
                            case 4:
                                TeleportToFront(npc, player, Distance.Sho);
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
                npc.damage = Damage;
                npc.defense = 9999;
                npc.velocity = Vector2.Zero;
                n.GetFrame((int)Frame.Blowfish);
                
                // 从第50帧到第490帧，每隔40帧发射一轮弹幕，共计12轮
                if ((n.Timer - 50) % 40 == 0 && n.Timer < 500)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -3; i < 8; i++)
                        {
                            if (random.Next(2) > 0)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                        npc.Center,
                                                        new Vector2(1, i),
                                                        ModContent.ProjectileType<GrimmShoot_Sides>(),
                                                        30,
                                                        0.2f,
                                                        Main.myPlayer);
                            }
                            if (random.Next(2) > 0)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                        npc.Center,
                                                        new Vector2(-1, i),
                                                        ModContent.ProjectileType<GrimmShoot_Sides>(),
                                                        30,
                                                        0.2f,
                                                        Main.myPlayer);
                            }                                
                        }
                        for (int i = -1; i < 2; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                        npc.Center,
                                                        new Vector2(i, 6),
                                                        ModContent.ProjectileType<GrimmShoot_Below>(),
                                                        30,
                                                        0.2f,
                                                        Main.myPlayer);
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
                npc.damage = 0;
                npc.defense = 0;
                Player player = Main.player[npc.target];
                n.GetFrame((int)Frame.Fly);
                                
                switch (n.Timer) 
                {
                    case 1:
                        // 召唤分身
                        for (int i = 0; i < 8; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                    npc.position,
                                                    new Vector2(i, 0),  // 用来区分不同的分身，实际的速度在Projectile的AI里设定
                                                    ModContent.ProjectileType<GrimmFly>(),
                                                    0,
                                                    0,
                                                    Main.myPlayer);
                        }
                        // 设定初速
                        npc.velocity = new Vector2(8, 4);
                        break;                       
                    case 300:
                        // 切换至Teleport状态
                        n.SetState<TeleportState>();
                        n.Timer = 0;
                        npc.netUpdate = true;
                        break;
                }

                // 乱飞
                float tarX = player.Center.X;
                float tarY = player.Center.Y - Distance.Fly;
                MoveBetween(npc, new Vector2(tarX, tarY), Distance.Fly);

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
                npc.damage = Damage;
                npc.defense = Defence;
                Player player = Main.player[npc.target];

                // 根据计时器切换子状态
                switch (n.Timer)
                {
                    case 1:
                        n.GetFrame((int)Frame.Flybird);
                        break;
                    case 32:
                    case 68:
                        // 弹幕
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                npc.Center,
                                                new Vector2(-npc.spriteDirection * 20, 0),
                                                ModContent.ProjectileType<GrimmFirebird>(),
                                                30,
                                                0.2f,
                                                Main.myPlayer);
                        }
                        break;
                    case 50:                    
                    case 86:
                        // 弹幕
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = -1; i < 2; i += 2)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                    npc.Center,
                                                    new Vector2(-npc.spriteDirection * 20, i * 10),
                                                    ModContent.ProjectileType<GrimmFirebird>(),
                                                    30,
                                                    0.2f,
                                                    Main.myPlayer);
                            }                            
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
                npc.damage = Damage;
                npc.defense = Defence;
                Player player = Main.player[npc.target];

                // 根据计时器切换子状态
                switch (n.Timer)
                {
                    case 1:
                        n.GetFrame((int)Frame.Thorn);
                        break;
                    case 20:
                        // 预警
                        for (int i = -12; i < 13; i++)
                        {
                            Rect.NewRect(new Vector2(npc.Center.X + i * 120, npc.Bottom.Y), 10, 10, new Color(255, 153, 153));
                        }                        
                        break;
                    case 60:
                        // 地刺
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = -12; i < 13; i++)
                            {
                                // 向上
                                Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                        npc.Center + new Vector2(i * 120 - 12, 120),
                                                        new Vector2(0, -20),
                                                        ModContent.ProjectileType<GrimmThorn>(),
                                                        30,
                                                        0.2f,
                                                        Main.myPlayer);
                                // 向下
                                Projectile.NewProjectile(npc.GetSource_FromAI(),
                                                        npc.Center + new Vector2(i * 120 - 12, 0),
                                                        new Vector2(0, 20),
                                                        ModContent.ProjectileType<GrimmThorn>(),
                                                        30,
                                                        0.2f,
                                                        Main.myPlayer);
                            }
                        }

                        // 清除预警
                        Rect.ClearRect();
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
                npc.damage = Damage;
                npc.defense = Defence;
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
                        // Dust to do
                        break;                    
                    case 25:                        
                        // 停顿
                        npc.velocity = player.Center.X - npc.Center.X > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                        n.GetFrame((int)Frame.Swoop2);
                        // Dust to do
                        break;
                    case 55:
                        n.GetFrame((int)Frame.Swoop3);
                        // 横冲
                        npc.velocity *= Speed.Swoop;
                        // Dust to do
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
                npc.damage = Damage;
                npc.defense = Defence;
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
                                                        new Vector2(i * 7, 0),
                                                        ModContent.ProjectileType<GrimmFireball>(),
                                                        30,
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

        public static void TeleportToFront(NPC npc, Player player, int distance) 
        {
            float x, y;
            // 传送至目标玩家面朝的方向
            x = player.direction > 0 ? player.Center.X + distance : player.Center.X - distance - npc.width;
            // 让玩家与NPC底部对齐
            y = player.Bottom.Y - npc.height;
            npc.position = new Vector2(x, y);
        }

        /// <summary>
        /// 用于瞬移前后产生Dust
        /// </summary>
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
