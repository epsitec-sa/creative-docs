//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.Platform
{
	/// <summary>
	/// The <c>UserAccountApi</c> class provides an interface to the platform
	/// user account APIs.
	/// </summary>
	internal static class UserAccountApi
	{
		/// <summary>
		/// Gets a value indicating whether the user is an administrator.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the user is an administrator; otherwise, <c>false</c>.
		/// </value>
		public static bool IsUserAnAdministrator
		{
			get
			{
				return Win32.UserAccountApi.IsUserAnAdministrator;
			}
		}
	}
}
