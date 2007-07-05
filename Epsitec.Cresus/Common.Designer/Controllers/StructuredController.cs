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

[assembly: Controller(typeof(StructuredController))]

namespace Epsitec.Common.Designer.Controllers
{
	public class StructuredController : AbstractController, Widgets.Layouts.IGridPermeable
	{
		public StructuredController(string parameter)
		{
		}

		public override object GetActualValue()
		{
			return this.structuredType;
		}

		protected override Widgets.Layouts.IGridPermeable GetGridPermeableLayoutHelper()
		{
			return this;
		}

		protected override void CreateUserInterface(INamedType namedType, Caption caption)
		{
			this.Placeholder.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

			this.button = new Button();
			this.button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.button.TabIndex = 1;
			this.button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.button.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.button.VerticalAlignment = VerticalAlignment.Stretch;
			this.button.Dock = DockStyle.Stacked;
			ToolTip.Default.SetToolTip(this.button, Res.Strings.Panel.Content.Tooltip.ChangeStructured);

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
				if (newValue == null)
				{
					this.button.Text = "";
				}
				else
				{
					this.structuredType = newValue as StructuredType;
					this.button.Text = this.structuredType.Caption.Name;
				}
			}
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			DesignerApplication mainWindow = this.MainWindow;
			Module module = mainWindow.CurrentModule;

			Druid druid = Druid.Empty;

			if (this.structuredType != null)
			{
				druid = this.structuredType.CaptionId;
			}

			Common.Dialogs.DialogResult result = mainWindow.DlgResourceSelector(Dialogs.ResourceSelector.Operation.Selection, module, ResourceAccess.Type.Types, ref druid, null);
			if (result != Common.Dialogs.DialogResult.Yes)  // annuler ?
			{
				return;
			}

			AbstractType at = module.AccessCaptions.DirectGetAbstractType(druid);
			System.Diagnostics.Debug.Assert(at is StructuredType);
			this.structuredType = at as StructuredType;

			this.OnActualValueChanged();
		}


		private StructuredType StructuredType
		{
			get
			{
				IProxy sourceProxy = this.Placeholder.ValueBinding.Source as IProxy;
				IEnumerable<Widget> sourceWidgets = sourceProxy.Widgets;  // liste des objets sélectionnés
				Widget sourceWidget = Collection.Extract<Widget>(sourceWidgets, 0);
				System.Diagnostics.Debug.Assert(sourceWidget != null);
				IStructuredType dataSource = DataObject.GetDataContext(sourceWidget).Source as IStructuredType;
				return dataSource.GetField("*").Type as StructuredType;
			}
		}

		private DesignerApplication MainWindow
		{
			get
			{
				DesignerApplication mainWindow = DesignerApplication.GetInstance(this.Placeholder.Window);
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


		private StructuredType			structuredType;
		private Button					button;
	}
}
