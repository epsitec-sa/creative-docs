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
			this.Placeholder.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

			this.title = new StaticText();
			this.title.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.title.VerticalAlignment = VerticalAlignment.Top;
			this.title.ContentAlignment = Drawing.ContentAlignment.TopCenter;
			this.title.Dock = DockStyle.Stacked;
			this.title.PreferredHeight = 24;

			this.button = new Button();
			this.button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.button.TabIndex = 1;
			this.button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.button.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.button.VerticalAlignment = VerticalAlignment.Stretch;
			this.button.Dock = DockStyle.Stacked;

			this.AddWidget(this.title);
			this.AddWidget(this.button);
		}

		protected override void PrepareUserInterfaceDisposal()
		{
			base.PrepareUserInterfaceDisposal();

			this.button.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
		}

		protected override void RefreshUserInterface(object oldValue, object newValue)
		{
			if (newValue != UndefinedValue.Instance &&
				newValue != InvalidValue.Instance &&
				newValue != null)
			{
				this.druid = this.ConvertFromValue(newValue);

				Druid d = Druid.Parse(this.druid);
				MainWindow mainWindow = this.MainWindow;
				Module module = mainWindow.SearchModule(d);

				if (module == null)
				{
					this.title.Text = "";
					this.button.Text = "";
					this.button.PreferredHeight = 8+14*1;  // place pour une seule ligne
				}
				else
				{
					string text = ResourceAccess.TypeDisplayName(module.AccessCaptions.DirectGetType(d), false);
					this.title.Text = string.Concat("<font size=\"150%\"><b>", text, "</b></font>");

					if (module == mainWindow.CurrentModule)
					{
						string part2 = module.AccessCaptions.DirectGetDisplayName(d);
						this.button.Text = part2;
						this.button.PreferredHeight = 8+14*1;  // place pour une seule ligne
					}
					else
					{
						string part1 = Misc.Italic(module.ModuleInfo.Name);
						string part2 = module.AccessCaptions.DirectGetDisplayName(d);
						this.button.Text = string.Concat(part1, "<br/>", part2);
						this.button.PreferredHeight = 8+14*2;  // place pour deux lignes
					}
				}
			}
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			MainWindow mainWindow = this.MainWindow;

			Druid d = Druid.Parse(this.druid);
			d = mainWindow.DlgResourceSelector(mainWindow.CurrentModule, ResourceAccess.Type.Unknow, d);
			this.druid = d.ToString();

			this.OnActualValueChanged();
		}

		private string ConvertFromValue(object newValue)
		{
			return newValue.ToString();
		}

		private MainWindow MainWindow
		{
			get
			{
				MainWindow mainWindow = MainWindow.GetInstance(this.Placeholder.Window);
				System.Diagnostics.Debug.Assert(mainWindow != null);
				return mainWindow;
			}
		}
		

		#region IGridPermeable Members
		IEnumerable<Widgets.Layouts.PermeableCell> Widgets.Layouts.IGridPermeable.GetChildren(int column, int row, int columnSpan, int rowSpan)
		{
			yield return new Widgets.Layouts.PermeableCell(this.title, column+0, row+0, columnSpan, 1);
			yield return new Widgets.Layouts.PermeableCell(this.button, column+0, row+1, columnSpan, rowSpan-1);
		}

		bool Widgets.Layouts.IGridPermeable.UpdateGridSpan(ref int columnSpan, ref int rowSpan)
		{
			columnSpan = System.Math.Max(columnSpan, 2);
			rowSpan    = System.Math.Max(rowSpan, 2);
			
			return true;
		}
		#endregion


		private string					druid;
		private StaticText				title;
		private Button					button;
	}
}
