//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
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
		}


		public string GetDisplayZipCode()
		{
			return this.Address.GetDisplayZipCode ().ToSimpleText ();
		}

		public string GetDisplayAddress()
		{
			return this.Address.GetDisplayAddress ().ToSimpleText ();
		}

		private IList<AiderGroupParticipantEntity> GetParticipations()
		{
			if (this.participations == null)
			{
				this.participations = this.ExecuteWithDataContext
				(
					d => this.FindParticipations (d),
					() => new List<AiderGroupParticipantEntity> ()
				);
			}

			return this.participations;
		}


		internal void AddParticipationInternal(AiderGroupParticipantEntity participation)
		{
			this.GetParticipations ().Add (participation);
		}

		internal void RemoveParticipationInternal(AiderGroupParticipantEntity participation)
		{
			this.GetParticipations ().Remove (participation);
		}


		private IList<AiderGroupParticipantEntity> FindParticipations(DataContext dataContext)
		{
			var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, true);

			return dataContext.GetByRequest<AiderGroupParticipantEntity> (request);
		}


		public void AddContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Add (contact);
		}

		public void RemoveContactInternal(AiderContactEntity contact)
		{
			this.GetContacts ().Remove (contact);
		}


		public static void Delete(BusinessContext businessContext, AiderLegalPersonEntity legalPerson)
		{
			//	TODO: ...
		}

		
		partial void OnParishGroupChanging(AiderGroupEntity oldValue, AiderGroupEntity newValue)
		{
			this.ParishGroupPathCache = AiderGroupEntity.GetPath (newValue);
		}

		partial void GetContacts(ref IList<AiderContactEntity> value)
		{
			value = this.GetContacts ().AsReadOnlyCollection ();
		}

		
		private IList<AiderContactEntity> GetContacts()
		{
			if (this.contacts == null)
			{
				this.contacts = this.ExecuteWithDataContext
				(
					d => this.GetContacts (d),
					() => new List<AiderContactEntity> ()
				);
			}

			return this.contacts;
		}

		private IList<AiderContactEntity> GetContacts(DataContext dataContext)
		{
			var example = new AiderContactEntity ()
			{
				LegalPerson = this,
			};

			return dataContext
				.GetByExample (example)
				.Where (x => x.Person.IsAlive)
				.ToList ();
		}


		private IList<AiderGroupParticipantEntity>	participations;
		private IList<AiderContactEntity>			contacts;
	}
}
