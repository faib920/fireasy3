namespace Fireasy.Data.Provider
{
    /// <summary>
    /// 特有的提供者。
    /// </summary>
    public interface IFeaturedProvider
    {
        /// <summary>
        /// 获取特征码。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        string? GetFeature(ConnectionString connectionString);

        /// <summary>
        /// 克隆一个副本。
        /// </summary>
        /// <param name="feature">特征码。</param>
        /// <returns></returns>
        IProvider? Clone(string feature);
    }
}
