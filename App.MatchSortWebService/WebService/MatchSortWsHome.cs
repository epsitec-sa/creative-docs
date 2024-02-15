using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Hosting.Self;

namespace App.MatchSortWebService
{
	public class MatchSortWsHome : NancyModule
	{
		public MatchSortWsHome()
		{
			Get["/WS/MATCHSORT/"] = _ => "Mat[CH]Sort Web Service";
		}
	}
}