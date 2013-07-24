﻿using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This class is used to defined what actions are available in an AbstractActionTile.
	/// </summary>
	internal sealed class ActionItem
	{


		public string ViewId
		{
			get;
			set;
		}


		public string Title
		{
			get;
			set;
		}


		public bool RequiresAdditionalEntity
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
			};
		}


	}


}
