//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

[assembly: Controller (typeof (Epsitec.Common.Widgets.Controllers.EnumController))]

namespace Epsitec.Common.Widgets.Controllers
{
	public class EnumController : AbstractController, Layouts.IGridPermeable
	{
		public EnumController(string parameter)
		{
		}

		public override object GetActualValue()
		{
			return this.combo.SelectedItem;
		}

		protected override Layouts.IGridPermeable GetGridPermeableLayoutHelper()
		{
			return this;
		}

		protected override void CreateUserInterface(INamedType namedType, string valueName)
		{
			this.Placeholder.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			IEnumType enumType = namedType as IEnumType;

			this.label = new StaticText ();
			this.combo = new TextFieldCombo ();

			this.label.HorizontalAlignment = HorizontalAlignment.Right;
			this.label.VerticalAlignment = VerticalAlignment.BaseLine;
			this.label.ContentAlignment = Drawing.ContentAlignment.MiddleRight;
			this.label.Dock = DockStyle.Stacked;
			
			if (! string.IsNullOrEmpty (valueName))
			{
				this.label.Text = string.Format ("{0} ", valueName);
				this.label.PreferredWidth = this.label.GetBestFitSize ().Width;
				this.label.Margins = new Drawing.Margins (4, 4, 0, 0);
			}
			
			this.combo.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.combo.VerticalAlignment = VerticalAlignment.BaseLine;
			this.combo.TextChanged += this.HandleComboTextChanged;
			this.combo.PreferredWidth = 40;

			this.combo.TabIndex = 1;
			this.combo.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.combo.Dock = DockStyle.Stacked;
			
			if (enumType != null)
			{
				foreach (IEnumValue enumValue in enumType.Values)
				{
					if (!enumValue.IsHidden)
					{
						this.combo.Items.Add (enumValue.Name);
					}
				}

				this.combo.IsReadOnly = enumType.IsCustomizable ? false : true;
			}
			
			this.AddWidget (this.label);
			this.AddWidget (this.combo);
		}

		protected override void PrepareUserInterfaceDisposal()
		{
			base.PrepareUserInterfaceDisposal ();
			
			this.combo.TextChanged -= this.HandleComboTextChanged;
		}

		protected override void RefreshUserInterface(object oldValue, object newValue)
		{
			if ((newValue != UndefinedValue.Instance) &&
				(newValue != InvalidValue.Instance) &&
				(newValue != null))
			{
				this.combo.Text = this.ConvertFromValue (newValue);
			}
		}

		private void HandleComboTextChanged(object sender)
		{
			this.OnActualValueChanged ();
		}

		private string ConvertFromValue(object newValue)
		{
			return newValue.ToString ();
		}
		
		#region IGridPermeable Members

		IEnumerable<Layouts.PermeableCell> Layouts.IGridPermeable.GetChildren(int column, int row, int columnSpan, int rowSpan)
		{
			yield return new Layouts.PermeableCell (this.label, column+0, row+0, 1, 1);
			yield return new Layouts.PermeableCell (this.combo, column+1, row+0, columnSpan-1, 1);
		}

		bool Layouts.IGridPermeable.UpdateGridSpan(ref int columnSpan, ref int rowSpan)
		{
			columnSpan = System.Math.Max (columnSpan, 2);
			rowSpan    = System.Math.Max (rowSpan, 1);
			
			return true;
		}

		#endregion
		
		private TextFieldCombo combo;
		private StaticText label;
	}
}
