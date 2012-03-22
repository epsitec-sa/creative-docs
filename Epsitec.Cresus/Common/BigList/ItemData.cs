//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public abstract class ItemData
	{
		protected ItemData(ItemState state)
		{
			this.state = state;
		}

		public abstract TData GetData<TData>();

		public TState CreateState<TState>()
			where TState : ItemState, new ()
		{
			var state = new TState ();

			state.CopyFrom (this.state);
			
			return state;
		}


		protected ItemState state;
	}
}
