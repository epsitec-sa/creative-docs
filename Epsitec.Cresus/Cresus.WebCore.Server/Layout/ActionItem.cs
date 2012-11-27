using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


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


		public Dictionary<string, object> ToDictionary()
		{
			return new Dictionary<string, object> ()
			{
				{ "viewId", this.ViewId },
				{ "title", this.Title },
			};
		}


	}


}
