//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>CoreDatabaseType</c> enumeration defines the types of databases (user data
	/// or pure template data).
	/// </summary>
	public enum CoreDatabaseType
	{
		None,

		UserData,
		PureTemplateData,
	}
}
