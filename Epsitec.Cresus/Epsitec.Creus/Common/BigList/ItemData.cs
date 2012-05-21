//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	/// <summary>
	/// The <c>ItemData</c> class is an abstract base class for <see cref="ItemData&lt;T&gt;"/>.
	/// It is used to cache data which is displayed in a big list, and store its state.
	/// </summary>
	public abstract class ItemData
	{
		protected ItemData(ItemState state)
		{
			this.state = state;
		}

		/// <summary>
		/// Gets the data stored in this instance.
		/// </summary>
		/// <exception cref="System.ArgumentException">When the <typeparamref name="TData"/> does
		/// not match the real data type.</exception>
		/// <typeparam name="TData">The type of the data.</typeparam>
		/// <returns>The native data.</returns>
		public abstract TData GetData<TData>();

		public TState CreateState<TState>()
			where TState : ItemState, new ()
		{
			return this.InitializeState (new TState ()) as TState;
		}


		protected virtual ItemState InitializeState(ItemState state)
		{
			state.CopyFrom (this.state);
			
			return state;
		}
		
		protected ItemState state;
	}
}
