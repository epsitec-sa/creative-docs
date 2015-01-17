using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.Responses;

namespace App.MatchSortWebService
{
	public class MatchSortWsGetHouse : NancyModule
	{
		/*
		public MatchSortWsGetHouse()
		{
			Get["/WS/MATCHSORT/PLACE/{zip}/STREET/{street}"] = parameters =>
			{
				return this.getHousesAtStreet(parameters.zip,parameters.street);
			};

            Get["/WS/MATCHSORT/PLACE/{zip}/STREET/{street}/HOUSE/{house}"] = parameters =>
            {
                return this.getHouseAtStreet(parameters.zip, parameters.street, parameters.house);
            };
		}

		private Response getHousesAtStreet(string zip,string streetName)
		{
			var jsonData = new List<object> ();

            //var houseQuery = MatchSortWebService.data.HousesAtStreet(zip,streetName);

            //foreach (var place in houseQuery)
            //{
            //    jsonData.Add (place);
            //}

            throw new NotImplementedException();

			return new JsonResponse (jsonData, new DefaultJsonSerializer ())
			{
				StatusCode = HttpStatusCode.OK,
			};
		}

        private Response getHouseAtStreet(string zip, string streetName,string houseNumber)
        {
            var jsonData = new List<object>();

            //var houseQuery = MatchSortWebService.data.HouseAtStreet(zip,streetName,houseNumber);

            //foreach (var place in houseQuery)
            //{
            //    jsonData.Add (place);
            //}

            throw new NotImplementedException();

            return new JsonResponse(jsonData, new DefaultJsonSerializer())
            {
                StatusCode = HttpStatusCode.OK,
            };
        }*/
	}
}
