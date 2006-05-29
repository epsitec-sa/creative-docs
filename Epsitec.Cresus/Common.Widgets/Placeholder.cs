//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Placeholder représente un conteneur utilisé par des widgets
	/// intelligents, remplis par data binding.
	/// </summary>
	public class Placeholder : AbstractGroup
	{
		public Placeholder()
		{
			Application.QueueAsyncCallback (this.CreateUserInterface);
		}
		
		public Placeholder(Widget embedder)
		{
			this.SetEmbedder (embedder);
		}


		public Binding ValueBinding
		{
			get
			{
				return this.GetBinding (Placeholder.ValueProperty);
			}
		}

		public object ValueTypeObject
		{
			get
			{
				return this.valueTypeObject;
			}
		}

		public string ValueName
		{
			get
			{
				return this.valueName;
			}
		}
		
		
		public object Value
		{
			get
			{
				return this.GetValue (Placeholder.ValueProperty);
			}
			set
			{
				this.SetValue (Placeholder.ValueProperty, value);
			}
		}

		public string Controller
		{
			get
			{
				return (string) this.GetValue (Placeholder.ControllerProperty);
			}
			set
			{
				this.SetValue (Placeholder.ControllerProperty, value);
			}
		}

		public string ControllerParameter
		{
			get
			{
				return (string) this.GetValue (Placeholder.ControllerParameterProperty);
			}
			set
			{
				this.SetValue (Placeholder.ControllerParameterProperty, value);
			}
		}

		private void DisposeUserInterface()
		{
			if (this.controller != null)
			{
				this.controller.DisposeUserInterface ();
				this.controller = null;
			}
		}

		private void CreateUserInterface()
		{
			if ((this.controller == null) &&
				(this.Controller != null))
			{
				this.controller = Controllers.Factory.CreateController (this.Controller, this.ControllerParameter);

				if (this.controller != null)
				{
					this.controller.Placeholder = this;
					this.controller.CreateUserInterface ();

					object value = this.Value;

					if (value != UndefinedValue.Instance)
					{
						this.controller.RefreshUserInterface (UndefinedValue.Instance, value);
					}
				}
			}
		}

		private void RecreateUserInterface()
		{
			if (this.controller != null)
			{
				this.DisposeUserInterface ();
				this.CreateUserInterface ();
			}
		}

		protected override void OnBindingChanged(DependencyProperty property)
		{
			if (property == Placeholder.ValueProperty)
			{
				this.UpdateValueTypeObject ();
			}
			
			base.OnBindingChanged (property);
		}

		private void UpdateValueTypeObject()
		{
			object oldValueTypeObject = this.valueTypeObject;
			object newValueTypeObject = null;
			string oldValueName = this.valueName;
			string newValueName = null;
			
			BindingExpression expression = this.GetBindingExpression (Placeholder.ValueProperty);

			if (expression != null)
			{
				newValueTypeObject = expression.GetSourceTypeObject ();
				newValueName = expression.GetSourceName ();
			}

			this.valueTypeObject = newValueTypeObject;
			this.valueName = newValueName;

			if (oldValueTypeObject == newValueTypeObject)
			{
				//	OK, do nothing...
			}
			else if ((oldValueTypeObject == null) ||
				/**/ (oldValueTypeObject.Equals (newValueTypeObject) == false))
			{
				this.UpdateValueTypeObject (oldValueTypeObject, newValueTypeObject);
			}

			if (oldValueName != newValueName)
			{
				this.UpdateValueName (oldValueName, newValueName);
			}
		}

		private void UpdateValueTypeObject(object oldValueTypeObject, object newValueTypeObject)
		{
			if (this.controller != null)
			{
				Application.QueueAsyncCallback (this.RecreateUserInterface);
			}
		}

		private void UpdateValueName(string oldValueName, string newValueName)
		{
			if (this.controller != null)
			{
				Application.QueueAsyncCallback (this.RecreateUserInterface);
			}
		}

		private void UpdateValue(object oldValue, object newValue)
		{
			if (this.controller != null)
			{
				this.controller.RefreshUserInterface (oldValue, newValue);
			}
		}


		private static void NotifyValueChanged(DependencyObject o, object oldValue, object newValue)
		{
			Placeholder that = (Placeholder) o;

			that.UpdateValueTypeObject ();
			that.UpdateValue (oldValue, newValue);
		}

		private static void NotifyControllerChanged(DependencyObject o, object oldValue, object newValue)
		{
			Placeholder that = (Placeholder) o;

			if (that.controller != null)
			{
				Application.QueueAsyncCallback (that.RecreateUserInterface);
			}
		}
		
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register ("Value", typeof (object), typeof (Placeholder), new DependencyPropertyMetadata (Placeholder.NotifyValueChanged));
		public static readonly DependencyProperty ControllerProperty = DependencyProperty.Register ("Controller", typeof (string), typeof (Placeholder), new DependencyPropertyMetadata (Placeholder.NotifyControllerChanged));
		public static readonly DependencyProperty ControllerParameterProperty = DependencyProperty.Register ("ControllerParameter", typeof (string), typeof (Placeholder), new DependencyPropertyMetadata (Placeholder.NotifyControllerChanged));


		private IController controller;
		private object valueTypeObject;
		private string valueName;
	}
}
