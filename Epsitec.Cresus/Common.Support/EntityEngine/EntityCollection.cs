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
		public EntityCollection(string id, AbstractEntity container)
		{
			this.id = id;
			this.container = container;
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
			switch (this.state)
			{
				case State.CopyOnWrite:
					return this.CreateCopyOnWrite ();

				case State.Copied:
					throw new System.InvalidOperationException ("Copied collection may not be changed");

				case State.Default:
					return this;

				default:
					throw new System.NotImplementedException ();
			}
		}

		private EntityCollection<T> CreateCopyOnWrite()
		{
			this.state = State.Copied;
			return this.container.CopyFieldCollection<T> (id, this);
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
