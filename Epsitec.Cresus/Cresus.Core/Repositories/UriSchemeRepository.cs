//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Repositories
{
	public class UriSchemeRepository : Repository<UriSchemeEntity>
	{
		public UriSchemeRepository(CoreData data, DataContext context = null)
			: base (data, context)
		{
		}


		public IEnumerable<UriSchemeEntity> GetByCode(string code)
		{
			var example = this.CreateExample ();
			example.Code = code;
			
			return this.GetByExample (example);
		}

		public IEnumerable<UriSchemeEntity> GetByName(FormattedText name)
		{
			var example = this.CreateExample ();
			example.Name = name;

			return this.GetByExample (example);
		}
	}
}
