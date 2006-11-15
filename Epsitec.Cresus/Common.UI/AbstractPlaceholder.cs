//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (AbstractPlaceholder))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class AbstractPlaceholder : AbstractGroup
	{
		protected AbstractPlaceholder()
		{
		}
		

		public Binding							ValueBinding
		{
			get
			{
				return this.GetBinding (AbstractPlaceholder.ValueProperty);
			}
		}
		
		public BindingExpression				ValueBindingExpression
		{
			get
			{
				return this.GetBindingExpression (AbstractPlaceholder.ValueProperty);
			}
		}


		public INamedType						ValueType
		{
			get
			{
				return this.valueType;
			}
		}

		public string							ValueName
		{
			get
			{
				return this.valueName;
			}
		}
		
		public Caption							ValueCaption
		{
			get
			{
				BindingExpression expression = this.ValueBindingExpression;
				
				if (expression == null)
				{
					return null;
				}

				Support.Druid captionId = expression.GetSourceCaptionId ();

				if (captionId.IsEmpty)
				{
					return null;
				}
				
				Support.ResourceManager manager = Widgets.Helpers.VisualTree.GetResourceManager (this);
				
				return Support.CaptionCache.Instance.GetCaption (manager, captionId);
			}
		}
		
		
		public object							Value
		{
			get
			{
				return this.GetValue (AbstractPlaceholder.ValueProperty);
			}
			set
			{
				this.SetValue (AbstractPlaceholder.ValueProperty, value);
			}
		}

		protected override void OnBindingChanged(DependencyProperty property)
		{
			if (property == AbstractPlaceholder.ValueProperty)
			{
				this.OnValueBindingChanged ();
			}

			base.OnBindingChanged (property);
		}

		protected virtual void OnValueBindingChanged()
		{
			this.UpdateValueType ();
		}

		internal INamedType UpdateValueType()
		{
			INamedType oldValueType = this.valueType;
			INamedType newValueType = null;
			string oldValueName = this.valueName;
			string newValueName = null;

			BindingExpression expression = this.GetBindingExpression (AbstractPlaceholder.ValueProperty);

			if (expression != null)
			{
				newValueType = expression.GetSourceNamedType ();
				newValueName = expression.GetSourceName ();
			}

			this.valueType = newValueType;
			this.valueName = newValueName;

			if (oldValueType == newValueType)
			{
				//	OK, do nothing...
			}
			else if ((oldValueType == null) ||
				/**/ (oldValueType.Equals (newValueType) == false))
			{
				this.UpdateValueType (oldValueType, newValueType);
			}

			if (oldValueName != newValueName)
			{
				this.UpdateValueName (oldValueName, newValueName);
			}

			return this.valueType;
		}

		protected virtual void UpdateValueType(object oldValueType, object newValueType)
		{
		}

		protected virtual void UpdateValueName(string oldValueName, string newValueName)
		{
		}

		protected virtual void UpdateValue(object oldValue, object newValue)
		{
		}

		protected virtual void HandleValueChanged(object oldValue, object newValue)
		{
			this.UpdateValueType ();
			this.UpdateValue (oldValue, newValue);
		}

		private static void NotifyValueChanged(DependencyObject o, object oldValue, object newValue)
		{
			AbstractPlaceholder that = (AbstractPlaceholder) o;
			that.HandleValueChanged (oldValue, newValue);
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register ("Value", typeof (object), typeof (AbstractPlaceholder), new DependencyPropertyMetadata (AbstractPlaceholder.NotifyValueChanged));
		
		private INamedType						valueType;
		private string							valueName;
	}
}
