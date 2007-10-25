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
		public Strings(Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication) : base(module, context, access, designerApplication)
		{
			//	Résumé des captions.
			MyWidgets.StackedPanel leftContainer, rightContainer;
			MyWidgets.ResetBox leftResetBox, rightResetBox;

			this.buttonMainExtend = this.CreateBand(out leftContainer, out rightContainer, "Résumé", BandMode.MainSummary, GlyphShape.ArrowDown, false, 0.3);
			this.buttonMainExtend.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			this.primarySummary = new StaticText(leftContainer.Container);
			this.primarySummary.MinHeight = 30;
			this.primarySummary.Dock = DockStyle.Fill;

			this.secondarySummary = new StaticText(rightContainer.Container);
			this.secondarySummary.MinHeight = 30;
			this.secondarySummary.Dock = DockStyle.Fill;

			//	Textes.
			this.buttonMainCompact = this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Labels.Title, BandMode.MainView, GlyphShape.ArrowUp, false, 0.3);
			this.buttonMainCompact.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			leftResetBox = new MyWidgets.ResetBox(leftContainer.Container);
			leftResetBox.Dock = DockStyle.Fill;

			rightResetBox = new MyWidgets.ResetBox(rightContainer.Container);
			rightResetBox.Dock = DockStyle.Fill;

			this.primaryText = new TextFieldMulti(leftResetBox.GroupBox);
			this.primaryText.AcceptsNullValue = true;
			this.primaryText.PreferredHeight = 10+14*6;
			this.primaryText.Dock = DockStyle.StackBegin;
			this.primaryText.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryText.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryText.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryText.TabIndex = this.tabIndex++;
			this.primaryText.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryText = new TextFieldMulti(rightResetBox.GroupBox);
			this.secondaryText.AcceptsNullValue = true;
			this.secondaryText.PreferredHeight = 10+14*6;
			this.secondaryText.Dock = DockStyle.StackBegin;
			this.secondaryText.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryText.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryText.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryText.TabIndex = this.tabIndex++;
			this.secondaryText.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Commentaires.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.About.Title, BandMode.MainView, GlyphShape.None, false, 0.3);

			leftResetBox = new MyWidgets.ResetBox(leftContainer.Container);
			leftResetBox.Dock = DockStyle.Fill;

			rightResetBox = new MyWidgets.ResetBox(rightContainer.Container);
			rightResetBox.Dock = DockStyle.Fill;

			this.primaryComment = new TextFieldMulti(leftResetBox.GroupBox);
			this.primaryComment.AcceptsNullValue = true;
			this.primaryComment.PreferredHeight = 10+14*4;
			this.primaryComment.Dock = DockStyle.StackBegin;
			this.primaryComment.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryComment.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryComment.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryComment.TabIndex = this.tabIndex++;
			this.primaryComment.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryComment = new TextFieldMulti(rightResetBox.GroupBox);
			this.secondaryComment.AcceptsNullValue = true;
			this.secondaryComment.PreferredHeight = 10+14*4;
			this.secondaryComment.Dock = DockStyle.StackBegin;
			this.secondaryComment.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryComment.CursorChanged += new EventHandler(this.HandleCursorChanged);
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
				this.primaryText.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.primaryText.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.primaryText.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryText.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.secondaryText.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.secondaryText.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.primaryComment.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.primaryComment.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.primaryComment.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryComment.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.secondaryComment.CursorChanged -= new EventHandler(this.HandleCursorChanged);
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

			this.primaryText.Enable = !this.designerApplication.IsReadonly;
			this.primaryComment.Enable = !this.designerApplication.IsReadonly;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			StructuredData data;

			data = item.GetCultureData(this.GetTwoLetters(0));
			this.primaryText.Text = data.GetValue (Support.Res.Fields.ResourceString.Text) as string ?? Support.ResourceBundle.Field.Null;
			this.primaryComment.Text = data.GetValue (Support.Res.Fields.ResourceBase.Comment) as string ?? Support.ResourceBundle.Field.Null;

			if (this.GetTwoLetters(1) == null || this.designerApplication.IsReadonly)
			{
				this.secondaryText.Text = "";
				this.secondaryComment.Text = "";
				this.secondaryText.Enable = false;
				this.secondaryComment.Enable = false;
			}
			else
			{
				data = item.GetCultureData(this.GetTwoLetters(1));
				this.secondaryText.Text = data.GetValue (Support.Res.Fields.ResourceString.Text) as string ?? Support.ResourceBundle.Field.Null;
				this.secondaryComment.Text = data.GetValue (Support.Res.Fields.ResourceBase.Comment) as string ?? Support.ResourceBundle.Field.Null;
				this.secondaryText.Enable = true;
				this.secondaryComment.Enable = true;
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
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.Label);

			check = new CheckButton(parent);
			check.Name = "1";  // (*)
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 0, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryText);

			check = new CheckButton(parent);
			check.Name = "2";  // (*)
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 0, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryText);

			check = new CheckButton(parent);
			check.Name = "3";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 16, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryAbout);

			check = new CheckButton(parent);
			check.Name = "4";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 16, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
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


		protected TextFieldMulti				primaryText;
		protected TextFieldMulti				secondaryText;
		protected TextFieldMulti				primaryComment;
		protected TextFieldMulti				secondaryComment;
	}
}
