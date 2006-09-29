using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class BindingAsyncOperation
	{
		public BindingAsyncOperation(BindingExpression binding)
		{
			this.binding = binding;
		}

		internal void QuerySourceValueAndUpdateTarget()
		{
			lock (this.exclusion)
			{
				if (this.callback == null)
				{
					this.callback = this.AsyncWork;
				}
				
				if (this.asyncResult == null)
				{
					this.asyncResult = this.callback.BeginInvoke (null, null);
				}
				else if (this.cleanupPending)
				{
					this.callback.EndInvoke (this.asyncResult);
					
					this.cleanupPending = false;
					this.asyncResult    = this.callback.BeginInvoke (null, null);
				}
				else
				{
					this.restartRequested = true;
				}
			}
		}

		private void AsyncWork()
		{
			bool work = true;

			while (work)
			{
				lock (this.exclusion)
				{
					this.restartRequested = false;
				}
				
				object value = this.binding.GetSourceValue ();

				if (BindingAsyncOperation.applicationThreadInvoker == null)
				{
					this.binding.InternalUpdateTarget (value);
				}
				else
				{
					BindingAsyncOperation.applicationThreadInvoker.Invoke
					(
						delegate ()
						{
							this.binding.InternalUpdateTarget (value);
						}
					);
				}
				
				lock (this.exclusion)
				{
					if (this.restartRequested == false)
					{
						work = false;
						this.cleanupPending = true;
					}
				}
			}
		}

		public interface IApplicationThreadInvoker
		{
			void Invoke(Support.SimpleCallback method);
		}

		public static void DefineApplicationThreadInvoker(IApplicationThreadInvoker value)
		{
			BindingAsyncOperation.applicationThreadInvoker = value;
		}

		private static IApplicationThreadInvoker applicationThreadInvoker;
		
		private Support.SimpleCallback callback;
		private System.IAsyncResult asyncResult;
		private object exclusion = new object ();
		private volatile bool restartRequested;
		private volatile bool cleanupPending;
		private BindingExpression binding;
		private BindingAsyncOperation expression;
	}
}
