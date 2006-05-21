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
				this.sourceBreadcrumbs = sourceBreadcrumbs;
			}
		}
		
		internal void DetachFromSource()
		{
			this.InternalDetachFromSource ();
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

			if ((root == null) ||
				(root == Binding.DoNothing))
			{
				return false;
			}

			if (string.IsNullOrEmpty (path))
			{
				//	There is no path, so we will simply consider the source as the data.
				
				type = BindingSourceType.SourceItself;

				objectSource   = root;
				objectProperty = null;

				return true;
			}
			else
			{
				DependencyObject   doSource = root as DependencyObject;
				IStructuredData    sdSource = root as IStructuredData;
				DependencyProperty property = null;
				string             name     = null;

				if ((doSource == null) &&
					(sdSource == null))
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

							object value;
							
							if ((doSource != null) &&
								(property != null))
							{
								breadcrumbs.AddNode (doSource, property);
								value = doSource.GetValue (property);
							}
							else if ((sdSource != null) &&
								/**/ (!string.IsNullOrEmpty (name)))
							{
								breadcrumbs.AddNode (sdSource, name);
								value = sdSource.GetValue (name);
							}
							else
							{
								value = null;
							}

							if (value == Binding.DoNothing)
							{
								value = null;
							}
							
							doSource = value as DependencyObject;
							sdSource = value as IStructuredData;

							if ((doSource == null) &&
								(sdSource == null))
							{
								return false;
							}
						}

						property = null;
						name     = null;
						
						if (doSource != null)
						{
							property = doSource.ObjectType.GetProperty (elements[i]);
						}
						if (sdSource != null)
						{
							name = elements[i];
						}

						if ((property == null) &&
							(name == null))
						{
							return false;
						}
					}

					if ((doSource != null) &&
						(property != null))
					{
						type = BindingSourceType.PropertyObject;
						
						objectSource = doSource;
						objectProperty = property;
					}
					else if ((sdSource != null) &&
						/**/ (!string.IsNullOrEmpty (name)))
					{
						type = BindingSourceType.StructuredData;
						
						objectSource   = sdSource;
						objectProperty = name;
					}

					//	If we have traversed several data source objects to arrive
					//	at the leaf 'source', we will register with them in order
					//	to detect changes; this is done in InternalAttachToSource,
					//	by the caller, based on the breadcrumbs.

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
					DependencyObject   doSource;
					DependencyProperty property;
					IStructuredData    sdSource;
					string             name;

					switch (this.sourceType)
					{
						case BindingSourceType.PropertyObject:
							doSource = (DependencyObject) this.sourceObject;
							property = (DependencyProperty) this.sourceProperty;
							
							if (this.binding.HasConverter)
							{
								value = this.binding.ConvertBackValue (value, property.PropertyType);
							}
							
							BindingExpression.SetValue (doSource, property, value);
							break;
						
						case BindingSourceType.StructuredData:
							sdSource = this.sourceObject as IStructuredData;
							name     = (string) this.sourceProperty;
							
							if (this.binding.HasConverter)
							{
								value = this.binding.ConvertBackValue (value, this.GetStructuredDataType (sdSource, name));
							}
							
							BindingExpression.SetValue (sdSource, name, value);
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

		private System.Type GetStructuredDataType(IStructuredData source, string name)
		{
			IStructuredTypeProvider typeProvider = source as IStructuredTypeProvider;

			if (typeProvider != null)
			{
				IStructuredType structuredType = typeProvider.GetStructuredType ();
				object typeObject = structuredType.GetFieldTypeObject (name);
				return TypeRosetta.GetSystemTypeFromTypeObject (typeObject);
			}
			
			return null;
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
			if ((this.sourceUpdateCounter == 0) &&
				(value != Binding.DoNothing))
			{
				System.Threading.Interlocked.Increment (ref this.targetUpdateCounter);
				
				try
				{
					if (this.binding.HasConverter)
					{
						value = this.binding.ConvertValue (value, this.targetPropery.PropertyType);
					}
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
				case BindingSourceType.StructuredData:
					break;
				case BindingSourceType.SourceItself:
					break;
				case BindingSourceType.Resource:
					break;
			}
		}
		private void InternalDetachFromSource()
		{
			if (this.sourceObject != null)
			{
				switch (this.sourceType)
				{
					case BindingSourceType.PropertyObject:
						BindingExpression.Detach (this, this.sourceObject as DependencyObject, (DependencyProperty) this.sourceProperty);
						break;
					case BindingSourceType.StructuredData:
						break;
					case BindingSourceType.SourceItself:
						break;
					case BindingSourceType.Resource:
						break;
				}

				this.sourceObject      = null;
				this.sourceProperty    = null;
				this.sourceType        = BindingSourceType.None;
			}
			
			if (this.sourceBreadcrumbs != null)
			{
				this.sourceBreadcrumbs.Dispose ();
				this.sourceBreadcrumbs = null;
			}
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
			DependencyObject doSource;
			IStructuredData sdSource;
			IResourceBoundSource resource;
			object data;

			switch (this.sourceType)
			{
				case BindingSourceType.PropertyObject:
					doSource = this.sourceObject as DependencyObject;
					data = doSource.GetValue ((DependencyProperty) this.sourceProperty);
					break;

				case BindingSourceType.StructuredData:
					sdSource = this.sourceObject as IStructuredData;
					data = sdSource.GetValue ((string) this.sourceProperty);
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

		private static void SetValue(IStructuredData target, string name, object value)
		{
			if (target != null)
			{
				if (value != Binding.DoNothing)
				{
					target.SetValue (name, value);
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
