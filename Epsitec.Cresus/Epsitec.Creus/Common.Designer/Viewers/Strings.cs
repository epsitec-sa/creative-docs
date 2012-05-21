using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Strings : Abstract
	{
		public Strings(Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication)
			: base (module, context, access, designerApplication)
		{
			//	Résumé des captions.
			MyWidgets.StackedPanel leftContainer, rightContainer;

			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Brief, BandMode.MainSummary, GlyphShape.ArrowDown, false, 0.3);
			this.buttonMainExtendLeft = leftContainer.ExtendButton;
			this.buttonMainExtendRight = rightContainer.ExtendButton;
			this.buttonMainExtendLeft.Clicked += this.HandleButtonCompactOrExtendClicked;
			this.buttonMainExtendRight.Clicked += this.HandleButtonCompactOrExtendClicked;

			this.primarySummary = new StaticText(leftContainer.Container);
			this.primarySummary.MinHeight = 30;
			this.primarySummary.Dock = DockStyle.Fill;

			this.secondarySummary = new StaticText(rightContainer.Container);
			this.secondarySummary.MinHeight = 30;
			this.secondarySummary.Dock = DockStyle.Fill;

			//	Textes.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Labels.Title, BandMode.MainView, GlyphShape.ArrowUp, false, 0.3);
			this.buttonMainCompactLeft = leftContainer.ExtendButton;
			this.buttonMainCompactRight = rightContainer.ExtendButton;
			this.buttonMainCompactLeft.Clicked += this.HandleButtonCompactOrExtendClicked;
			this.buttonMainCompactRight.Clicked += this.HandleButtonCompactOrExtendClicked;

			this.groupPrimaryText = new MyWidgets.ResetBox(leftContainer.Container);
			this.groupPrimaryText.IsPatch = this.module.IsPatch;
			this.groupPrimaryText.Dock = DockStyle.Fill;
			this.groupPrimaryText.ResetButton.Clicked += this.HandleResetButtonClicked;

			this.groupSecondaryText = new MyWidgets.ResetBox(rightContainer.Container);
			this.groupSecondaryText.IsPatch = this.module.IsPatch;
			this.groupSecondaryText.Dock = DockStyle.Fill;
			this.groupSecondaryText.ResetButton.Clicked += this.HandleResetButtonClicked;

			this.primaryText = new TextFieldMulti(this.groupPrimaryText.GroupBox);
			this.primaryText.MaxLength = 50000;
			this.primaryText.AcceptsNullValue = true;
			this.primaryText.PreferredHeight = 10+14*6;
			this.primaryText.Dock = DockStyle.StackBegin;
			this.primaryText.TextChanged += this.HandleTextChanged;
			this.primaryText.CursorChanged += this.HandleCursorChanged;
			this.primaryText.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryText.TabIndex = this.tabIndex++;
			this.primaryText.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryText = new TextFieldMulti(this.groupSecondaryText.GroupBox);
			this.secondaryText.MaxLength = 50000;
			this.secondaryText.AcceptsNullValue = true;
			this.secondaryText.PreferredHeight = 10+14*6;
			this.secondaryText.Dock = DockStyle.StackBegin;
			this.secondaryText.TextChanged += this.HandleTextChanged;
			this.secondaryText.CursorChanged += this.HandleCursorChanged;
			this.secondaryText.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryText.TabIndex = this.tabIndex++;
			this.secondaryText.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Commentaires.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.About.Title, BandMode.MainView, GlyphShape.None, false, 0.3);

			this.groupPrimaryComment = new MyWidgets.ResetBox(leftContainer.Container);
			this.groupPrimaryComment.IsPatch = this.module.IsPatch;
			this.groupPrimaryComment.Dock = DockStyle.Fill;
			this.groupPrimaryComment.ResetButton.Clicked += this.HandleResetButtonClicked;

			this.groupSecondaryComment = new MyWidgets.ResetBox(rightContainer.Container);
			this.groupSecondaryComment.IsPatch = this.module.IsPatch;
			this.groupSecondaryComment.Dock = DockStyle.Fill;
			this.groupSecondaryComment.ResetButton.Clicked += this.HandleResetButtonClicked;

			this.primaryComment = new TextFieldMulti(this.groupPrimaryComment.GroupBox);
			this.primaryComment.AcceptsNullValue = true;
			this.primaryComment.PreferredHeight = 10+14*4;
			this.primaryComment.Dock = DockStyle.StackBegin;
			this.primaryComment.TextChanged += this.HandleTextChanged;
			this.primaryComment.CursorChanged += this.HandleCursorChanged;
			this.primaryComment.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryComment.TabIndex = this.tabIndex++;
			this.primaryComment.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryComment = new TextFieldMulti(this.groupSecondaryComment.GroupBox);
			this.secondaryComment.AcceptsNullValue = true;
			this.secondaryComment.PreferredHeight = 10+14*4;
			this.secondaryComment.Dock = DockStyle.StackBegin;
			this.secondaryComment.TextChanged += this.HandleTextChanged;
			this.secondaryComment.CursorChanged += this.HandleCursorChanged;
			this.secondaryComment.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryComment.TabIndex = this.tabIndex++;
			this.secondaryComment.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			if (this.access.IsJustLoaded)
			{
				this.access.IsJustLoaded = false;
				this.access.CollectionView.MoveCurrentToFirst();
			}

			this.UpdateAll();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.groupPrimaryText.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupSecondaryText.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupPrimaryComment.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupSecondaryComment.ResetButton.Clicked -= this.HandleResetButtonClicked;
				
				this.primaryText.TextChanged -= this.HandleTextChanged;
				this.primaryText.CursorChanged -= this.HandleCursorChanged;
				this.primaryText.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryText.TextChanged -= this.HandleTextChanged;
				this.secondaryText.CursorChanged -= this.HandleCursorChanged;
				this.secondaryText.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.primaryComment.TextChanged -= this.HandleTextChanged;
				this.primaryComment.CursorChanged -= this.HandleCursorChanged;
				this.primaryComment.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryComment.TextChanged -= this.HandleTextChanged;
				this.secondaryComment.CursorChanged -= this.HandleCursorChanged;
				this.secondaryComment.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Strings;
			}
		}

		
		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			base.UpdateEdit();

			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			bool usesOriginalData;

			this.groupPrimaryText.Enable = !this.designerApplication.IsReadonly;
			this.groupPrimaryComment.Enable = !this.designerApplication.IsReadonly;
			this.groupSecondaryText.Enable = !this.designerApplication.IsReadonly;
			this.groupSecondaryComment.Enable = !this.designerApplication.IsReadonly;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			CultureMapSource source = this.access.GetCultureMapSource(item);
			StructuredData data = null;

			if (item == null)
			{
				this.primaryText.Text = "";
				this.primaryComment.Text = "";
				this.groupPrimaryText.Enable = false;
				this.groupPrimaryComment.Enable = false;
			}
			else
			{
				data = item.GetCultureData(this.GetTwoLetters(0));
				this.primaryText.Text = data.GetValue(Support.Res.Fields.ResourceString.Text, out usesOriginalData) as string ?? Support.ResourceBundle.Field.Null;
				this.ColorizeResetBox(this.groupPrimaryText, source, usesOriginalData);
				this.primaryComment.Text = data.GetValue(Support.Res.Fields.ResourceBase.Comment, out usesOriginalData) as string ?? Support.ResourceBundle.Field.Null;
				this.ColorizeResetBox(this.groupPrimaryComment, source, usesOriginalData);
			}

			if (item == null || data == null || this.GetTwoLetters(1) == null)
			{
				this.secondaryText.Text = "";
				this.secondaryComment.Text = "";
				this.groupSecondaryText.Enable = false;
				this.groupSecondaryComment.Enable = false;
			}
			else
			{
				data = item.GetCultureData(this.GetTwoLetters(1));
				this.secondaryText.Text = data.GetValue(Support.Res.Fields.ResourceString.Text, out usesOriginalData) as string ?? Support.ResourceBundle.Field.Null;
				this.ColorizeResetBox(this.groupSecondaryText, source, usesOriginalData);
				this.secondaryComment.Text = data.GetValue(Support.Res.Fields.ResourceBase.Comment, out usesOriginalData) as string ?? Support.ResourceBundle.Field.Null;
				this.ColorizeResetBox(this.groupSecondaryComment, source, usesOriginalData);
			}

			this.ignoreChange = iic;
		}


		protected override string GetSummary(string twoLettersCulture)
		{
			//	Retourne le texte résumé de la ressource sélectionnée.
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();

			if (twoLettersCulture == null)
			{
				buffer.Append(Misc.Italic("(indéfini)"));
			}
			else
			{
				CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;

				if (item == null)
				{
					buffer.Append (Misc.Italic ("(indéfini)"));
				}
				else
				{
					StructuredData data = item.GetCultureData(twoLettersCulture);

					string text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;
					if (text == null)
					{
						buffer.Append(Misc.Italic("(indéfini)"));
					}
					else if (text.Length == 0)
					{
						buffer.Append(Misc.Italic("(vide)"));
					}
					else
					{
						buffer.Append(text);
					}

					string comment = data.GetValue(Support.Res.Fields.ResourceBase.Comment) as string;
					if (!string.IsNullOrEmpty(comment))
					{
						buffer.Append("<br/>");
						buffer.Append(Misc.Italic(comment));
					}
				}
			}

			return buffer.ToString();
		}


		public static void SearchCreateFilterGroup(AbstractGroup parent, EventHandler handler)
		{
			StaticText label;
			CheckButton check;
			
			label = new StaticText(parent);
			label.PreferredWidth = 80;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Text = Res.Strings.Viewers.Strings.Edit;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 0, 0);

			label = new StaticText(parent);
			label.PreferredWidth = 80;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Text = Res.Strings.Viewers.Strings.About;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 16, 0);

			check = new CheckButton(parent);
			check.Name = "0";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*0, 0, 0, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += handler;
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.Label);

			check = new CheckButton(parent);
			check.Name = "1";  // (*)
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 0, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += handler;
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryText);

			check = new CheckButton(parent);
			check.Name = "2";  // (*)
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 0, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += handler;
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryText);

			check = new CheckButton(parent);
			check.Name = "3";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 16, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += handler;
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryAbout);

			check = new CheckButton(parent);
			check.Name = "4";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 16, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += handler;
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryAbout);

			// (*)	Ce numéro correspond à field dans ResourceAccess.SearcherIndexToAccess !
		}

		protected override void TextFieldToIndex(AbstractTextField textField, out int field, out int subfield)
		{
			//	Cherche les index correspondant à un texte éditable.
			subfield = 0;

			if (textField == this.labelEdit)
			{
				field = 0;
				return;
			}

			if (textField == this.primaryText)
			{
				field = 1;
				return;
			}

			if (textField == this.primaryComment)
			{
				field = 3;
				return;
			}

			if (this.secondaryCulture != null)
			{
				if (textField == this.secondaryText)
				{
					field = 2;
					return;
				}

				if (textField == this.secondaryComment)
				{
					field = 4;
					return;
				}
			}

			field = -1;
			subfield = -1;
		}

		protected override AbstractTextField IndexToTextField(int field, int subfield)
		{
			//	Cherche le TextField permettant d'éditer des index.
			if (subfield == 0)
			{
				switch (field)
				{
					case 0:
						return this.labelEdit;

					case 1:
						return this.primaryText;

					case 2:
						return this.secondaryText;

					case 3:
						return this.primaryComment;

					case 4:
						return this.secondaryComment;

				}
			}

			return null;
		}

		
		private void HandleTextChanged(object sender)
		{
			//	Un texte éditable a changé.
			if (this.ignoreChange)
			{
				return;
			}

			AbstractTextField edit = sender as AbstractTextField;
			string text = edit.Text;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;

			if (edit == this.primaryText)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				this.SetValue(item, data, Support.Res.Fields.ResourceString.Text, text, true);
			}

			if (edit == this.secondaryText)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(1));
				this.SetValue(item, data, Support.Res.Fields.ResourceString.Text, text, true);
			}

			if (edit == this.primaryComment)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				this.SetValue(item, data, Support.Res.Fields.ResourceBase.Comment, text, true);
			}

			if (edit == this.secondaryComment)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(1));
				this.SetValue(item, data, Support.Res.Fields.ResourceBase.Comment, text, true);
			}

			this.UpdateModificationsCulture();
		}


		public override string GetColumnText(CultureMap item, string twoLettersCulture)
		{
			//	Retourne le texte pour une colonne primaire ou secondaire.
			if (twoLettersCulture == null)
			{
				return "";
			}
			else
			{
				StructuredData data = item.GetCultureData(twoLettersCulture);
				string text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;
				return (text == null) ? "" : text;
			}
		}

		private void HandleResetButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;

			if (button == this.groupPrimaryText.ResetButton)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				this.access.Accessor.ResetToOriginalValue(item, data, Support.Res.Fields.ResourceString.Text);
			}

			if (button == this.groupSecondaryText.ResetButton)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(1));
				this.access.Accessor.ResetToOriginalValue(item, data, Support.Res.Fields.ResourceString.Text);
			}

			if (button == this.groupPrimaryComment.ResetButton)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				this.access.Accessor.ResetToOriginalValue(item, data, Support.Res.Fields.ResourceBase.Comment);
			}

			if (button == this.groupSecondaryComment.ResetButton)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(1));
				this.access.Accessor.ResetToOriginalValue(item, data, Support.Res.Fields.ResourceBase.Comment);
			}

			this.UpdateEdit();
			this.access.SetLocalDirty();
		}


		protected MyWidgets.ResetBox			groupPrimaryText;
		protected TextFieldMulti				primaryText;
		protected MyWidgets.ResetBox			groupSecondaryText;
		protected TextFieldMulti				secondaryText;
		protected MyWidgets.ResetBox			groupPrimaryComment;
		protected TextFieldMulti				primaryComment;
		protected MyWidgets.ResetBox			groupSecondaryComment;
		protected TextFieldMulti				secondaryComment;
	}
}
