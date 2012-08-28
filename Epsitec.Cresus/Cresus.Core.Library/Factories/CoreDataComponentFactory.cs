//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>CoreDataComponentFactory</c> class provides methods to register and setup
	/// components implementing <see cref="CoreDataComponent"/>, used as components for
	/// <see cref="CoreData"/>.
	/// </summary>
	public class CoreDataComponentFactory : CoreComponentFactory<CoreData, ICoreDataComponentFactory, CoreDataComponent>
	{
	}
}
