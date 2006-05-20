//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	/// <summary>
	/// The <c>BindingExpression</c> class is used to maintain the real binding
	/// between a source and a target, whereas the <see cref="T:Binding"/> class
	/// can be specified more than once (i.e. <c>Binding</c> is more general).
	/// </summary>
	public sealed class BindingExpression : System.IDisposable
	{
		private BindingExpression()
		{
		}
		
		public Binding							ParentBinding
		{
			get
			{
				return this.binding;
			}
		}
		public DependencyObject					TargetObject
		{
			get
			{
				return this.targetObject;
			}
		}
		public DependencyProperty				TargetProperty
		{
			get
			{
				return this.targetPropery;
			}
		}

		public void Synchronize()
		{
			this.UpdateTarget (BindingUpdateMode.Reset);
			this.UpdateSource ();
		}
		
		public void UpdateSource()
		{
			this.AssertBinding ();
			
			switch (this.binding.Mode)
			{
				case BindingMode.OneWayToSource:
				case BindingMode.TwoWay:
					this.InternalUpdateSource ();
					break;
			}
		}
		public void UpdateTarget()
		{
			this.UpdateTarget (BindingUpdateMode.Default);
		}
		public void UpdateTarget(BindingUpdateMode mode)
		{
			this.AssertBinding ();

			switch (this.binding.Mode)
			{
				case BindingMode.OneTime:
					if (mode == BindingUpdateMode.Reset)
					{
						this.InternalUpdateTarget ();
					}
					break;
				
				case BindingMode.OneWay:
				case BindingMode.TwoWay:
					this.InternalUpdateTarget ();
					break;
			}
		}

		internal void RefreshSourceBinding()
		{
			this.DetachFromSource ();
			this.AttachToSource ();
			this.UpdateTarget (BindingUpdateMode.Reset);
		}
		
		internal void AttachToSource()
		{
			object						sourceObject;
			object						sourceProperty;
			BindingSourceType			sourceType;
			BindingBreadcrumbs			sourceBreadcrumbs;

			System.Diagnostics.Debug.Assert (this.sourceBreadcrumbs == null);

			if (this.FindDataSource (out sourceObject, out sourceProperty, out sourceType, out sourceBreadcrumbs))
			{
				this.sourceObject      = sourceObject;
				this.sourceProperty    = sourceProperty;
				this.sourceType        = sourceType;
				this.sourceBreadcrumbs = sourceBreadcrumbs;

				this.InternalAttachToSource ();
			}
			else
			{
				this.sourceObject      = null;
				this.sourceProperty    = null;
				this.sourceType        = BindingSourceType.None;
				this.sourceBreadcrumbs = null;
			}
		}
		
		internal void DetachFromSource()
		{
			if (this.sourceObject != null)
			{
				this.InternalDetachFromSource ();
			}
		}
		
		internal static BindingExpression BindToTarget(DependencyObject target, DependencyProperty property, Binding binding)
		{
			BindingExpression expression = new BindingExpression ();

			binding.Add (expression);
			
			expression.binding = binding;
			expression.targetObject = target;
			expression.targetPropery = property;

			expression.InternalAttachToTarget ();

			if (!binding.Deferred)
			{
				expression.AttachToSource ();
				expression.UpdateTarget (BindingUpdateMode.Reset);
			}
			
			return expression;
		}

		#region IDisposable Members
		public void Dispose()
		{
			//	Detach from the target, and from the source.
			
			this.AssertBinding ();

			this.binding.Remove (this);
			this.InternalDetachFromSource ();
			this.InternalDetachFromTarget ();
			this.ClearDataContext ();
			
			this.binding = null;
			this.targetObject = null;
			this.targetPropery = null;
		}
		#endregion
		
		private void AssertBinding()
		{
			if ((this.binding == null) ||
				(this.targetObject == null) ||
				(this.targetPropery == null))
			{
				throw new System.InvalidOperationException ("Broken binding");
			}
		}

		private bool FindDataSource(out object objectSource, out object objectProperty, out BindingSourceType type, out BindingBreadcrumbs breadcrumbs)
		{
			type           = BindingSourceType.None;
			objectSource   = null;
			objectProperty = null;
			breadcrumbs    = null;

			object root;
			string path;

			this.FindDataSourceRoot (out root, out path);

			DependencyObject   source   = root as DependencyObject;
			DependencyProperty property = null;
			
			if ((source != null) &&
				(source != Binding.DoNothing) &&
				(path != null) &&
				(path.Length > 0))
			{
				//	Resolve the path to get at the real source element, starting
				//	at the root.

				string[] elements = path.Split ('.');
				
				for (int i = 0; i < elements.Length; i++)
				{
					if (i > 0)
					{
						if (breadcrumbs == null)
						{
							breadcrumbs = new BindingBreadcrumbs (this.HandleBreadcrumbsChanged);
						}
						
						breadcrumbs.AddNode (source, property);
						
						source = source.GetValue (property) as DependencyObject;
						
						if ((source == null) ||
							(source == Binding.DoNothing))
						{
							return false;
						}
					}
					
					property = source.ObjectType.GetProperty (elements[i]);
					
					if (property == null)
					{
						return false;
					}
				}

				type = BindingSourceType.PropertyObject;
				
				objectSource = source;
				objectProperty = property;
				
				//	If we have traversed several data source objects to arrive
				//	at the leaf 'source', we will register with them in order
				//	to detect changes; this is done in InternalAttachToSource,
				//	by the caller, based on the breadcrumbs.

				return true;
			}
			
			if ((root != null) &&
				(root != Binding.DoNothing))
			{
				if ((path != null) &&
					(path.Length > 0))
				{
					if (root is IResourceBoundSource)
					{
						type = BindingSourceType.Resource;
						
						objectSource   = root;
						objectProperty = path;

						return true;
					}
				}
				else
				{
					type = BindingSourceType.SourceItself;
					
					objectSource   = root;
					objectProperty = null;

					return true;
				}
			}
			
			return false;
		}
		private void FindDataSourceRoot(out object source, out string path)
		{
			source = this.binding.Source;
			path   = this.binding.Path;

			if (source == null)
			{
				//	Our binding is not explicitely attached to a source; we will
				//	have to use the target's DataContext instead.

				this.SetDataContext (DataObject.GetDataContext (this.targetObject));

				if (this.dataContext != null)
				{
					source = this.dataContext.Source;
					path   = DependencyPropertyPath.Combine (this.dataContext.Path, path);
				}
			}
			else
			{
				this.ClearDataContext ();
			}
		}

		private void ClearDataContext()
		{
			if (this.dataContext != null)
			{
				this.dataContext.Remove (this);
				this.dataContext = null;
			}
			
			if (this.isDataContextBound)
			{
				this.targetObject.RemoveEventHandler (DataObject.DataContextProperty, this.HandleDataContextChanged);
				this.isDataContextBound = false;
			}
		}

		private void SetDataContext(Binding value)
		{
			if (this.dataContext != value)
			{
				if (this.dataContext != null)
				{
					this.dataContext.Remove (this);
				}
				
				this.dataContext = value;
				
				//	Attach to data context in order to be informed if the
				//	source changes:

				if (this.dataContext != null)
				{
					this.dataContext.Add (this);
				}
			}
			
			//	Attach to data context changes in order to be informed if the
			//	data context changes:
			
			if (this.isDataContextBound == false)
			{
				this.targetObject.AddEventHandler (DataObject.DataContextProperty, this.HandleDataContextChanged);
				this.isDataContextBound = true;
			}
		}

		private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.RefreshSourceBinding ();
		}

		private void InternalUpdateSource()
		{
			this.InternalUpdateSource (this.targetObject.GetValue (this.targetPropery));
		}
		private void InternalUpdateSource(object value)
		{
			if (this.targetUpdateCounter == 0)
			{
				System.Threading.Interlocked.Increment (ref this.sourceUpdateCounter);

				try
				{
					DependencyObject source;

					switch (this.sourceType)
					{
						case BindingSourceType.PropertyObject:
							source = this.sourceObject as DependencyObject;
							BindingExpression.SetValue (source, (DependencyProperty) this.sourceProperty, value);
							break;
						case BindingSourceType.SourceItself:
							throw new System.InvalidOperationException ("Cannot update source: BindingSourceType set to SourceItself");
						case BindingSourceType.Resource:
							throw new System.InvalidOperationException ("Cannot update source: BindingSourceType set to Resource");
					}
				}
				finally
				{
					System.Threading.Interlocked.Decrement (ref this.sourceUpdateCounter);
				}
			}
		}
		
		private void InternalUpdateTarget()
		{
			if (this.sourceType != BindingSourceType.None)
			{
				this.InternalUpdateTarget (this.GetSourceValue ());
			}
		}
		private void InternalUpdateTarget(object value)
		{
			if (this.sourceUpdateCounter == 0)
			{
				System.Threading.Interlocked.Increment (ref this.targetUpdateCounter);
				
				try
				{
					BindingExpression.SetValue (this.targetObject, this.TargetProperty, value);
				}
				finally
				{
					System.Threading.Interlocked.Decrement (ref this.targetUpdateCounter);
				}
			}
		}

		private void InternalAttachToSource()
		{
			System.Diagnostics.Debug.Assert (this.sourceObject != null);
			
			switch (this.sourceType)
			{
				case BindingSourceType.PropertyObject:
					BindingExpression.Attach (this, this.sourceObject as DependencyObject, (DependencyProperty) this.sourceProperty);
					break;
				case BindingSourceType.SourceItself:
					break;
				case BindingSourceType.Resource:
					break;
			}
		}
		private void InternalDetachFromSource()
		{
			System.Diagnostics.Debug.Assert (this.sourceObject != null);
			
			switch (this.sourceType)
			{
				case BindingSourceType.PropertyObject:
					BindingExpression.Detach (this, this.sourceObject as DependencyObject, (DependencyProperty) this.sourceProperty);
					break;
				case BindingSourceType.SourceItself:
					break;
				case BindingSourceType.Resource:
					break;
			}

			if (this.sourceBreadcrumbs != null)
			{
				this.sourceBreadcrumbs.Dispose ();
				this.sourceBreadcrumbs = null;
			}

			this.sourceObject      = null;
			this.sourceProperty    = null;
			this.sourceType        = BindingSourceType.None;
		}
		
		private void InternalAttachToTarget()
		{
			switch (this.binding.Mode)
			{
				case BindingMode.OneWayToSource:
				case BindingMode.TwoWay:
					this.targetObject.AddEventHandler (this.targetPropery, this.HandleTargetPropertyChanged);
					break;
			}
		}
		private void InternalDetachFromTarget()
		{
			switch (this.binding.Mode)
			{
				case BindingMode.OneWayToSource:
				case BindingMode.TwoWay:
					this.targetObject.RemoveEventHandler (this.targetPropery, this.HandleTargetPropertyChanged);
					break;
			}
		}

		private void HandleSourcePropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.InternalUpdateTarget (e.NewValue);
		}
		private void HandleBreadcrumbsChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.RefreshSourceBinding ();
		}
		private void HandleTargetPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.InternalUpdateSource (e.NewValue);
		}

		private object GetSourceValue()
		{
			DependencyObject source;
			IResourceBoundSource resource;
			object data;

			switch (this.sourceType)
			{
				case BindingSourceType.PropertyObject:
					source = this.sourceObject as DependencyObject;
					data   = source.GetValue ((DependencyProperty) this.sourceProperty);
					break;

				case BindingSourceType.SourceItself:
					data = this.sourceObject;
					break;
				
				case BindingSourceType.Resource:
					resource = (IResourceBoundSource) this.sourceObject;
					data = resource.GetValue ((string) this.sourceProperty);
					break;

				default:
					throw new System.NotImplementedException (string.Format ("BindingSourceType.{0} not implemented", this.sourceType));
			}

			return data;
		}
		
		private static void SetValue(DependencyObject target, DependencyProperty property, object value)
		{
			if (target != null)
			{
				if (value != Binding.DoNothing)
				{
					target.SetValue (property, value);
				}
			}
		}
		
		private static void Attach(BindingExpression expression, DependencyObject source, DependencyProperty property)
		{
			source.AddEventHandler (property, expression.HandleSourcePropertyChanged);
		}
		private static void Detach(BindingExpression expression, DependencyObject source, DependencyProperty property)
		{
			source.RemoveEventHandler (property, expression.HandleSourcePropertyChanged);
		}


		private Binding							binding;
		private DependencyObject				targetObject;			//	immutable
		private DependencyProperty				targetPropery;			//	immutable
		private object							sourceObject;
		private object							sourceProperty;
		private BindingSourceType				sourceType;
		private BindingBreadcrumbs				sourceBreadcrumbs;
		private Binding							dataContext;
		private bool							isDataContextBound;
		private int								sourceUpdateCounter;
		private int								targetUpdateCounter;
	}
}
