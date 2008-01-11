//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	public sealed class DialogSearchController
	{
		/// <summary>
		/// Gets or sets the entity resolver.
		/// </summary>
		/// <value>The entity resolver.</value>
		public IEntityResolver Resolver
		{
			get
			{
				return this.entityResolver;
			}
			set
			{
				this.entityResolver = value;
			}
		}

		public void NotifySearchTemplateChanged(AbstractEntity entityData, EntityFieldPath path, DependencyPropertyChangedEventArgs e)
		{
			if ((this.suspendSearchHandler > 0) ||
				(this.entityResolver == null))
			{
				return;
			}
			
			Placeholder placeholder = PlaceholderContext.InteractivePlaceholder;

			if (placeholder == null)
			{
				//	Non-interactive update of the template; we won't kick in here.

				return;
			}

			BindingExpression bindingExpression = placeholder.ValueBindingExpression;

			using (this.SuspendSearchHandler ())
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Search contents changed: path={0}, id={1}, value={2}", path, e.PropertyName, e.NewValue ?? "<null>"));
				System.Diagnostics.Debug.WriteLine (string.Format (" source: {0}, property: {1}", bindingExpression.GetSourceObject (), bindingExpression.GetSourceProperty ()));

				EntityContext  context  = entityData.GetEntityContext ();
				AbstractEntity template = context.CreateEmptyEntity (entityData.GetEntityStructuredTypeId ());

				template.DisableCalculations ();
				template.InternalSetValue (e.PropertyName, e.NewValue);

				AbstractEntity result = EntityResolver.Resolve (this.entityResolver, template);

				if (result != null)
				{
					//	TODO: ...
				}
			}
		}

		private System.IDisposable SuspendSearchHandler()
		{
			return new SuspendSearchHandlerHelper (this);
		}


		#region SuspendSearchHandlerHelper Class

		private sealed class SuspendSearchHandlerHelper : System.IDisposable
		{
			public SuspendSearchHandlerHelper(DialogSearchController host)
			{
				this.host = host;
				System.Threading.Interlocked.Increment (ref this.host.suspendSearchHandler);
			}

			#region IDisposable Members

			public void Dispose()
			{
				System.Threading.Interlocked.Decrement (ref this.host.suspendSearchHandler);
				System.GC.SuppressFinalize (this);
			}

			#endregion

			private readonly DialogSearchController host;
		}

		#endregion
		
		private int								suspendSearchHandler;
		private IEntityResolver					entityResolver;
	}
}
