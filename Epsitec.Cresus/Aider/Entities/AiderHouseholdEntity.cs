using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

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
			// TODO Obtain the DataContext.

			BusinessContext businessContext = null;

			if (businessContext == null)
			{
				// TMP stuff to have something in the collection.

				value = new List<AiderPersonEntity> ();

				if (this.Head1.UnwrapNullEntity () != null)
				{
					value.Add (this.Head1);
				}

				if (this.Head2.UnwrapNullEntity () != null)
				{
					value.Add (this.Head2);
				}

				value = value.AsReadOnlyCollection ();
			}
			else
			{
				value = this.GetMembers (businessContext).AsReadOnlyCollection ();
			}
		}


		public IEnumerable<AiderPersonEntity> GetMembers(BusinessContext businessContext)
		{
			var example1 = new AiderPersonEntity ()
			{
				Household1 = this,
			};

			var example2 = new AiderPersonEntity ()
			{
				Household2 = this,
			};

			DataContext dataContext = businessContext.DataContext;

			var members = new HashSet<AiderPersonEntity> ();

			members.AddRange (dataContext.GetByExample (example1));
			members.AddRange (dataContext.GetByExample (example2));

			return members;
		}


		public IList<AiderPersonEntity> GetHeads()
		{
			var heads = new List<AiderPersonEntity> ();

			var head1 = this.Head1;

			if (head1.IsNotNull ())
			{
				heads.Add (head1);
			}

			var head2 = this.Head2;

			if (head2.IsNotNull ())
			{
				heads.Add (head2);
			}

			return heads;
		}


	}


}
