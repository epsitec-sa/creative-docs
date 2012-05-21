//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class MemberInfoExtensions
	{
		public static IEnumerable<T> GetCustomAttributes<T>(this System.Reflection.MemberInfo memberInfo, bool inherit = false)
			where T : System.Attribute
		{
			return memberInfo.GetCustomAttributes (typeof (T), inherit).Cast<T> ();
		}
	}
}
