//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business.UserManagement
{
	/// <summary>
	/// The <c>UserAuthenticationMethod</c> enumeration defines the supported authentication
	/// methods.
	/// </summary>
	[DesignerVisible]
	public enum UserAuthenticationMethod
	{
		/// <summary>
		/// No authentication. Ad hoc login.
		/// </summary>
		None,

		/// <summary>
		/// Authentication provided by the operating system; the user name is the same
		/// as the login of the active Windows user.
		/// </summary>
		System,
		
		/// <summary>
		/// Password authentication by Cresus Core itself.
		/// </summary>
		Password,

		/// <summary>
		/// No authentication is possible; this is used for internal system users which
		/// cannot be used by interactive users.
		/// </summary>
		Disabled,
	}
}
