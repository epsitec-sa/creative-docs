//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		protected abstract IEnumerable<BindingExpression> GetExpressions();

		private void BeginDefer()
		{
			System.Threading.Interlocked.Increment (ref this.deferCounter);
		}

		private void EndDefer()
		{
			if (System.Threading.Interlocked.Decrement (ref this.deferCounter) == 0)
			{
				this.AttachAfterChanges ();
			}
		}
		
		#region Private DeferManager Class
		
		private sealed class DeferManager : System.IDisposable
		{
			public DeferManager(AbstractBinding binding)
			{
				this.binding = binding;
				this.binding.BeginDefer ();
			}

			#region IDisposable Members
			
			void System.IDisposable.Dispose()
			{
				if (this.binding != null)
				{
					AbstractBinding binding = this.binding;
					this.binding = null;
					binding.EndDefer ();
				}
			}
			
#endregion

			private AbstractBinding binding;
		}
		
#endregion

		private int deferCounter;
	}
}
