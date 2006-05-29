//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

[assembly: Controller (typeof (Epsitec.Common.Widgets.Controllers.StringController))]

namespace Epsitec.Common.Widgets.Controllers
{
	public class StringController : AbstractController
	{
		public StringController(string parameter)
		{
		}
		
		protected override void CreateUserInterface(object valueTypeObject, string valueName)
		{
			this.typeObject = valueTypeObject;

			this.label = new StaticText ();
			this.field = new TextField ();

			this.label.Dock = DockStyle.Left;
			this.label.HorizontalAlignment = HorizontalAlignment.Right;
			this.label.ContentAlignment = Epsitec.Common.Drawing.ContentAlignment.MiddleRight;
			
			if (! string.IsNullOrEmpty (valueName))
			{
				this.label.Text = string.Format ("{0}: ", valueName);
			}
			
			this.field.Dock = DockStyle.Fill;
			this.field.TextChanged += this.HandleFieldTextChanged;

			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.AddWidget (this.label);
			this.AddWidget (this.field);
		}

		protected override void PrepareUserInterfaceDisposal()
		{
			base.PrepareUserInterfaceDisposal ();
			
			this.field.TextChanged -= this.HandleFieldTextChanged;
		}

		protected override void RefreshUserInterface(object oldValue, object newValue)
		{
			if ((newValue != UndefinedValue.Instance) &&
				(newValue != InvalidValue.Instance) &&
				(newValue != null))
			{
				this.field.Text = this.ConvertFromValue (newValue);
			}
		}

		private void HandleFieldTextChanged(object sender)
		{
			object value = this.ConvertToValue (this.field.Text);

			if (value != InvalidValue.Instance)
			{
				this.Placeholder.Value = value;
			}
		}

		private object ConvertToValue(string text)
		{
			System.Type type  = TypeRosetta.GetSystemTypeFromTypeObject (this.typeObject);
			object      value = InvalidValue.Instance;

			if (type == typeof (string))
			{
				value = text;
			}
			else if (type == typeof (int))
			{
				int result;

				if (int.TryParse (text, out result))
				{
					value = result;
				}
			}
			else if (type == typeof (decimal))
			{
				decimal result;

				if (decimal.TryParse (text, out result))
				{
					value = result;
				}
			}

			if (value != InvalidValue.Instance)
			{
				IDataConstraint constraint = this.typeObject as IDataConstraint;

				if (constraint != null)
				{
					if (constraint.ValidateValue (value) == false)
					{
						value = InvalidValue.Instance;
					}
				}
			}
			
			return value;
		}


		private string ConvertFromValue(object newValue)
		{
			return newValue.ToString ();
		}
		
		private TextField field;
		private StaticText label;
		private object typeObject;
	}
}
