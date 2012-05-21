using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Guides contient tous les panneaux des repères.
	/// </summary>
	public class Guides : Abstract
	{
		public Guides(Document document) : base(document)
		{
			this.radioGroup = new Widget(this);
			this.radioGroup.PreferredHeight = 20;
			this.radioGroup.Dock = DockStyle.Top;
			this.radioGroup.Margins = new Margins(0, 0, 0, 0);
			this.radioGroup.TabIndex = 1;
			this.radioGroup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.radioGlobal = new RadioButton(this.radioGroup);
			this.radioGlobal.PreferredWidth = 80;
			this.radioGlobal.Text = Res.Strings.Container.Guides.RadioGlobal;
			this.radioGlobal.Dock = DockStyle.Left;
			this.radioGlobal.Margins = new Margins(0, 10, 0, 0);
			this.radioGlobal.ActiveStateChanged += this.HandleRadioChanged;
			this.radioGlobal.Index = 1;
			this.radioGlobal.TabIndex = 1;
			this.radioGlobal.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.radioPage = new RadioButton(this.radioGroup);
			this.radioPage.PreferredWidth = 100;
			this.radioPage.Text = Res.Strings.Container.Guides.RadioPage;
			this.radioPage.Dock = DockStyle.Left;
			this.radioPage.Margins = new Margins(0, 0, 0, 0);
			this.radioPage.ActiveStateChanged += this.HandleRadioChanged;
			this.radioPage.Index = 2;
			this.radioPage.TabIndex = 2;
			this.radioPage.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.toolBar = new HToolBar(this);
			this.toolBar.Dock = DockStyle.Top;
			this.toolBar.Margins = new Margins(0, 0, 0, -1);
			this.toolBar.TabIndex = 2;
			this.toolBar.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			int index = 0;

			this.buttonNewH = new IconButton(Misc.Icon("GuideNewH"));
			this.buttonNewH.Clicked += this.HandleButtonNewH;
			this.buttonNewH.TabIndex = index++;
			this.buttonNewH.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonNewH);
			ToolTip.Default.SetToolTip(this.buttonNewH, Res.Strings.Action.GuideNewH);

			this.buttonNewV = new IconButton(Misc.Icon("GuideNewV"));
			this.buttonNewV.Clicked += this.HandleButtonNewV;
			this.buttonNewV.TabIndex = index++;
			this.buttonNewV.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonNewV);
			ToolTip.Default.SetToolTip(this.buttonNewV, Res.Strings.Action.GuideNewV);

			this.buttonDuplicate = new IconButton(Misc.Icon("DuplicateItem"));
			this.buttonDuplicate.Clicked += this.HandleButtonDuplicate;
			this.buttonDuplicate.TabIndex = index++;
			this.buttonDuplicate.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonDuplicate);
			ToolTip.Default.SetToolTip(this.buttonDuplicate, Res.Strings.Action.GuideDuplicate);

			this.buttonXfer = new IconButton(Misc.Icon("GuideXfer"));
			this.buttonXfer.Clicked += this.HandleButtonXfer;
			this.buttonXfer.TabIndex = index++;
			this.buttonXfer.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonXfer);
			ToolTip.Default.SetToolTip(this.buttonXfer, Res.Strings.Action.GuideXfer);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton(Misc.Icon("Up"));
			this.buttonUp.Clicked += this.HandleButtonUp;
			this.buttonUp.TabIndex = index++;
			this.buttonUp.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonUp);
			ToolTip.Default.SetToolTip(this.buttonUp, Res.Strings.Action.GuideUp);

			this.buttonDown = new IconButton(Misc.Icon("Down"));
			this.buttonDown.Clicked += this.HandleButtonDown;
			this.buttonDown.TabIndex = index++;
			this.buttonDown.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonDown);
			ToolTip.Default.SetToolTip(this.buttonDown, Res.Strings.Action.GuideDown);

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton(Misc.Icon("DeleteItem"));
			this.buttonDelete.Clicked += this.HandleButtonDelete;
			this.buttonDelete.TabIndex = index++;
			this.buttonDelete.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.toolBar.Items.Add(this.buttonDelete);
			ToolTip.Default.SetToolTip(this.buttonDelete, Res.Strings.Action.GuideDelete);

			this.table = new CellTable(this);
			this.table.Dock = DockStyle.Fill;
			this.table.SelectionChanged += this.HandleTableSelectionChanged;
			this.table.FlyOverChanged += this.HandleTableFlyOverChanged;
			this.table.StyleH  = CellArrayStyles.ScrollNorm;
			this.table.StyleH |= CellArrayStyles.Header;
			this.table.StyleH |= CellArrayStyles.Separator;
			this.table.StyleH |= CellArrayStyles.Mobile;
			this.table.StyleV  = CellArrayStyles.ScrollNorm;
			this.table.StyleV |= CellArrayStyles.Separator;
			this.table.StyleV |= CellArrayStyles.SelectLine;
			this.table.DefHeight = 16;
			this.table.TabIndex = 3;
			this.table.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.editGroup = new Widget(this);
			this.editGroup.PreferredWidth = this.PreferredWidth;
			this.editGroup.PreferredHeight = 22;
			this.editGroup.Dock = DockStyle.Bottom;
			this.editGroup.Margins = new Margins(0, 0, 5, 0);
			this.editGroup.TabIndex = 4;
			this.editGroup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.editType = new TextFieldCombo(this.editGroup);
			this.editType.PreferredWidth = 181;
			this.editType.IsReadOnly = true;
			foreach ( int value in System.Enum.GetValues(typeof(Settings.GuideType)) )
			{
				Settings.GuideType type = (Settings.GuideType)value;
				if ( type == Settings.GuideType.None )  continue;
				this.editType.Items.Add(Settings.Guide.TypeToString(type));
			}
			this.editType.Dock = DockStyle.Left;
			this.editType.Margins = new Margins(3, 1, 0, 0);
			this.editType.TextChanged += this.HandleEditTypeChanged;
			this.editType.ComboClosed += this.HandleEditTypeClosed;
			this.editType.TabIndex = 1;
			this.editType.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.editPosition = new TextFieldReal(this.editGroup);
			this.document.Modifier.AdaptTextFieldRealDimension(this.editPosition);
			this.editPosition.PreferredWidth = 60+10;
			this.editPosition.Dock = DockStyle.Left;
			this.editPosition.Margins = new Margins(1, 0, 0, 0);
			this.editPosition.ValueChanged += this.HandleEditPositionChanged;
			this.editPosition.TabIndex = 2;
			this.editPosition.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.DoUpdateContent();
		}
		

		public int SelectGuide
		{
			//	Repère sélectionné.
			get
			{
				return this.document.Settings.GuidesSelected;
			}

			set
			{
				if ( this.document.Settings.GuidesSelected != value )
				{
					this.document.Settings.GuidesSelected = value;

					this.ignoreChanged = true;
					this.UpdateEdits();
					this.UpdateTable();
					this.table.ShowSelect();
					this.ignoreChanged = false;
				}
			}
		}

		protected override void DoUpdateContent()
		{
			//	Effectue la mise à jour du contenu.
			int sel = this.document.Settings.GuidesSelected;
			if ( sel >= this.document.Settings.GuidesCount )
			{
				sel = this.document.Settings.GuidesCount-1;
				this.document.Modifier.OpletQueueEnable = false;
				this.document.Settings.GuidesSelected = sel;
				this.document.Modifier.OpletQueueEnable = true;
			}

			this.UpdateTable();
			this.UpdateEdits();
		}

		protected void UpdateToolBar()
		{
			//	Met à jour les boutons de la toolbar.
			int total = this.document.Settings.GuidesCount;
			int sel = this.document.Settings.GuidesSelected;

			this.buttonDuplicate.Enable = (sel != -1);
			this.buttonXfer.Enable = (sel != -1 && total > 0);
			this.buttonUp.Enable = (sel != -1 && sel > 0);
			this.buttonDown.Enable = (sel != -1 && sel < total-1);
			this.buttonDelete.Enable = (sel != -1 && total > 0);
		}

		protected void UpdateTable()
		{
			//	Met à jour le contenu de la table.
			this.radioGlobal.ActiveState =  this.document.Settings.GlobalGuides ? ActiveState.Yes : ActiveState.No;
			this.radioPage.ActiveState   = !this.document.Settings.GlobalGuides ? ActiveState.Yes : ActiveState.No;

			int rows = this.document.Settings.GuidesCount;
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(2, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 181);
				this.table.SetWidthColumn(1, 60);
			}

			this.table.SetHeaderTextH(0, Res.Strings.Container.Guides.Header.Type);
			this.table.SetHeaderTextH(1, Res.Strings.Container.Guides.Header.Position);

			for ( int i=0 ; i<rows ; i++ )
			{
				this.TableFillRow(i);
				this.TableUpdateRow(i);
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
					st.ContentAlignment = (column==0) ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight;
					st.Dock = DockStyle.Fill;
					st.Margins = new Margins(4, 4, 0, 0);
					this.table[column, row].Insert(st);
				}
			}
		}

		protected void TableUpdateRow(int row)
		{
			//	Met à jour le contenu d'une ligne de la table.
			Settings.Guide guide = this.document.Settings.GuidesGet(row);
			StaticText st;

			st = this.table[0, row].Children[0] as StaticText;
			st.Text = Settings.Guide.TypeToString(guide.Type);

			st = this.table[1, row].Children[0] as StaticText;
			if ( guide.Position == Settings.Guide.Undefined )
			{
				st.Text = Res.Strings.Container.Guides.Undefined;
			}
			else
			{
				double value = guide.Position/this.document.Modifier.RealScale;
				value *= 100.0;
				value = System.Math.Floor(value+0.5);  // arrondi à la 2ème décimale
				value /= 100.0;
				st.Text = value.ToString();
			}

			this.table.SelectRow(row, row==this.document.Settings.GuidesSelected);
		}

		protected void UpdateEdits()
		{
			//	Met à jour les widgets d'édition en fonction de la ligne sélectionnée.
			this.ignoreChanged = true;

			int sel = this.document.Settings.GuidesSelected;
			if ( sel < 0 )
			{
				this.editType.Enable = false;
				this.editPosition.Enable = false;
				this.editType.Text = "";
				this.editPosition.Text = "";
			}
			else
			{
				this.editType.Enable = true;
				this.editPosition.Enable = true;
				Settings.Guide guide = this.document.Settings.GuidesGet(sel);
				this.editType.Text = Settings.Guide.TypeToString(guide.Type);
				this.editPosition.InternalValue = (decimal) guide.Position;
			}

			this.ignoreChanged = false;
			this.UpdateToolBar();
		}


		private void HandleRadioChanged(object sender)
		{
			//	Un bouton radio a changé.
			RadioButton radio = sender as RadioButton;
			if ( radio == null )  return;
			if ( radio.ActiveState != ActiveState.Yes )  return;

			this.document.Settings.GlobalGuides = (radio == this.radioGlobal);
		}


		protected void CreateGuide(Settings.GuideType type)
		{
			//	Crée un nouveau repère.
			int sel = this.document.Settings.GuidesSelected+1;
			string action = (type == Settings.GuideType.HorizontalBottom) ? Res.Strings.Action.GuideNewH : Res.Strings.Action.GuideNewV;
			this.document.Modifier.OpletQueueBeginAction(action, "ChangeGuide", sel);
			Settings.Guide guide = new Settings.Guide(this.document);
			guide.Type = type;
			guide.Position = 0.0;
			this.document.Settings.GuidesInsert(sel, guide);
			this.document.Settings.GuidesSelected = sel;
			this.document.Modifier.OpletQueueValidateAction();
			this.UpdateEdits();
			this.editPosition.SelectAll();
			this.editPosition.Focus();
		}

		private void HandleButtonNewH(object sender, MessageEventArgs e)
		{
			//	Crée un nouveau repère horizontal.
			this.CreateGuide(Settings.GuideType.HorizontalBottom);
		}

		private void HandleButtonNewV(object sender, MessageEventArgs e)
		{
			//	Crée un nouveau repère vertical.
			this.CreateGuide(Settings.GuideType.VerticalLeft);
		}

		private void HandleButtonDuplicate(object sender, MessageEventArgs e)
		{
			//	Duplique un repère.
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.GuideDuplicate);
			int sel = this.document.Settings.GuidesSelected;
			Settings.Guide guide = new Settings.Guide(this.document);
			this.document.Settings.GuidesGet(sel).CopyTo(guide);
			this.document.Settings.GuidesInsert(++sel, guide);
			this.document.Settings.GuidesSelected = sel;
			this.document.Modifier.OpletQueueValidateAction();
			this.UpdateEdits();
			this.editPosition.SelectAll();
			this.editPosition.Focus();
		}

		private void HandleButtonXfer(object sender, MessageEventArgs e)
		{
			//	Transfère un repère (global <-> local).
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.GuideXfer);
			int sel = this.document.Settings.GuidesSelected;

			Settings.Guide guide = new Settings.Guide(this.document);
			this.document.Settings.GuidesGet(sel).CopyTo(guide);
			this.document.Settings.GuidesAddOther(guide);

			this.document.Settings.GuidesRemoveAt(sel);

			if ( sel >= this.document.Settings.GuidesCount )
			{
				sel = this.document.Settings.GuidesCount-1;
				this.table.SelectRow(sel, true);
				this.document.Settings.GuidesSelected = sel;
				this.UpdateEdits();
			}
			this.document.Modifier.OpletQueueValidateAction();
		}

		private void HandleButtonUp(object sender, MessageEventArgs e)
		{
			//	Monte d'une ligne le repère sélectionné.
			int sel = this.document.Settings.GuidesSelected;
			if ( sel < 1 )  return;
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.GuideUp);
			Settings.Guide guide = this.document.Settings.GuidesGet(sel);
			this.document.Settings.GuidesRemoveAt(sel);
			sel --;
			this.document.Settings.GuidesInsert(sel, guide);
			this.document.Settings.GuidesSelected = sel;
			this.document.Modifier.OpletQueueValidateAction();
			this.UpdateEdits();
			this.editPosition.SelectAll();
			this.editPosition.Focus();
		}

		private void HandleButtonDown(object sender, MessageEventArgs e)
		{
			//	Descend d'une ligne le repère sélectionné.
			int sel = this.document.Settings.GuidesSelected;
			if ( sel > this.document.Settings.GuidesCount-2 )  return;
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.GuideDown);
			Settings.Guide guide = this.document.Settings.GuidesGet(sel);
			this.document.Settings.GuidesRemoveAt(sel);
			sel ++;
			this.document.Settings.GuidesInsert(sel, guide);
			this.document.Settings.GuidesSelected = sel;
			this.document.Modifier.OpletQueueValidateAction();
			this.UpdateEdits();
			this.editPosition.SelectAll();
			this.editPosition.Focus();
		}

		private void HandleButtonDelete(object sender, MessageEventArgs e)
		{
			//	Supprime le repère sélectionné.
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.GuideDelete);
			int sel = this.document.Settings.GuidesSelected;
			this.document.Settings.GuidesRemoveAt(sel);
			if ( sel >= this.document.Settings.GuidesCount )
			{
				sel = this.document.Settings.GuidesCount-1;
				this.table.SelectRow(sel, true);
				this.document.Settings.GuidesSelected = sel;
				this.UpdateEdits();
			}
			this.document.Modifier.OpletQueueValidateAction();
		}

		private void HandleTableSelectionChanged(object sender)
		{
			//	Liste cliquée.
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.GuideSelect);
			this.document.Settings.GuidesSelected = this.table.SelectedRow;
			this.document.Modifier.OpletQueueValidateAction();
			this.UpdateEdits();
		}

		private void HandleTableFlyOverChanged(object sender)
		{
			//	Liste survolée.
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

			int sel = this.document.Settings.GuidesSelected;
			Settings.Guide guide = this.document.Settings.GuidesGet(sel);

			this.ignoreChanged = true;
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.GuideChangeType, "ChangeGuide", sel);
			guide.Type = Settings.Guide.StringToType(this.editType.Text);
			this.document.Modifier.OpletQueueValidateAction();
			this.UpdateTable();
			this.ignoreChanged = false;
		}

		private void HandleEditTypeClosed(object sender)
		{
			this.editPosition.SelectAll();
			this.editPosition.Focus();
		}

		private void HandleEditPositionChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			int sel = this.document.Settings.GuidesSelected;
			Settings.Guide guide = this.document.Settings.GuidesGet(sel);

			this.ignoreChanged = true;
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.GuideChangePosition, "ChangeGuide", sel);
			guide.Position = (double) this.editPosition.InternalValue;
			this.document.Modifier.OpletQueueValidateAction();
			this.UpdateTable();
			this.ignoreChanged = false;
		}


		protected Widget				radioGroup;
		protected RadioButton			radioGlobal;
		protected RadioButton			radioPage;
		protected HToolBar				toolBar;
		protected IconButton			buttonNewH;
		protected IconButton			buttonNewV;
		protected IconButton			buttonDuplicate;
		protected IconButton			buttonXfer;
		protected IconButton			buttonUp;
		protected IconButton			buttonDown;
		protected IconButton			buttonDelete;
		protected CellTable				table;
		protected Widget				editGroup;
		protected TextFieldCombo		editType;
		protected TextFieldReal			editPosition;
		protected bool					ignoreChanged = false;
	}
}
