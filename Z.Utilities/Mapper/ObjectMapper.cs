//-----------------------------------------------------------------------
// <copyright file="ObjectMapper.cs" company="HB">
//     Copyright HB All rights reserved.
// </copyright>
// <author>JiangWeiPeng</author>
// <date>2016-04-13</date>
//-----------------------------------------------------------------------

namespace Z.Utilities.Mapper
{
    public static class ObjectMapper
    {
        public static TDestination DynamicMap<TSource, TDestination>(TSource source)
        {
            return AutoMapper.Mapper.DynamicMap<TSource, TDestination>(source);
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            return AutoMapper.Mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// 用于非列表数据类型
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源数据</param>
        /// <param name="ignoreMembers">需要忽略的属性</param>
        /// <returns>映射后的值</returns>
        public static TDestination Map<TSource, TDestination>(TSource source, IgnoreMember<TDestination> ignoreMembers)
        {
            if (ignoreMembers != null)
            {
                return AutoMapper.Mapper.Map<TSource, TDestination>(source, ignoreMembers.IgnorePropertyNames);
            }
            else
            {
                return Map<TSource, TDestination>(source);
            }
        }

        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination, IgnoreMember<TDestination> ignoreMembers)
        {
            if (ignoreMembers != null)
            {
                return AutoMapper.Mapper.Map<TSource, TDestination>(source, destination, ignoreMembers.IgnorePropertyNames);
            }
            else
            {
                return AutoMapper.Mapper.Map(source, destination);
            }
        }

        /// <summary>
        /// 用于列表类型数据的映射
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <typeparam name="TWhenConvertTo">目标类型中内部属性的类型</typeparam>
        /// <param name="source">源数据</param>
        /// <param name="ignoreMembers">需要忽略的属性</param>
        /// <returns>映射后的对象</returns>
        public static TDestination Map<TSource, TDestination, TWhenConvertTo>(TSource source, IgnoreMember<TWhenConvertTo> ignoreMembers)
        {
            if (ignoreMembers != null && ignoreMembers.IgnorePropertyNames.Length > 0)
            {
                return AutoMapper.Mapper.Map<TSource, TDestination>(source, ignoreMembers.IgnorePropertyNames);
            }
            else
            {
                return AutoMapper.Mapper.Map<TSource, TDestination>(source);
            }
        }

        public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return AutoMapper.Mapper.Map(source, destination);
        }
    }
}
