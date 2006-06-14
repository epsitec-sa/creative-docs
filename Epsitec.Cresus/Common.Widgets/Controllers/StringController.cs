//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

[assembly: Controller (typeof (Epsitec.Common.Widgets.Controllers.StringController))]

namespace Epsitec.Common.Widgets.Controllers
{
	public class StringController : AbstractController, Layouts.IGridPermeable
	{
		public StringController(string parameter)
		{
		}

		protected override Layouts.IGridPermeable GetGridPermeableLayoutHelper()
		{
			return this;
		}
		
		protected override void CreateUserInterface(object valueTypeObject, string valueName)
		{
			this.Placeholder.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			this.typeObject = valueTypeObject;

			this.label = new StaticText ();
			this.field = new TextField ();

			this.label.HorizontalAlignment = HorizontalAlignment.Right;
			this.label.VerticalAlignment = VerticalAlignment.BaseLine;
			this.label.ContentAlignment = Drawing.ContentAlignment.MiddleRight;
			this.label.Dock = DockStyle.Stacked;
			
			if (! string.IsNullOrEmpty (valueName))
			{
				this.label.Text = string.Format ("{0}: ", valueName);
				this.label.PreferredWidth = this.label.GetBestFitSize ().Width;
				this.label.Margins = new Drawing.Margins (4, 4, 0, 0);
			}
			
			this.field.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.field.VerticalAlignment = VerticalAlignment.BaseLine;
			this.field.TextChanged += this.HandleFieldTextChanged;
			this.field.PreferredWidth = 40;

			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.field.Dock = DockStyle.Stacked;
			
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
			else if (type == typeof (double))
			{
				double result;

				if (double.TryParse (text, out result))
				{
					value = result;
				}
			}

			if (value != InvalidValue.Instance)
			{
				IDataConstraint constraint = this.typeObject as IDataConstraint;

				if (constraint != null)
				{
					if (constraint.IsValidValue (value) == false)
					{
						value = InvalidValue.Instance;
					}
				}
			}
			
			if (value == InvalidValue.Instance)
			{
				System.Diagnostics.Debug.WriteLine ("Invalid value: " + text);
			}
			
			return value;
		}


		private string ConvertFromValue(object newValue)
		{
			return newValue.ToString ();
		}
		
		#region IGridPermeable Members

		IEnumerable<Layouts.PermeableCell> Layouts.IGridPermeable.GetChildren(int column, int row, int columnSpan, int rowSpan)
		{
			yield return new Layouts.PermeableCell (this.label, column+0, row+0, 1, 1);
			yield return new Layouts.PermeableCell (this.field, column+1, row+0, columnSpan-1, 1);
		}

		bool Layouts.IGridPermeable.UpdateGridSpan(ref int columnSpan, ref int rowSpan)
		{
			columnSpan = System.Math.Max (columnSpan, 2);
			rowSpan    = System.Math.Max (rowSpan, 1);
			
			return true;
		}

		#endregion
		
		private TextField field;
		private StaticText label;
		private object typeObject;
	}
}
