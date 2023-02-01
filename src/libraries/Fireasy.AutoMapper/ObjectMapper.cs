// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.ObjectMapping;
using AM = AutoMapper;

namespace Fireasy.AutoMapper
{
    /// <summary>
    /// 
    /// </summary>
    public class ObjectMapper : IObjectMapper
    {
        private readonly AM.IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        public ObjectMapper(AM.IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// 将源对象转换为目标类型。
        /// </summary>
        /// <typeparam name="TSource">源对象类型。</typeparam>
        /// <typeparam name="TDestination">目标对象类型。</typeparam>
        /// <param name="source">源对象。</param>
        /// <returns></returns>
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// 将源对象转换为目标类型。
        /// </summary>
        /// <typeparam name="TSource">源对象类型。</typeparam>
        /// <typeparam name="TDestination">目标对象类型。</typeparam>
        /// <param name="source">源对象。</param>
        /// <param name="destination">目标对象。</param>
        /// <returns></returns>
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _mapper.Map(source, destination);
        }
    }
}
