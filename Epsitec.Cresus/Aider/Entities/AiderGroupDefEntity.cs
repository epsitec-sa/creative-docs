//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;
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
			return TextFormatter.FormatText (this.Classification, "~, ", this.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return TextFormatter.FormatText (this.Name);
		}

		public AiderGroupEntity Instantiate(BusinessContext businessContext, GroupPathInfo info)
		{
			var group = AiderGroupEntity.Create (businessContext, this, info);

			// TODO Add more stuff to the group, such as root, start date, etc.

			foreach (var subGroupDef in this.Subgroups)
			{
				var subInfo  = new GroupPathInfo (subGroupDef.Name, subGroupDef.PathTemplate, group.Path + subGroupDef.PathTemplate.SubstringEnd (4), info.Level + 1);
				var subGroup = subGroupDef.Instantiate (businessContext, subInfo);
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
