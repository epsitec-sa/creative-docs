//	Copyright � 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;

using System;

using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupParticipantEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.StartDate, "�", TextFormatter.Command.IfEmpty, "�", this.EndDate, "�", TextFormatter.Command.IfEmpty);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.StartDate, "�", TextFormatter.Command.IfEmpty, "�", this.EndDate, "�", TextFormatter.Command.IfEmpty);
		}

		public FormattedText GetSummaryWithHierarchicalGroupName()
		{
			var name = this.Group.GetHierarchicalName (this.Person);

			return TextFormatter.FormatText (name);
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield break;
		}

		
		public void Delete(BusinessContext businessContext)
		{
			if (this.Comment.IsNotNull ())
			{
				businessContext.DeleteEntity (this.Comment);
			}
			
			businessContext.DeleteEntity (this);
		}


		public static AiderGroupParticipantEntity StartParticipation(BusinessContext businessContext, Participation what)
		{
			return AiderGroupParticipantEntity.StartParticipation (businessContext, what, null);
		}

		public static AiderGroupParticipantEntity StartParticipation(BusinessContext businessContext, Participation what, Date? startDate)
		{
			if (!what.Group.CanHaveMembers ())
			{
				throw new InvalidOperationException ("This group cannot have members.");
			}

			var participation = businessContext.CreateAndRegisterEntity<AiderGroupParticipantEntity> ();

			participation.Assign (what);
			participation.StartDate = startDate;

			if (!startDate.HasValue || startDate <= Date.Today)
			{
				what.Person.AddParticipationInternal (participation);
			}

			return participation;
		}

		public static AiderGroupParticipantEntity StartParticipation(BusinessContext businessContext, Participation what, Date? startDate, FormattedText comment)
		{
			var participation = AiderGroupParticipantEntity.StartParticipation (businessContext, what, startDate);

			if (comment.IsNullOrEmpty () == false)
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


		public static AiderGroupParticipantEntity ImportParticipation(BusinessContext businessContext, Participation what, Date? startDate, Date? endDate, FormattedText comment)
		{
			var participation = AiderGroupParticipantEntity.StartParticipation (businessContext, what, startDate, comment);

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

		public static Request CreateFunctionMemberRequest(DataContext dataContext, AiderGroupEntity group, bool current, bool sort)
		{
			var personExample = new AiderPersonEntity ();
			var participationExample = new AiderGroupParticipantEntity ()
			{
				Person = personExample
			};

			var request = new Request ()
			{
				RootEntity = participationExample,
				RequestedEntity = personExample,
			};

			AiderGroupParticipantEntity.AddFunctionMemberCondition (dataContext, request, participationExample, group);

			if (current)
			{
				AiderGroupParticipantEntity.AddCurrentCondition (dataContext, request, participationExample);
			}

			if (sort)
			{
				request.AddSortClause (ValueField.Create (participationExample.Person, p => p.DisplayName), SortOrder.Ascending);
			}

			return request;
		}


		public static void AddCurrentCondition(DataContext dataContext, Request request, AiderGroupParticipantEntity participation)
		{
			request.AddCondition (dataContext, participation, g => g.StartDate == null || g.StartDate <= Date.Today);
			request.AddCondition (dataContext, participation, g => g.EndDate == null || g.EndDate > Date.Today);
		}

		public static void AddFunctionMemberCondition(DataContext dataContext, Request request, AiderGroupParticipantEntity participation, AiderGroupEntity group)
		{
			if (participation.Group == null)
			{
				participation.Group = new AiderGroupEntity ();
			}

			// Here we don't use a like clause on the path because it is too slow when there are a
			// lot of participations in the database.
			var functions = group.GroupDef.Function.Subgroups;
			var paths = functions
				.Select (f => AiderGroupIds.CreateSubgroupPathFromFullPath (group.Path, f.PathTemplate))
				.ToList ();

			request.AddCondition
			(
				dataContext,
				participation,
				x => SqlMethods.IsInSet (x.Group.Path, paths)
			);

			request.Distinct = true;
		}
		
		
		private void Assign(Participation what)
		{
			this.Group       = what.Group;
			this.Person      = what.Person;
			this.LegalPerson = what.LegalPerson;
			this.Contact     = what.Contact;
		}
	}
}
