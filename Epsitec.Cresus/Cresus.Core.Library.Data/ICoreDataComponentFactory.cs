//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core
{
	public interface ICoreDataComponentFactory
	{
		bool CanCreate(CoreData data);
		CoreDataComponent Create(CoreData data);
		System.Type GetComponentType();
	}
}
