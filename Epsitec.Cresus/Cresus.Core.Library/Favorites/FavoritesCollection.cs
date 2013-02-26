//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Favorites
{
	/// <summary>
	/// The <c>FavoritesCollection</c> is an abstract class used to represend collection of
	/// entities, identified by their database keys.
	/// </summary>
	public abstract class FavoritesCollection
	{
		public FavoritesCollection(IEnumerable<AbstractEntity> collection, Druid databaseId)
		{
			var buffer = new List<long> ();
			this.databaseId = databaseId;

			if (collection.Any ())
			{
				var entity = collection.First ();
				var context = DataContextPool.GetDataContext (entity);

				buffer.AddRange (collection.Select (e => context.GetNormalizedEntityKey (e).Value.RowKey.Id.Value));
			}

			this.ids = buffer.ToArray ();

			//	Compute a strong hash based on the entity name and on the array of ids. This
			//	will be unique (risk of collision very, very low = zero for our purposes).
			
			var source = Epsitec.Common.IO.BitConverter.ToBytes (this.ids).Concat (System.Text.Encoding.UTF8.GetBytes (this.GetEntityName ()));
			var stream = new Epsitec.Common.IO.ByteStream (this.ids.Length*8, source);

			this.strongHash = Epsitec.Common.IO.Checksum.ComputeMd5Hash (stream);
		}

		public string StrongHash
		{
			get
			{
				return this.strongHash;
			}
		}

		public int Count
		{
			get
			{
				return this.ids.Length;
			}
		}

		public Druid DatabaseId
		{
			get
			{
				return this.databaseId;
			}
		}

		public abstract string GetEntityName();
		
		public abstract AbstractEntity GetExample();

		public abstract Druid GetTypeId();

		public Request GetRequest()
		{
			var example = this.GetExample ();
			var request = new Request ()
			{
				RequestedEntity = example,
				RootEntity = example
			};

			request.Conditions.Add (new ValueSetComparison (InternalField.CreateId (example), SetComparator.In, this.ids.Select (id => new Constant (id))));

			return request;
		}

		public IEnumerable<AbstractEntity> GetByRequest(DataContext dataContext)
		{
			return dataContext.GetByRequest (this.GetRequest ());
		}

		
		private readonly long[]					ids;
		private readonly string					strongHash;
		private readonly Druid					databaseId;
	}
}
