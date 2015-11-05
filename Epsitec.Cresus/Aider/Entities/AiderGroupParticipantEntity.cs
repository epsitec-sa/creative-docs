//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;

using System;

using System.Linq;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Helpers;

namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupParticipantEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.StartDate, "—", TextFormatter.Command.IfEmpty, "…", this.EndDate, "—", TextFormatter.Command.IfEmpty);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.StartDate, "—", TextFormatter.Command.IfEmpty, "…", this.EndDate, "—", TextFormatter.Command.IfEmpty);
		}

		public FormattedText GetSummaryWithHierarchicalGroupName()
		{
			var name = this.Group.GetHierarchicalName (this.Person);

			return TextFormatter.FormatText (name);
		}

		public string GetRolePathOrHierarchicalName ()
		{
			if (string.IsNullOrWhiteSpace (this.RolePathCache))
			{
				return this.GetSummaryWithHierarchicalGroupName ().ToSimpleText ();
			}
			else
			{
				return this.RolePathCache;
			}
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield break;
		}
		
		public void Delete(BusinessContext businessContext)
		{
			AiderGroupParticipantEntity.StopParticipationInternal (this);

			if (this.Comment.IsNotNull ())
			{
				businessContext.DeleteEntity (this.Comment);
			}
			
			businessContext.DeleteEntity (this);
		}

		public static AiderGroupParticipantEntity StartParticipation(BusinessContext businessContext, AiderGroupEntity group, ParticipationData participationData)
		{
			return AiderGroupParticipantEntity.StartParticipation (businessContext, group, participationData, null);
		}

		public static AiderGroupParticipantEntity StartParticipation(BusinessContext businessContext, AiderGroupEntity group, ParticipationData participationData, Date? startDate)
		{
			if (!group.CanHaveMembers ())
			{
				throw new InvalidOperationException ("This group cannot have members.");
			}

			var participation = businessContext.CreateAndRegisterEntity<AiderGroupParticipantEntity> ();

			participation.Assign (participationData);
			participation.Group = group;
			participation.StartDate = startDate;

			if (!startDate.HasValue || startDate <= Date.Today)
			{
				AiderGroupParticipantEntity.StartParticipationInternal (participation);
			}

			if (!group.GroupDef.RoleCacheDisabled)
			{
				participation.RoleCacheDisabled = false;
				participation.RoleCache = AiderParticipationsHelpers.BuildRoleFromParticipation (participation).GetRole (participation);
				participation.RolePathCache = AiderParticipationsHelpers.GetRolePath (participation);
			}
			else
			{
				participation.RoleCacheDisabled = true;
			}

			return participation;
		}

		public static AiderGroupParticipantEntity StartParticipation(BusinessContext businessContext, AiderGroupEntity group, ParticipationData participationData, Date? startDate, FormattedText comment)
		{
			var participation = AiderGroupParticipantEntity.StartParticipation (businessContext, group, participationData, startDate);

			if (comment.IsNullOrEmpty () == false)
			{
				participation.Comment.Text = comment;
			}

			return participation;
		}

		public static AiderGroupParticipantEntity RestoreParticipation(BusinessContext businessContext, AiderGroupParticipantEntity participation)
		{
			participation.EndDate = null;
			AiderGroupParticipantEntity.RestoreParticipationInternal (participation);
			return participation;
		}

		
		public static void StopParticipation(AiderGroupParticipantEntity participation, Date endDate)
		{
			participation.EndDate = endDate;

			if (endDate <= Date.Today)
			{
				AiderGroupParticipantEntity.StopParticipationInternal (participation);
			}
		}

		public static void StopParticipation(AiderGroupParticipantEntity participation, Date endDate, FormattedText comment)
		{
			AiderGroupParticipantEntity.StopParticipation (participation, endDate);

			if (comment.Length > 0 || participation.Comment.Text.Length > 0)
			{
				participation.Comment.Text = comment;
			}
		}


		public static AiderGroupParticipantEntity ImportParticipation(BusinessContext businessContext, AiderGroupEntity group, ParticipationData participationData, Date? startDate, Date? endDate, FormattedText comment)
		{
			var participation = AiderGroupParticipantEntity.StartParticipation (businessContext, group, participationData, startDate, comment);

			if (endDate.HasValue)
			{
				AiderGroupParticipantEntity.StopParticipation (participation, endDate.Value);
			}

			return participation;
		}

		
		public static Request CreateParticipantRequest(DataContext dataContext, AiderGroupEntity group, bool current)
		{
			var participation = new AiderGroupParticipantEntity ()
			{
				Group = new AiderGroupEntity (),
			};

			var request = new Request ()
			{
				RootEntity = participation,
			};

			request.AddCondition (dataContext, participation, g => g.Group == group);

			if (current)
			{
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);
			}

			return request;
		}

		public static Request CreateParticipantRequest(DataContext dataContext, AiderPersonEntity person, bool current)
		{
			var participation = new AiderGroupParticipantEntity ()
			{
				Person = new AiderPersonEntity (),
				Group = new AiderGroupEntity (),
			};

			var request = new Request ()
			{
				RootEntity = participation,
			};

			request.AddCondition (dataContext, participation, g => g.Person == person);

			if (current)
			{
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);
			}

			return request;
		}

		public static Request CreateParticipantRequest(DataContext dataContext, AiderContactEntity contact, bool current)
		{
			var participation = new AiderGroupParticipantEntity ()
			{
				Contact = new AiderContactEntity (),
				Group = new AiderGroupEntity (),
			};

			var request = new Request ()
			{
				RootEntity = participation,
			};

			request.AddCondition (dataContext, participation, g => g.Contact == contact);

			if (current)
			{
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);
			}

			return request;
		}

		public static Request CreateRoleCacheParticipantRequest(DataContext dataContext, AiderContactEntity contact, bool current)
		{
			var participation = new AiderGroupParticipantEntity ()
			{
				Contact = new AiderContactEntity (),
				Group = new AiderGroupEntity (),
				RoleCacheDisabled = false
			};

			var request = new Request ()
			{
				RootEntity = participation,
			};

			request.AddCondition (dataContext, participation, p => p.Contact == contact);

			if (current)
			{
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);
			}

			return request;
		}

		public static Request CreateParticipantRequest(DataContext dataContext, AiderLegalPersonEntity legalPerson, bool current)
		{
			var participation = new AiderGroupParticipantEntity ()
			{
				LegalPerson = new AiderLegalPersonEntity ()
			};

			var request = new Request ()
			{
				RootEntity = participation,
			};

			request.AddCondition (dataContext, participation, g => g.LegalPerson == legalPerson);

			if (current)
			{
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);
			}

			return request;
		}

		public static Request CreateGroupAndSubGroupMemberRequest(DataContext dataContext, AiderGroupEntity group, bool current)
		{
			var contact = new AiderContactEntity ();
			var participation = new AiderGroupParticipantEntity ()
			{
				Contact = contact,
			};

			var request = new Request ()
			{
				RootEntity = participation,
				RequestedEntity = contact,
				Distinct = true,
			};

			AiderGroupParticipantEntity.AddGroupAndSubGroupMemberCondition (dataContext, request, participation, group);

			if (current)
			{
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);
			}

			return request;
		}

		public static Request CreateGroupAndSubGroupParticipantRequest(DataContext dataContext, AiderGroupEntity group, bool current)
		{
			var participation = new AiderGroupParticipantEntity ();

			var request = new Request ()
			{
				RootEntity = participation,
				RequestedEntity = participation,
				Distinct = true,
			};

			AiderGroupParticipantEntity.AddGroupAndSubGroupMemberCondition (dataContext, request, participation, group);

			if (current)
			{
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);
			}

			return request;
		}


		public static void AddCurrentCondition(DataContext dataContext, Request request, AiderGroupParticipantEntity participation)
		{
			request.AddCondition (dataContext, participation, g => g.StartDate == null || g.StartDate <= Date.Today);
			request.AddCondition (dataContext, participation, g => g.EndDate == null || g.EndDate > Date.Today);
		}

		public static void AddGroupAndSubGroupMemberCondition(DataContext dataContext, Request request, AiderGroupParticipantEntity participation, AiderGroupEntity group)
		{
			if (participation.Group == null)
			{
				participation.Group = new AiderGroupEntity ();
			}

			request.AddCondition
			(
				dataContext,
				participation,
				x => SqlMethods.Like (x.Group.Path, group.Path + SqlMethods.TextWildcard)
			);
		}

		private static void StartParticipationInternal(AiderGroupParticipantEntity participation)
		{
			var person = participation.Person;
			var legalPerson = participation.LegalPerson;
			
			if (person.IsNotNull ())
			{
				person.AddParticipationInternal (participation);
			}

			if (legalPerson.IsNotNull ())
			{
				legalPerson.AddParticipationInternal (participation);
			}
		}

		private static void RestoreParticipationInternal(AiderGroupParticipantEntity participation)
		{
			var person = participation.Person;
			var legalPerson = participation.LegalPerson;

			if (person.IsNotNull ())
			{
				person.RestoreParticipationInternal (participation);
			}

			if (legalPerson.IsNotNull ())
			{
				legalPerson.RestoreParticipationInternal (participation);
			}
		}

		private static void StopParticipationInternal(AiderGroupParticipantEntity participation)
		{
			var person = participation.Person;
			var legalPerson = participation.LegalPerson;

			if (person.IsNotNull ())
			{
				person.RemoveParticipationInternal (participation);
			}

			if (legalPerson.IsNotNull ())
			{
				legalPerson.RemoveParticipationInternal (participation);
			}
		}
		
		
		private void Assign(ParticipationData participationData)
		{
			this.Person = participationData.Person;
			this.LegalPerson = participationData.LegalPerson;
			this.Contact = participationData.Contact;
		}
	}
}
