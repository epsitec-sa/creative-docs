//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name, "\n", this.Description);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return this.Name;
			yield return this.Description;
		}

		public static AiderGroupEntity Create(BusinessContext businessContext, AiderGroupDefEntity groupDefinition, string name)
		{
			var group = businessContext.CreateEntity<AiderGroupEntity> ();

			group.Name = name;

			if (groupDefinition != null)
			{
				group.GroupDef = groupDefinition;
			}

			return group;
		}

		public IEnumerable<AiderGroupEntity> FindSubGroups(BusinessContext businessContext)
		{
			var example = new AiderGroupRelationshipEntity ()
			{
				Group1 = this,
				Type = GroupRelationshipType.Inclusion,
			};

			return businessContext.DataContext.GetByExample(example).Select(r => r.Group2);
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
	}
}
