//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	/// <summary>
	/// The <c>Reservoir</c> class is used to reuse a collection of entities and manage
	/// the creation/deletion if the number of required entities differ from what was
	/// available. This is used to regenerate the VAT lines in a business document, by
	/// first removing all lines (and feeding them to the reservoir), and then pulling
	/// as many lines as needed (and this recycling them, or creating them).
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	class Reservoir<T>
			where T : AbstractEntity, new ()
	{
		public Reservoir(DataContext context, IEnumerable<T> source)
		{
			this.context = context;
			this.pool = new Queue<T> (source);
		}

		public IEnumerable<T> Pool
		{
			get
			{
				return this.pool;
			}
		}

		/// <summary>
		/// Pulls an instance of the expected type <typeparamref name="T"/>. If there is
		/// no more any existing instance available, create a new one.
		/// </summary>
		/// <returns></returns>
		public T Pull()
		{
			if (this.pool.Count > 0)
			{
				return this.pool.Dequeue ();
			}
			else
			{
				return this.context.CreateEntity<T> ();
			}
		}

		/// <summary>
		/// Deletes the unused entities.
		/// </summary>
		public void DeleteUnused()
		{
			while (this.pool.Count > 0)
			{
				this.context.DeleteEntity (this.pool.Dequeue ());
			}
		}


		private readonly Queue<T>				pool;
		private readonly DataContext			context;
	}
}
