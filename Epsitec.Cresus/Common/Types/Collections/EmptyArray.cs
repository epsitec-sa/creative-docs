//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>EmptyArray</c> class provides a singleton of an empty array of
	/// type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the array.</typeparam>
	public static class EmptyArray<T>
	{
		public static readonly T[] Instance = new T[0];
	}
}
