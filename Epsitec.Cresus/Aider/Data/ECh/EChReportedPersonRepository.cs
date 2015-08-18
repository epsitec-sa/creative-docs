//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.ECh
{
	internal class EChReportedPersonRepository
	{
		public EChReportedPersonRepository(string xmlDataToLoad)
		{
			this.data = EChDataLoader.Load (new System.IO.FileInfo (xmlDataToLoad), int.MaxValue);
		}


		public IEnumerable<System.Tuple<EChReportedPerson,bool,bool>> GetHouseholdsInfo(string personId)
		{
			if (this.LookupByMemberId.Contains (personId))
			{
				var households = this.LookupByMemberId[personId];

				return households.Select (h => System.Tuple.Create<EChReportedPerson, bool, bool> (h, h.IsHead1 (personId), h.IsHead2 (personId)));
			}
			else
			{
				return Enumerable.Empty<System.Tuple<EChReportedPerson, bool, bool>> ();
			}
		}


		public EChReportedPerson FindAdult(string id)
		{
			return this.data.FirstOrDefault (x => x.Adult1 != null && x.Adult1.Id == id)
				?? this.data.FirstOrDefault (x => x.Adult2 != null && x.Adult2.Id == id);
		}
		
		public Dictionary<string, EChReportedPerson> ByAdult1Id
		{
			get
			{
				return this.data.ToDictionary (k => k.Adult1.Id, v => v);
			}		
		}

		public Dictionary<string, EChReportedPerson> ByAdult2Id
		{	
			get 
			{
				return this.data.Where (h => h.Adult2 != null)
					/**/        .ToDictionary (k => k.Adult2.Id, v => v);
			}		
		}

		public ILookup<string, EChReportedPerson> ByChildrenId
		{
			get 
			{
				var collection = this.data.Where (h => h.Children.Count > 0).SelectMany (
					h => h.Children,
					(h, c) => new
					{
						Household = h,
						Id = c.Id
					});
				
				return collection.ToLookup (k => k.Id, v => v.Household);
			}
		}

		public ILookup<string, EChReportedPerson> LookupByMemberId
		{
			get
			{
				var collection = this.data.SelectMany (
					h => h.GetMembers (),
					(h, c) => new
					{
						Household = h,
						Id = c.Id
					});

				return collection.ToLookup (k => k.Id, v => v.Household);
			}
		}

		
		private readonly IList<EChReportedPerson> data;
	}
}
