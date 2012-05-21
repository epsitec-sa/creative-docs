//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>InterfaceImplementationTester</c> class provides a single <c>Check</c> method
	/// which can be used to verify very efficiently on run-time if a class implements a
	/// given interface.
	/// </summary>
	/// <typeparam name="T">The type of the class to test.</typeparam>
	/// <typeparam name="TInterface">The type of the interface that <c>T</c> should implement.</typeparam>
	public static class InterfaceImplementationTester<T, TInterface>
	{
		/// <summary>
		/// Checks whether the type <c>T</c> implements interface <c>TInterface</c>.
		/// </summary>
		/// <returns><c>true</c> if type <c>T</c> implements interface <c>TInterface</c>; otherwise, <c>false</c>.</returns>
		public static bool Check()
		{
			if (InterfaceImplementationTester<T, TInterface>.result.HasValue == false)
			{
				InterfaceImplementationTester<T, TInterface>.result = typeof (T).GetInterfaces ().Contains (typeof (TInterface));
			}

			return InterfaceImplementationTester<T, TInterface>.result.Value;
		}

		[System.ThreadStatic]
		private static bool? result;
	}
}
