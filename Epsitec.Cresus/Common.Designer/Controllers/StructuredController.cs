//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Daniel ROUX

using System.Collections.Generic;

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.Controllers;
using Epsitec.Common.Widgets;

[assembly: Controller(typeof(Epsitec.Common.Designer.Controllers.StructuredController))]

namespace Epsitec.Common.Designer.Controllers
{
	public class StructuredController : AbstractController, Widgets.Layouts.IGridPermeable
	{
		public StructuredController(string parameter)
		{
		}

		public override object GetActualValue()
		{
			return this.field;
		}

		protected override Widgets.Layouts.IGridPermeable GetGridPermeableLayoutHelper()
		{
			return this;
		}

		protected override void CreateUserInterface(INamedType namedType, string valueName, Caption caption)
		{
			this.Placeholder.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

			this.button = new Button();
			this.button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.button.TabIndex = 1;
			this.button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.button.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.button.VerticalAlignment = VerticalAlignment.Stretch;
			this.button.Dock = DockStyle.Stacked;

			this.AddWidget(this.button);
		}

		protected override void PrepareUserInterfaceDisposal()
		{
			base.PrepareUserInterfaceDisposal();

			this.button.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
		}

		protected override void RefreshUserInterface(object oldValue, object newValue)
		{
			if (!UndefinedValue.IsUndefinedValue(newValue) &&
				!InvalidValue.IsInvalidValue(newValue) &&
				!PendingValue.IsPendingValue(newValue) &&
				newValue != null)
			{
				this.field = newValue as string;

				this.button.Text = this.field;
			}
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			MainWindow mainWindow = this.MainWindow;

			IProxy sourceProxy = this.Placeholder.ValueBinding.Source as IProxy;
			IEnumerable<Widget> sourceWidgets = sourceProxy.Widgets;  // liste des objets sélectionnés
			Widget sourceWidget = Collection.Extract<Widget>(sourceWidgets, 0);
			System.Diagnostics.Debug.Assert(sourceWidget != null);
			IStructuredType dataSource = DataObject.GetDataContext(sourceWidget) as IStructuredType;
			StructuredType type = dataSource.GetField("*").Type as StructuredType;
			string field = this.field;

			field = mainWindow.DlgStructuredSelector(mainWindow.CurrentModule, mainWindow.CurrentModule.AccessCaptions, type, field);
			if (field == null)  // annuler ?
			{
				return;
			}

			this.field = field;

			this.OnActualValueChanged();
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
			yield return new Widgets.Layouts.PermeableCell(this.button, column+0, row+0, columnSpan, 1);
		}

		bool Widgets.Layouts.IGridPermeable.UpdateGridSpan(ref int columnSpan, ref int rowSpan)
		{
			columnSpan = System.Math.Max(columnSpan, 2);
			rowSpan    = System.Math.Max(rowSpan, 1);
			
			return true;
		}
		#endregion


		private string					field;
		private Button					button;
	}
}
