using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Tabs permet de choisir les groupements de paragraphes.
	/// </summary>
	[SuppressBundleSupport]
	public class Tabs : Abstract
	{
		public Tabs(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Tabs.Title;

			this.fixIcon.Text = Misc.Image("TextTabs");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Tabs.Title);

			this.table = new CellTable(this);
			this.table.StyleH |= CellArrayStyle.Header;
			this.table.StyleH |= CellArrayStyle.Separator;
			this.table.StyleV |= CellArrayStyle.ScrollNorm;
			this.table.StyleV |= CellArrayStyle.Separator;
			this.table.StyleV |= CellArrayStyle.SelectLine;
			this.table.FinalSelectionChanged += new EventHandler(this.HandleTableSelectionChanged);
			this.UpdateTable();

			this.buttonNew    = this.CreateIconButton(Misc.Icon("TabNew"), Res.Strings.TextPanel.Tabs.Tooltip.New,    new MessageEventHandler(this.HandleNewClicked),    false);
			this.buttonDelete = this.CreateIconButton(Misc.Icon("Delete"), Res.Strings.TextPanel.Tabs.Tooltip.Delete, new MessageEventHandler(this.HandleDeleteClicked), false);

			this.fieldPos = this.CreateTextFieldLabel(Res.Strings.TextPanel.Tabs.Tooltip.Pos, "", "", 0.0,  1.0, 0.0, 1.0, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandlePosValueChanged));

			this.buttonType = this.CreateIconButton(Misc.Icon("TabLeft"), Res.Strings.Action.Text.Ruler.TabChoice, new MessageEventHandler(this.HandleTypeClicked), true);

			this.ParagraphWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.ParagraphWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.ParagraphWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.ParagraphWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise � jour apr�s avoir attach� le wrappers.
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau �tendu ?
				{
					h += 34 + 17*5;
				}
				else
				{
					h += 34 + 17*2;
				}

				return h;
			}
		}


		protected void HandleWrapperChanged(object sender)
		{
			//	Le wrapper associ� a chang�.
			this.UpdateAfterChanging();
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.buttonNew == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Width = 126;
			this.table.Bounds = r;

			r = rect;
			r.Bottom = r.Top-20;
			r.Left = r.Left+120+10+2;
			r.Width = 20;
			this.buttonNew.Bounds = r;
			this.buttonNew.Visibility = this.isExtendedSize;
			r.Offset(20, 0);
			this.buttonDelete.Bounds = r;
			this.buttonDelete.Visibility = this.isExtendedSize;

			r = rect;
			r.Top -= 25;
			r.Bottom = r.Top-20;
			r.Left = r.Left+120+10;
			r.Width = 50;
			this.fieldPos.Bounds = r;
			this.fieldPos.Visibility = this.isExtendedSize;

			r = rect;
			r.Top -= 50;
			r.Bottom = r.Top-20;
			r.Left = r.Left+120+10+2;
			r.Width = 50-3;
			this.buttonType.Bounds = r;
			this.buttonType.Visibility = this.isExtendedSize;
		}


		protected override void UpdateAfterChanging()
		{
			//	Met � jour apr�s un changement du wrapper.
			base.UpdateAfterChanging();
			
			if ( this.ParagraphWrapper.IsAttached == false )  return;

			this.UpdateTable();
			this.UpdateWidgets();
		}


		protected void UpdateTable()
		{
			//	Met � jour le contenu de la liste des tabulateurs.
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.tabsName = null;
			if ( this.isStyle )
			{
				Text.TextStyle style = this.ParagraphWrapper.AttachedStyle;
				if ( style != null )
				{
					if ( this.ParagraphWrapper.Defined.IsTabsDefined )
					{
						this.tabsName = this.ParagraphWrapper.Defined.Tabs;
					}
				}
			}
			else
			{
				this.tabsName = this.ParagraphWrapper.AttachedTextNavigator.GetAllTabTags();
			}

			int columns = 2;
			int rows = 0;

			if ( this.tabsName != null )
			{
				this.document.TextContext.TabList.SortTabs(this.tabsName);
				rows = this.tabsName.Length;
			}

			int initialColumns = this.table.Columns;
			this.table.SetArraySize(columns, rows);

			if ( initialColumns != this.table.Columns )
			{
				this.table.SetWidthColumn(0, 53);
				this.table.SetWidthColumn(1, 50);
			}

			this.table.SetHeaderTextH(0, Res.Strings.TextPanel.Tabs.Table.Pos);
			this.table.SetHeaderTextH(1, Res.Strings.TextPanel.Tabs.Table.Type);

			if ( this.tabsName != null )
			{
				for ( int i=0 ; i<this.tabsName.Length ; i++ )
				{
					this.TableFillRow(i);
					this.TableUpdateRow(i, this.tabsName[i]);
				}
			}
		}

		protected void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si n�cessaire.
			if ( this.table[0, row].IsEmpty )
			{
				StaticText st = new StaticText();
				st.Alignment = ContentAlignment.MiddleLeft;
				st.Dock = DockStyle.Fill;
				st.DockMargins = new Drawing.Margins(10, 0, 0, 0);
				this.table[0, row].Insert(st);
			}

			if ( this.table[1, row].IsEmpty )
			{
				StaticText st = new StaticText();
				st.Alignment = ContentAlignment.MiddleLeft;
				st.Dock = DockStyle.Fill;
				st.DockMargins = new Drawing.Margins(10, 0, 0, 0);
				this.table[1, row].Insert(st);
			}
		}

		protected void TableUpdateRow(int row, string tag)
		{
			//	Met � jour le contenu d'une ligne de la table.
			double tabPos;
			TextTabType type;
			Objects.AbstractText.GetTextTab(this.document, tag, out tabPos, out type);

			StaticText st;

			st = this.table[0, row].Children[0] as StaticText;
			st.Text = this.document.Modifier.RealToString(tabPos);

			st = this.table[1, row].Children[0] as StaticText;
			st.Text = Tabs.ConvType2Button(type);
		}


		protected void UpdateWidgets()
		{
			this.ignoreChanged = true;

			int sel = this.table.SelectedRow;
			this.buttonDelete.Enable = (sel != -1);

			if ( sel == -1 )
			{
				this.fieldPos.Enable = false;
				this.fieldPos.TextFieldReal.ClearText();

				this.buttonType.Enable = false;
				this.buttonType.Text = " ";
			}
			else
			{
				string tag = this.tabsName[sel];
				double tabPos;
				TextTabType type;
				Objects.AbstractText.GetTextTab(this.document, tag, out tabPos, out type);
				
				this.fieldPos.Enable = true;
				this.fieldPos.TextFieldReal.InternalValue = (decimal) tabPos;

				this.buttonType.Enable = true;
				this.buttonType.Text = Tabs.ConvType2Button(type);
			}

			this.ignoreChanged = false;
		}

		protected static string ConvType2Button(TextTabType type)
		{
			string image = Widgets.HRuler.ConvType2Image(type);
			string mark  = Widgets.HRuler.ConvType2Mark(type);
			if ( mark == null )
			{
				return image;
			}
			else
			{
				return string.Format("{0} ({1})", image, mark);
			}
		}


		private void HandleTableSelectionChanged(object sender)
		{
			//	Liste des tabulateurs cliqu�e.
			this.UpdateWidgets();
		}

		private void HandleNewClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.ParagraphWrapper.SuspendSynchronizations();
			// TODO: ...
			this.ParagraphWrapper.ResumeSynchronizations();
			this.document.IsDirtySerialize = true;
		}

		private void HandleDeleteClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			this.ParagraphWrapper.SuspendSynchronizations();
			// TODO: ...
			this.ParagraphWrapper.ResumeSynchronizations();
			this.document.IsDirtySerialize = true;
		}

		private void HandlePosValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			int sel = this.table.SelectedRow;
			if ( sel == -1 )  return;
			string tag = this.tabsName[sel];

			double tabPos;
			TextTabType type;
			Objects.AbstractText.GetTextTab(this.document, tag, out tabPos, out type);
			tabPos = (double) this.fieldPos.TextFieldReal.InternalValue;
			Objects.AbstractText.SetTextTab(this.document, this.document.Wrappers.TextFlow.TextNavigator, ref tag, tabPos, type, this.isStyle);

			this.document.IsDirtySerialize = true;
		}

		private void HandleTypeClicked(object sender, MessageEventArgs e)
		{
			//	Appel� lors le bouton pour choisir le type du tabulateur est cliqu�.
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			int sel = this.table.SelectedRow;
			if ( sel == -1 )  return;
			string tag = this.tabsName[sel];

			double tabPos;
			TextTabType type;
			Objects.AbstractText.GetTextTab(this.document, tag, out tabPos, out type);

			Point pos = this.buttonType.MapClientToScreen(new Point(0, 1));
			VMenu menu = Widgets.HRuler.CreateMenu(new MessageEventHandler(this.HandleMenuPressed), type);
			if ( menu == null )  return;
			menu.Host = this;

			ScreenInfo info = ScreenInfo.Find(pos);
			Drawing.Rectangle area = info.WorkingArea;

			if ( pos.Y-menu.Height < area.Bottom )  // d�passe en bas ?
			{
				pos = this.buttonType.MapClientToScreen(new Drawing.Point(0, this.buttonType.Height-1));
				pos.Y += menu.Height;  // d�roule contre le haut ?
			}

			if ( pos.X+menu.Width > area.Right )  // d�passe � droite ?
			{
				pos.X -= pos.X+menu.Width-area.Right;
			}

			menu.ShowAsContextMenu(this.Window, pos);
		}

		private void HandleMenuPressed(object sender, MessageEventArgs e)
		{
			//	Appel� lorsqu'une case du menu est press�e.
			MenuItem item = sender as MenuItem;

			int sel = this.table.SelectedRow;
			if ( sel == -1 )  return;
			string tag = this.tabsName[sel];

			double tabPos;
			TextTabType type;
			Objects.AbstractText.GetTextTab(this.document, tag, out tabPos, out type);
			type = Widgets.HRuler.ConvName2Type(item.Name);
			Objects.AbstractText.SetTextTab(this.document, this.document.Wrappers.TextFlow.TextNavigator, ref tag, tabPos, type, this.isStyle);

			this.document.IsDirtySerialize = true;
		}

		
		protected CellTable					table;
		protected IconButton				buttonNew;
		protected IconButton				buttonDelete;
		protected Widgets.TextFieldLabel	fieldPos;
		protected IconButton				buttonType;

		protected string[]					tabsName;
	}
}
