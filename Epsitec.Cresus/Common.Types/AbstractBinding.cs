//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using System.Globalization;

namespace Epsitec.Common.Types
{
	public abstract class AbstractBinding
	{
		protected AbstractBinding()
		{
		}
		
		internal bool Deferred
		{
			get
			{
				return this.deferCounter > 0;
			}
		}

		/// <summary>
		/// Defers the changes; this method should be used in a <c>using</c>
		/// clause when changing several properties of the binding, to
		/// avoid immediate changes.
		/// </summary>
		/// <returns>An <see cref="T:System.IDisposable"/> object which should
		/// be disposed to reactivate the normal change propagation.</returns>
		public System.IDisposable DeferChanges()
		{
			return new DeferManager (this);
		}

		public abstract IEnumerable<BindingExpression> GetExpressions();

		internal void Add(BindingExpression expression)
		{
			//	The binding expression is referenced through a weak binding
			//	by the binding itself, which allows for the expression to be
			//	garbage collected when its property dies.

			this.AddExpression (expression);
		}

		internal void Remove(BindingExpression expression)
		{
			this.RemoveExpression (expression);
		}

		protected void NotifyBeforeChange()
		{
			this.DetachBeforeChanges ();
		}

		protected void NotifyAfterChange()
		{
			if (this.deferCounter == 0)
			{
				this.AttachAfterChanges ();
			}
		}

		protected abstract void DetachBeforeChanges();

		protected abstract void AttachAfterChanges();

		protected abstract void AddExpression(BindingExpression expression);

		protected abstract void RemoveExpression(BindingExpression expression);
		
		#region Private DeferManager Class
		
		private struct DeferManager : System.IDisposable
		{
			public DeferManager(AbstractBinding binding)
			{
				this.binding = binding;
				System.Threading.Interlocked.Increment (ref this.binding.deferCounter);
			}

			#region IDisposable Members
			public void Dispose()
			{
				if (System.Threading.Interlocked.Decrement (ref this.binding.deferCounter) == 0)
				{
					this.binding.AttachAfterChanges ();
				}
			}
			#endregion

			private AbstractBinding binding;
		}
		
#endregion

		private int deferCounter;
	}
}
