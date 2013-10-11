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
				return true;
			}
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool>(this.Execute);
		}

		private void Execute(bool confirmed)
		{
			var warning    = this.Entity;
			var person     = warning.Person;
			var households = new HashSet<AiderHouseholdEntity> (person.Households);
			var members    = households.SelectMany (x => x.Members).Distinct ().ToList ();

			if (confirmed)
			{
				foreach (var member in members)
				{
					if (this.CountBlockingWarningsForMember(member) > 0)
					{
						var message = "Il faut d'abord traiter l'avertissement sur ce membre: " + member.GetCompactSummary();

						throw new BusinessRuleException(message);
					}
				}

				this.ClearWarningAndRefreshCaches ();
			}
		}

		private int CountBlockingWarningsForMember(AiderPersonEntity member)
		{
			return member.Warnings.Count(w =>	w.WarningType == WarningType.EChHouseholdAdded ||		
												w.WarningType == WarningType.EChProcessArrival ||
												w.WarningType == WarningType.EChProcessDeparture);
		}

		
#if false
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
#endif
		protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
		{
			form
			.Title (this.GetTitle ())
			.Field<bool> ()
				.Title ("Contrôler et supprimer l'avertissement")
				.InitialValue (true)
			.End ();		
		}

#if false
		private FormattedText   analyse = new FormattedText ();
		private List<eCH_PersonEntity> contactToAdd     = new List<eCH_PersonEntity> ();
		private List<AiderPersonEntity> contactToRemove = new List<AiderPersonEntity> ();
#endif

	}
}
