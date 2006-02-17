//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

		internal void AttachToSource()
		{
			Object            sourceObject;
			Property          sourceProperty;
			BindingSourceType sourceType;

			if (this.AnalyseSource (out sourceObject, out sourceProperty, out sourceType))
			{
				this.sourceObject   = sourceObject;
				this.sourceProperty = sourceProperty;
				this.sourceType     = sourceType;
				
				this.InternalAttachToSource ();
			}
		}
		internal void DetachFromSource()
		{
			this.InternalDetachFromSource ();
		}
		
		internal static BindingExpression BindToTarget(Object target, Property property, Binding binding)
		{
			BindingExpression expression = new BindingExpression ();

			binding.Add (expression);
			
			expression.binding = binding;
			expression.targetObject = target;
			expression.targetPropery = property;
			
			return expression;
		}

		#region IDisposable Members
		public void Dispose()
		{
			//	Detach from the target, and from the source.
			
			this.AssertBinding ();

			this.binding.Remove (this);
			this.InternalDetachFromSource ();
			
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
		
		private bool AnalyseSource(out Object source, out Property property, out BindingSourceType type)
		{
			type     = BindingSourceType.None;
			source   = this.binding.Source as Object;
			property = null;

			PropertyPath path = this.binding.Path;
			
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

				return true;
			}
			
			return false;
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
			if (this.sourceObject != null)
			{
				switch (this.sourceType)
				{
					case BindingSourceType.PropertyObject:
						BindingExpression.Attach (this, this.sourceObject as Object, this.sourceProperty);
						break;
				}
			}
		}
		private void InternalDetachFromSource()
		{
			if (this.sourceObject != null)
			{
				switch (this.sourceType)
				{
					case BindingSourceType.PropertyObject:
						BindingExpression.Detach (this, this.sourceObject as Object, this.sourceProperty);
						break;
				}
			}
		}

		private void HandleSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.InternalUpdateTarget (e.NewValue);
		}

		private static void Attach(BindingExpression expression, Object source, Property property)
		{
			source.AddEventHandler (property, new PropertyChangedEventHandler (expression.HandleSourcePropertyChanged));
		}
		private static void Detach(BindingExpression expression, Object source, Property property)
		{
			source.RemoveEventHandler (property, new PropertyChangedEventHandler (expression.HandleSourcePropertyChanged));
		}

		private Binding							binding;
		private Object							targetObject;
		private Property						targetPropery;
		private object							sourceObject;
		private Property						sourceProperty;
		private BindingSourceType				sourceType;
	}
}
