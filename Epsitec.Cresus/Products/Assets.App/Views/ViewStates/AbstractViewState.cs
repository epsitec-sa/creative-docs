//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ViewStates
{
	public abstract class AbstractViewState : IGuid
	{
		public AbstractViewState()
		{
			this.guid = Guid.NewGuid ();
		}


		public ViewType							ViewType;
		public PageType							PageType;
		public ObjectField						Field;
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


		public virtual bool ApproximatelyEquals(AbstractViewState other)
		{
			return this.StrictlyEquals (other);
		}

		public virtual bool StrictlyEquals(AbstractViewState other)
		{
			if (other == null)
			{
				return false;
			}
			else
			{
				return this.ViewType == other.ViewType;
			}
		}


		public virtual LastViewNode GetNavigationNode(DataAccessor accessor)
		{
			return new LastViewNode (this.guid, this.ViewType, ViewMode.Unknown, EventType.Unknown, this.PageType, null, this.GetDescription (accessor), this.Pin);
		}

		protected abstract string GetDescription(DataAccessor accessor);


		protected readonly Guid					guid;
	}
}
