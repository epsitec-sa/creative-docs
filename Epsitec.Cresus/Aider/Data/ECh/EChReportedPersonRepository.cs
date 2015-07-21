using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Aider.Data.ECh
{
	internal class EChReportedPersonRepository
	{
		
		public EChReportedPersonRepository (string xmlDataToLoad)
		{
			this.data = EChDataLoader.Load (new System.IO.FileInfo (xmlDataToLoad), int.MaxValue);
		}

		public IEnumerable<Tuple<EChReportedPerson,bool,bool>> GetHouseholdsInfo(string personId)
		{
			if (this.LookupByMemberId.Contains (personId))
			{
				var household = this.LookupByMemberId[personId];
				return household.Select (
					/**/		h => Tuple.Create<EChReportedPerson, bool, bool> (h, h.IsHead1 (personId), h.IsHead2 (personId)));
			}
			else
			{
				return Enumerable.Empty<Tuple<EChReportedPerson, bool, bool>> ();
			}
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
				return this.data.Where (h => h.Adult2 != null).ToDictionary (k => k.Adult2.Id, v => v);
			}		
		}

		public ILookup<string, EChReportedPerson> ByChildrenId
		{
			get 
			{
				return this.data.Where (h => h.Children.Count > 0).SelectMany (h => h.Children, (h, c) => new
				{
					household = h,
					id = c.Id
				}).ToLookup (k => k.id, v => v.household);
			}
		}

		public ILookup<string, EChReportedPerson> LookupByMemberId
		{
			get
			{

				return this.data.SelectMany (h => h.GetMembers (), 
				(h, c) => new
				{
					household = h,
					id = c.Id
				}).ToLookup (k => k.id, v => v.household);
			}
		}

		private IList<EChReportedPerson> data;
	}
}
