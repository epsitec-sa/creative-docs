//	Copyright © 2006-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		/// <param name="bindingExpression">The binding that is using this instance.</param>
		public BindingAsyncOperation(BindingExpression bindingExpression)
		{
			this.bindingExpression = bindingExpression;
			this.binding = this.bindingExpression.ParentBinding;
		}

		public BindingAsyncOperation(Binding binding, BindingExpression[] expressions)
		{
			this.binding = binding;
			this.expressions = expressions;
		}

		/// <summary>
		/// Asynchronously queries the source value and then updates the target.
		/// This method call returns immediately.
		/// </summary>
		public void QuerySourceValueAndUpdateTarget()
		{
			lock (this.exclusion)
			{
				if (this.asyncWork == null)
				{
					this.asyncWork = this.AsyncUpdate;
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
		/// Asynchronously attaches to the source and updates all targets. This
		/// method call returns immediately.
		/// </summary>
		public void AttachToSourceAndUpdateTargets()
		{
			System.Diagnostics.Debug.Assert (this.bindingExpression == null);
			System.Diagnostics.Debug.Assert (this.asyncWork == null);
			System.Diagnostics.Debug.Assert (this.asyncResult == null);

			lock (this.exclusion)
			{
				this.asyncWork   = this.AsyncAttach;
				this.asyncResult = this.asyncWork.BeginInvoke (this.NotifyAsyncAttachDone, null);
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

		private void AsyncUpdate()
		{
			bool work = true;

			while (work)
			{
				lock (this.exclusion)
				{
					this.restartRequested = false;
				}

				BindingAsyncOperation.GetSourceAndUpdateTarget (this.bindingExpression);

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

		private void AsyncAttach()
		{
			foreach (BindingExpression expression in this.expressions)
			{
				BindingAsyncOperation.AttachToSource (expression);
				BindingAsyncOperation.GetSourceAndUpdateTarget (expression);
			}

			this.binding.NotifyAttachCompleted ();
		}

		private void NotifyAsyncAttachDone(System.IAsyncResult result)
		{
			lock (this.exclusion)
			{
				this.asyncWork.EndInvoke (result);

				this.asyncWork   = null;
				this.asyncResult = null;
			}
		}

		private static void AttachToSource(BindingExpression expression)
		{
			expression.AttachToSource ();
		}

		private static void GetSourceAndUpdateTarget(BindingExpression expression)
		{
			//	The slow operation is here: query the binding expression
			//	to get the source value.
			if (expression.DataSourceType != DataSourceType.None)
			{
				object value = expression.GetSourceValue ();

				//	Update the target value; this must be done on the same thread
				//	than that of the UI of the application, or else WinForms won't
				//	be happy.

				if (BindingAsyncOperation.applicationThreadInvoker == null)
				{
					expression.InternalUpdateTarget (value);
				}
				else
				{
					BindingAsyncOperation.applicationThreadInvoker.Invoke
					(
						delegate ()
						{
							expression.InternalUpdateTarget (value);
						}
					);
				}
			}
		}

		private static IApplicationThreadInvoker	applicationThreadInvoker;
		
		private BindingExpression				bindingExpression;
		private Binding							binding;
		private BindingExpression[]				expressions;
		
		private Support.SimpleCallback			asyncWork;
		private System.IAsyncResult				asyncResult;
		private object							exclusion = new object ();
		private volatile bool					restartRequested;
		private volatile bool					cleanupPending;
	}
}
