//	Copyright � 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Data;
using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Entities.Helpers;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return this.Name;
		}

		public static AiderGroupEntity Create(BusinessContext businessContext, AiderGroupDefEntity groupDefinition,
			/**/							  GroupPathInfo info)
		{
			var name = info.Name;
			var level = info.Level;
			var path = info.MapPath (groupDefinition);
			
			return AiderGroupEntity.Create (businessContext, groupDefinition, name, level, path);
		}

		public static AiderGroupEntity Create(BusinessContext businessContext, AiderGroupDefEntity groupDefinition, string name, int level, string path)
		{
			var group = businessContext.CreateAndRegisterEntity<AiderGroupEntity> ();

			group.Name = name;
			group.GroupLevel = level;
			group.Path = path;
			group.GroupDef = groupDefinition;

			return group;
		}


		public static AiderGroupEntity CreateSubGroup(BusinessContext businessContext, AiderGroupEntity superGroup, string name, int subGroupNumber)
		{
			if (subGroupNumber > 99)
			{
				throw new NotSupportedException ("Groups cannot have more that 99 children.");
			}

			var subGroup = businessContext.CreateAndRegisterEntity<AiderGroupEntity> ();

			subGroup.Name = name;
			subGroup.GroupLevel = superGroup.GroupLevel + 1;
			subGroup.Path = AiderGroupIds.CreateSubGroupPath (superGroup.Path, subGroupNumber);

			return subGroup;
		}


		public IEnumerable<AiderGroupEntity> FindSubgroups(BusinessContext businessContext)
		{
			// If we are at the maximum group level, there's no point in looking for sub groups in
			// the database. We won't find any. So we return directly an empty sequence. Note that
			// this optimization avoids to make a request with a group path longer than the maximum
			// group path which Firebird don't like at all. It considers such requests invalid.

			if (this.GroupLevel >= AiderGroupEntity.maxGroupLevel)
			{
				return Enumerable.Empty<AiderGroupEntity> ();
			}
			
			var dataContext = businessContext.DataContext;

			var example = new AiderGroupEntity ();
			var request = Request.Create (example);

			var path  = this.Path + AiderGroupIds.SubgroupSqlWildcard;
			var level = this.GroupLevel + 1;

			request.AddCondition (dataContext, example, x => x.GroupLevel == level && SqlMethods.Like (x.Path, path));
			request.AddSortClause (ValueField.Create (example, x => x.Path));

			return dataContext.GetByRequest (request);
		}

		public IEnumerable<AiderGroupParticipantEntity> FindParticipants(BusinessContext businessContext)
		{
			var example = new AiderGroupParticipantEntity ();
			var request = Request.Create (example);

			example.Group = this;

			return businessContext.DataContext.GetByRequest (request);
		}

		public AiderGroupParticipantEntity AddParticipant(BusinessContext businessContext, AiderPersonEntity aiderPerson, Date? startDate, Date? endDate, string comment)
		{
			var aiderGroupParticipant = businessContext.CreateAndRegisterEntity<AiderGroupParticipantEntity> ();

			aiderGroupParticipant.Group = this;
			aiderGroupParticipant.Person = aiderPerson;

			aiderGroupParticipant.StartDate = startDate;
			aiderGroupParticipant.EndDate = endDate;

			if (!string.IsNullOrWhiteSpace (comment))
			{
				var aiderComment = businessContext.CreateAndRegisterEntity<AiderCommentEntity> ();

				aiderComment.Text = TextFormatter.FormatText (comment);

				aiderGroupParticipant.Comment = aiderComment;
			}

			return aiderGroupParticipant;
		}


		public static IList<AiderGroupEntity> FindRootGroups(BusinessContext businessContext)
		{
			var example = new AiderGroupEntity ()
			{
				GroupLevel = 0,
			};

			var request = Request.Create (example);
			request.AddSortClause (ValueField.Create (example, x => x.Path));

			return businessContext.DataContext.GetByRequest (request);
		}


		public static AiderGroupEntity FindParishGroup(BusinessContext businessContext, string parishName)
		{
			// Here I don't use a simple request by example, because the parish names that are in
			// the database have their multiple parts separated by " � " such as in "Saint-Fran�ois
			// � Saint-Jacques". The name that we might get in the file is likely to be "Saint-
			// Fran�ois-Saint-Jacques". We have two problems here, the spaces around the "�" and the
			// fact that "�" is not "-". Look closer if you don't trust me. Yeah, you can bet I lost
			// a lot of time on this one :-P Anyway, in this case, there is no way we can know that
			// we would have to convert the second "-" separating the parish names but not the first
			// and the third one that are part of the parish names. So it's easier to use a request
			// with like and that's what we do here.

			// TODO Add something more on the condition, such as the group type def or the group
			// root, etc.

			var example = new AiderGroupEntity ();

			Request request = new Request ()
			{
				RootEntity = example,
				RequestedEntity = example,
			};

			var rootGroupName = AiderGroupEntity.GetParishGroupName(parishName);
			var rootGroupNamePattern = rootGroupName.Replace("-", "%");

			request.Conditions.Add (
				new BinaryComparison (
					new ValueField (example, new Druid ("[LVAA4]")),
					BinaryComparator.IsLike,
					new Constant (rootGroupNamePattern)
				)
			);

			return businessContext.DataContext
				.GetByRequest<AiderGroupEntity> (request)
				.FirstOrDefault ();
		}


		public static AiderGroupEntity FindRegionGroup(BusinessContext businessContext, int regionNumber)
		{
			// TODO Add something more on the condition, such as the group type def or the group
			// root, etc.

			var example = new AiderGroupEntity ()
			{
				Name = AiderGroupEntity.GetRegionGroupName (regionNumber),
			};

			return businessContext.DataContext.GetByExample (example).FirstOrDefault ();
		}


		public static string GetRegionGroupName(int regionNumber)
		{
			return "R�gion " + InvariantConverter.ToString (regionNumber);
		}

		public static string GetParishGroupName(string parishName)
		{
			return "Paroisse de " + parishName;
		}


		public IEnumerable<AiderGroupEntity> GetGroupChain(BusinessContext businessContext)
		{
			foreach (var path in AiderGroupIds.GetGroupChainPaths (this.Path))
			{
				var example = new AiderGroupEntity ()
				{
					Path = path
				};

				yield return businessContext.DataContext.GetByExample (example).Single ();
			}
		}


		partial void GetSubgroups(ref IList<AiderGroupEntity> value)
		{
			if (this.subgroupsList == null)
			{
				var context = BusinessContextPool.GetCurrentContext (this);

				this.subgroupsList = new List<AiderGroupEntity> ();
				this.subgroupsList.AddRange (this.FindSubgroups (context));
			}

			value = this.subgroupsList;
		}

		partial void GetParticipants(ref IList<AiderPersonEntity> value)
		{
			if (this.participantsList == null)
			{
				this.participantsList = new AiderGroupPersonList (this);
			}

			value = this.participantsList;
		}


		public FormattedText GetParticipantsTitle()
		{
			var nbParticipants = this.GetNbParticipants ();

			return TextFormatter.FormatText ("Particpants (", nbParticipants, ")");
		}


		public FormattedText GetParticipantsSummary()
		{
			int count = 10;

			var groups = this.GetParticipants (count + 1)
				.Select (g => g.GetCompactSummary ())
				.CreateSummarySequence (count, "...");

			return FormattedText.Join (FormattedText.FromSimpleText ("\n"), groups);
		}


		private int GetNbParticipants()
		{
			var businessContext = BusinessContextPool.GetCurrentContext (this);
			var dataContext = businessContext.DataContext;

			var request = AiderGroupEntity.CreateParticipantRequest (dataContext, this, false);

			return dataContext.GetCount (request);
		}


		private IEnumerable<AiderPersonEntity> GetParticipants(int count)
		{
			var businessContext = BusinessContextPool.GetCurrentContext (this);
			var dataContext = businessContext.DataContext;

			var request = AiderGroupEntity.CreateParticipantRequest (dataContext, this, true);
			request.Skip = 0;
			request.Take = count;

			return dataContext.GetByRequest<AiderPersonEntity> (request);
		}


		public static Request CreateParticipantRequest(DataContext dataContext, AiderGroupEntity group, bool sort)
		{
			var echPerson = new eCH_PersonEntity ();

			var aiderPerson = new AiderPersonEntity ()
			{
				eCH_Person = echPerson
			};

			var participation = new AiderGroupParticipantEntity ()
			{
				Person = aiderPerson
			};

			var request = new Request ()
			{
				RootEntity = participation,
				RequestedEntity = aiderPerson,
			};

			request.AddCondition (dataContext, participation, g => g.Group == group);
			request.AddCondition (dataContext, participation, g => g.StartDate == null || g.StartDate <= Date.Today);
			request.AddCondition (dataContext, participation, g => g.EndDate == null || g.EndDate > Date.Today);

			if (sort)
			{
				request.AddSortClause (ValueField.Create (echPerson, p => p.PersonOfficialName), SortOrder.Ascending);
				request.AddSortClause (ValueField.Create (echPerson, p => p.PersonFirstNames), SortOrder.Ascending);
			}

			return request;
		}


		private List<AiderGroupEntity> subgroupsList;
		private AiderGroupPersonList participantsList;

		private static readonly int maxGroupLevel = 6;

	}
}
