//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	/// <summary>
	///	Noeud correspondant à une ligne de la liste des anciennes vues.
	/// </summary>
	public struct LastViewNode
	{
		public LastViewNode(Guid navigationGuid, ViewType viewType, PageType pageType, Timestamp? timestamp, string description, bool pin)
		{
			this.NavigationGuid = navigationGuid;
			this.ViewType       = viewType;
			this.PageType       = pageType;
			this.Timestamp      = timestamp;
			this.Description    = description;
			this.Pin            = pin;
		}

		public bool IsEmpty
		{
			get
			{
				return this.NavigationGuid.IsEmpty
					&& this.ViewType    == ViewType.Unknown
					&& this.PageType    == PageType.Unknown
					&& !this.Timestamp.HasValue
					&& this.Description == null
					&& this.Pin         == false;
			}
		}

		public static LastViewNode Empty = new LastViewNode (Guid.Empty, ViewType.Unknown, PageType.Unknown, null, null, false);

		public readonly Guid				NavigationGuid;
		public readonly ViewType			ViewType;
		public readonly PageType			PageType;
		public readonly Timestamp?			Timestamp;
		public readonly string				Description;
		public readonly bool				Pin;
	}
}
