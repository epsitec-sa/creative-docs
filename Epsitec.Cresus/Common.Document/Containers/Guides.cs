using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Guides contient tous les panneaux des repères.
	/// </summary>
	[SuppressBundleSupport]
	public class Guides : Abstract
	{
		public Guides(Document document) : base(document)
		{
			this.toolBar = new HToolBar(this);
			this.toolBar.Dock = DockStyle.Top;
			this.toolBar.DockMargins = new Margins(0, 0, 0, -1);

			this.buttonNewH = new IconButton(@"file:images/guidenewh.icon");
			this.buttonNewH.Clicked += new MessageEventHandler(this.HandleButtonNewH);
			this.toolBar.Items.Add(this.buttonNewH);
			ToolTip.Default.SetToolTip(this.buttonNewH, "Nouveau repère horizontal");

			this.buttonNewV = new IconButton(@"file:images/guidenewv.icon");
			this.buttonNewV.Clicked += new MessageEventHandler(this.HandleButtonNewV);
			this.toolBar.Items.Add(this.buttonNewV);
			ToolTip.Default.SetToolTip(this.buttonNewV, "Nouveau repère vertical");

			this.buttonDuplicate = new IconButton(@"file:images/duplicate.icon");
			this.buttonDuplicate.Clicked += new MessageEventHandler(this.HandleButtonDuplicate);
			this.toolBar.Items.Add(this.buttonDuplicate);
			ToolTip.Default.SetToolTip(this.buttonDuplicate, "Dupliquer le repère");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton(@"file:images/up.icon");
			this.buttonUp.Clicked += new MessageEventHandler(this.HandleButtonUp);
			this.toolBar.Items.Add(this.buttonUp);
			ToolTip.Default.SetToolTip(this.buttonUp, "Repère avant");

			this.buttonDown = new IconButton(@"file:images/down.icon");
			this.buttonDown.Clicked += new MessageEventHandler(this.HandleButtonDown);
			this.toolBar.Items.Add(this.buttonDown);
			ToolTip.Default.SetToolTip(this.buttonDown, "Repère après");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton(@"file:images/delete.icon");
			this.buttonDelete.Clicked += new MessageEventHandler(this.HandleButtonDelete);
			this.toolBar.Items.Add(this.buttonDelete);
			ToolTip.Default.SetToolTip(this.buttonDelete, "Supprimer le repère");

			this.table = new CellTable(this);
			this.table.Dock = DockStyle.Fill;
			this.table.SelectionChanged += new EventHandler(this.HandleTableSelectionChanged);
			this.table.FlyOverChanged += new EventHandler(this.HandleTableFlyOverChanged);
			this.table.StyleH  = CellArrayStyle.ScrollNorm;
			this.table.StyleH |= CellArrayStyle.Header;
			this.table.StyleH |= CellArrayStyle.Separator;
			this.table.StyleH |= CellArrayStyle.Mobile;
			this.table.StyleV  = CellArrayStyle.ScrollNorm;
			this.table.StyleV |= CellArrayStyle.Separator;
			this.table.StyleV |= CellArrayStyle.SelectLine;
			this.table.DefHeight = 16;

			this.editGroup = new Widget(this);
			this.editGroup.Width = this.Width;
			this.editGroup.Height = 22;
			this.editGroup.Dock = DockStyle.Bottom;
			this.editGroup.DockMargins = new Margins(0, 0, 5, 0);

			this.editType = new TextFieldCombo(this.editGroup);
			this.editType.Width = 181;
			this.editType.IsReadOnly = true;
			foreach ( int value in System.Enum.GetValues(typeof(Settings.GuideType)) )
			{
				Settings.GuideType type = (Settings.GuideType)value;
				if ( type == Settings.GuideType.None )  continue;
				this.editType.Items.Add(Settings.Guide.TypeToString(type));
			}
			this.editType.Dock = DockStyle.Left;
			this.editType.DockMargins = new Margins(3, 1, 0, 0);
			this.editType.TextChanged += new EventHandler(this.HandleEditTypeChanged);

			this.editPosition = new TextFieldSlider(this.editGroup);
			this.editPosition.Width = 60+10;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.editPosition.MinValue = -200.0M;
				this.editPosition.MaxValue = 200.0M;
				this.editPosition.Step = 1.0M;
				this.editPosition.Resolution = 1.0M;
			}
			else
			{
				this.editPosition.MinValue = -2000.0M;
				this.editPosition.MaxValue = 2000.0M;
				this.editPosition.Step = 1.0M;
				this.editPosition.Resolution = 0.1M;
			}
			this.editPosition.Dock = DockStyle.Left;
			this.editPosition.DockMargins = new Margins(1, 0, 0, 0);
			this.editPosition.ValueChanged += new EventHandler(this.HandleEditPositionChanged);

			this.DoUpdateContent();
		}
		

		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			this.UpdateTable();
			this.UpdateToolBar();
			this.UpdateEdits();
		}

		// Met à jour les boutons de la toolbar.
		protected void UpdateToolBar()
		{
			int total = this.document.Settings.GuidesCount;
			int sel = this.table.SelectedRow;

			this.buttonDuplicate.SetEnabled(sel != -1);
			this.buttonUp.SetEnabled(sel != -1 && sel > 0);
			this.buttonDown.SetEnabled(sel != -1 && sel < total-1);
			this.buttonDelete.SetEnabled(sel != -1 && total > 0);
		}

		// Met à jour le contenu de la table.
		protected void UpdateTable()
		{
			int rows = this.document.Settings.GuidesCount;
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(2, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 181);
				this.table.SetWidthColumn(1, 60);
			}

			this.table.SetHeaderTextH(0, "Type");
			this.table.SetHeaderTextH(1, "Position");

			for ( int i=0 ; i<rows ; i++ )
			{
				this.TableFillRow(i);
				this.TableUpdateRow(i);
			}
		}

		// Peuple une ligne de la table, si nécessaire.
		protected void TableFillRow(int row)
		{
			for ( int column=0 ; column<this.table.Columns ; column++ )
			{
				if ( this.table[column, row].IsEmpty )
				{
					StaticText st = new StaticText();
					st.Alignment = (column==0) ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight;
					st.Dock = DockStyle.Fill;
					this.table[column, row].Insert(st);
				}
			}
		}

		// Met à jour le contenu d'une ligne de la table.
		protected void TableUpdateRow(int row)
		{
			Settings.Guide guide = this.document.Settings.GuidesGet(row);
			StaticText st;

			st = this.table[0, row].Children[0] as StaticText;
			st.Text = " " + Settings.Guide.TypeToString(guide.Type);

			st = this.table[1, row].Children[0] as StaticText;
			st.Text = guide.Position.ToString() + " ";

			this.table.SelectRow(row, row==this.tableRowSelected);
		}

		// Met à jour les widgets d'édition en fonction de la ligne sélectionnée.
		protected void UpdateEdits()
		{
			this.ignoreChanged = true;

			if ( this.tableRowSelected < 0 )
			{
				this.editType.Text = "";
				this.editPosition.Text = "";
			}
			else
			{
				Settings.Guide guide = this.document.Settings.GuidesGet(this.tableRowSelected);
				this.editType.Text = Settings.Guide.TypeToString(guide.Type);
				this.editPosition.Value = (decimal) guide.Position;
			}

			this.ignoreChanged = false;
			this.UpdateToolBar();
		}


		// Crée un nouveau repère.
		protected void CreateGuide(Settings.GuideType type)
		{
			Settings.Guide guide = new Settings.Guide(this.document);
			guide.Type = type;
			guide.Position = 0.0;
			this.document.Settings.GuidesInsert(++this.tableRowSelected, guide);
			this.UpdateEdits();
			this.editPosition.SelectAll();
			this.editPosition.Focus();
		}

		// Crée un nouveau repère horizontal.
		private void HandleButtonNewH(object sender, MessageEventArgs e)
		{
			this.CreateGuide(Settings.GuideType.HorizontalBottom);
		}

		// Crée un nouveau repère vertical.
		private void HandleButtonNewV(object sender, MessageEventArgs e)
		{
			this.CreateGuide(Settings.GuideType.VerticalLeft);
		}

		// Duplique un repère.
		private void HandleButtonDuplicate(object sender, MessageEventArgs e)
		{
			Settings.Guide guide = new Settings.Guide(this.document);
			this.document.Settings.GuidesGet(this.tableRowSelected).CopyTo(guide);
			this.document.Settings.GuidesInsert(++this.tableRowSelected, guide);
			this.UpdateEdits();
			this.editPosition.SelectAll();
			this.editPosition.Focus();
		}

		// Monte d'une ligne le repère sélectionné.
		private void HandleButtonUp(object sender, MessageEventArgs e)
		{
			if ( this.tableRowSelected < 1 )  return;
			Settings.Guide guide = this.document.Settings.GuidesGet(this.tableRowSelected);
			this.document.Settings.GuidesRemoveAt(this.tableRowSelected);
			this.tableRowSelected --;
			this.document.Settings.GuidesInsert(this.tableRowSelected, guide);
			this.UpdateEdits();
			this.editPosition.SelectAll();
			this.editPosition.Focus();
		}

		// Descend d'une ligne le repère sélectionné.
		private void HandleButtonDown(object sender, MessageEventArgs e)
		{
			if ( this.tableRowSelected > this.document.Settings.GuidesCount-2 )  return;
			Settings.Guide guide = this.document.Settings.GuidesGet(this.tableRowSelected);
			this.document.Settings.GuidesRemoveAt(this.tableRowSelected);
			this.tableRowSelected ++;
			this.document.Settings.GuidesInsert(this.tableRowSelected, guide);
			this.UpdateEdits();
			this.editPosition.SelectAll();
			this.editPosition.Focus();
		}

		// Supprime le repère sélectionné.
		private void HandleButtonDelete(object sender, MessageEventArgs e)
		{
			this.document.Settings.GuidesRemoveAt(this.tableRowSelected);
			if ( this.tableRowSelected >= this.document.Settings.GuidesCount )
			{
				this.tableRowSelected = this.document.Settings.GuidesCount-1;
				this.table.SelectRow(this.tableRowSelected, true);
				this.UpdateEdits();
			}
		}

		// Liste cliquée.
		private void HandleTableSelectionChanged(object sender)
		{
			this.tableRowSelected = this.table.SelectedRow;
			this.UpdateEdits();
		}

		// Liste survolée.
		private void HandleTableFlyOverChanged(object sender)
		{
			int rank = this.table.FlyOverRow;
			int total = this.document.Settings.GuidesCount;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Guide guide = this.document.Settings.GuidesGet(i);
				guide.Hilite = (i == rank);
			}
		}


		private void HandleEditTypeChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			Settings.Guide guide = this.document.Settings.GuidesGet(this.tableRowSelected);

			this.ignoreChanged = true;
			guide.Type = Settings.Guide.StringToType(this.editType.Text);
			this.UpdateTable();
			this.ignoreChanged = false;
		}

		private void HandleEditPositionChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			Settings.Guide guide = this.document.Settings.GuidesGet(this.tableRowSelected);

			this.ignoreChanged = true;
			guide.Position = (double) this.editPosition.Value;
			this.UpdateTable();
			this.ignoreChanged = false;
		}


		protected HToolBar				toolBar;
		protected IconButton			buttonNewH;
		protected IconButton			buttonNewV;
		protected IconButton			buttonDuplicate;
		protected IconButton			buttonUp;
		protected IconButton			buttonDown;
		protected IconButton			buttonDelete;
		protected int					tableRowSelected = -1;
		protected CellTable				table;
		protected Widget				editGroup;
		protected TextFieldCombo		editType;
		protected TextFieldSlider		editPosition;
		protected bool					ignoreChanged = false;
	}
}
