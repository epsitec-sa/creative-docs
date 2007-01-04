//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Daniel ROUX

using System.Collections.Generic;

using Epsitec.Common.Designer.Controllers;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.Controllers;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

[assembly: Controller(typeof(TableController))]

namespace Epsitec.Common.Designer.Controllers
{
	public class TableController : AbstractController, Widgets.Layouts.IGridPermeable
	{
		public TableController(string parameter)
		{
		}

		public override object GetActualValue()
		{
			return this.columns;
		}

		protected override Widgets.Layouts.IGridPermeable GetGridPermeableLayoutHelper()
		{
			return this;
		}

		protected override void CreateUserInterface(INamedType namedType, Caption caption)
		{
			this.Placeholder.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

			this.button = new Button();
			this.button.Text = Res.Strings.Dialog.TableDescription.Title;
			this.button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.button.TabIndex = 1;
			this.button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.button.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.button.VerticalAlignment = VerticalAlignment.Stretch;
			this.button.Dock = DockStyle.Stacked;
			this.button.Margins = new Margins(0, 0, 3, 0);
			//?ToolTip.Default.SetToolTip(this.button, Res.Strings.Panel.Content.Tooltip.ChangeBinding);

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
				!PendingValue.IsPendingValue(newValue))
			{
				IEnumerable<UI.ItemTableColumn> source = newValue as IEnumerable<UI.ItemTableColumn>;
				List<UI.ItemTableColumn> list = new List<ItemTableColumn>(source);
				this.columns = list;
			}
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			MainWindow mainWindow = this.MainWindow;

			IProxy sourceProxy = this.Placeholder.ValueBinding.Source as IProxy;
			IEnumerable<Widget> sourceWidgets = sourceProxy.Widgets;  // liste des objets sélectionnés
			foreach (Widget obj in sourceWidgets)
			{
				ObjectModifier.ObjectType type = ObjectModifier.GetObjectType(obj);
				if (type == ObjectModifier.ObjectType.Table)
				{
					StructuredType structuredType = ObjectModifier.GetTableStructuredType(obj);
					List<UI.ItemTableColumn> columns = mainWindow.DlgTableConfiguration(mainWindow.CurrentModule, structuredType, this.columns);
					if (columns != null)
					{
						this.columns = columns;
						this.OnActualValueChanged();
					}
					return;
				}
			}
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


		private List<UI.ItemTableColumn>	columns;
		private Button						button;
	}
}
