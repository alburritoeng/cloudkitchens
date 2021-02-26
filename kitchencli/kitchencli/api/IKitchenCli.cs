namespace kitchencli.api
{
    public enum DispatchCourierMatchEnum
    {
        Unknown =0,
        M = 1,
        F = 2
    }

    /// <summary>
    /// interface implemented by the bootstrapper to control life-cycle of the application
    /// </summary>
    interface IKitchenCli
    {
        /// <summary>
        /// starts all objects this bootstrapper is responsible for
        /// </summary>
        void Start();

        /// <summary>
        /// Initialize all objects and parse input from CLI
        /// </summary>
        /// <param name="jsonFile"></param>
        /// <param name="dipatcherType"></param>
        void Initialize(string jsonFile, DispatchCourierMatchEnum dipatcherType);

        /// <summary>
        /// Stop all objects this bootstrapper is responsible for
        /// </summary>
        void Stop();
    }
}
