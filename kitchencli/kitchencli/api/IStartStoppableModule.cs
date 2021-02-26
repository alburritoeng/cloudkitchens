namespace kitchencli.api
{
    /// <summary>
    /// interface for adding life-cycle control over modules
    /// This is useful for starting/stopping any internal producer/consumers
    /// and for disposing objects when stopping
    /// </summary>
    public interface IStartStoppableModule
    {
        /// <summary>
        /// Stop a module
        /// </summary>
        void Start();

        /// <summary>
        /// Start a module
        /// </summary>
        void Stop();
    }
}