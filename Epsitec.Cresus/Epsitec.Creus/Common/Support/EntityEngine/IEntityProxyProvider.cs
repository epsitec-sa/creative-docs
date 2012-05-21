//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>IEntityProxyProvider</c> interface gives access to the <see cref="IEntityProxy"/>
	/// interface.
	/// </summary>
	public interface IEntityProxyProvider
	{
		/// <summary>
		/// Gets the entity proxy.
		/// </summary>
		/// <returns>The entity proxy or <c>null</c>.</returns>
		IEntityProxy GetEntityProxy();
	}
}
