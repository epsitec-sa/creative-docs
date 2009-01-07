//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Services.Extensions
{
	/// <summary>
	/// The <c>SystemTypeExtensions</c> class provides extension methods for the
	/// <see cref="System.Type"/> class.
	/// </summary>
	static class SystemTypeExtensions
	{
		/// <summary>
		/// Determines whether the specified type has a service behavior attribute.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// 	<c>true</c> the specified type has a service behavior attribute; otherwise, <c>false</c>.
		/// </returns>
		public static bool HasServiceBehaviorAttribute(this System.Type type)
		{
			return type.GetCustomAttributes (typeof (ServiceBehaviorAttribute), false).Length > 0;
		}

		/// <summary>
		/// Determines whether the specified type has a service contract attribute.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// 	<c>true</c> the specified type has a service contract attribute; otherwise, <c>false</c>.
		/// </returns>
		public static bool HasServiceContractAttribute(this System.Type type)
		{
			return type.GetCustomAttributes (typeof (ServiceContractAttribute), false).Length > 0;
		}
	}
}
