//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Panels
{
	using CultureInfo = System.Globalization.CultureInfo;
	
	/// <summary>
	/// La classe StringEditPanel réalise un panneau pour l'édition de
	/// textes contenus dans un bundle.
	/// </summary>
	public class StringEditPanel : AbstractPanel
	{
		public StringEditPanel(StringEditController.Store store)
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
		
		public StringEditController.Store		Store
		{
			get
			{
				return this.store;
			}
		}
		
		
		public static Drawing.Size				DefaultSize
		{
			get
			{
				return new Drawing.Size (298, 400);
			}
		}
		
		
		protected override void CreateWidgets(Widget parent)
		{
			double dx = parent.Client.Width;
			double dy = parent.Client.Height;
			
			this.edit_array = new EditArray (parent);
			this.lang_combo = new TextFieldCombo ();
			this.lang_swap  = new IconButton ("manifest:Epsitec.Common.Designer.Images.SwapLang.icon");
			
			this.FillLangCombo ();
			
			EditArray.Header     title = new EditArray.Header (this.edit_array);
			EditArray.Controller ctrl  = new EditArray.Controller (this.edit_array, "Table");
			
			this.edit_array.CommandDispatcher = new Support.CommandDispatcher ("StringEditTable", true);
			this.edit_array.Anchor            = AnchorStyles.All;
			this.edit_array.AnchorMargins     = new Drawing.Margins (4, 4, 4, 65);
			this.edit_array.ColumnCount       = 2;
			this.edit_array.RowCount          = 0;
			
			TextFieldEx column_0_edit_model = new TextFieldEx ();
			
			column_0_edit_model.ButtonShowCondition = ShowCondition.WhenModified;
			column_0_edit_model.DefocusAction       = DefocusAction.Modal;
			
			new Common.Widgets.Validators.RegexValidator (column_0_edit_model, Support.RegexFactory.ResourceName, false);
			new UniqueNameValidator (column_0_edit_model);
			
			this.edit_array.Columns[0].HeaderText = "Clef";
			this.edit_array.Columns[0].IsReadOnly = true;
			this.edit_array.Columns[0].EditionWidgetModel = column_0_edit_model;
			this.edit_array.Columns[1].HeaderText = "Valeur";
			this.edit_array.Columns[1].IsReadOnly = false;
			
			this.edit_array.SetColumnWidth (0, 110);
			this.edit_array.SetColumnWidth (1, this.edit_array.GetColumnWidth (1) + this.edit_array.FreeTableWidth);
			this.edit_array.TextArrayStore = this.store;
			this.edit_array.TitleWidget = title;
			this.edit_array.SearchCaption = @"<b>Recherche. </b><font size=""90%"">Tapez le texte à chercher ci-dessous&#160;:</font>";
			
			this.edit_array.SelectedIndexChanged    += new EventHandler(this.HandleEditArraySelectedIndexChanged);
			this.edit_array.DoubleClicked           += new MessageEventHandler (this.HandleEditArrayDoubleClicked);
			this.edit_array.InteractionModeChanging += new EventHandler (this.HandleEditArrayInteractionModeChanging);
			this.edit_array.InteractionModeChanged  += new EventHandler (this.HandleEditArrayInteractionModeChanged);
			this.edit_array.TabIndex = 0;
			this.edit_array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			this.lang_combo.IsReadOnly = true;
			this.lang_combo.Width      = 60;
			this.lang_combo.Dock       = this.edit_array.ArrayToolBar.OppositeIconDockStyle;
			this.lang_combo.SelectedIndexChanged += new EventHandler (this.HandleLanguageComboSelectedIndexChanged);
			
			this.lang_swap.Dock   = this.edit_array.ArrayToolBar.OppositeIconDockStyle;
			this.lang_swap.Clicked += new MessageEventHandler (this.HandleLangSwapClicked);
			
			this.edit_array.ArrayToolBar.Items.Add (this.lang_combo);
			this.edit_array.ArrayToolBar.Items.Add (this.lang_swap);
			
			StaticText     text_label = new StaticText (parent);
			TextFieldMulti text_field = new TextFieldMulti (parent);
			
			text_label.Height           = 15;
			text_label.Text             = "Co<m>m</m>mentaire :";
			text_label.Anchor           = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			text_label.AnchorMargins    = new Drawing.Margins (4, 4, 0, 50);
			text_label.ShortcutPressed += new EventHandler (this.HandleCommentTextLabelShortcutPressed);
			
			text_field.Height        = 44;
			text_field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			text_field.Anchor        = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			text_field.AnchorMargins = new Drawing.Margins (4, 4, 0, 4);
			text_field.TabIndex      = 1;
			text_field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
//			title.Caption = @"<font size=""120%"">Bundle.</font> Édition des données contenues dans la ressource.";
			
			ctrl.CreateCommands ();
			ctrl.CreateToolBarButtons ();
			ctrl.StartReadOnly ();
			
			this.comment = text_field;
			this.comment.TextChanged += new EventHandler (this.HandleCommentTextChanged);
			
			this.edit_array.SelectedIndex = -1;
			this.lang_combo.SelectedIndex = 0;
		}
		
		
		protected virtual void FillLangCombo()
		{
			this.lang_combo.Items.Clear ();
			
			string[] names;
			string[] captions;
			
			this.store.GetLevelNamesAndCaptions (out names, out captions);
			
			for (int i = 0; i < names.Length; i++)
			{
				this.lang_combo.Items.Add (names[i], captions[i]);
			}
		}
		
		
		private void HandleEditArraySelectedIndexChanged(object sender)
		{
			int index = this.edit_array.SelectedIndex;
			
			if (index >= 0)
			{
				this.comment.Text = this.store.DefaultBundle[index].About;
			}
			else
			{
				this.comment.Text = "";
			}
		}
		
		private void HandleCommentTextChanged(object sender)
		{
			int index = this.edit_array.SelectedIndex;
			
			if (index >= 0)
			{
				this.store.DefaultBundle[index].SetAbout (this.comment.Text);
			}
		}
		
		private void HandleCommentTextLabelShortcutPressed(object sender)
		{
			if (this.comment.ContainsFocus)
			{
				this.edit_array.Focus ();
			}
			else
			{
				this.comment.SelectAll ();
				this.comment.Focus ();
			}
		}
		
		private void HandleEditArrayDoubleClicked(object sender, MessageEventArgs e)
		{
			int row, column;
			
			this.edit_array.HitTestTable (e.Point, out row, out column);
			this.edit_array.StartEdition (row, column);
		}
		
		private void HandleEditArrayInteractionModeChanging(object sender)
		{
			System.Diagnostics.Debug.Assert (this.edit_array == sender);
			
			this.old_edit_array_mode = this.edit_array.InteractionMode;
		}
		
		private void HandleEditArrayInteractionModeChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.edit_array == sender);
			
			if ((this.old_edit_array_mode == ScrollInteractionMode.Edition) &&
				(this.edit_array.InteractionMode != ScrollInteractionMode.Edition))
			{
				
				//	L'utilisateur a terminé l'édition. Si la ligne actuellement sélectionnée
				//	contient un nom invalide, on la supprime.
				
				int index = this.edit_array.SelectedIndex;
				
				if (index != -1)
				{
					if (this.store.GetCellText (index, 0) == "")
					{
						this.store.RemoveRows (index, 1);
						
						if (index >= this.edit_array.RowCount)
						{
							this.edit_array.SelectedIndex = index - 1;
						}
					}
				}
			}
		}
		
		
		private void HandleLanguageComboSelectedIndexChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.lang_combo == sender);
			
			string suffix = this.lang_combo.SelectedName;
			
			if (suffix != this.lang_suffix_1)
			{
				ResourceLevel level;
				CultureInfo   culture;
				
				Resources.MapFromSuffix (suffix, out level, out culture);
				
				this.edit_array.ValidateEdition (true);
				this.store.SetActive (level, culture);
				this.edit_array.Columns[0].IsReadOnly = ! this.store.IsDefaultActive;
				this.edit_array.InvalidateContents ();
				
				this.lang_suffix_2 = this.lang_suffix_1;
				this.lang_suffix_1 = suffix;
			}
			
			this.edit_array.Focus ();
		}
		
		private void HandleLangSwapClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.lang_swap == sender);
			
			if ((this.lang_suffix_2 != null) &&
				(this.lang_suffix_1 != this.lang_suffix_2))
			{
				this.lang_combo.SelectedName = this.lang_suffix_2;
			}
		}
		
		
		
		private class UniqueNameValidator : Common.Widgets.Validators.AbstractTextValidator
		{
			public UniqueNameValidator() : base (null)
			{
			}
			
			public UniqueNameValidator(Widget widget) : base (widget)
			{
			}
			
			
			protected override void ValidateText(string text)
			{
				Widget iter = this.widget;
				
				this.state = Support.ValidationState.Ok;
				
				while (iter != null)
				{
					EditArray edit_array = iter as EditArray;
					
					if ((edit_array != null) &&
						(edit_array.InteractionMode == ScrollInteractionMode.Edition))
					{
						int                        index = edit_array.SelectedIndex;
						StringEditController.Store store = edit_array.TextArrayStore as StringEditController.Store;
						
						int max_rows = store.GetRowCount ();
						
						for (int i = 0; i < max_rows; i++)
						{
							if (i != index)
							{
								if (store.GetCellText (i, 0) == text)
								{
									this.state = Support.ValidationState.Error;
									return;
								}
							}
						}
						
						break;
					}
					
					iter = iter.Parent;
				}
			}
		}
		
		
		protected EditArray						edit_array;
		protected TextFieldCombo				lang_combo;
		protected IconButton					lang_swap;
		protected string						lang_suffix_1;
		protected string						lang_suffix_2;
		protected AbstractTextField				comment;
		protected StringEditController.Store	store;
		protected ScrollInteractionMode			old_edit_array_mode;
	}
}
