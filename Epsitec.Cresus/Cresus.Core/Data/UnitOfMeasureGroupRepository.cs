//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;


using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{
	public class UnitOfMeasureGroupRepository : Repository<UnitOfMeasureGroupEntity>
	{
		public UnitOfMeasureGroupRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<UnitOfMeasureGroupEntity> GetUnitOfMeasureGroupByExample(UnitOfMeasureGroupEntity example)
		{
			return this.GetEntitiesByExample<UnitOfMeasureGroupEntity> (example);
		}


		public IEnumerable<UnitOfMeasureGroupEntity> GetUnitOfMeasureGroupByRequest(Request request)
		{
			return this.GetEntitiesByRequest<UnitOfMeasureGroupEntity> (request);
		}


		public IEnumerable<UnitOfMeasureGroupEntity> GetUnitOfMeasureGroupByExample(UnitOfMeasureGroupEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<UnitOfMeasureGroupEntity> (example, index, count);
		}


		public IEnumerable<UnitOfMeasureGroupEntity> GetUnitOfMeasureGroupByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<UnitOfMeasureGroupEntity> (request, index, count);
		}


		public IEnumerable<UnitOfMeasureGroupEntity> GetAllUnitOfMeasureGroup()
		{
			UnitOfMeasureGroupEntity example = this.CreateUnitOfMeasureGroupExample ();

			return this.GetUnitOfMeasureGroupByExample (example);
		}


		public IEnumerable<UnitOfMeasureGroupEntity> GetAllUnitOfMeasureGroup(int index, int count)
		{
			UnitOfMeasureGroupEntity example = this.CreateUnitOfMeasureGroupExample ();

			return this.GetUnitOfMeasureGroupByExample (example, index, count);
		}



		public UnitOfMeasureGroupEntity CreateUnitOfMeasureGroupExample()
		{
			return this.CreateExample<UnitOfMeasureGroupEntity> ();
		}
	}
}
