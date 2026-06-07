namespace IGT.FloorNet.Tools.ServiceSimulator.Utils
{
    /// <summary>
    /// For any processes/tasks that require Jwt it's possible to assign
    /// an Observable class that will notify these with new tokens asynchronously.
    /// </summary>
    public interface IJwtObservable
    {
        /// <summary>
        /// Registers a said observer to receive new Jwt tokens. 
        /// </summary>
        /// <param name="observer"></param>
        void Register(IJwtObserver observer);
    }
}
