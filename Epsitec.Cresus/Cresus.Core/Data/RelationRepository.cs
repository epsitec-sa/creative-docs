using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Browser;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class RelationRepository : Repository
	{


		public RelationRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<RelationEntity> GetRelationsByExample(RelationEntity example)
		{
			return this.GetEntitiesByExample<RelationEntity> (example);
		}


		public IEnumerable<RelationEntity> GetRelationsByRequest(Request request)
		{
			return this.GetEntitiesByRequest<RelationEntity> (request);
		}


		public IEnumerable<RelationEntity> GetRelationsByExample(RelationEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<RelationEntity> (example, index, count);
		}


		public IEnumerable<RelationEntity> GetRelationsByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<RelationEntity> (request, index, count);
		}


		public IEnumerable<RelationEntity> GetAllRelations()
		{
			RelationEntity example = this.CreateRelationExample ();

			return this.GetRelationsByExample (example);
		}


		public IEnumerable<RelationEntity> GetAllRelations(int index, int count)
		{
			RelationEntity example = this.CreateRelationExample ();

			return this.GetRelationsByExample (example, index, count);
		}


		public RelationEntity CreateRelationExample()
		{
			return this.CreateExample<RelationEntity> ();
		}


	}


}
