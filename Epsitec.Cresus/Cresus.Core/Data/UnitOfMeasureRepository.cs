//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;


using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{
	public class UnitOfMeasureRepository : Repository
	{
		public UnitOfMeasureRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<UnitOfMeasureEntity> GetUnitOfMeasureByExample(UnitOfMeasureEntity example)
		{
			return this.GetEntitiesByExample<UnitOfMeasureEntity> (example);
		}


		public IEnumerable<UnitOfMeasureEntity> GetUnitOfMeasureByRequest(Request request)
		{
			return this.GetEntitiesByRequest<UnitOfMeasureEntity> (request);
		}


		public IEnumerable<UnitOfMeasureEntity> GetUnitOfMeasureByExample(UnitOfMeasureEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<UnitOfMeasureEntity> (example, index, count);
		}


		public IEnumerable<UnitOfMeasureEntity> GetUnitOfMeasureByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<UnitOfMeasureEntity> (request, index, count);
		}


		public IEnumerable<UnitOfMeasureEntity> GetAllUnitOfMeasure()
		{
			UnitOfMeasureEntity example = this.CreateUnitOfMeasureExample ();

			return this.GetUnitOfMeasureByExample (example);
		}


		public IEnumerable<UnitOfMeasureEntity> GetAllUnitOfMeasure(int index, int count)
		{
			UnitOfMeasureEntity example = this.CreateUnitOfMeasureExample ();

			return this.GetUnitOfMeasureByExample (example, index, count);
		}



		public UnitOfMeasureEntity CreateUnitOfMeasureExample()
		{
			return this.CreateExample<UnitOfMeasureEntity> ();
		}
	}
}
