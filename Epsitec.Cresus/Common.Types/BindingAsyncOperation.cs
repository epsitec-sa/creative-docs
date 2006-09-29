//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>BindingAsyncOperation</c> class manages the asynchronous update
	/// of a target based on a slow data source.
	/// </summary>
	public sealed class BindingAsyncOperation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BindingAsyncOperation"/> class.
		/// </summary>
		/// <param name="binding">The binding that is using this instance.</param>
		public BindingAsyncOperation(BindingExpression binding)
		{
			this.binding = binding;
		}

		/// <summary>
		/// Asynchronously queries the source value and then updates the target.
		/// This method call return immediately.
		/// </summary>
		public void QuerySourceValueAndUpdateTarget()
		{
			lock (this.exclusion)
			{
				if (this.asyncWork == null)
				{
					this.asyncWork = this.AsyncWork;
				}
				
				if (this.asyncResult == null)
				{
					this.asyncResult = this.asyncWork.BeginInvoke (null, null);
				}
				else if (this.cleanupPending)
				{
					//	We can't simply overwrite the IAsyncResult value since this
					//	could lead to uncollected garbage, it seems. So we make sure
					//	we follow the recommendations and call EndInvoke.
					
					this.asyncWork.EndInvoke (this.asyncResult);
					
					this.cleanupPending = false;
					this.asyncResult    = this.asyncWork.BeginInvoke (null, null);
				}
				else
				{
					//	The asynchronous thread is still working, just tell it to
					//	restart before it returns.
					
					this.restartRequested = true;
				}
			}
		}

		/// <summary>
		/// Defines the application thread invoker required to excute methods
		/// on the UI main thread.
		/// </summary>
		/// <param name="value">The application thread invoker.</param>
		public static void DefineApplicationThreadInvoker(IApplicationThreadInvoker value)
		{
			BindingAsyncOperation.applicationThreadInvoker = value;
		}

		#region IApplicationThreadInvoker Interface

		public interface IApplicationThreadInvoker
		{
			void Invoke(Support.SimpleCallback method);
		}

		#endregion

		private void AsyncWork()
		{
			bool work = true;

			while (work)
			{
				lock (this.exclusion)
				{
					this.restartRequested = false;
				}

				//	The slow operation is here: query the binding expression
				//	to get the source value.

				object value = this.binding.GetSourceValue ();

				//	Update the target value; this must be done on the same thread
				//	than that of the UI of the application, or else WinForms won't
				//	be happy.

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

				//	We are done, but maybe someone already asked us for a new
				//	update of the value in the meantime. Just restart the loop
				//	in that case.

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

		static IApplicationThreadInvoker		applicationThreadInvoker;
		
		private BindingExpression				binding;
		
		private Support.SimpleCallback			asyncWork;
		private System.IAsyncResult				asyncResult;
		private object							exclusion = new object ();
		private volatile bool					restartRequested;
		private volatile bool					cleanupPending;
	}
}
