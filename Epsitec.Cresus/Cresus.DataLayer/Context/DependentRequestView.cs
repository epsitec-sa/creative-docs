using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Context
{


	public sealed class DependentRequestView : AbstractRequestView
	{


		internal DependentRequestView(DataContext dataContext, Request request)
			: base (dataContext, request)
		{
		}


		public override IList<AbstractEntity> GetEntities(Request request)
		{
			return this.DataLoader.GetByRequest (request);
		}


		public override IList<EntityKey> GetKeys(Request request)
		{
			return this.GetEntities (request)
				.Select (e => this.DataContext.GetNormalizedEntityKey (e).Value)
				.ToList ();
		}


		public override int? GetIndex(Request request, EntityKey entityKey)
		{
			return this.DataLoader.GetIndex (request, entityKey);
		}


		public override int GetCount(Request request)
		{
			return this.DataLoader.GetCount (request);
		}


	}


}
