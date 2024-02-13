//	Copyright © 2006-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Threading.Tasks;

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
            this.binding = bindingExpression.ParentBinding;
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
            this.asyncTask?.Wait();
            this.asyncTask = Task.Run(
                () => BindingAsyncOperation.GetSourceAndUpdateTarget(this.bindingExpression)
            );
        }

        /// <summary>
        /// Asynchronously attaches to the source and updates all targets. This
        /// method call returns immediately.
        /// </summary>
        public void AttachToSourceAndUpdateTargets()
        {
            System.Diagnostics.Debug.Assert(this.bindingExpression == null); // This means that this method can only be called once per instance
            System.Diagnostics.Debug.Assert(this.asyncTask == null);

            this.asyncTask = Task.Run(() => this.AsyncAttach());
        }

        /// <summary>
        /// Defines the application thread invoker required to execute methods
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

        private void AsyncAttach()
        {
            foreach (BindingExpression expression in this.expressions)
            {
                expression.AttachToSource();
                BindingAsyncOperation.GetSourceAndUpdateTarget(expression);
            }

            this.binding.NotifyAttachCompleted();
        }

        private static void GetSourceAndUpdateTarget(BindingExpression expression)
        {
            //	The slow operation is here: query the binding expression
            //	to get the source value.
            if (expression.DataSourceType != DataSourceType.None)
            {
                object value = expression.GetSourceValue();

                //	Update the target value; this must be done on the same thread
                //	than that of the UI of the application, or else WinForms won't
                //	be happy.

                if (BindingAsyncOperation.applicationThreadInvoker == null)
                {
                    expression.InternalUpdateTarget(value);
                }
                else
                {
                    BindingAsyncOperation.applicationThreadInvoker.Invoke(
                        delegate()
                        {
                            expression.InternalUpdateTarget(value);
                        }
                    );
                }
            }
        }

        private static IApplicationThreadInvoker applicationThreadInvoker;

        private BindingExpression bindingExpression;
        private Binding binding;
        private BindingExpression[] expressions;

        private Task asyncTask;
    }
}
