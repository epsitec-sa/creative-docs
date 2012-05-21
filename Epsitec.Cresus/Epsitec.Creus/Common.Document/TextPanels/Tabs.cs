using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Tabs permet de choisir les groupements de paragraphes.
	/// </summary>
	public class Tabs : Abstract
	{
		public Tabs(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Tabs.Title;

			this.fixIcon.Text = Misc.Image("TextTabs");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Tabs.Title);

			this.table = new CellTable(this);
			this.table.StyleH |= CellArrayStyles.Header;
			this.table.StyleH |= CellArrayStyles.Separator;
			this.table.StyleV |= CellArrayStyles.ScrollNorm;
			this.table.StyleV |= CellArrayStyles.Separator;
			this.table.StyleV |= CellArrayStyles.SelectLine;
			this.table.FinalSelectionChanged += this.HandleTableSelectionChanged;
			this.UpdateTable();  // pour afficher les noms des colonnes

			this.buttonNew    = this.CreateIconButton(Misc.Icon("TabNew"), Res.Strings.TextPanel.Tabs.Tooltip.New,    this.HandleNewClicked,    false);
			this.buttonDelete = this.CreateIconButton(Misc.Icon("Delete"), Res.Strings.TextPanel.Tabs.Tooltip.Delete, this.HandleDeleteClicked, false);

			this.fieldPos = new Epsitec.Common.Document.Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldPos.SetRangeDimension(this.document, 0.0, 1.0, 0.0, 1.0);
			this.fieldPos.TextFieldReal.TextChanged += this.HandlePosValueChanged;
			this.fieldPos.TabIndex = this.tabIndex++;
			this.fieldPos.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldPos, Res.Strings.TextPanel.Tabs.Tooltip.Pos);
			this.ProposalTextFieldLabel(this.fieldPos, false);  // toujours défini

			this.buttonType = this.CreateIconButton(Misc.Icon("TabLeft"), Res.Strings.Action.Text.Ruler.TabChoice, this.HandleTypeClicked, true);

			this.ParagraphWrapper.Active.Changed  += this.HandleWrapperChanged;
			this.ParagraphWrapper.Defined.Changed += this.HandleWrapperChanged;

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.table.FinalSelectionChanged -= this.HandleTableSelectionChanged;
				this.ParagraphWrapper.Active.Changed  -= this.HandleWrapperChanged;
				this.ParagraphWrapper.Defined.Changed -= this.HandleWrapperChanged;
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise à jour après avoir attaché le wrappers.
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
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
			//	Le wrapper associé a changé.
			this.UpdateAfterChanging();
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonNew == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Width = 126;
			this.table.SetManualBounds(r);

			r = rect;
			r.Bottom = r.Top-20;
			r.Left = r.Left+120+10+2;
			r.Width = 20;
			this.buttonNew.SetManualBounds(r);
			this.buttonNew.Visibility = this.isExtendedSize;
			r.Offset(20, 0);
			this.buttonDelete.SetManualBounds(r);
			this.buttonDelete.Visibility = this.isExtendedSize;

			r = rect;
			r.Top -= 25;
			r.Bottom = r.Top-20;
			r.Left = r.Left+120+10;
			r.Width = 50;
			this.fieldPos.SetManualBounds(r);
			this.fieldPos.Visibility = this.isExtendedSize;

			r = rect;
			r.Top -= 50;
			r.Bottom = r.Top-20;
			r.Left = r.Left+120+10+2;
			r.Width = 50-3;
			this.buttonType.SetManualBounds(r);
			this.buttonType.Visibility = this.isExtendedSize;
		}


		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();
			
			if ( this.ParagraphWrapper.IsAttached == false )  return;

			this.UpdateTable();
			this.UpdateWidgets();
		}


		protected void UpdateTable()
		{
			//	Met à jour le contenu de la liste des tabulateurs.
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
				string[] all = this.ParagraphWrapper.AttachedTextNavigator.GetAllTabTags();
				int count = 0;
				foreach ( string tag in all )
				{
					TabClass tc = TabList.GetTabClass(tag);
					if ( tc == TabClass.Auto )
					{
						count ++;
					}
				}

				if ( count > 0 )
				{
					this.tabsName = new string[count];
					int i = 0;
					foreach ( string tag in all )
					{
						TabClass tc = TabList.GetTabClass(tag);
						if ( tc == TabClass.Auto )
						{
							this.tabsName[i++] = tag;
						}
					}
				}
			}

			int columns = 2;
			int rows = 0;

			if ( this.tabsName == null )
			{
				this.tabSelected = null;
			}
			else
			{
				this.document.TextContext.TabList.SortTabs(this.tabsName);

				if ( this.GetTabRank(this.tabSelected) == -1 )  // tabulateur à sélectionner inexistant ?
				{
					this.tabSelected = null;  // supprime la sélection
				}

				rows = this.tabsName.Length;
			}

			int initialColumns = this.table.Columns;
			this.table.SetArraySize(columns, rows);

			if ( initialColumns != this.table.Columns )  // changement du nombre de colonnes ?
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
			//	Peuple une ligne de la table, si nécessaire.
			if ( this.table[0, row].IsEmpty )
			{
				StaticText st = new StaticText();
				st.ContentAlignment = ContentAlignment.MiddleLeft;
				st.Dock = DockStyle.Fill;
				st.Margins = new Drawing.Margins(10, 0, 0, 0);
				this.table[0, row].Insert(st);
			}

			if ( this.table[1, row].IsEmpty )
			{
				StaticText st = new StaticText();
				st.ContentAlignment = ContentAlignment.MiddleLeft;
				st.Dock = DockStyle.Fill;
				st.Margins = new Drawing.Margins(10, 0, 0, 0);
				this.table[1, row].Insert(st);
			}
		}

		protected void TableUpdateRow(int row, string tag)
		{
			//	Met à jour le contenu d'une ligne de la table.
			double tabPos;
			TextTabType type;
			Objects.AbstractText.GetTextTab(this.document, tag, out tabPos, out type);

			StaticText st;

			st = this.table[0, row].Children[0] as StaticText;
			st.Text = this.document.Modifier.RealToString(tabPos);

			st = this.table[1, row].Children[0] as StaticText;
			st.Text = Tabs.ConvType2Button(type);

			bool selected = (tag == this.tabSelected);  // voir (**) dans Objects.AbstractText !
			this.table.SelectRow(row, selected);
		}


		protected void UpdateWidgets()
		{
			//	Met à jour les boutons.
			this.ignoreChanged = true;

			this.buttonDelete.Enable = (this.tabSelected != null);

			if ( this.tabSelected == null )
			{
				this.fieldPos.Enable = false;
				this.fieldPos.TextFieldReal.ClearText();

				this.buttonType.Enable = false;
				this.buttonType.Text = " ";
			}
			else
			{
				double tabPos;
				TextTabType type;
				Objects.AbstractText.GetTextTab(this.document, this.tabSelected, out tabPos, out type);
				
				this.fieldPos.Enable = true;
				this.fieldPos.TextFieldReal.InternalValue = (decimal) tabPos;

				this.buttonType.Enable = true;
				this.buttonType.Text = Tabs.ConvType2Button(type);
			}

			this.ignoreChanged = false;
		}

		protected static string ConvType2Button(TextTabType type)
		{
			//	Donne le texte à mettre dans un button pour représenter un type de tabulateur.
			//	Les tabulateurs décimaux sont suivis du caractère de marque entre parenthèses.
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

		protected int GetTabRank(string tag)
		{
			//	Cherche le rang d'un tabulateur d'après son nom.
			for ( int i=0 ; i<this.tabsName.Length ; i++ )
			{
				if ( this.tabsName[i] == tag )  return i;
			}
			return -1;
		}


		private void HandleTableSelectionChanged(object sender)
		{
			//	Sélection d'un tabulateur dans la liste.
			int sel = this.table.SelectedRow;
			if ( sel == -1 )
			{
				this.tabSelected = null;
			}
			else
			{
				this.tabSelected = this.tabsName[sel];  // this.tabSelected <- nom du tabulateur sélectionné
			}

			this.UpdateWidgets();
		}

		private void HandleNewClicked(object sender, MessageEventArgs e)
		{
			//	Crée un nouveau tabulateur.
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;

			double tabPos = 0;
			TextTabType type = TextTabType.Left;

			double add;
			if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
			{
				add = 100;  // 10mm
			}
			else
			{
				add = 127;  // 0.5in
			}

			if ( this.tabsName == null || this.tabsName.Length == 0 )
			{
				tabPos = add;
				type = TextTabType.Left;
			}
			else
			{
				if ( this.tabSelected == null )
				{
					string tag = this.tabsName[this.tabsName.Length-1];
					Objects.AbstractText.GetTextTab(this.document, tag, out tabPos, out type);  // dernier
					tabPos += add;
				}
				else
				{
					int rank = this.GetTabRank(this.tabSelected);
					if ( rank != -1 && rank < this.tabsName.Length-1 )
					{
						Objects.AbstractText.GetTextTab(this.document, this.tabsName[rank+1], out tabPos, out type);
						double pos = tabPos;
						Objects.AbstractText.GetTextTab(this.document, this.tabsName[rank+0], out tabPos, out type);
						tabPos = (tabPos+pos)/2;  // entre les 2
					}
					else
					{
						Objects.AbstractText.GetTextTab(this.document, this.tabSelected, out tabPos, out type);
						tabPos += add;
					}
				}
			}
			
			Objects.AbstractText.NewTextTab(this.document, this.document.Wrappers.TextFlow, out this.tabSelected, tabPos, type, this.isStyle);
		}

		private void HandleDeleteClicked(object sender, MessageEventArgs e)
		{
			//	Supprime le tabulateur sélectionné.
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;
			if ( this.tabSelected == null )  return;

			Objects.AbstractText.DeleteTextTab(this.document, this.document.Wrappers.TextFlow, this.tabSelected, this.isStyle);
		}

		private void HandlePosValueChanged(object sender)
		{
			//	Changement de la position d'un tabulateur.
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;
			if ( this.tabSelected == null )  return;

			double tabPos;
			TextTabType type;
			Objects.AbstractText.GetTextTab(this.document, this.tabSelected, out tabPos, out type);
			tabPos = (double) this.fieldPos.TextFieldReal.InternalValue;
			Objects.AbstractText.SetTextTab(this.document, this.document.Wrappers.TextFlow, ref this.tabSelected, tabPos, type, true, this.isStyle);

			if ( this.isStyle )
			{
				this.UpdateTable();
				this.UpdateWidgets();
			}
		}

		private void HandleTypeClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour choisir le type du tabulateur est cliqué.
			if ( this.ignoreChanged )  return;
			if ( !this.ParagraphWrapper.IsAttached )  return;
			if ( this.tabSelected == null )  return;

			IconButton button = sender as IconButton;
			double tabPos;
			TextTabType type;
			Objects.AbstractText.GetTextTab(this.document, this.tabSelected, out tabPos, out type);

			VMenu menu = Widgets.HRuler.CreateMenu(this.HandleMenuPressed, type);
			if ( menu == null )  return;
			menu.Host = this;
			menu.MinWidth = button.ActualWidth;
			TextFieldCombo.AdjustComboSize(button, menu, false);
			menu.ShowAsComboList(button, Point.Zero, this.buttonType);
		}

		private void HandleMenuPressed(object sender, MessageEventArgs e)
		{
			//	Appelé lorsqu'une case du menu est pressée.
			MenuItem item = sender as MenuItem;

			double tabPos;
			TextTabType type;
			Objects.AbstractText.GetTextTab(this.document, this.tabSelected, out tabPos, out type);
			type = Widgets.HRuler.ConvName2Type(item.Name);
			Objects.AbstractText.SetTextTab(this.document, this.document.Wrappers.TextFlow, ref this.tabSelected, tabPos, type, true, this.isStyle);

			if ( this.isStyle )
			{
				this.UpdateTable();
				this.UpdateWidgets();
			}
		}

		
		protected CellTable					table;
		protected IconButton				buttonNew;
		protected IconButton				buttonDelete;
		protected Widgets.TextFieldLabel	fieldPos;
		protected IconButton				buttonType;

		protected string[]					tabsName;		// noms des tabulateurs dans la liste
		protected string					tabSelected;	// nom du tabulateur sélectionné
	}
}
