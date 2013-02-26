//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Eerv;

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

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
			return this.Instantiate (businessContext, null, info);
		}

		public AiderGroupEntity Instantiate(BusinessContext businessContext, AiderGroupEntity parent, GroupPathInfo info)
		{
			var name = info.Name;
			var level = info.Level;
			var path = info.MapPath (this);

			var group = AiderGroupEntity.Create (businessContext, parent, this, name, level, path);

			foreach (var subGroupDef in this.Subgroups)
			{
				var subInfo = new GroupPathInfo
				(
					name: subGroupDef.Name,
					template: subGroupDef.PathTemplate,
					output: group.Path + subGroupDef.PathTemplate.SubstringEnd (4),
					level: info.Level + 1
				);
				
				subGroupDef.Instantiate (businessContext, group, subInfo);
			}

			if (this.Function.IsNotNull ())
			{
				foreach (var functionGroupDef in this.Function.Subgroups)
				{
					var functionGroup = group.CreateSubgroup (businessContext, functionGroupDef.Name);
					functionGroup.GroupDef = functionGroupDef;
				}
			}

			return group;
		}
	}
}
