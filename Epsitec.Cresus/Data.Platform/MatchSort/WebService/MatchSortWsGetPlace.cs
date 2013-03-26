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
			Get["/WS/MATCHSORT/PLACE/{zip}"] = parameters =>
			{
				return this.getPlace(parameters.zip);
			};
		}

		private Response getPlace(string zip)
		{
			var jsonData = new List<object> ();

            var placeQuery = MatchSortWebService.data.filteredPlaces(zip);

            foreach (var place in placeQuery)
            {
                jsonData.Add(place);
            }
			return new JsonResponse (jsonData, new DefaultJsonSerializer ())
			{
				StatusCode = HttpStatusCode.OK,
			};
		}
	}
}
