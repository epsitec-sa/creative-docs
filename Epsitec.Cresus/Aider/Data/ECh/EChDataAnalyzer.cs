//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel Loup, Maintainer: Samuel Loup

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Aider.Data.ECh
{
	internal class EChDataAnalyzer : EChDataComparer
	{
		public EChDataAnalyzer(string oldEchFile, string newEchFile)
			: base (oldEchFile, newEchFile)
		{
			this.FamilyToAdd    = new Dictionary<string, EChReportedPerson> ();
			this.FamilyToRemove = new Dictionary<string, EChReportedPerson> ();
			this.PersonToAdd    = new Dictionary<string, EChPerson> ();
			this.PersonToRemove = new Dictionary<string, EChPerson> ();

			//	Result 
			this.NewHouseHolds     = new HashSet<EChReportedPerson> (EChDataAnalyzer.HouseholdComparer);
			this.MissingHouseHolds = new HashSet<EChReportedPerson> (EChDataAnalyzer.HouseholdComparer);
			this.PersonMovedOut    = new Dictionary<EChPerson, List<EChReportedPerson>> (EChDataAnalyzer.PersonComparer);
			this.PersonMovedIn     = new Dictionary<EChPerson, List<EChReportedPerson>> (EChDataAnalyzer.PersonComparer);
			this.NewPersons        = new HashSet<EChPerson> (EChDataAnalyzer.PersonComparer);
			this.MissingPersons    = new HashSet<EChPerson> (EChDataAnalyzer.PersonComparer);

			this.NewUnion                          = new List<EChReportedPerson> ();
			this.NewFamily                         = new List<EChReportedPerson> ();
			this.NewFamilyWithChildren             = new List<EChReportedPerson> ();
			this.NewFamilyMono                     = new List<EChReportedPerson> ();
			this.NewFamilyMonoWithChildren         = new List<EChReportedPerson> ();
			this.FamilyWithNewChildren             = new List<EChReportedPerson> ();
			this.FamilyWithChildMissing            = new List<EChReportedPerson> ();
			this.NewChildren                       = new List<System.Tuple<EChReportedPerson, EChPerson>> ();
			this.ChildrenMissing                   = new List<System.Tuple<EChReportedPerson, EChPerson>> ();
			this.ChildrenMove                      = new List<System.Tuple<EChReportedPerson, EChPerson>> ();
			this.GainMajority                      = new List<EChReportedPerson> ();
			this.MissingUnion                      = new List<EChReportedPerson> ();
			this.WidowFamily                       = new List<EChReportedPerson> ();
			this.MissingFamily                     = new List<EChReportedPerson> ();
			this.MissingFamilyWithChildren         = new List<EChReportedPerson> ();
			this.MissingFamilyMono                 = new List<EChReportedPerson> ();
			this.MissingFamilyMonoWithChildren     = new List<EChReportedPerson> ();
			this.ChildrenLeaveHouseholdForMajority = new List<System.Tuple<EChReportedPerson, EChPerson>> ();
			this.AddCaseToResolve                  = new List<EChReportedPerson> ();

			foreach (var person in this.GetPersonsToAdd ())
			{
				this.PersonToAdd.Add (person.Id, person);
			}

			foreach (var person in this.GetPersonsToRemove ())
			{
				this.PersonToRemove.Add (person.Id, person);
			}

			foreach (var family in this.GetFamiliesToAdd ())
			{
				this.FamilyToAdd.Add (family.Adult1.Id, family);
			}

			foreach (var family in this.GetFamiliesToRemove ())
			{
				foreach (var adult in family.GetAdults ())
				{
					this.FamilyToRemove.Add (adult.Id, family);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.NewHouseHolds     = null;
				this.MissingHouseHolds = null;
				this.PersonMovedOut    = null;
				this.PersonMovedIn     = null;
				this.NewPersons        = null;
				this.MissingPersons    = null;

				this.DisposeReporting ();
			}

			base.Dispose (disposing);
		}

		
		public IEnumerable<EChReportedPerson> GetNewFamilies()
		{
			return this.NewHouseHolds;
		}

		public IEnumerable<EChReportedPerson> GetMissingFamilies()
		{
			return this.MissingHouseHolds;
		}

		
		protected void DisposeReporting()
		{
			this.FamilyToAdd                       = null;
			this.FamilyToRemove                    = null;
			this.PersonToAdd                       = null;
			this.PersonToRemove                    = null;

			this.NewUnion                          = null;
			this.NewFamily                         = null;
			this.NewFamilyWithChildren             = null;
			this.NewFamilyMono                     = null;
			this.NewFamilyMonoWithChildren         = null;
			this.FamilyWithNewChildren             = null;
			this.FamilyWithChildMissing            = null;
			this.NewChildren                       = null;
			this.ChildrenMissing                   = null;
			this.ChildrenMove                      = null;
			this.GainMajority                      = null;
			this.MissingUnion                      = null;
			this.WidowFamily                       = null;
			this.MissingFamily                     = null;
			this.MissingFamilyWithChildren         = null;
			this.MissingFamilyMono                 = null;
			this.MissingFamilyMonoWithChildren     = null;
			this.ChildrenLeaveHouseholdForMajority = null;
			this.AddCaseToResolve                  = null;

			System.GC.Collect (System.GC.MaxGeneration, System.GCCollectionMode.Forced);
		}


		protected readonly static IEqualityComparer<EChPerson>			PersonComparer    = new LambdaComparer<EChPerson> ((a, b) => a.Id == b.Id, a => a.Id.GetHashCode ());
		protected readonly static IEqualityComparer<EChReportedPerson>	HouseholdComparer = new LambdaComparer<EChReportedPerson> ((a, b) => a.FamilyKey == b.FamilyKey, a => a.FamilyKey.GetHashCode ());


		protected Dictionary<string, EChReportedPerson> FamilyToAdd;
		protected Dictionary<string, EChReportedPerson> FamilyToRemove;
		protected Dictionary<string, EChPerson> PersonToAdd;
		protected Dictionary<string, EChPerson> PersonToRemove;

		protected List<EChReportedPerson>		NewUnion;
		protected List<EChReportedPerson>		NewFamily;
		protected List<EChReportedPerson>		NewFamilyWithChildren;
		protected List<EChReportedPerson>		NewFamilyMono;
		protected List<EChReportedPerson>		NewFamilyMonoWithChildren;
		protected List<EChReportedPerson>		FamilyWithNewChildren;
		protected List<EChReportedPerson>		FamilyWithChildMissing;
		
		protected List<EChReportedPerson>		GainMajority;
		protected List<EChReportedPerson>		MissingUnion;
		protected List<EChReportedPerson>		WidowFamily;
		protected List<EChReportedPerson>		MissingFamily;
		protected List<EChReportedPerson>		MissingFamilyWithChildren;
		protected List<EChReportedPerson>		MissingFamilyMono;
		protected List<EChReportedPerson>		MissingFamilyMonoWithChildren;
		protected List<EChReportedPerson>		AddCaseToResolve;

		protected List<System.Tuple<EChReportedPerson, EChPerson>> NewChildren;
		protected List<System.Tuple<EChReportedPerson, EChPerson>> ChildrenMissing;
		protected List<System.Tuple<EChReportedPerson, EChPerson>> ChildrenMove;
		protected List<System.Tuple<EChReportedPerson, EChPerson>> ChildrenLeaveHouseholdForMajority;


		//RESULT DICTIONARY
		protected ISet<EChReportedPerson>		NewHouseHolds;
		protected ISet<EChReportedPerson>		MissingHouseHolds;
		protected ISet<EChPerson>				NewPersons;
		protected ISet<EChPerson>				MissingPersons;
		protected Dictionary<EChPerson, List<EChReportedPerson>> PersonMovedOut;
		protected Dictionary<EChPerson, List<EChReportedPerson>> PersonMovedIn;


	}
}
