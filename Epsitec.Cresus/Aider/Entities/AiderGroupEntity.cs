//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Cresus.Database;

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
			var group = businessContext.CreateEntity<AiderGroupEntity> ();

			group.Name = info.Name;
			group.GroupLevel = info.Level;
			group.Path = info.MapPath (groupDefinition);

			if (groupDefinition != null)
			{
				group.GroupDef = groupDefinition;
			}

			return group;
		}

		public IEnumerable<AiderGroupEntity> FindSubgroups(BusinessContext businessContext)
		{
			var example = new AiderGroupEntity ();
			var request = Request.Create (example);

			var path  = this.Path + EervGroupIds.SubgroupSqlWildcard;
			var level = this.GroupLevel + 1;

			request.AddCondition (example, x => x.GroupLevel == level && SqlMethods.Like (x.Path, path));

			return businessContext.DataContext.GetByRequest (request);
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
			var aiderGroupParticipant = businessContext.CreateEntity<AiderGroupParticipantEntity> ();

			aiderGroupParticipant.Group = this;
			aiderGroupParticipant.Person = aiderPerson;

			aiderGroupParticipant.StartDate = startDate;
			aiderGroupParticipant.EndDate = endDate;

			if (!string.IsNullOrWhiteSpace (comment))
			{
				var aiderComment = businessContext.CreateEntity<AiderCommentEntity> ();

				aiderComment.Text = TextFormatter.FormatText (comment);

				aiderGroupParticipant.Comment = aiderComment;
			}

			return aiderGroupParticipant;
		}


		public static AiderGroupEntity FindParishGroup(BusinessContext businessContext, string parishName)
		{
			// Here I don't use a simple request by example, because the parish names that are in
			// the database have their multiple parts separated by " – " such as in "Saint-François
			// – Saint-Jacques". The name that we might get in the file is likely to be "Saint-
			// François-Saint-Jacques". We have two problems here, the spaces around the "–" and the
			// fact that "–" is not "-". Look closer if you don't trust me. Yeah, you can bet I lost
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
			return "Région " + InvariantConverter.ToString (regionNumber);
		}

		public static string GetParishGroupName(string parishName)
		{
			return "Paroisse de " + parishName;
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

		partial void GetParticipants(ref IList<AiderGroupParticipantEntity> value)
		{
			if (this.participantsList == null)
			{
				var context = BusinessContextPool.GetCurrentContext (this);

				this.participantsList = new List<AiderGroupParticipantEntity> ();
				this.participantsList.AddRange (this.FindParticipants (context));
			}

			value = this.participantsList;
		}

		private List<AiderGroupEntity> subgroupsList;
		private List<AiderGroupParticipantEntity> participantsList;

	}
}
