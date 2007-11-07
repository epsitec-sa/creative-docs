//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	public sealed class EntityCollection<T> : ObservableList<T> where T : AbstractEntity
	{
		public EntityCollection(string id, AbstractEntity container, bool copyOnWrite)
		{
			this.id = id;
			this.container = container;
			this.state = copyOnWrite ? State.CopyOnWrite : State.Default;
		}

		public void SetCopyOnWrite(bool enable)
		{
			if (enable)
			{
				this.state = State.CopyOnWrite;
			}
			else
			{
				this.state = State.Default;
			}
		}

		protected override ObservableList<T> GetWorkingList()
		{
			if (this.container.IsDefiningOriginalValues)
			{
				return this;
			}
			else
			{
				switch (this.state)
				{
					case State.CopyOnWrite:
						this.state = State.Copied;
						return this.container.CopyFieldCollection<T> (this.id, this);

					case State.Copied:
						throw new System.InvalidOperationException ("Copied collection may not be changed");

					case State.Default:
						return this;

					default:
						throw new System.NotImplementedException ();
				}
			}
		}

		private enum State
		{
			Default,
			CopyOnWrite,
			Copied
		}

		private string id;
		private AbstractEntity container;
		private State state;
	}
}
