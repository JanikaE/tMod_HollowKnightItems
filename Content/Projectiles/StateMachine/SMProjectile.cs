namespace HollowKnightItems.Content.Projectiles.StateMachine
{
    /// <summary>
    /// 基于状态机的ModProjectile类，一定要先在Initialize里注册弹幕的状态才能使用
    /// </summary>
    public abstract class SMProjectile : ModProjectile
    {
        public ProjState CurrentState => ProjStates[State - 1];
        public List<ProjState> ProjStates = new();
        public Dictionary<string, int> StateDict = new();

        public Vector2 Target;

        public int State
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = (int)value; }
        }
        public int Timer
        {
            get { return (int)Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }

        /// <summary>
        /// 把当前状态变为指定的弹幕状态实例
        /// </summary>
        /// <typeparam name="T">注册过的<see cref="ProjState"/>类名</typeparam>
        public void SetState<T>() where T : ProjState
        {
            var name = typeof(T).FullName;
            if (!StateDict.ContainsKey(name)) throw new ArgumentException("这个状态并不存在");
            State = StateDict[name];            
        }

        /// <summary>
        /// 注册状态
        /// </summary>
        /// <typeparam name="T">需要注册的<see cref="ProjState"/>类</typeparam>
        /// <param name="state">需要注册的<see cref="ProjState"/>类的实例</param>
        protected void RegisterState<T>(T state) where T : ProjState
        {
            var name = typeof(T).FullName;
            if (StateDict.ContainsKey(name)) throw new ArgumentException("这个状态已经注册过了");
            ProjStates.Add(state);
            StateDict.Add(name, ProjStates.Count);
        }

        /// <summary>
        /// 初始化函数，用于注册弹幕状态
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
        public virtual void AIBefore() { }

        /// <summary>
        /// 在状态机执行之后要执行的代码，可以重写
        /// </summary>
        public virtual void AIAfter() { }
    }

    public abstract class ProjState
    {
        // AI函数接受一个SMProjectile类型的mod弹幕对象
        public abstract void AI(SMProjectile proj);
    }
}