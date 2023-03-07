using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HollowKnightItems.Content.NPCs.StateMachine
{
    /// <summary>
    /// 基于状态机的ModNPC类，一定要先在Initialize里注册弹幕的状态才能使用
    /// </summary>
    public abstract class SMNPC : ModNPC
    {
        public NPCState CurrentState => NPCStates[State - 1];
        public List<NPCState> NPCStates = new();
        public Dictionary<string, int> StateDict = new();

        /// <summary>
        /// 状态
        /// </summary>
        public int State
        {
            get { return (int)NPC.ai[0]; }
            set { NPC.ai[0] = (int)value; }
        }
        /// <summary>
        /// 计时器
        /// </summary>
        public int Timer
        {
            get { return (int)NPC.ai[1]; }
            set { NPC.ai[1] = value; }
        }
        /// <summary>
        /// 阶段
        /// </summary>
        public int Stage
        {
            get { return (int)NPC.ai[2]; }
            set { NPC.ai[2] = value; }
        }
        /// <summary>
        /// 自定义
        /// </summary>
        public int Any
        {
            get { return (int)NPC.ai[3]; }
            set { NPC.ai[3] = value; }
        }

        /// <summary>
        /// 把当前状态变为指定的NPC状态实例
        /// </summary>
        /// <typeparam name="T">注册过的<see cref="NPCState"/>类名</typeparam>
        public void SetState<T>() where T : NPCState
        {
            var name = typeof(T).FullName;
            if (!StateDict.ContainsKey(name)) throw new ArgumentException("这个状态并不存在");
            State = StateDict[name];
            Main.NewText(name);
        }

        /// <summary>
        /// 注册状态
        /// </summary>
        /// <typeparam name="T">需要注册的<see cref="NPCState"/>类</typeparam>
        /// <param name="state">需要注册的<see cref="NPCState"/>类的实例</param>
        protected void RegisterState<T>(T state) where T : NPCState
        {
            var name = typeof(T).FullName;
            if (StateDict.ContainsKey(name)) throw new ArgumentException("这个状态已经注册过了");
            NPCStates.Add(state);
            StateDict.Add(name, NPCStates.Count);
        }

        /// <summary>
        /// 初始化函数，用于注册NPC状态
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// 在子类无法重写AI函数，只能用before和after函数
        /// </summary>
        public sealed override void AI()
        {
            if (State == 0)
            {
                Initialize();
                State = 1;
            }
            AIBefore();
            CurrentState.AI(this);
            AIAfter();
        }

        /// <summary>
        /// 在状态机执行之前要执行的代码，可以重写
        /// </summary>
        public virtual void AIAfter() { }

        /// <summary>
        /// 在状态机执行之后要执行的代码，可以重写
        /// </summary>
        public virtual void AIBefore() { }
    }

    public abstract class NPCState
    {
        // AI函数接受一个SMNPC类型的mod NPC对象
        public abstract void AI(SMNPC n);
    }
}
