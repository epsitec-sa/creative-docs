//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractViewState : System.IEquatable<AbstractViewState>
	{
		public ViewType							ViewType;


		#region IEquatable<AbstractViewState> Members
		public virtual bool Equals(AbstractViewState other)
		{
			return this.ViewType == other.ViewType;
		}
		#endregion
	}
}
