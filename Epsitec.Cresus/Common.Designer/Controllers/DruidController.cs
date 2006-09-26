//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Controllers;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

[assembly: Controller(typeof(Epsitec.Common.Designer.Controllers.DruidController))]

namespace Epsitec.Common.Designer.Controllers
{
	public class DruidController : AbstractController, Widgets.Layouts.IGridPermeable
	{
		public DruidController(string parameter)
		{
		}

		public override object GetActualValue()
		{
			return this.druid;
		}

		protected override Widgets.Layouts.IGridPermeable GetGridPermeableLayoutHelper()
		{
			return this;
		}

		protected override void CreateUserInterface(INamedType namedType, string valueName, Caption caption)
		{
			this.Placeholder.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			this.button = new Button();
			this.button.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.button.TabIndex = 1;
			this.button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.button.Dock = DockStyle.Stacked;

			this.AddWidget(this.button);
		}

		protected override void PrepareUserInterfaceDisposal()
		{
			base.PrepareUserInterfaceDisposal ();

			this.button.Pressed -= new MessageEventHandler(this.HandleButtonPressed);
		}

		protected override void RefreshUserInterface(object oldValue, object newValue)
		{
			if ((newValue != UndefinedValue.Instance) &&
				(newValue != InvalidValue.Instance) &&
				(newValue != null))
			{
				this.druid = this.ConvertFromValue(newValue);
				this.button.Text = string.Format("Druid = {0}", this.druid);
			}
		}

		private void HandleButtonPressed(object sender, MessageEventArgs e)
		{
			//	TODO:
			Druid d = Druid.Parse(this.druid);
			Druid dd = new Druid(d.Module, d.Developer, d.Local+1);
			this.druid = dd.ToString();

			this.OnActualValueChanged();
		}

		private string ConvertFromValue(object newValue)
		{
			return newValue.ToString();
		}
		

		#region IGridPermeable Members
		IEnumerable<Widgets.Layouts.PermeableCell> Widgets.Layouts.IGridPermeable.GetChildren(int column, int row, int columnSpan, int rowSpan)
		{
			yield return new Widgets.Layouts.PermeableCell(this.button, column, row, columnSpan, 1);
		}

		bool Widgets.Layouts.IGridPermeable.UpdateGridSpan(ref int columnSpan, ref int rowSpan)
		{
			columnSpan = System.Math.Max(columnSpan, 2);
			rowSpan    = System.Math.Max(rowSpan, 1);
			
			return true;
		}
		#endregion


		private string druid;
		private Button button;
	}
}
