//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>CoreDataComponent</c> class is the base class used by all components, which are
	/// dynamically instanciated and attached to the <see cref="CoreData"/> host.
	/// </summary>
	public abstract class CoreDataComponent : CoreComponent<CoreData, CoreDataComponent>
	{
		protected CoreDataComponent(CoreData data)
			: base (data)
		{
		}
	}
}
