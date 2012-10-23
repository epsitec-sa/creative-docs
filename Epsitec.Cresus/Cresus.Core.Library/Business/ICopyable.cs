//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>ICopyableEntity</c> interface is used to copy an entity (this can be a mix
	/// between a deep and a shallow copy, depending on the entity graph, for instance).
	/// </summary>
	/// <typeparam name="T">The type of the entity, which will be copied.</typeparam>
	public interface ICopyableEntity<T>
		where T : AbstractEntity
	{
		void CopyTo(BusinessContext businessContext, T copy);
	}
}
