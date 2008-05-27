//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public DataSourceType					DataSourceType
		{
			get
			{
				return this.sourceType;
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

		public INamedType GetSourceNamedType()
		{
			INamedType namedType = null;

			switch (this.sourceType)
			{
				case DataSourceType.PropertyObject:
					namedType = BindingExpression.GetSourceNamedType (this.sourceObject as DependencyObject, this.sourceProperty as DependencyProperty);
					break;

				case DataSourceType.StructuredData:
				case DataSourceType.SourceItself:
				case DataSourceType.Resource:
					namedType = null;
					break;
			}
			
			if (namedType == null)
			{
				object typeObject = this.GetSourceTypeObject ();
				
				if (typeObject != null)
				{
					namedType = TypeRosetta.GetNamedTypeFromTypeObject (typeObject);
				}
			}

			return namedType;
		}

		public Support.Druid GetSourceCaptionId()
		{
			Support.Druid captionId = Support.Druid.Empty;

			switch (this.sourceType)
			{
				case DataSourceType.PropertyObject:
					captionId = BindingExpression.GetSourceCaptionId (this.sourceObject as DependencyObject, this.sourceProperty as DependencyProperty);
					break;

				case DataSourceType.StructuredData:
					captionId = BindingExpression.GetSourceCaptionId ((IStructuredData) this.sourceObject, (string) this.sourceProperty);
					break;
				case DataSourceType.SourceItself:
				case DataSourceType.Resource:
					captionId = Support.Druid.Empty;
					break;
			}

			return captionId;
		}

		public object GetSourceTypeObject()
		{
			object typeObject = null;
			
			switch (this.sourceType)
			{
				case DataSourceType.PropertyObject:
					typeObject = this.sourceProperty;
					break;
				
				case DataSourceType.StructuredData:
					typeObject = BindingExpression.GetTypeObjectFromStructuredData ((IStructuredData) this.sourceObject, (string) this.sourceProperty);
					break;
					
				case DataSourceType.SourceItself:
					typeObject = TypeRosetta.GetTypeObjectFromValue (this.sourceObject);
					break;

				case DataSourceType.Resource:
					if (this.sourceObject != null)
					{
						IResourceBoundSource resource = (IResourceBoundSource) this.sourceObject;
						typeObject = TypeRosetta.GetTypeObjectFromValue (resource.GetValue ((string) this.sourceProperty));
					}
					break;
			}

			return typeObject;
		}
		
		public string GetSourceName()
		{
			string sourceName = null;

			DependencyProperty property;
			
			switch (this.sourceType)
			{
				case DataSourceType.PropertyObject:
					property   = (DependencyProperty) this.sourceProperty;
					sourceName = property.Name;
					break;
				
				case DataSourceType.StructuredData:
					sourceName = (string) this.sourceProperty;
					break;
			}
			
			return sourceName;
		}

		public object ConvertValue(object value)
		{
			System.Diagnostics.Debug.Assert (this.binding != null);

			if ((this.binding.Mode == BindingMode.OneTime) ||
				(this.binding.Mode == BindingMode.OneWay) ||
				(this.binding.Mode == BindingMode.OneWayToSource) ||
				(this.binding.Mode == BindingMode.TwoWay))
			{
				ICollectionView cv = this.FindCollectionView (value);

				if (cv != null)
				{
					if (this.IsTargetObjectExpectingCollectionView ())
					{
						//	Nothing to do, since the target will be happy to get a collection view.

						return cv;
					}
					else
					{
						value = cv.CurrentItem;
					}
				}
			}
			
			return this.binding.ConvertValue (value, this.targetPropery.PropertyType);
		}

		public object ConvertBackValue(object value)
		{
			System.Type type;

			switch (this.sourceType)
			{
				case DataSourceType.PropertyObject:
					type = ((DependencyProperty) this.sourceProperty).PropertyType;
					break;

				case DataSourceType.StructuredData:
					type = BindingExpression.GetSystemTypeFromStructuredData (this.sourceObject as IStructuredData, (string) this.sourceProperty);
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Cannot convert back to source type {0}", this.sourceType));
			}

			return this.binding.ConvertBackValue (value, type);
		}

		public void SetSourceValue(object value)
		{
			switch (this.binding.Mode)
			{
				case BindingMode.OneWayToSource:
				case BindingMode.TwoWay:
					this.InternalUpdateSource (value);
					this.InternalUpdateTarget ();
					break;

				case BindingMode.OneWay:
					this.InternalUpdateTarget ();
					break;
			}
		}

		#region Internal Methods

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
			DataSourceType				sourceType;
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
				this.sourceType        = DataSourceType.None;
				this.sourceBreadcrumbs = sourceBreadcrumbs;
			}
		}
		
		internal void DetachFromSource()
		{
			this.InternalDetachFromSource ();
		}
		
		internal static BindingExpression BindToTarget(DependencyObject target, DependencyProperty property, AbstractBinding binding)
		{
			BindingExpression expression = new BindingExpression ();

			binding.Add (expression);
			
			expression.binding = binding as Binding;
			expression.targetObject = target;
			expression.targetPropery = property;

			expression.InternalAttachToTarget ();

			if (!binding.Deferred)
			{
				expression.binding.AttachExpressionsToSource (expression);
			}
			
			return expression;
		}

		#endregion

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

		private bool FindDataSource(out object objectSource, out object objectProperty, out DataSourceType type, out BindingBreadcrumbs breadcrumbs)
		{
			type           = DataSourceType.None;
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
				//	There is no path, so we will simply consider the source itself
				//	as the data.
				
				type = DataSourceType.SourceItself;

				objectSource   = root;
				objectProperty = null;

				return true;
			}
			else
			{
				DependencyObject   doSource = root as DependencyObject;
				IStructuredData    sdSource = root as IStructuredData;
				ICollectionView    cvSource = this.FindCollectionView (root);
				DependencyProperty property = null;
				string             name     = null;

				if ((doSource == null) &&
					(sdSource == null) &&
					(cvSource == null))
				{
					if (root is IResourceBoundSource)
					{
						type = DataSourceType.Resource;
						
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
					int firstItemIndex = 0;

					for (int i = 0; i < elements.Length; i++)
					{
						firstItemIndex++;
						
						if (elements[i] == "*")
						{
							if ((doSource != null) ||
								(cvSource != null))
							{
								//	Skip "*" elements. We cannot - ever - have a DependencyObject
								//	source with such a property name.
								
								continue;
							}
							if (sdSource != null)
							{
								object star = sdSource.GetValue ("*");

								if ((UnknownValue.IsUnknownValue (star)) ||
									(UndefinedValue.IsUndefinedValue (star)))
								{
									//	If the structured data source does not contain a "*" node,
									//	then simply ignore the "*" element.

									continue;
								}
							}
						}

						firstItemIndex--;

						if (i > firstItemIndex)
						{
							if ((doSource != null) &&
								(property != null))
							{
								this.AddBreadcrumb (ref breadcrumbs, doSource, property);
								root = doSource.GetValue (property);
							}
							else if ((sdSource != null) &&
								     (!string.IsNullOrEmpty (name)))
							{
								this.AddBreadcrumb (ref breadcrumbs, sdSource, name);
								root = sdSource.GetValue (name);
							}
							else
							{
								root = null;
							}

							if (root == Binding.DoNothing)
							{
								root = null;
							}

							doSource = root as DependencyObject;
							sdSource = root as IStructuredData;
							cvSource = this.FindCollectionView (root);

							if ((doSource == null) &&
								(sdSource == null) &&
								(cvSource == null))
							{
								return false;
							}
						}

						if (cvSource != null)
						{
							this.AddBreadcrumb (ref breadcrumbs, cvSource);
							root = cvSource.CurrentItem;

							doSource = root as DependencyObject;
							sdSource = root as IStructuredData;
							cvSource = this.FindCollectionView (root);

							System.Diagnostics.Debug.Assert (cvSource == null, "CollectionView.CollectionView not accepted here");
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
						type = DataSourceType.PropertyObject;
						
						objectSource   = doSource;
						objectProperty = property;
						
						root = doSource.GetValue (property);
					}
					else if ((sdSource != null) &&
						     (!string.IsNullOrEmpty (name)))
					{
						type = DataSourceType.StructuredData;
						
						objectSource   = sdSource;
						objectProperty = name;

						root = sdSource.GetValue (name);
					}

					System.Diagnostics.Debug.Assert (type != DataSourceType.None);


					cvSource = this.FindCollectionView (root);

					if (cvSource != null)
					{
						//	We have successfully resolved the real source object (the last
						//	element at the end of the source path) and it happens to be a
						//	collection view backed object.

						if (this.IsTargetObjectExpectingCollectionView ())
						{
							//	The target object is expecting a collection view, so we do
							//	not need to step in.
						}
						else
						{
							this.AddBreadcrumb (ref breadcrumbs, cvSource);
						}
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

		private void AddBreadcrumb(ref BindingBreadcrumbs breadcrumbs, DependencyObject source, DependencyProperty property)
		{
			if (breadcrumbs == null)
			{
				breadcrumbs = new BindingBreadcrumbs (this.HandleBreadcrumbsChanged);
			}

			breadcrumbs.AddNode (source, property);
		}

		private void AddBreadcrumb(ref BindingBreadcrumbs breadcrumbs, IStructuredData source, string name)
		{
			if (breadcrumbs == null)
			{
				breadcrumbs = new BindingBreadcrumbs (this.HandleBreadcrumbsChanged);
			}

			breadcrumbs.AddNode (source, name);
		}

		private void AddBreadcrumb(ref BindingBreadcrumbs breadcrumbs, ICollectionView source)
		{
			if (breadcrumbs == null)
			{
				breadcrumbs = new BindingBreadcrumbs (this.HandleBreadcrumbsChanged);
			}

			breadcrumbs.AddNode (source);
		}

		private bool IsTargetObjectExpectingCollectionView()
		{
			if (this.targetPropery != null)
			{
				System.Type propertyType = this.targetPropery.PropertyType;

				if (TypeRosetta.DoesTypeImplementInterface (propertyType, typeof (ICollectionView)))
				{
					return true;
				}
			}
			
			return false;
		}

		/// <summary>
		/// Finds the <c>ICollectionView</c> for a given object; the object must
		/// be a collection for this to work.
		/// </summary>
		/// <param name="collection">The probable collection object.</param>
		/// <returns>An <c>ICollectionView</c> which represents the collection.</returns>
		private ICollectionView FindCollectionView(object collection)
		{
			Binding context = this.dataContext;

			if (context == null)
			{
				context = DataObject.GetDataContext (this.targetObject);
			}

			return Binding.FindCollectionView (collection, context);
		}

		/// <summary>
		/// Finds the data source root based on the binding definition. This will either
		/// use the binding source directly (if available) or derive the source from the
		/// locally visible <c>DataContext</c>.
		/// </summary>
		/// <param name="sourceRoot">The source root.</param>
		/// <param name="sourcePath">The path from the root to the source.</param>
		private void FindDataSourceRoot(out object sourceRoot, out string sourcePath)
		{
			sourceRoot = this.binding.Source;
			sourcePath = this.binding.Path;

			if (sourceRoot == null)
			{
				//	Our binding is not explicitely attached to a source; we will
				//	have to use the target's DataContext instead.

				this.SetDataContext (DataObject.GetDataContext (this.targetObject));

				if (this.dataContext != null)
				{
					sourceRoot = this.dataContext.Source;
					sourcePath = DependencyPropertyPath.Combine (this.dataContext.Path, sourcePath);
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
						case DataSourceType.PropertyObject:
							doSource = (DependencyObject) this.sourceObject;
							property = (DependencyProperty) this.sourceProperty;
							
							BindingExpression.SetValue (doSource, property, this.ConvertBackValue (value));
							break;
						
						case DataSourceType.StructuredData:
							sdSource = this.sourceObject as IStructuredData;
							name     = (string) this.sourceProperty;
							
							BindingExpression.SetValue (sdSource, name, this.ConvertBackValue (value));
							break;
						
						case DataSourceType.SourceItself:
							throw new System.InvalidOperationException ("Cannot update source: DataSourceType set to SourceItself");
						
						case DataSourceType.Resource:
							throw new System.InvalidOperationException ("Cannot update source: DataSourceType set to Resource");
					}
				}
				finally
				{
					System.Threading.Interlocked.Decrement (ref this.sourceUpdateCounter);
				}
			}
		}

		private static System.Type GetSystemTypeFromStructuredData(IStructuredData source, string fieldId)
		{
			IStructuredTypeProvider typeProvider = source as IStructuredTypeProvider;

			if (typeProvider != null)
			{
				IStructuredType structuredType = typeProvider.GetStructuredType ();
				
				if (structuredType != null)
				{
					StructuredTypeField field = structuredType.GetField (fieldId);
					
					if ((field != null) &&
						(field.Type != null))
					{
						return field.Type.SystemType;
					}
				}
			}

			return null;
		}

		private static object GetTypeObjectFromStructuredData(IStructuredData source, string fieldId)
		{
			IStructuredTypeProvider typeProvider = source as IStructuredTypeProvider;

			if (typeProvider != null)
			{
				IStructuredType structuredType = typeProvider.GetStructuredType ();
				return structuredType.GetField (fieldId).Type;
			}

			return null;
		}

		private void InternalUpdateTarget()
		{
			if (this.sourceType != DataSourceType.None)
			{
				if (this.binding.IsAttached)
				{
					if (this.binding.IsAsync)
					{
						//	If the binding requires an asynchronous access to the source
						//	value, we delegate the work to the BindingAsyncOperation class.

						if (this.asyncOperation == null)
						{
							this.asyncOperation = new BindingAsyncOperation (this);
						}

						//	First, tell the target that the data is still pending. This is
						//	immediate :

						this.InternalUpdateTarget (PendingValue.Value);

						//	Now, queue up an asynchronous call to GetSourceValue followed
						//	by a call to InternalUpdateTarget, but don't wait for the value
						//	to be successfully set and return immediately.

						this.asyncOperation.QuerySourceValueAndUpdateTarget ();
					}
					else
					{
						this.InternalUpdateTarget (this.GetSourceValue ());
					}
				}
			}
			else
			{
				this.InternalResetTarget ();
			}
		}

		internal void InternalUpdateTarget(object value)
		{
			if ((this.sourceUpdateCounter == 0) &&
				(Binding.IsRealValue (value)))
			{
				System.Threading.Interlocked.Increment (ref this.targetUpdateCounter);
				
				try
				{
					BindingExpression.SetValue (this.targetObject, this.TargetProperty, this.ConvertValue (value));
				}
				finally
				{
					System.Threading.Interlocked.Decrement (ref this.targetUpdateCounter);
				}
			}
		}

		internal void InternalResetTarget()
		{
			if (this.sourceUpdateCounter == 0)
			{
				System.Threading.Interlocked.Increment (ref this.targetUpdateCounter);

				try
				{
					BindingExpression.ClearValue (this.targetObject, this.TargetProperty);
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
				case DataSourceType.PropertyObject:
					BindingExpression.Attach (this, this.sourceObject as DependencyObject, (DependencyProperty) this.sourceProperty);
					break;
				case DataSourceType.StructuredData:
					BindingExpression.Attach (this, this.sourceObject as IStructuredData, (string) this.sourceProperty);
					break;
				case DataSourceType.SourceItself:
					break;
				case DataSourceType.Resource:
					break;
			}
		}
		
		private void InternalDetachFromSource()
		{
			if (this.sourceObject != null)
			{
				switch (this.sourceType)
				{
					case DataSourceType.PropertyObject:
						BindingExpression.Detach (this, this.sourceObject as DependencyObject, (DependencyProperty) this.sourceProperty);
						break;
					case DataSourceType.StructuredData:
						BindingExpression.Detach (this, this.sourceObject as IStructuredData, (string) this.sourceProperty);
						break;
					case DataSourceType.SourceItself:
						break;
					case DataSourceType.Resource:
						break;
				}

				this.sourceObject      = null;
				this.sourceProperty    = null;
				this.sourceType        = DataSourceType.None;
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

		public object GetSourceObject()
		{
			return this.sourceObject;
		}

		public DataSourceType GetSourceType()
		{
			return this.sourceType;
		}

		public object GetSourceProperty()
		{
			return this.sourceProperty;
		}

		public object GetSourceValue()
		{
			DependencyObject doSource;
			IStructuredData sdSource;
			IResourceBoundSource resource;
			object data;

			switch (this.sourceType)
			{
				case DataSourceType.PropertyObject:
					doSource = this.sourceObject as DependencyObject;
					data = doSource.GetValue ((DependencyProperty) this.sourceProperty);
					break;

				case DataSourceType.StructuredData:
					sdSource = this.sourceObject as IStructuredData;
					data = sdSource.GetValue ((string) this.sourceProperty);
					break;

				case DataSourceType.SourceItself:
					data = this.sourceObject;
					break;
				
				case DataSourceType.Resource:
					resource = (IResourceBoundSource) this.sourceObject;
					data = resource.GetValue ((string) this.sourceProperty);
					break;

				default:
					throw new System.NotImplementedException (string.Format ("DataSourceType {0} not implemented", this.sourceType));
			}

			return data;
		}
		
		private static void SetValue(DependencyObject target, DependencyProperty property, object value)
		{
			if (target != null)
			{
				if (Binding.IsRealValue (value))
				{
					target.SetValue (property, value);
				}
			}
		}

		private static void SetValue(IStructuredData target, string name, object value)
		{
			if (target != null)
			{
				if (Binding.IsRealValue (value))
				{
					target.SetValue (name, value);
				}
			}
		}

		private static void ClearValue(DependencyObject target, DependencyProperty property)
		{
			if (target != null)
			{
				target.ClearValue (property);
			}
		}

		private static void ClearValue(IStructuredData target, string name)
		{
			if (target != null)
			{
				target.SetValue (name, UndefinedValue.Value);
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

		private static void Attach(BindingExpression expression, IStructuredData source, string name)
		{
			source.AttachListener (name, expression.HandleSourcePropertyChanged);
		}
		
		private static void Detach(BindingExpression expression, IStructuredData source, string name)
		{
			source.DetachListener (name, expression.HandleSourcePropertyChanged);
		}

		private static INamedType GetSourceNamedType(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
		{
			if ((dependencyObject == null) ||
				(dependencyProperty == null))
			{
				return null;
			}
			else
			{
				return dependencyProperty.GetMetadata (dependencyObject).NamedType;
			}
		}

		private static Support.Druid GetSourceCaptionId(DependencyObject source, DependencyProperty property)
		{
			if ((source == null) ||
				(property == null))
			{
				return Support.Druid.Empty;
			}
			else
			{
				Support.Druid id = property.GetMetadata (source).CaptionId;
				
				if (id.IsEmpty)
				{
					id = property.CaptionId;
				}

				return id;
			}
		}

		private static Support.Druid GetSourceCaptionId(IStructuredData source, string fieldId)
		{
			IStructuredTypeProvider typeProvider = source as IStructuredTypeProvider;

			if ((typeProvider == null) ||
				(fieldId == null))
			{
				return Support.Druid.Empty;
			}
			
			IStructuredType structuredType = typeProvider.GetStructuredType ();

			if (structuredType == null)
			{
				return Support.Druid.Empty;
			}

			StructuredTypeField field = structuredType.GetField (fieldId);

			return field.CaptionId;
		}
		
		private Binding							binding;
		private DependencyObject				targetObject;			//	immutable
		private DependencyProperty				targetPropery;			//	immutable
		private object							sourceObject;
		private object							sourceProperty;
		private DataSourceType					sourceType;
		private BindingBreadcrumbs				sourceBreadcrumbs;
		private Binding							dataContext;
		private bool							isDataContextBound;
		private int								sourceUpdateCounter;
		private int								targetUpdateCounter;
		private BindingAsyncOperation			asyncOperation;
	}
}
