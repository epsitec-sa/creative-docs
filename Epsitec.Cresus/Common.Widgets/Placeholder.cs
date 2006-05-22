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
			if (this.controller == null)
			{
				this.controller = Controllers.Factory.CreateController (this.Controller, this.ControllerParameter);

				if (this.controller != null)
				{
					this.controller.Placeholder = this;
					this.controller.CreateUserInterface ();
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
			
			BindingExpression expression = this.GetBindingExpression (Placeholder.ValueProperty);

			if (expression != null)
			{
				newValueTypeObject = expression.GetSourceTypeObject ();
			}

			this.valueTypeObject = newValueTypeObject;

			if (oldValueTypeObject == newValueTypeObject)
			{
			}
			else if ((oldValueTypeObject == null) ||
				/**/ (oldValueTypeObject.Equals (newValueTypeObject) == false))
			{
				//	TODO: signaler le changement de type
			}
		}

		private void UpdateValue(object oldValue, object newValue)
		{
			//	TODO: signale le changement au contrôleur
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
	}
}
