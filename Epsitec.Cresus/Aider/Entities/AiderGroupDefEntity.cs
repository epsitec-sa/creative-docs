//	Copyright � 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupDefEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name, "\n", this.DefType);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return this.Name;
		}

		public AiderGroupEntity Instantiate(BusinessContext businessContext, string name)
		{
			var group = businessContext.CreateEntity<AiderGroupEntity> ();

			group.Name = name;
			group.GroupDef = this;

			// TODO Add more stuff to the group, such as root, start date, etc.

			foreach (var subGroupDef in this.Subgroups)
			{
				var subGroup = subGroupDef.Instantiate (businessContext, subGroupDef.Name);

				var groupRelationship = businessContext.CreateEntity<AiderGroupRelationshipEntity> ();

				groupRelationship.Group1 = group;
				groupRelationship.Group2 = subGroup;
				groupRelationship.Type = GroupRelationshipType.Inclusion;
			}

			return group;
		}


		public static AiderGroupDefEntity Find(BusinessContext businessContext, string name)
		{
			var example = new AiderGroupDefEntity ()
			{
				Name = name,
			};

			return businessContext.DataContext
				.GetByExample<AiderGroupDefEntity> (example)
				.FirstOrDefault ();
		}
	}
}
