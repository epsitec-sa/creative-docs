//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Runtime.InteropServices;
namespace Epsitec.Common.Support.Platform.Win32
{
	/// <summary>
	/// The <c>UserAccountApi</c> class provides an interface to the Win32
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
				try
				{
					return UserAccountApi.IsUserAnAdmin ();
				}
				catch
				{
					return false;
				}
			}
		}

		
		[DllImport ("shell32.dll", ExactSpelling=true, SetLastError=true)]
		private static extern bool IsUserAnAdmin();
	}
}
