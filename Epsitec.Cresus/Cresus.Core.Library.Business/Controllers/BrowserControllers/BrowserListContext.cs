//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserListContext
	{
		public BrowserListContext()
		{

		}

		public FormattedText GetDisplayText(AbstractEntity entity)
		{
			if (entity == null)
			{
				return CollectionTemplate.DefaultEmptyText;
			}
			else
			{
				var text = entity.GetCompactSummary ();

				if (text.IsNullOrEmpty)
				{
					return CollectionTemplate.DefaultEmptyText;
				}
				else
				{
					return text;
				}
			}
		}
	}
}
