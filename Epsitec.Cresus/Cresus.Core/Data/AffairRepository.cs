//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Data
{
	public class AffairRepository : Repository<AffairEntity>
	{
		public AffairRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<AffairEntity> GetAffairsByExample(AffairEntity example)
		{
			return this.GetEntitiesByExample<AffairEntity> (example);
		}


		public IEnumerable<AffairEntity> GetAffairsByRequest(Request request)
		{
			return this.GetEntitiesByRequest<AffairEntity> (request);
		}


		public IEnumerable<AffairEntity> GetAffairsByExample(AffairEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<AffairEntity> (example, index, count);
		}


		public IEnumerable<AffairEntity> GetAffairsByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<AffairEntity> (request, index, count);
		}


		public IEnumerable<AffairEntity> GetAllAffairs()
		{
			AffairEntity example = this.CreateAffairExample ();

			return this.GetAffairsByExample (example);
		}


		public IEnumerable<AffairEntity> GetAllAffairs(int index, int count)
		{
			AffairEntity example = this.CreateAffairExample ();

			return this.GetAffairsByExample (example, index, count);
		}



		public AffairEntity CreateAffairExample()
		{
			return this.CreateExample<AffairEntity> ();
		}
	}
}
