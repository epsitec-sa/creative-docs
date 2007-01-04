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

[assembly: Controller(typeof(BindingController))]

namespace Epsitec.Common.Designer.Controllers
{
	public class BindingController : AbstractController, Widgets.Layouts.IGridPermeable
	{
		public BindingController(string parameter)
		{
		}

		public override object GetActualValue()
		{
			return this.binding;
		}

		protected override Widgets.Layouts.IGridPermeable GetGridPermeableLayoutHelper()
		{
			return this;
		}

		protected override void CreateUserInterface(INamedType namedType, Caption caption)
		{
			this.Placeholder.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

			this.title = new StaticText();
			this.title.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.title.VerticalAlignment = VerticalAlignment.Top;
			this.title.ContentAlignment = ContentAlignment.TopCenter;
			this.title.Dock = DockStyle.Stacked;
			this.title.Margins = new Margins(0, 0, 10, 0);
			this.title.PreferredHeight = 24;

			this.button = new Button();
			this.button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			this.button.TabIndex = 1;
			this.button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.button.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.button.VerticalAlignment = VerticalAlignment.Stretch;
			this.button.Dock = DockStyle.Stacked;
			ToolTip.Default.SetToolTip(this.button, Res.Strings.Panel.Content.Tooltip.ChangeBinding);

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
			if (!UndefinedValue.IsUndefinedValue(newValue) &&
				!InvalidValue.IsInvalidValue(newValue) &&
				!PendingValue.IsPendingValue(newValue))
			{
				StructuredType type = this.StructuredType;
				if (type == null)
				{
					this.title.Text = "";
				}
				else
				{
					string text = type.Caption.Name;
					this.title.Text = string.Concat("<font size=\"150%\"><b>", text, "</b></font>");
				}

				if (newValue == null)
				{
					this.button.Text = Misc.Italic(Res.Strings.Dialog.BindingSelector.Button.Inherit);
				}
				else
				{
					this.binding = newValue as Binding;

					string path = this.binding == null ? "" : this.binding.Path;
					string field = path.StartsWith("*.") ? path.Substring(2) : path;
					this.button.Text = field;
				}
			}
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			MainWindow mainWindow = this.MainWindow;
			StructuredType type = this.StructuredType;
			Binding binding = this.binding;
			ObjectModifier.ObjectType oType = ObjectModifier.ObjectType.Unknow;

			IProxy sourceProxy = this.Placeholder.ValueBinding.Source as IProxy;
			IEnumerable<Widget> sourceWidgets = sourceProxy.Widgets;  // liste des objets sélectionnés
			foreach (Widget obj in sourceWidgets)
			{
				oType = ObjectModifier.GetObjectType(obj);
			}

			if (!mainWindow.DlgBindingSelector(mainWindow.CurrentModule, type, oType, ref binding))
			{
				return;
			}

			this.binding = binding;

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


		private Binding					binding;
		private Button					button;
		private StaticText				title;
	}
}
