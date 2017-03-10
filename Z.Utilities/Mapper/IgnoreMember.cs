//-----------------------------------------------------------------------
// <copyright file="IgnoreMember.cs" company="HB">
//     Copyright HB All rights reserved.
// </copyright>
// <author>JiangWeiPeng</author>
// <date>2016-04-13</date>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Z.Utilities.Mapper
{
    public class IgnoreMember<T>
    {
        private List<string> _ignoreNames = new List<string>();

        public string[] IgnorePropertyNames
        {
            get { return _ignoreNames.Distinct().ToArray(); }
        }

        public IgnoreMember<T> Ignore(Expression<Func<T, object>> ignoreMember)
        {
            var memberInfo = ReflectionHelper.FindProperty(ignoreMember);
            _ignoreNames.Add(memberInfo.Name);
            return this;
        }
    }
}
