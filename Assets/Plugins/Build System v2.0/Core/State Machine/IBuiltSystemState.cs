namespace CustomBuildSystem
{
    public abstract class BuiltSystemState
    {
        protected BuildSystem buildSystem;

        internal void Init(BuildSystem buildSystem)
        {
            this.buildSystem = buildSystem;
        }

        public virtual void OnStart() {}
        public virtual void OnUpdate() { }
    }
}