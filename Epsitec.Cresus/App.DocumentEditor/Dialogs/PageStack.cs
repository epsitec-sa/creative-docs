using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;
	using Objects = Epsitec.Common.Document.Objects;

	/// <summary>
	/// Dialogue "Structure de la page".
	/// </summary>
	public class PageStack : Abstract
	{
		public PageStack(DocumentEditor editor) : base(editor)
		{
		}

		// Cr�e et montre la fen�tre du dialogue.
		public override void Show()
		{
			if ( this.window == null )
			{
				this.window = new Window();
				//?this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("PageStack", 500, 300, true);
				this.window.Text = Res.Strings.Dialog.PageStack.Title;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource("Epsitec.App.DocumentEditor.Images.Application.icon", this.GetType().Assembly);
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.MinSize = new Size(240, 200);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.AnchorMargins = new Margins(0, 0, 0, 0);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				this.tabIndex = 0;

				this.help = new TextFieldMulti(this.window.Root);
				this.help.IsReadOnly = true;
				this.help.Height = 84;
				this.help.Dock = DockStyle.Top;
				this.help.DockMargins = new Margins(6, 6, 6, 0);

				System.Text.StringBuilder b = new System.Text.StringBuilder();
				string chip = "<list type=\"fix\" width=\"1.5\"/>";
				b.Append(chip);
				b.Append(Res.Strings.Dialog.PageStack.Help1);
				b.Append("<br/>");
				b.Append(chip);
				b.Append(Res.Strings.Dialog.PageStack.Help2);
				b.Append("<br/>");
				b.Append(chip);
				b.Append(Res.Strings.Dialog.PageStack.Help3);
				b.Append("<br/>");
				b.Append(chip);
				b.Append(Res.Strings.Dialog.PageStack.Help4);
				b.Append("<br/>");
				b.Append(chip);
				b.Append(Res.Strings.Dialog.PageStack.Help5);
				this.help.Text = b.ToString();
				this.help.Visibility = true;

				this.table = new CellTable(this.window.Root);
				this.table.StyleH  = CellArrayStyle.ScrollNorm;
				this.table.StyleH |= CellArrayStyle.Header;
				this.table.StyleH |= CellArrayStyle.Separator;
				this.table.StyleH |= CellArrayStyle.Mobile;
				this.table.StyleV  = CellArrayStyle.ScrollNorm;
				this.table.StyleV |= CellArrayStyle.Separator;
				this.table.DefHeight = 16;
				this.table.Dock = DockStyle.Fill;
				this.table.DockMargins = new Margins(6, 6, 6, 34);

				// Bouton de fermeture.
				double posx = 6;
				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.AnchorMargins = new Margins(posx, 0, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = this.tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, Res.Strings.Dialog.Tooltip.Close);

				// Bouton d'aide.
				posx += buttonClose.Width+6;
				Button buttonHelp = new Button(this.window.Root);
				buttonHelp.Width = 75;
				buttonHelp.Text = Res.Strings.Dialog.Button.Help;
				buttonHelp.Anchor = AnchorStyles.BottomLeft;
				buttonHelp.AnchorMargins = new Margins(posx, 0, 0, 6);
				buttonHelp.Clicked += new MessageEventHandler(this.HandleButtonHelpClicked);
				buttonHelp.TabIndex = this.tabIndex++;
				buttonHelp.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonHelp, Res.Strings.Dialog.Tooltip.Help);

				double dim = buttonClose.Height;

				posx += buttonHelp.Width+30;
				this.pagePrev = new GlyphButton(this.window.Root);
				this.pagePrev.GlyphShape = GlyphShape.ArrowLeft;
				this.pagePrev.Width = dim;
				this.pagePrev.Height = dim;
				this.pagePrev.Anchor = AnchorStyles.BottomLeft;
				this.pagePrev.AnchorMargins = new Margins(posx, 0, 0, 6);
				this.pagePrev.Clicked += new MessageEventHandler(this.HandlePagePrevClicked);
				this.pagePrev.TabIndex = this.tabIndex++;
				this.pagePrev.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.pagePrev, DocumentEditor.GetRes("Action.PagePrev"));

				posx += pagePrev.Width+1;
				this.pageMenu = new Button(this.window.Root);
				this.pageMenu.Width = dim*2.0;
				this.pageMenu.Height = dim;
				this.pageMenu.Anchor = AnchorStyles.BottomLeft;
				this.pageMenu.AnchorMargins = new Margins(posx, 0, 0, 6);
				this.pageMenu.Clicked += new MessageEventHandler(this.HandlePageMenuClicked);
				this.pageMenu.TabIndex = this.tabIndex++;
				this.pageMenu.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.pageMenu, DocumentEditor.GetRes("Action.PageMenu"));

				posx += pageMenu.Width+1;
				this.pageNext = new GlyphButton(this.window.Root);
				this.pageNext.GlyphShape = GlyphShape.ArrowRight;
				this.pageNext.Width = dim;
				this.pageNext.Height = dim;
				this.pageNext.Anchor = AnchorStyles.BottomLeft;
				this.pageNext.AnchorMargins = new Margins(posx, 0, 0, 6);
				this.pageNext.Clicked += new MessageEventHandler(this.HandlePageNextClicked);
				this.pageNext.TabIndex = this.tabIndex++;
				this.pageNext.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.pageNext, DocumentEditor.GetRes("Action.PageNext"));

				// Bouton page courante.
				posx += pageNext.Width+8;
				this.buttonCurrent = new Button(this.window.Root);
				this.buttonCurrent.Width = 100;
				this.buttonCurrent.Text = Res.Strings.Dialog.PageStack.Button.Current;
				this.buttonCurrent.Anchor = AnchorStyles.BottomLeft;
				this.buttonCurrent.AnchorMargins = new Margins(posx, 0, 0, 6);
				this.buttonCurrent.Clicked += new MessageEventHandler(this.HandleButtonCurrentClicked);
				this.buttonCurrent.TabIndex = this.tabIndex++;
				this.buttonCurrent.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.buttonCurrent, Res.Strings.Dialog.PageStack.Tooltip.Current);
			}

			this.showedPage = this.editor.CurrentDocument.Modifier.ActiveViewer.DrawingContext.CurrentPage;
			this.UpdateTable();
			this.window.Show();
		}

		// Enregistre la position de la fen�tre du dialogue.
		public override void Save()
		{
			this.WindowSave("PageStack");
		}

		// Reconstruit le dialogue.
		public override void Rebuild()
		{
			if ( !this.editor.IsCurrentDocument )  return;
			if ( this.window == null )  return;
			this.UpdateTable();
		}

		// Met � jour le contenu de la table.
		public void Update()
		{
			if ( this.window == null )  return;
			if ( !this.window.IsVisible )  return;
			this.UpdateTable();
		}


		// Met � jour le contenu de la table.
		protected void UpdateTable()
		{
			System.Collections.ArrayList infos;
			if ( this.editor.IsCurrentDocument )
			{
				Document doc = this.editor.CurrentDocument;
				DrawingContext context = doc.Modifier.ActiveViewer.DrawingContext;
				this.showedPage = System.Math.Min(this.showedPage, context.TotalPages()-1);
				infos = doc.Modifier.GetPageStackInfos(this.showedPage);
				infos.Reverse();
			}
			else
			{
				infos = new System.Collections.ArrayList();
			}

			int rows = infos.Count;
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(5, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 43);
				this.table.SetWidthColumn(1, 164);
				this.table.SetWidthColumn(2, 44);
				this.table.SetWidthColumn(3, 50);
				this.table.SetWidthColumn(4, 164);
			}

			this.table.SetHeaderTextH(0, Res.Strings.Dialog.PageStack.Page);
			this.table.SetHeaderTextH(1, "");
			this.table.SetHeaderTextH(2, Res.Strings.Dialog.PageStack.Layer);
			this.table.SetHeaderTextH(3, "");
			this.table.SetHeaderTextH(4, "");

			for ( int i=0 ; i<rows ; i++ )
			{
				this.TableFillRow(i);

				Common.Document.Modifier.PageStackInfos info = infos[i] as Common.Document.Modifier.PageStackInfos;
				StaticText st;

				st = this.table[0, i].Children[0] as StaticText;
				st.Text = info.Page.ShortName;

				st = this.table[1, i].Children[0] as StaticText;
				st.Text = info.Page.Name;

				st = this.table[2, i].Children[0] as StaticText;
				st.Text = info.LayerShortName;

				st = this.table[3, i].Children[0] as StaticText;
				st.Text = info.LayerAutoName;

				st = this.table[4, i].Children[0] as StaticText;
				st.Text = info.Layer.Name;

				this.table.SelectRow(i, !info.Master);
			}

			if ( this.editor.IsCurrentDocument )
			{
				Document doc = this.editor.CurrentDocument;
				DrawingContext context = doc.Modifier.ActiveViewer.DrawingContext;

				this.pagePrev.Enable = (this.showedPage > 0);
				this.pageMenu.Enable = true;
				this.pageNext.Enable = (this.showedPage < context.TotalPages()-1);
				this.buttonCurrent.Enable = (this.showedPage != context.CurrentPage);

				Objects.Page page = doc.GetObjects[this.showedPage] as Objects.Page;
				this.pageMenu.Text = page.ShortName;
			}
			else
			{
				this.pagePrev.Enable = false;
				this.pageMenu.Enable = false;
				this.pageNext.Enable = false;
				this.buttonCurrent.Enable = false;
			}
		}

		// Peuple une ligne de la table, si n�cessaire.
		protected void TableFillRow(int row)
		{
			for ( int column=0 ; column<this.table.Columns ; column++ )
			{
				if ( this.table[column, row].IsEmpty )
				{
					StaticText st = new StaticText();
					st.Alignment = (column==0 || column==2) ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
					st.Dock = DockStyle.Fill;
					this.table[column, row].Insert(st);
				}
			}
		}


		private void HandlePagePrevClicked(object sender, MessageEventArgs e)
		{
			this.showedPage --;
			this.UpdateTable();
		}

		private void HandlePageNextClicked(object sender, MessageEventArgs e)
		{
			this.showedPage ++;
			this.UpdateTable();
		}

		private void HandlePageMenuClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(0, button.Height));
			VMenu menu = this.CreatePagesMenu();
			menu.Host = button.Window;
			pos.Y += menu.Height;
			menu.ShowAsContextMenu(button.Window, pos);
		}

		// Construit le menu pour choisir une page.
		public VMenu CreatePagesMenu()
		{
			UndoableList pages = this.editor.CurrentDocument.GetObjects;  // liste des pages
			MessageEventHandler message = new MessageEventHandler(this.HandleMenuPressed);
			return Objects.Page.CreateMenu(pages, this.showedPage, message);
		}

		private void HandleMenuPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			this.showedPage = System.Convert.ToInt32(item.Name);
			this.UpdateTable();
		}

		
		private void HandleButtonCurrentClicked(object sender, MessageEventArgs e)
		{
			this.showedPage = this.editor.CurrentDocument.Modifier.ActiveViewer.DrawingContext.CurrentPage;
			this.UpdateTable();
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonHelpClicked(object sender, MessageEventArgs e)
		{
			this.help.Visibility = (!this.help.IsVisible);
		}


		protected int					showedPage;
		protected TextFieldMulti		help;
		protected CellTable				table;
		protected GlyphButton			pagePrev;
		protected Button				pageMenu;
		protected GlyphButton			pageNext;
		protected Button				buttonCurrent;
		protected int					tabIndex;
	}
}
