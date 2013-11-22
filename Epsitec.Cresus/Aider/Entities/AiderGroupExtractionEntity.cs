//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupExtractionEntity
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
			yield return TextFormatter.FormatText (this.Name);
		}

		
		public IEnumerable<AiderContactEntity> GetAllContacts(DataContext dataContext)
		{
			var groups   = this.ExecuteSearch (dataContext);
			var contacts = groups.SelectMany (g => g.GetAllGroupAndSubGroupParticipants ());

			System.Diagnostics.Debug.WriteLine ("Groups found: {0}", groups.Count ());
			System.Diagnostics.Debug.WriteLine ("Contacts found: {0}", contacts.Count ());

			return contacts;
		}

		
		private IEnumerable<AiderGroupEntity> ExecuteSearch(DataContext dataContext)
		{
			if (this.Match == GroupExtractionMatch.SameFunction)
			{
				var example = new AiderGroupEntity ();

				example.GroupDef = this.SearchGroup.GroupDef;

				return dataContext.GetByExample (example);
			}
			else
			{
				var path  = this.GetGroupSearchPathPattern () + SqlMethods.TextWildcard;

				var example = new AiderGroupEntity ();
				var request = Request.Create (example);

				request.AddCondition (dataContext, example, x => SqlMethods.Like (x.Path, path));

				return dataContext.GetByRequest (request);
			}
		}

		private string GetGroupSearchPathPattern()
		{
			var path = this.SearchGroup.Path ?? "";
			
			switch (this.Match)
			{
				case GroupExtractionMatch.Path:
					break;

				case GroupExtractionMatch.AnyRegionAnyParish:
					if (AiderGroupIds.IsWithinRegion (path))
					{
						path = AiderGroupIds.ReplaceSubgroupWithWildcard (path, 0);
					}
					if (AiderGroupIds.IsWithinParish (path))
					{
						path = AiderGroupIds.ReplaceSubgroupWithWildcard (path, 1);
					}
					break;

				case GroupExtractionMatch.OneRegionAnyParish:
					if (AiderGroupIds.IsWithinParish (path))
					{
						path = AiderGroupIds.ReplaceSubgroupWithWildcard (path, 1);
					}
					break;

				case GroupExtractionMatch.SameFunction:
					return null;

				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", this.Match.GetQualifiedName ()));
			}

			return path;
		}
	}
}

