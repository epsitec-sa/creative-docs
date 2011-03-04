//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>ICoreDataComponentFactory</c> interface must be implemented by all factories
	/// which provide <see cref="CoreDataComponent"/> instances for <see cref="CoreData"/>.
	/// </summary>
	public interface ICoreDataComponentFactory : ICoreComponentFactory<CoreData, CoreDataComponent>
	{
	}
}