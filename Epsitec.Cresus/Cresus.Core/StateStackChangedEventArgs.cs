//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	public sealed class StateStackChangedEventArgs : System.EventArgs
	{
		public StateStackChangedEventArgs(StateStackChange change, States.AbstractState state)
		{
			this.change = change;
			this.state  = state;
		}


		public StateStackChange Change
		{
			get
			{
				return this.change;
			}
		}

		public States.AbstractState State
		{
			get
			{
				return this.state;
			}
		}

		
		private readonly StateStackChange change;
		private readonly States.AbstractState state;
	}
}
