//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Enumerations;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Data.Platform;
using Epsitec.Aider.Data.ECh;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (5)]
	public sealed class ActionAiderPersonWarningViewController5ProcessHouseholdChange : ActionAiderPersonWarningViewControllerInteractive
	{
		public override bool IsEnabled
		{
			get
			{
				//	TODO: refactor and validate this implementation
				return false;
			}
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool,bool>(this.Execute);
		}

		private void Execute(bool correctHousehold,bool confirmed)
		{        
			if (confirmed)
			{
				var householdMembers = this.Entity.Person.Contacts.Where(c => c.Household.Address.IsNotNull()).First().Household.Members;
				foreach (var member in householdMembers)
				{
					var warnCount = member.Warnings.Where (w => w.WarningType.Equals (WarningType.EChPersonMissing) ||
																w.WarningType.Equals (WarningType.EChHouseholdAdded) ||		
																w.WarningType.Equals(WarningType.EChProcessArrival) ||
																w.WarningType.Equals (WarningType.EChProcessDeparture)).ToList ().Count;
					if (warnCount > 0)
					{
						var message = "Il faut d'abord traiter l'avertissement sur ce membre: " + member.GetCompactSummary();

						throw new BusinessRuleException(message);
					}
				}


				var newHousehold = this.GetNewHousehold();
				if (newHousehold.Adult2.IsNotNull())
				{
					var adult2WarnCount = this.GetAiderPerson(newHousehold.Adult2).Warnings.Where(w => w.WarningType.Equals(WarningType.EChPersonMissing) ||
																w.WarningType.Equals(WarningType.EChHouseholdAdded) ||
																w.WarningType.Equals(WarningType.EChProcessArrival) ||
																w.WarningType.Equals(WarningType.EChProcessDeparture)).ToList().Count;

					if (adult2WarnCount > 0)
					{
						var message = "Il faut d'abord traiter l'avertissement sur ce membre: " + newHousehold.Adult2.GetCompactSummary();
						throw new BusinessRuleException(message);
					}
				}
				
				foreach (var child in newHousehold.Children)
				{


					var childWarnCount = this.GetAiderPerson(child).Warnings.Where(w => w.WarningType.Equals(WarningType.EChPersonMissing) ||
																w.WarningType.Equals(WarningType.EChProcessArrival) ||
																w.WarningType.Equals(WarningType.EChProcessDeparture)).ToList().Count;
					if (childWarnCount > 0)
					{
						var message = "Il faut d'abord traiter l'avertissement sur ce membre: " + child.GetCompactSummary();

						throw new BusinessRuleException(message);
					}

				}

				this.ClearWarningAndRefreshCaches ();
			}

			var household = this.Entity.Person.Contacts.Where (c => c.Household.Address.IsNotNull ()).First ().Household;
			if (correctHousehold && household.IsNotNull ())
			{
				var result = this.analyseChanges();
					
				switch (result)
				{
					case -1:
						foreach (var contactToAdd in this.contactToAdd)
						{
							var aiderPerson = this.GetAiderPerson(contactToAdd);
							AiderContactEntity.Create(this.BusinessContext, aiderPerson, household, false);
						}
						break;
					case 1:
						foreach (var personToRemove in this.contactToRemove)
						{
							var contact = personToRemove.Contacts.Where(c => c.Household == household).First();
							this.BusinessContext.DeleteEntity(contact);
						}
						break;
				}

				this.ClearWarningAndRefreshCaches ();
			}
		}

		private AiderTownEntity GetAiderTownEntity(eCH_AddressEntity address)
		{
			var townExample = new AiderTownEntity()
			{
				SwissZipCodeId = address.SwissZipCodeId
			};

			return this.BusinessContext.DataContext.GetByExample<AiderTownEntity>(townExample).FirstOrDefault();
		}

		private eCH_ReportedPersonEntity GetNewHousehold()
		{
			var echHouseholdExample = new eCH_ReportedPersonEntity()
			{
				Adult1 = this.Entity.Person.eCH_Person
			};
			return this.BusinessContext.DataContext.GetByExample<eCH_ReportedPersonEntity>(echHouseholdExample).FirstOrDefault();
		}

		private eCH_ReportedPersonEntity GetEChHousehold(eCH_PersonEntity person)
		{
			var echHouseholdExample = new eCH_ReportedPersonEntity ()
			{
				Adult1 = person
			};
			return this.BusinessContext.DataContext.GetByExample<eCH_ReportedPersonEntity> (echHouseholdExample).FirstOrDefault ();
		}

		private AiderPersonEntity GetAiderPerson(eCH_PersonEntity person)
		{
			var personExample = new AiderPersonEntity()
			{
				eCH_Person = person
			};
			return this.BusinessContext.DataContext.GetByExample<AiderPersonEntity>(personExample).FirstOrDefault();
		}

		private int analyseChanges()
		{
			var aiderHouseholdMembers = this.Entity.Person.Contacts.Where (c => c.Household.Address.IsNotNull ()).First ().Household.Members.Where(p => p.IsGovernmentDefined).ToList ();
			var newEChHousehold = this.GetNewHousehold ();
			this.analyse = this.analyse.AppendLine (TextFormatter.FormatText ("Résultat de l'analyse:\n"));
			var newHouseholdIds = new HashSet<string>();
			newHouseholdIds.Add(newEChHousehold.Adult1.PersonId);
			if (newEChHousehold.Adult2.IsNotNull())
			{
				newHouseholdIds.Add(newEChHousehold.Adult2.PersonId);
			}           
			foreach (var child in newEChHousehold.Children)
			{
				if (child.IsNotNull())
				{
					newHouseholdIds.Add(child.PersonId);
				}          
			}
		  
			if (aiderHouseholdMembers.Count < newEChHousehold.MembersCount)
			{
				this.analyse = this.analyse.AppendLine (TextFormatter.FormatText ("Le ménage ECh contient plus de membres : "));

				if (newEChHousehold.Adult2.IsNotNull())
				{
					if (!aiderHouseholdMembers.Select(m => m.eCH_Person.PersonId).Contains(newEChHousehold.Adult2.PersonId))
					{
						this.analyse = this.analyse.AppendLine(TextFormatter.FormatText(newEChHousehold.Adult2.PersonOfficialName + " " + newEChHousehold.Adult2.PersonFirstNames));
						this.contactToAdd.Add(newEChHousehold.Adult2);
					}
				}
				

				foreach (var child in newEChHousehold.Children)
				{
					if (!aiderHouseholdMembers.Select (m => m.eCH_Person.PersonId).Contains (child.PersonId))
					{
						this.analyse = this.analyse.AppendLine (TextFormatter.FormatText (child.PersonOfficialName + " " + child.PersonFirstNames));
						this.contactToAdd.Add(child);
					}
				}		
	
				return 1;
			}

			if (aiderHouseholdMembers.Count > newEChHousehold.MembersCount)
			{
				this.analyse = this.analyse.AppendLine (TextFormatter.FormatText ("Le ménage ECh contient moins de membres : "));

				var missing = aiderHouseholdMembers.Where(p => !newHouseholdIds.Contains(p.eCH_Person.PersonId) && p.eCH_Person.IsNotNull());
				foreach (var miss in missing)
				{
					this.analyse = this.analyse.AppendLine (TextFormatter.FormatText (miss.DisplayName));
					this.contactToRemove.Add (miss);
				}
				
				return -1;
			}

			if (aiderHouseholdMembers.Count == newEChHousehold.MembersCount)
			{
				var mismatch = false;
				foreach (var member in aiderHouseholdMembers)
				{
					if (!newHouseholdIds.Contains(member.eCH_Person.PersonId))
					{
						this.analyse = this.analyse.AppendLine(member.DisplayName + " est à controler");
						mismatch = true;
					}
				}

				if (mismatch)
				{
					return 2;
				}
			}

			this.analyse = this.analyse.AppendLine (TextFormatter.FormatText ("OK"));
			return 0;
		}


		protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
		{
			this.analyseChanges ();

			form
			.Title (this.GetTitle ())
			.Text (analyse)
			.Field<bool>()
				.Title("Adapter à la composition ECh")
				.InitialValue(false)
			.End ()
			.Field<bool> ()
				.Title ("Contrôler et supprimer l'avertissement")
				.InitialValue (true)
			.End ();
			
		}

		private FormattedText   analyse = new FormattedText ();
		private List<eCH_PersonEntity> contactToAdd     = new List<eCH_PersonEntity> ();
		private List<AiderPersonEntity> contactToRemove = new List<AiderPersonEntity> ();
	}
}
