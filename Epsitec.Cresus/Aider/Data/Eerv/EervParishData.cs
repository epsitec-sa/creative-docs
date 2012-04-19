using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Collections.ObjectModel;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervParishData
	{


		public EervParishData(string id, IEnumerable<EervHousehold> households, IEnumerable<EervPerson> persons, IEnumerable<EervLegalPerson> legalPersons, IEnumerable<EervGroup> groups)
		{
			this.Id = id;
			this.Households = households.AsReadOnlyCollection();
			this.Persons = persons.AsReadOnlyCollection ();
			this.LegalPersons = legalPersons.AsReadOnlyCollection ();
			this.Groups = groups.AsReadOnlyCollection ();
		}


		public readonly string Id;
		public readonly ReadOnlyCollection<EervHousehold> Households;
		public readonly ReadOnlyCollection<EervPerson> Persons;
		public readonly ReadOnlyCollection<EervLegalPerson> LegalPersons;
		public readonly ReadOnlyCollection<EervGroup> Groups;


	}


}
