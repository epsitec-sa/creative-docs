//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractViewState : System.IEquatable<AbstractViewState>, IGuid
	{
		public AbstractViewState()
		{
			this.guid = Guid.NewGuid ();
		}


		public ViewType							ViewType;
		public PageType							PageType;
		public bool								Pin;


		#region IGuid Members
		public Guid								Guid
		{
			get
			{
				return this.guid;
			}
		}
		#endregion

		#region IEquatable<AbstractViewState> Members
		public virtual bool Equals(AbstractViewState other)
		{
			if (other == null)
			{
				return false;
			}
			{
				return this.ViewType == other.ViewType;
			}
		}
		#endregion


		public virtual LastViewNode GetNavigationNode(DataAccessor accessor)
		{
			return new LastViewNode (this.guid, this.ViewType, this.PageType, null, this.GetDescription (accessor), this.Pin);
		}

		protected virtual string GetDescription(DataAccessor accessor)
		{
			return null;
		}


		protected readonly Guid					guid;
	}
}
