//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<PropertyChangedEventArgs>;
	
	/// <summary>
	/// The BindingExpression class is used to maintain the real binding
	/// between a source and a target, whereas the Binding class can be
	/// specified more than once (Binding is more general).
	/// </summary>
	public sealed class BindingExpression : System.IDisposable
	{
		private BindingExpression()
		{
		}
		
		public Binding							Binding
		{
			get
			{
				return this.binding;
			}
		}
		public Object							TargetObject
		{
			get
			{
				return this.targetObject;
			}
		}
		public Property							TargetProperty
		{
			get
			{
				return this.targetPropery;
			}
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
			Object						sourceObject;
			Property					sourceProperty;
			BindingSourceType			sourceType;
			List<SourcePropertyPair>	sourceBreadcrumbs;

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
		
		internal static BindingExpression BindToTarget(Object target, Property property, Binding binding)
		{
			BindingExpression expression = new BindingExpression ();

			binding.Add (expression);
			
			expression.binding = binding;
			expression.targetObject = target;
			expression.targetPropery = property;

			expression.AttachToSource ();
			expression.UpdateTarget (BindingUpdateMode.Reset);
			
			return expression;
		}

		#region IDisposable Members
		public void Dispose()
		{
			//	Detach from the target, and from the source.
			
			this.AssertBinding ();

			this.binding.Remove (this);
			this.InternalDetachFromSource ();
			this.SetDataContext (null);
			
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

		private bool FindDataSource(out Object source, out Property property, out BindingSourceType type, out List<SourcePropertyPair> breadcrumbs)
		{
			type        = BindingSourceType.None;
			source      = null;
			property    = null;
			breadcrumbs = null;

			object root;
			PropertyPath path;

			this.FindDataSourceRoot (out root, out path);

			source = root as Object;
			
			if ((source != null) &&
				(path != null))
			{
				//	Resolve the path to get at the real source element, starting
				//	at the root.

				string[] elements = path.GetFullPath ().Split ('.');
				
				for (int i = 0; i < elements.Length; i++)
				{
					if (i > 0)
					{
						if (breadcrumbs == null)
						{
							breadcrumbs = new List<SourcePropertyPair> ();
						}
						
						breadcrumbs.Add (new SourcePropertyPair (source, property));
						
						source = source.GetValue (property) as Object;
						
						if (source == null)
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
				
				//	If we have traversed several data source objects to arrive
				//	at the leaf 'source', we must register with them in order
				//	to detect changes :
				


				return true;
			}
			
			return false;
		}

		private void FindDataSourceRoot(out object source, out PropertyPath path)
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
					path   = PropertyPath.Combine (this.dataContext.Path, path);
				}
			}
			else
			{
				this.SetDataContext (null);
			}
		}

		private void SetDataContext(Binding value)
		{
			if (this.dataContext != value)
			{
				bool wasRegistered = false;
				
				if (this.dataContext != null)
				{
					this.dataContext.Remove (this);
					wasRegistered = true;
				}
				
				this.dataContext = value;
				
				//	Attach to data context in order to be informed if the
				//	source changes:

				if (this.dataContext != null)
				{
					this.dataContext.Add (this);

					if (!wasRegistered)
					{
						this.targetObject.AddEventHandler (DataObject.DataContextProperty, this.HandleDataContextChanged);
					}
				}
				else
				{
					if (wasRegistered)
					{
						this.targetObject.RemoveEventHandler (DataObject.DataContextProperty, this.HandleDataContextChanged);
					}
				}
			}
		}

		private void HandleDataContextChanged(object sender, PropertyChangedEventArgs e)
		{
			this.RefreshSourceBinding ();
		}

		private void InternalUpdateSource()
		{
			//	TODO: update source
		}
		private void InternalUpdateSource(object value)
		{
			//	TODO: update source
		}
		private void InternalUpdateTarget()
		{
			Object source;
			
			switch (this.sourceType)
			{
				case BindingSourceType.PropertyObject:
					source = this.sourceObject as Object;
					this.InternalUpdateTarget (source.GetValue (this.sourceProperty));
					break;
			}
		}
		private void InternalUpdateTarget(object value)
		{
			this.targetObject.SetValue (this.targetPropery, value);
		}

		private void InternalAttachToSource()
		{
			System.Diagnostics.Debug.Assert (this.sourceObject != null);
			
			switch (this.sourceType)
			{
				case BindingSourceType.PropertyObject:
					BindingExpression.Attach (this, this.sourceObject as Object, this.sourceProperty);
					BindingExpression.Attach (this, this.sourceBreadcrumbs);
					break;
			}
		}

		private static void Attach(BindingExpression expression, List<SourcePropertyPair> list)
		{
			if (list != null)
			{
				foreach (SourcePropertyPair pair in list)
				{
					pair.Source.AddEventHandler (pair.Property, expression.HandleBreadcrumbChanged);
				}
			}
		}
		
		private void InternalDetachFromSource()
		{
			System.Diagnostics.Debug.Assert (this.sourceObject != null);
			
			switch (this.sourceType)
			{
				case BindingSourceType.PropertyObject:
					BindingExpression.Detach (this, this.sourceObject as Object, this.sourceProperty);
					BindingExpression.Detach (this, this.sourceBreadcrumbs);
					break;
			}

			this.sourceObject      = null;
			this.sourceProperty    = null;
			this.sourceType        = BindingSourceType.None;
			this.sourceBreadcrumbs = null;
		}

		private static void Detach(BindingExpression expression, List<SourcePropertyPair> list)
		{
			if (list != null)
			{
				foreach (SourcePropertyPair pair in list)
				{
					pair.Source.RemoveEventHandler (pair.Property, expression.HandleBreadcrumbChanged);
				}
			}
		}

		private void HandleSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.InternalUpdateTarget (e.NewValue);
		}
		private void HandleBreadcrumbChanged(object sender, PropertyChangedEventArgs e)
		{
			this.RefreshSourceBinding ();
		}

		private static void Attach(BindingExpression expression, Object source, Property property)
		{
			source.AddEventHandler (property, expression.HandleSourcePropertyChanged);
		}
		private static void Detach(BindingExpression expression, Object source, Property property)
		{
			source.RemoveEventHandler (property, expression.HandleSourcePropertyChanged);
		}

		private struct SourcePropertyPair
		{
			public SourcePropertyPair(Object source, Property property)
			{
				this.source = source;
				this.property = property;
			}
			
			public Object						Source
			{
				get
				{
					return this.source;
				}
			}
			public Property						Property
			{
				get
				{
					return this.property;
				}
			}
			
			private Object source;
			private Property property;
		}

		private Binding							binding;
		private Object							targetObject;
		private Property						targetPropery;
		private object							sourceObject;
		private Property						sourceProperty;
		private BindingSourceType				sourceType;
		private List<SourcePropertyPair>		sourceBreadcrumbs;
		private Binding							dataContext;
	}
}
