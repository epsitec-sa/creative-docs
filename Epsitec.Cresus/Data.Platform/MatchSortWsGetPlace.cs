using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.Responses;

namespace Epsitec.Data.Platform
{
	public class MatchSortWsGetPlace : NancyModule
	{
		public MatchSortWsGetPlace()
		{
			Get["/WS/MATCHSORT/{zip}"] = parameters =>
			{
				return this.getPlace(parameters.zip);
			};
		}

		private Response getPlace(string zip)
		{
			var jsonData = new Dictionary<string, object> ();

			return new JsonResponse (jsonData, new DefaultJsonSerializer ())
			{
				StatusCode = HttpStatusCode.InternalServerError,
			};
		}
	}
}
