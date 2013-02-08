//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;

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

		public FormattedText GetSummaryWithGroupName()
		{
			return TextFormatter.FormatText (this.Group.Name);
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield break;
		}

		public static AiderGroupParticipantEntity StartParticipation(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity group, Date? startDate)
		{
			var participation = businessContext.CreateAndRegisterEntity<AiderGroupParticipantEntity> ();

			participation.Person = person;
			participation.Group = group;
			participation.StartDate = startDate;

			if (!startDate.HasValue || startDate <= Date.Today)
			{
				person.AddParticipationInternal (participation);
			}

			return participation;
		}

		public static AiderGroupParticipantEntity StartParticipation(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity group, Date? startDate, FormattedText comment)
		{
			var participation = AiderGroupParticipantEntity.StartParticipation (businessContext, person, group, startDate);

			if (comment.Length > 0)
			{
				participation.Comment.Text = comment;
			}

			return participation;
		}

		public static void StopParticipation(AiderGroupParticipantEntity participation, Date endDate)
		{
			participation.EndDate = endDate;

			if (endDate <= Date.Today)
			{
				participation.Person.RemoveParticipationInternal (participation);
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


		public static AiderGroupParticipantEntity ImportParticipation(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity group, Date? startDate, Date? endDate, FormattedText comment)
		{
			var participation = AiderGroupParticipantEntity.StartParticipation (businessContext, person, group, startDate, comment);

			if (endDate.HasValue)
			{
				AiderGroupParticipantEntity.StopParticipation (participation, endDate.Value);
			}

			return participation;
		}

		public static Request CreateParticipantRequest(DataContext dataContext, AiderGroupEntity group, bool sort, bool current, bool returnPersons)
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

			if (returnPersons)
			{
				request.RequestedEntity = participation.Person;
			}

			request.AddCondition (dataContext, participation, g => g.Group == group);

			if (current)
			{
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);
			}

			if (sort)
			{
				request.AddSortClause (ValueField.Create (participation.Person, p => p.DisplayName), SortOrder.Ascending);
			}

			return request;
		}


		public static Request CreateParticipantRequest(DataContext dataContext, AiderPersonEntity person, bool sort, bool current, bool returnGroups)
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

			if (returnGroups)
			{
				request.RequestedEntity = participation.Group;
			}

			request.AddCondition (dataContext, participation, g => g.Person == person);

			if (current)
			{
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participation);
			}

			if (sort)
			{
				request.AddSortClause (ValueField.Create (participation.Group, g => g.Name), SortOrder.Ascending);
			}

			return request;
		}


		public static Request CreateParticipantRequest(DataContext dataContext, AiderPersonEntity person, string path)
		{
			var example = new AiderGroupParticipantEntity ()
			{
				Person = person,
				Group = new AiderGroupEntity ()
				{
					Path = path
				},
			};

			var request = new Request ()
			{
				RootEntity = example
			};

			AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, example);

			return request;
		}


		public static void AddCurrentCondition(DataContext dataContext, Request request, AiderGroupParticipantEntity participation)
		{
			request.AddCondition (dataContext, participation, g => g.StartDate == null || g.StartDate <= Date.Today);
			request.AddCondition (dataContext, participation, g => g.EndDate == null || g.EndDate > Date.Today);
		}
	}
}
