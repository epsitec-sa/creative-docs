//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// The <c>ActionItem</c> class is used to define which actions are available
	/// in an <see cref="AbstractActionTile"/>.
	/// </summary>
	internal sealed class ActionItem
	{
		public string							ViewId
		{
			get;
			set;
		}

		public string							Title
		{
			get;
			set;
		}

		public bool								RequiresAdditionalEntity
		{
			get;
			set;
		}

		public ActionItemDisplayMode			DisplayMode
		{
			get;
			set;
		}


		public Dictionary<string, object> ToDictionary()
		{
			return new Dictionary<string, object> ()
			{
				{ "viewId", this.ViewId },
				{ "title", this.Title },
				{ "requiresAdditionalEntity", this.RequiresAdditionalEntity },
				{ "displayMode", this.DisplayMode.ToString () },
			};
		}
	}
}