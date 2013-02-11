//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderLegalPersonEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name, "\n", this.Type);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Comment);
				a.Accumulate ((this.Type == LegalPersonType.None ? EntityStatus.Empty : EntityStatus.Valid).TreatAsOptional ());

				return a.EntityStatus;
			}
		}

		public void RefreshCache()
		{
			this.DisplayZipCode = this.GetDisplayZipCode ();
			this.DisplayAddress = this.GetDisplayAddress ();
			this.ParishGroupPathCache = this.GetParishGroupPathCache ();
		}


		public string GetDisplayZipCode()
		{
			return this.Address.GetDisplayZipCode ().ToSimpleText ();
		}


		public string GetDisplayAddress()
		{
			return this.Address.GetDisplayAddress ().ToSimpleText ();
		}


		public string GetParishGroupPathCache()
		{
			return this.ParishGroup.Path;
		}

		partial void GetContacts(ref IList<AiderContactEntity> value)
		{
			value = this.GetContacts ().AsReadOnlyCollection ();
		}

		private IList<AiderContactEntity> GetContacts()
		{
			if (this.contacts == null)
			{
				this.contacts = new List<AiderContactEntity> ();

				var dataContext = DataContextPool.GetDataContext (this);

				if ((dataContext != null) &&
					(dataContext.IsPersistent (this)))
				{
					var example = new AiderContactEntity ()
					{
						LegalPerson = this,
					};

					var contacts = dataContext.GetByExample (example);
					var alive    = contacts.Where (x => x.Person.IsAlive);

					this.contacts.AddRange (alive);
				}
			}

			return this.contacts;
		}

		public void AddContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Add (contact);
		}

		public void RemoveContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Remove (contact);
		}

		private List<AiderContactEntity> contacts;
	}
}
