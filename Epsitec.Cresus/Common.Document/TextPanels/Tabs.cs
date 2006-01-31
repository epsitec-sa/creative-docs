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
			this.UpdateTable();

			this.buttonNew    = this.CreateIconButton(Misc.Icon("New"),    Res.Strings.TextPanel.Tabs.Tooltip.New,    new MessageEventHandler(this.HandleNewClicked),    false);
			this.buttonDelete = this.CreateIconButton(Misc.Icon("Delete"), Res.Strings.TextPanel.Tabs.Tooltip.Delete, new MessageEventHandler(this.HandleDeleteClicked), false);

			this.fieldPos = this.CreateTextFieldLabel(Res.Strings.TextPanel.Tabs.Tooltip.Pos, "", "", 0.0,  0.1, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandlePosValueChanged));

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
					h += 120;
				}
				else
				{
					h += 55;
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
			r.Width = 120;
			this.table.Bounds = r;

			r = rect;
			r.Bottom = r.Top-20;
			r.Left = r.Left+120+10;
			r.Width = 20;
			this.buttonNew.Bounds = r;
			r.Offset(20, 0);
			this.buttonDelete.Bounds = r;

			r = rect;
			r.Top -= 25;
			r.Bottom = r.Top-20;
			r.Left = r.Left+120+10;
			r.Width = 50;
			this.fieldPos.Bounds = r;
		}


		protected void UpdateTable()
		{
			int columns = 2;
			int rows = 0;
			string[] tabsName = null;
			Text.TextStyle style = this.ParagraphWrapper.AttachedStyle;
			if ( style != null && this.ParagraphWrapper.IsAttached )
			{
				if ( this.ParagraphWrapper.Defined.IsTabsDefined )
				{
					tabsName = this.ParagraphWrapper.Defined.Tabs;
					if ( tabsName != null )
					{
						rows = tabsName.Length;
					}
				}
			}

			int initialColumns = this.table.Columns;
			this.table.SetArraySize(columns, rows);

			if ( initialColumns != this.table.Columns )
			{
				this.table.SetWidthColumn(0, 50);
				this.table.SetWidthColumn(1, 47);

				this.table.SetHeaderTextH(0, Res.Strings.TextPanel.Tabs.Table.Pos);
				this.table.SetHeaderTextH(1, Res.Strings.TextPanel.Tabs.Table.Type);
			}

			if ( tabsName != null )
			{
				//?Text.Properties.TabsProperty tabsProp = style[Text.Properties.WellKnownType.Tabs] as Text.Properties.TabsProperty;
				for ( int i=0 ; i<tabsName.Length ; i++ )
				{
					this.TableFillRow(i);
					this.TableUpdateRow(i);
				}
			}
		}

		protected void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si nécessaire.
			if ( this.table[0, row].IsEmpty )
			{
				StaticText st = new StaticText();
				st.Alignment = ContentAlignment.MiddleLeft;
				st.Dock = DockStyle.Fill;
				st.DockMargins = new Drawing.Margins(2, 2, 0, 0);
				this.table[0, row].Insert(st);
			}

			if ( this.table[1, row].IsEmpty )
			{
				StaticText st = new StaticText();
				st.Alignment = ContentAlignment.MiddleCenter;
				st.Dock = DockStyle.Fill;
				st.DockMargins = new Drawing.Margins(2, 2, 0, 0);
				this.table[1, row].Insert(st);
			}
		}

		protected void TableUpdateRow(int row)
		{
			//	Met à jour le contenu d'une ligne de la table.
			StaticText st;

			st = this.table[0, row].Children[0] as StaticText;
			st.Text = "coucou";
		}


		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();
			
			if ( this.ParagraphWrapper.IsAttached == false )  return;

			this.UpdateTable();
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

			this.ParagraphWrapper.SuspendSynchronizations();
			// TODO: ...
			this.ParagraphWrapper.ResumeSynchronizations();
			this.document.IsDirtySerialize = true;
		}

		
		protected CellTable					table;
		protected IconButton				buttonNew;
		protected IconButton				buttonDelete;
		protected Widgets.TextFieldLabel	fieldPos;
	}
}
