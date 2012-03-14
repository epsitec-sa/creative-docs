﻿using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;

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


		public FormattedText GetAddressSummary()
		{
			return this.Address.GetSummary ();
		}


		partial void GetMembers(ref IList<AiderPersonEntity> value)
		{
			var members = new HashSet<AiderPersonEntity> ();

			var example1 = new AiderPersonEntity ()
			{
				Household1 = this,
			};

			var example2 = new AiderPersonEntity ()
			{
				Household2 = this,
			};

			// TODO Obtain the DataContext.

			DataContext dataContext = null;

			members.AddRange (this.GetMembers (dataContext, example1));
			members.AddRange (this.GetMembers (dataContext, example2));

			value = members.AsReadOnlyCollection ();
		}


		private IEnumerable<AiderPersonEntity> GetMembers(DataContext dataContext, AiderPersonEntity example)
		{
			if (dataContext == null)
			{
				// TMP stuff to have something in the collection.

				List<AiderPersonEntity> result = new List<AiderPersonEntity> ();

				if (this.Head1.UnwrapNullEntity () != null)
				{
					result.Add (this.Head1);
				}

				if (this.Head2.UnwrapNullEntity () != null)
				{
					result.Add (this.Head2);
				}

				return result;
			}

			return dataContext.GetByExample (example);
		}


	}
}
