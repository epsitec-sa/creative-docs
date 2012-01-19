using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;


namespace Epsitec.Aider.Entities
{


	public partial class AiderHouseholdEntity
	{


		public override FormattedText GetSummary()
		{
			var headNames = new List<string> ();

			var headName1 = this.Head1.eCH_Person.PersonOfficialName;
			var headName2 = this.Head2.eCH_Person.PersonOfficialName;

			if (headName1 != null)
			{
				headNames.Add (headName1);
			}

			if (headName2 != null)
			{
				headNames.Add (headName2);
			}

			var text = "Famille " + headNames.Distinct ().Join (" ");

			return TextFormatter.FormatText (text);
		}


		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}


		partial void GetMembers(ref IList<AiderPersonEntity> value)
		{
			var members = new List<AiderPersonEntity> ();

			value = members.AsReadOnly ();

			var example = new AiderPersonEntity ()
			{
				Household = this,
			};

			DataContext dataContext = null;
			
			// TODO Obtain the DataContext.

			if (dataContext == null)
			{
				// TMP stuff to have something in the collection.

				if (this.Head1.UnwrapNullEntity () != null)
				{
					members.Add (this.Head1);
				}

				if (this.Head2.UnwrapNullEntity () != null)
				{
					members.Add (this.Head2);
				}

				return;
			}

			// TODO Add ordering ?
			var result = dataContext.GetByExample (example);

			members.AddRange (result);
		}
	
	
	}
}
