//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Script.Developer.Panels
{
	/// <summary>
	/// Summary description for ParameterInfoPanel.
	/// </summary>
	public class ParameterInfoPanel : AbstractPanel
	{
		public ParameterInfoPanel()
		{
			this.size = ParameterInfoPanel.DefaultSize;
			this.edit_store = new Helpers.ParameterInfoStore ();
		}
		
		
		public static Drawing.Size				DefaultSize
		{
			get
			{
				return new Drawing.Size (200, 120);
			}
		}
		
		public Source.ParameterInfo[]			Parameters
		{
			get
			{
				return this.edit_store.GetContents ();
			}
			set
			{
				this.DetachStore ();
				this.edit_store = new Helpers.ParameterInfoStore (value);
				this.AttachStore ();
			}
		}
		
		
#if false
		internal EditArray						EditArray
		{
			get
			{
				return this.edit_array;
			}
		}
#endif
		
		
		protected virtual void DetachStore()
		{
			if (this.edit_store != null)
			{
				this.edit_store.StoreContentsChanged -= new EventHandler(this.HandleEditStoreContentsChanged);
			}
		}
		
		protected virtual void AttachStore()
		{
			if (this.edit_store != null)
			{
#if false
				if (this.edit_array != null)
				{
					this.edit_array.TextArrayStore = this.edit_store;
				}
#endif		
				this.edit_store.StoreContentsChanged += new EventHandler(this.HandleEditStoreContentsChanged);
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.local_dispatcher != null)
				{
					this.local_dispatcher.Dispose ();
					this.local_dispatcher = null;
				}
			}
			
			base.Dispose (disposing);
		}
		
		protected override void CreateWidgets(Widget parent)
		{
#if false
			System.Diagnostics.Debug.Assert (this.edit_store != null);
			System.Diagnostics.Debug.Assert (this.edit_array == null);
			System.Diagnostics.Debug.Assert (this.local_dispatcher == null);
			
			this.local_dispatcher = new CommandDispatcher ("ParameterInfoPanel", CommandDispatcherLevel.Secondary);
			
			EditArray            edit  = new EditArray (parent);
			EditArray.Header     title = new EditArray.Header (edit);
			EditArray.Controller ctrl  = new EditArray.Controller (edit, "Table");
			
			edit.AttachCommandDispatcher (this.local_dispatcher);
			edit.Dock        = DockStyle.Fill;
			edit.Margins     = new Drawing.Margins (0, 0, 0, 0);
			edit.ColumnCount = 3;
			edit.RowCount    = 0;
			
			edit.HScroller.Hide ();
			
			TextFieldCombo column_0_edit_model = new TextFieldCombo ();
			TextFieldCombo column_1_edit_model = new TextFieldCombo ();
			TextFieldEx    column_2_edit_model = new TextFieldEx ();
			
			column_0_edit_model.IsReadOnly = true;
			column_1_edit_model.IsReadOnly = true;
			column_0_edit_model.ButtonShowCondition = ShowCondition.WhenFocused;
			column_1_edit_model.ButtonShowCondition = ShowCondition.WhenFocused;
			
			this.edit_store.FillDirectionNames (column_0_edit_model.Items);
			this.edit_store.FillTypeNames (column_1_edit_model.Items);
			
			column_2_edit_model.ButtonShowCondition = ShowCondition.WhenModified;
			column_2_edit_model.DefocusAction       = DefocusAction.Modal;
			
			new Widgets.Validators.RegexValidator (column_2_edit_model, Support.RegexFactory.AlphaNumName, false);
			new EditArray.UniqueValueValidator (column_2_edit_model, 2);
			
			edit.Columns[0].HeaderText = "Dir.";
			edit.Columns[0].Width      = 40;
			edit.Columns[0].EditionWidgetModel = column_0_edit_model;
			edit.Columns[1].HeaderText = "Type";
			edit.Columns[1].EditionWidgetModel = column_1_edit_model;
			edit.Columns[1].Width      = 80;
			edit.Columns[1].Elasticity = 0.5;
			edit.Columns[2].HeaderText = "Name";
			edit.Columns[2].EditionWidgetModel = column_2_edit_model;
			edit.Columns[2].Elasticity = 1.0;
			
			ctrl.CreateCommands ();
			ctrl.CreateToolBarButtons ();
			ctrl.StartReadOnly ();
			
			edit.TextArrayStore = this.edit_store;
			
			title.Caption = "Method arguments";
			
			this.edit_array = edit;
#endif
		}
		
		
		private void HandleEditStoreContentsChanged(object sender)
		{
			this.IsModified = true;
		}
		
		
		protected Helpers.ParameterInfoStore	edit_store;
#if false
		protected EditArray						edit_array;
#endif
		protected CommandDispatcher				local_dispatcher;
	}
}
