using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.NancyModules;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class EntityValue
	{


		public string Displayed
		{
			get;
			set;
		}


		public string Submitted
		{
			get;
			set;
		}


		public Dictionary<string, object> ToDictionary()
		{
			return new Dictionary<string, object> ()
			{
				{ "displayed", this.Displayed },
				{ "submitted", this.Submitted },
			};
		}


	}


}
