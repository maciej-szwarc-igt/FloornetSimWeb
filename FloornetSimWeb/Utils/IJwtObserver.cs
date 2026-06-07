namespace IGT.FloorNet.Tools.ServiceSimulator.Utils
{
    /// <summary>
    /// Observer implementation for Jwt
    /// </summary>
    public interface IJwtObserver
    {
        /// <summary>
        /// Receives new Jwt token
        /// </summary>
        /// <param name="NewJwt"></param>
        void UpdateJwt(string NewJwt);
    }
}
