//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class BindingExpression
	{
		public BindingExpression()
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
		
		internal static BindingExpression BindToTarget(Object target, Property property, Binding binding)
		{
			BindingExpression expression = new BindingExpression ();
			
			expression.binding = binding;
			expression.targetObject = target;
			expression.targetPropery = property;
			
			return expression;
		}
		
		private void AssertBinding()
		{
			if (this.binding == null)
			{
				throw new System.InvalidOperationException ("Broken binding");
			}
		}
		private void InternalUpdateSource()
		{
			//	TODO: update source
		}
		private void InternalUpdateTarget()
		{
			//	TODO: update target
		}

		private Binding							binding;
		private Object							targetObject;
		private Property						targetPropery;
	}
}
