//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (AbstractPlaceholder))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>AbstractPlaceholder</c> class is the base class for the
	/// <see cref="Placeholder"/> and <see cref="PanelPlaceholder"/> classes.
	/// Data binding with a source is done by attaching a <c>Binding</c> to
	/// the <c>AbstractPlaceholder.ValueProperty</c>.
	/// </summary>
	public abstract class AbstractPlaceholder : Widgets.FrameBox
	{
		protected AbstractPlaceholder()
		{
		}


		/// <summary>
		/// Gets the binding for the value property.
		/// </summary>
		/// <value>The binding for the value property.</value>
		public Binding							ValueBinding
		{
			get
			{
				BindingExpression expression = this.ValueBindingExpression;
				return expression == null ? null : expression.ParentBinding;
			}
		}

		/// <summary>
		/// Gets the binding expression for the value property.
		/// </summary>
		/// <value>The binding expression for the value property.</value>
		public BindingExpression				ValueBindingExpression
		{
			get
			{
				return this.GetBindingExpression (this.GetValueProperty ());
			}
		}

		/// <summary>
		/// Gets the type of the data-bound value.
		/// </summary>
		/// <value>The type of the data-bound value.</value>
		public INamedType						ValueType
		{
			get
			{
				return this.valueType;
			}
		}

		/// <summary>
		/// Gets the name of the data-bound value.
		/// </summary>
		/// <value>The name of the data-bound value.</value>
		public string							ValueName
		{
			get
			{
				return this.valueName;
			}
		}

		/// <summary>
		/// Gets the caption of the data-bound value.
		/// </summary>
		/// <value>The caption of the data-bound value.</value>
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

				Support.ICaptionResolver resolver = Widgets.Helpers.VisualTree.GetCaptionResolver (this);
				return Support.CaptionCache.Instance.GetCaption (resolver, captionId);
			}
		}

		/// <summary>
		/// Gets or sets the value of the data-bound value.
		/// </summary>
		/// <value>The value.</value>
		public virtual object					Value
		{
			get
			{
				return this.GetValue (AbstractPlaceholder.ValueProperty);
			}
			set
			{
				if (value == UndefinedValue.Value)
				{
					this.ClearValue (AbstractPlaceholder.ValueProperty);
				}
				else
				{
					this.SetValue (AbstractPlaceholder.ValueProperty, value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the placeholder suggestion mode.
		/// </summary>
		/// <value>The suggestion mode.</value>
		public PlaceholderSuggestionMode		SuggestionMode
		{
			get
			{
				return (PlaceholderSuggestionMode) this.GetValue (Placeholder.SuggestionModeProperty);
			}
			set
			{
				this.SetValue (Placeholder.SuggestionModeProperty, value);
			}
		}

		/// <summary>
		/// Gets the controller instance associated with this placeholder.
		/// </summary>
		/// <value>The controller instance.</value>
		public abstract IController				ControllerInstance
		{
			get;
		}

		public virtual DependencyProperty GetValueProperty()
		{
			return AbstractPlaceholder.ValueProperty;
		}

		/// <summary>
		/// Simulates an edition of the value, which is useful to force a refresh
		/// of the user interface.
		/// </summary>
		public virtual void SimulateEdition()
		{
			object value = this.Value;
			this.InvalidateProperty (Placeholder.ValueProperty, value, value);
		}

		/// <summary>
		/// Gets the type of the value after executing <c>UpdateValueType</c>.
		/// This method is reserved for internal use.
		/// </summary>
		/// <returns>The named type for the value or <c>null</c>.</returns>
		internal INamedType InternalUpdateValueType()
		{
			this.UpdateValueType ();
			return this.valueType;
		}

		/// <summary>
		/// Sets the value of this placeholder; called by <see cref="Controllers.AbstractController"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		public void InternalControllerSetValue(object value)
		{
			this.Value = value;
		}

		/// <summary>
		/// Forces an update as if the placeholder changed from the old value
		/// to the new value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		public void InternalUpdateValue(object oldValue, object newValue)
		{
			this.UpdateValue (oldValue, newValue);
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

		private void UpdateValueType()
		{
			INamedType oldValueType = this.valueType;
			INamedType newValueType = null;
			string oldValueName = this.valueName;
			string newValueName = null;

			BindingExpression expression = this.ValueBindingExpression;

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
				     (oldValueType.Equals (newValueType) == false))
			{
				this.UpdateValueType (oldValueType, newValueType);
			}

			if (oldValueName != newValueName)
			{
				this.UpdateValueName (oldValueName, newValueName);
			}
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

		protected void HandleValueChanged(object oldValue, object newValue)
		{
			this.UpdateValueType ();
			this.UpdateValue (oldValue, newValue);
		}

		private static void NotifyValueChanged(DependencyObject o, object oldValue, object newValue)
		{
			AbstractPlaceholder that = (AbstractPlaceholder) o;
			that.HandleValueChanged (oldValue, newValue);
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register ("Value", typeof (object), typeof (AbstractPlaceholder), new DependencyPropertyMetadata (UndefinedValue.Value, AbstractPlaceholder.NotifyValueChanged).MakeNotSerializable ());
		public static readonly DependencyProperty SuggestionModeProperty = DependencyProperty.Register ("SuggestionMode", typeof (PlaceholderSuggestionMode), typeof (AbstractPlaceholder), new DependencyPropertyMetadata (PlaceholderSuggestionMode.None));
		
		private INamedType						valueType;
		private string							valueName;
	}
}
