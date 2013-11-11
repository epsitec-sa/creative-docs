//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

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
	public sealed class FavoritesCollection
	{
		public FavoritesCollection(IEnumerable<AbstractEntity> collection, System.Type type, Druid databaseId)
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

			var source = Epsitec.Common.IO.BitConverter.ToBytes (this.ids).Concat (System.Text.Encoding.UTF8.GetBytes (type.FullName));
			var stream = new Epsitec.Common.IO.ByteStream (this.ids.Length*8, source);

			this.strongHash = Epsitec.Common.IO.Checksum.ComputeMd5Hash (stream);
		}

		public string							StrongHash
		{
			get
			{
				return this.strongHash;
			}
		}

		public Druid							DatabaseId
		{
			get
			{
				return this.databaseId;
			}
		}

		
		public DataExpression CreateCondition (AbstractEntity example)
		{
			if (this.ids.Length == 0)
			{
				//	Return an empty set. We could probable use another type of Data Expression
				//	here, which would be more efficient yet, but this does the job, as there is
				//	no entity with a zero ID:

				return new ValueSetComparison (InternalField.CreateId (example), SetComparator.In, new Constant[] { new Constant (0) });
			}
			else
			{
				return new ValueSetComparison (InternalField.CreateId (example), SetComparator.In, this.ids.Select (id => new Constant (id)));
			}
		}

		
		private readonly long[]					ids;
		private readonly string					strongHash;
		private readonly Druid					databaseId;
	}
}
