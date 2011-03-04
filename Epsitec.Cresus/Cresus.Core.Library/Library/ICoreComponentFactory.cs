//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Library
{
	public interface ICoreComponentFactory<THost, TComponent>
	{
		bool CanCreate(THost data);
		TComponent Create(THost data);
		System.Type GetComponentType();
	}
}
