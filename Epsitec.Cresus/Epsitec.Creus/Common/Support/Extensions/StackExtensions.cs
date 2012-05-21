//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class StackExtensions
	{
		public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> collection)
		{
			foreach (var item in collection)
			{
				stack.Push (item);
			}
		}
	}
}
