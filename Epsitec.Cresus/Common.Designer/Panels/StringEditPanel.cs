using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Panels
{
	/// <summary>
	/// La classe StringEditPanel réalise un panneau pour l'édition de
	/// textes contenus dans un bundle.
	/// </summary>
	public class StringEditPanel : AbstractPanel
	{
		public StringEditPanel(Support.Data.ITextArrayStore store)
		{
			this.size  = StringEditPanel.DefaultSize;
			this.store = store;
		}
		
		
		public EditArray						EditArray
		{
			get
			{
				if (this.edit_array == null)
				{
					Widget widget = this.Widget;
				}
				
				return this.edit_array;
			}
		}
		
		public static Drawing.Size				DefaultSize
		{
			get
			{
				return new Drawing.Size (500, 400);
			}
		}
		
		
		protected override void CreateWidgets(Widget parent)
		{
			double dx = parent.Client.Width;
			double dy = parent.Client.Height;
			
			this.edit_array = new EditArray (parent);
			
			EditArray.Header     title = new EditArray.Header (this.edit_array);
			EditArray.Controller ctrl  = new EditArray.Controller (this.edit_array, "Table");
			
			this.edit_array.CommandDispatcher = new Support.CommandDispatcher ("StringEditTable", true);
			this.edit_array.Bounds            = new Drawing.Rectangle (5, 5, dx - 10, dy - 10);
			this.edit_array.Anchor            = AnchorStyles.All;
			this.edit_array.ColumnCount       = 2;
			this.edit_array.RowCount          = 0;
			
			this.edit_array.SetColumnWidth (0, 160);
			this.edit_array.SetColumnWidth (1, this.edit_array.GetColumnWidth (1) + this.edit_array.FreeTableWidth);
			this.edit_array.SetHeaderText (0, "Clef");
			this.edit_array.SetHeaderText (1, "Valeur");
			this.edit_array.TextArrayStore = this.store;
			this.edit_array.TitleWidget = title;
			this.edit_array.SearchCaption = @"<b>Recherche. </b><font size=""90%"">Tapez le texte à chercher ci-dessous&#160;:</font>";
			
			this.edit_array.DoubleClicked += new MessageEventHandler (this.HandleEditArrayDoubleClicked);
			
			title.Caption = @"<font size=""120%"">Bundle.</font> Édition des données contenues dans la ressource.";
			
			ctrl.CreateCommands ();
			ctrl.CreateToolBarButtons ();
			ctrl.StartReadOnly ();
		}
		
		private void HandleEditArrayDoubleClicked(object sender, MessageEventArgs e)
		{
			int row, column;
			
			this.edit_array.HitTestTable (e.Point, out row, out column);
			this.edit_array.StartEdition (row, column);
		}
		
		
		protected EditArray						edit_array;
		protected Support.Data.ITextArrayStore	store;
	}
}
