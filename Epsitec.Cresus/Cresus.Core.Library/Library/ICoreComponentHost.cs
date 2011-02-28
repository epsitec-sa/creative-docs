//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Text;

namespace Epsitec.Cresus.Core.Library
{
	public interface ICoreComponentHost<TComponent>
		where TComponent : class, ICoreComponent
	{
		T GetComponent<T>()
			where T : TComponent;

		IEnumerable<TComponent> GetComponents();

		bool ContainsComponent<T>()
			where T : TComponent;

		void RegisterComponent<T>(T component)
			where T : TComponent;
	}
}
