//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>IEntityResolver</c> interface is used to resolve entities based
	/// on partial information.
	/// </summary>
	public interface IEntityResolver
	{
		IEnumerable<AbstractEntity> Resolve(AbstractEntity template);
	}
}
