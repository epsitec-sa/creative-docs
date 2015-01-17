using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.Responses;

namespace App.MatchSortWebService
{
	public class MatchSortWsGetPlace : NancyModule
	{
		/*
		public MatchSortWsGetPlace()
		{
			Get["/WS/MATCHSORT/PLACE/ZIP/{zip}"] = parameters =>
			{
                return this.getPlaceFilteredBy(parameters.zip,null);
			};

            Get["/WS/MATCHSORT/PLACE/NAME/{name}"] = parameters =>
            {
                return this.getPlaceFilteredBy(null, parameters.name);
            };
		}
		
		private Response getPlaceFilteredBy(string zipFilter,string nameFilter)
		{
			var jsonData = new List<object> ();

            //var placeQuery = MatchSortWebService.data.placesFilteredBy(zipFilter,nameFilter);

            //foreach (var place in placeQuery)
            //{
            //    jsonData.Add(place);
            //}

            throw new NotImplementedException();

			return new JsonResponse (jsonData, new DefaultJsonSerializer ())
			{
				StatusCode = HttpStatusCode.OK,
			};
		}*/
	}
}
