//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserListContext
	{
		public BrowserListContext(DataContext dataContext)
		{
			this.dataContext = dataContext;
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


		internal EntityKey GetEntityKey(AbstractEntity entity)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			var key = this.dataContext.GetNormalizedEntityKey (entity);

			if (key == null)
			{
				throw new System.ArgumentException ("Cannot resolve entity");
			}

			return key.Value;
		}



		private readonly DataContext			dataContext;
	}
}
