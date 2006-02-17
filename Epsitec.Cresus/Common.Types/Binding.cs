//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class Binding
	{
		public Binding()
		{
		}
		
		public BindingMode						Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				if (this.mode != value)
				{
					this.InternalDetach ();
					this.mode = value;
					this.InternalAttach ();
				}
			}
		}
		public object							Source
		{
			get
			{
				return this.source;
			}
			set
			{
				if (this.source != value)
				{
					this.InternalDetach ();
					this.source = value;
					this.InternalAttach ();
				}
			}
		}
		public PropertyPath						Path
		{
			get
			{
				return this.path;
			}
			set
			{
				if (this.path != value)
				{
					this.InternalDetach ();
					this.path = value;
					this.InternalAttach ();
				}
			}
		}

		public System.IDisposable DeferChanges()
		{
			return new DeferManager (this);
		}

		private void InternalAttach()
		{
			if (this.deferCounter == 0)
			{
				this.InternalAttachAfterChanges ();
			}
		}
		private void InternalDetach()
		{
			if (this.state == State.Attached)
			{
				//	TODO: détacher de la source
				this.state = State.Detached;
			}
		}

		private void InternalAttachAfterChanges()
		{
			if (this.state == State.Detached)
			{
				//	TODO: attacher à la source
				this.state = State.Attached;
			}
		}

		#region Private DeferManager Class
		private struct DeferManager : System.IDisposable
		{
			public DeferManager(Binding binding)
			{
				this.binding = binding;
				System.Threading.Interlocked.Increment (ref this.binding.deferCounter);
			}

			#region IDisposable Members
			public void Dispose()
			{
				if (System.Threading.Interlocked.Decrement (ref this.binding.deferCounter) == 0)
				{
					this.binding.InternalAttachAfterChanges ();
				}
			}
			#endregion
			
			private Binding						binding;
		}
		#endregion

		#region Private State Enumeration
		private enum State
		{
			Invalid,
			
			Attached,
			Detached
		}
		#endregion

		public static readonly object			DoNothing = new object ();

		private BindingMode						mode;
		private object							source;
		private PropertyPath					path;
		private int								deferCounter;
		private State							state = State.Detached;
	}
}
