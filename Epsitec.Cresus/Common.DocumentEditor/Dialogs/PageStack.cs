using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

using System.Collections.Generic;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;
	using Objects = Epsitec.Common.Document.Objects;
	using Document = Common.Document.Document;

	/// <summary>
	/// Dialogue "Structure de la page".
	/// </summary>
	public class PageStack : Abstract
	{
		public PageStack(DocumentEditor editor) : base(editor)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				//?this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("PageStack", 500, 300, true);
				this.window.Text = Res.Strings.Dialog.PageStack.Title;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource ("Epsitec.Common.DocumentEditor.Images.Application.icon", this.GetType ().Assembly);
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.MinSize = new Size(240, 200);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, 0, 0, 0);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				this.tabIndex = 0;

				this.help = new TextFieldMulti(this.window.Root);
				this.help.IsReadOnly = true;
				this.help.PreferredHeight = 84;
				this.help.Dock = DockStyle.Top;
				this.help.Margins = new Margins (6, 6, 6, 0);

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
				this.table.StyleH  = CellArrayStyles.ScrollNorm;
				this.table.StyleH |= CellArrayStyles.Header;
				this.table.StyleH |= CellArrayStyles.Separator;
				this.table.StyleH |= CellArrayStyles.Mobile;
				this.table.StyleV  = CellArrayStyles.ScrollNorm;
				this.table.StyleV |= CellArrayStyles.Separator;
				this.table.DefHeight = 16;
				this.table.Dock = DockStyle.Fill;
				this.table.Margins = new Margins (6, 6, 6, 34);

				//	Bouton de fermeture.
				Button buttonClose = new Button(this.window.Root);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.ButtonStyle = ButtonStyle.DefaultAcceptAndCancel;
				buttonClose.Anchor = AnchorStyles.BottomRight;
				buttonClose.Margins = new Margins(0, 6+75+6, 0, 6);
				buttonClose.Clicked += this.HandleButtonCloseClicked;
				buttonClose.TabIndex = this.tabIndex++;
				buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, Res.Strings.Dialog.Tooltip.Close);

				//	Bouton d'aide.
				Button buttonHelp = new Button(this.window.Root);
				buttonHelp.PreferredWidth = 75;
				buttonHelp.Text = Res.Strings.Dialog.Button.Help;
				buttonHelp.Anchor = AnchorStyles.BottomRight;
				buttonHelp.Margins = new Margins(0, 6, 0, 6);
				buttonHelp.Clicked += this.HandleButtonHelpClicked;
				buttonHelp.TabIndex = this.tabIndex++;
				buttonHelp.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonHelp, Res.Strings.Dialog.Tooltip.Help);

				double posx = 6;
				double dim = buttonClose.PreferredHeight;

				this.pagePrev = new GlyphButton(this.window.Root);
				this.pagePrev.GlyphShape = GlyphShape.ArrowLeft;
				this.pagePrev.PreferredWidth = dim;
				this.pagePrev.PreferredHeight = dim;
				this.pagePrev.Anchor = AnchorStyles.BottomLeft;
				this.pagePrev.Margins = new Margins(posx, 0, 0, 6);
				this.pagePrev.Clicked += this.HandlePagePrevClicked;
				this.pagePrev.TabIndex = this.tabIndex++;
				this.pagePrev.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.pagePrev, DocumentEditor.GetRes("Action.PagePrev"));

				posx += pagePrev.PreferredWidth+1;
				this.pageMenu = new Button(this.window.Root);
				this.pageMenu.PreferredWidth = dim*2.0;
				this.pageMenu.PreferredHeight = dim;
				this.pageMenu.Anchor = AnchorStyles.BottomLeft;
				this.pageMenu.Margins = new Margins(posx, 0, 0, 6);
				this.pageMenu.Clicked += this.HandlePageMenuClicked;
				this.pageMenu.TabIndex = this.tabIndex++;
				this.pageMenu.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.pageMenu, DocumentEditor.GetRes("Action.PageMenu"));

				posx += pageMenu.PreferredWidth+1;
				this.pageNext = new GlyphButton(this.window.Root);
				this.pageNext.GlyphShape = GlyphShape.ArrowRight;
				this.pageNext.PreferredWidth = dim;
				this.pageNext.PreferredHeight = dim;
				this.pageNext.Anchor = AnchorStyles.BottomLeft;
				this.pageNext.Margins = new Margins(posx, 0, 0, 6);
				this.pageNext.Clicked += this.HandlePageNextClicked;
				this.pageNext.TabIndex = this.tabIndex++;
				this.pageNext.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.pageNext, DocumentEditor.GetRes("Action.PageNext"));

				//	Bouton page courante.
				posx += pageNext.PreferredWidth+8;
				this.buttonCurrent = new Button(this.window.Root);
				this.buttonCurrent.PreferredWidth = 100;
				this.buttonCurrent.Text = Res.Strings.Dialog.PageStack.Button.Current;
				this.buttonCurrent.Anchor = AnchorStyles.BottomLeft;
				this.buttonCurrent.Margins = new Margins(posx, 0, 0, 6);
				this.buttonCurrent.Clicked += this.HandleButtonCurrentClicked;
				this.buttonCurrent.TabIndex = this.tabIndex++;
				this.buttonCurrent.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(this.buttonCurrent, Res.Strings.Dialog.PageStack.Tooltip.Current);
			}

			this.showedPage = this.editor.CurrentDocument.Modifier.ActiveViewer.DrawingContext.CurrentPage;
			this.UpdateTable();
			this.window.Show();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("PageStack");
		}

		public override void Rebuild()
		{
			//	Reconstruit le dialogue.
			if ( !this.editor.HasCurrentDocument )  return;
			if ( this.window == null )  return;
			this.UpdateTable();
		}

		public void Update()
		{
			//	Met à jour le contenu de la table.
			if ( this.window == null )  return;
			if ( !this.window.IsVisible )  return;
			this.UpdateTable();
		}


		protected void UpdateTable()
		{
			//	Met à jour le contenu de la table.
			List<Modifier.PageStackInfos> infos;
			if ( this.editor.HasCurrentDocument )
			{
				Document doc = this.editor.CurrentDocument;
				DrawingContext context = doc.Modifier.ActiveViewer.DrawingContext;
				this.showedPage = System.Math.Min(this.showedPage, context.TotalPages()-1);
				infos = doc.Modifier.GetPageStackInfos(this.showedPage);
				infos.Reverse();
			}
			else
			{
				infos = new List<Modifier.PageStackInfos> ();
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

			if ( this.editor.HasCurrentDocument )
			{
				Document doc = this.editor.CurrentDocument;
				DrawingContext context = doc.Modifier.ActiveViewer.DrawingContext;

				this.pagePrev.Enable = (this.showedPage > 0);
				this.pageMenu.Enable = true;
				this.pageNext.Enable = (this.showedPage < context.TotalPages()-1);
				this.buttonCurrent.Enable = (this.showedPage != context.CurrentPage);

				Objects.Page page = doc.DocumentObjects[this.showedPage] as Objects.Page;
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

		protected void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si nécessaire.
			for ( int column=0 ; column<this.table.Columns ; column++ )
			{
				if ( this.table[column, row].IsEmpty )
				{
					StaticText st = new StaticText();
					st.ContentAlignment = (column==0 || column==2) ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
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
			VMenu menu = this.CreatePagesMenu();
			menu.Host = button.Window;
			TextFieldCombo.AdjustComboSize(button, menu, false);
			menu.ShowAsComboList(button, Point.Zero, button);
		}

		public VMenu CreatePagesMenu()
		{
			//	Construit le menu pour choisir une page.
			UndoableList pages = this.editor.CurrentDocument.DocumentObjects;  // liste des pages
			return Objects.Page.CreateMenu (pages, this.showedPage, null, this.HandleMenuPressed);
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
			this.CloseWindow();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();
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
