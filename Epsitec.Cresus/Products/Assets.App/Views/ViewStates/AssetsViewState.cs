//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ViewStates
{
	public class AssetsViewState : AbstractViewState
	{
		public ViewMode							ViewMode;
		public bool								IsShowEvents;
		public Timestamp?						SelectedTimestamp;
		public Guid								SelectedGuid;
		public Guid								FilterTreeTableGuid;
		public Guid								FilterTimelinesGuid;
		public bool								ShowGraphic;


		public override bool StrictlyEquals(AbstractViewState other)
		{
			var o = other as AssetsViewState;
			if (o == null)
			{
				return false;
			}

			return this.ViewType          == o.ViewType
				&& this.ViewMode          == o.ViewMode
				&& this.PageType          == o.PageType
				&& this.IsShowEvents      == o.IsShowEvents
				&& this.SelectedTimestamp == o.SelectedTimestamp
				&& this.SelectedGuid      == o.SelectedGuid
				&& this.ShowGraphic       == o.ShowGraphic;
		}


		public override LastViewNode GetNavigationNode(DataAccessor accessor)
		{
			var eventType = this.GetEventType (accessor);
			return new LastViewNode (this.guid, this.ViewType, this.ViewMode, eventType, this.PageType, this.SelectedTimestamp, this.GetDescription (accessor), this.Pin);
		}

		private EventType GetEventType(DataAccessor accessor)
		{
			if (!this.SelectedGuid.IsEmpty && this.SelectedTimestamp.HasValue)
			{
				var obj = accessor.GetObject (BaseType.Assets, this.SelectedGuid);
				if (obj != null)
				{
					var e = obj.GetEvent (this.SelectedTimestamp.Value);
					if (e != null)
					{
						return e.Type;
					}
				}
			}

			return EventType.Unknown;
		}

		protected override string GetDescription(DataAccessor accessor)
		{
			if (!this.SelectedGuid.IsEmpty)
			{
				var list = new List<string> ();

				list.Add (AssetsLogic.GetSummary (accessor, this.SelectedGuid));

				if (this.SelectedTimestamp.HasValue)
				{
					list.Add (TypeConverters.DateToString (this.SelectedTimestamp.Value.Date));
				}

				if (this.PageType != Views.PageType.Unknown)
				{
					list.Add (StaticDescriptions.GetObjectPageDescription (this.PageType));
				}

				return UniversalLogic.NiceJoin (list.ToArray ());
			}

			return null;
		}
	}
}
