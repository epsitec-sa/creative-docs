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
	public abstract class AbstractCaptions : Abstract
	{
		public AbstractCaptions(Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication)
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

			this.primarySummaryIcon = new IconButton(leftContainer.Container);
			this.primarySummaryIcon.MinSize = new Size(30, 30);
			this.primarySummaryIcon.Dock = DockStyle.Right;

			this.secondarySummary = new StaticText(rightContainer.Container);
			this.secondarySummary.MinHeight = 30;
			this.secondarySummary.Dock = DockStyle.Fill;

			this.secondarySummaryIcon = new IconButton(rightContainer.Container);
			this.secondarySummaryIcon.MinSize = new Size(30, 30);
			this.secondarySummaryIcon.Dock = DockStyle.Right;

			//	Textes.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Labels.Title, BandMode.MainView, GlyphShape.ArrowUp, false, 0.3);
			this.buttonMainCompactLeft = leftContainer.ExtendButton;
			this.buttonMainCompactRight = rightContainer.ExtendButton;
			this.buttonMainCompactLeft.Clicked += this.HandleButtonCompactOrExtendClicked;
			this.buttonMainCompactRight.Clicked += this.HandleButtonCompactOrExtendClicked;

			this.groupPrimaryLabels = new MyWidgets.ResetBox(leftContainer.Container);
			this.groupPrimaryLabels.IsPatch = this.module.IsPatch;
			this.groupPrimaryLabels.Dock = DockStyle.Fill;
			this.groupPrimaryLabels.ResetButton.Clicked += this.HandleResetButtonClicked;

			this.groupSecondaryLabels = new MyWidgets.ResetBox(rightContainer.Container);
			this.groupSecondaryLabels.IsPatch = this.module.IsPatch;
			this.groupSecondaryLabels.Dock = DockStyle.Fill;
			this.groupSecondaryLabels.ResetButton.Clicked += this.HandleResetButtonClicked;

			this.primaryLabels = new MyWidgets.StringCollection(this.groupPrimaryLabels.GroupBox);
			this.primaryLabels.Dock = DockStyle.StackBegin;
			this.primaryLabels.StringTextChanged += this.HandleStringTextCollectionChanged;
			this.primaryLabels.StringFocusChanged += this.HandleStringFocusCollectionChanged;
			this.primaryLabels.TabIndex = this.tabIndex++;
			this.primaryLabels.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.secondaryLabels = new MyWidgets.StringCollection(this.groupSecondaryLabels.GroupBox);
			this.secondaryLabels.Dock = DockStyle.StackBegin;
			this.secondaryLabels.StringTextChanged += this.HandleStringTextCollectionChanged;
			this.secondaryLabels.StringFocusChanged += this.HandleStringFocusCollectionChanged;
			this.secondaryLabels.TabIndex = this.tabIndex++;
			this.secondaryLabels.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			//	Description.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Description.Title, BandMode.MainView, GlyphShape.None, false, 0.3);

			this.groupPrimaryDescription = new MyWidgets.ResetBox(leftContainer.Container);
			this.groupPrimaryDescription.IsPatch = this.module.IsPatch;
			this.groupPrimaryDescription.Dock = DockStyle.Fill;
			this.groupPrimaryDescription.ResetButton.Clicked += this.HandleResetButtonClicked;

			this.groupSecondaryDescription = new MyWidgets.ResetBox(rightContainer.Container);
			this.groupSecondaryDescription.IsPatch = this.module.IsPatch;
			this.groupSecondaryDescription.Dock = DockStyle.Fill;
			this.groupSecondaryDescription.ResetButton.Clicked += this.HandleResetButtonClicked;

			this.primaryDescription = new TextFieldMulti(this.groupPrimaryDescription.GroupBox);
			this.primaryDescription.AcceptsNullValue = true;
			this.primaryDescription.PreferredHeight = 10+14*4;
			this.primaryDescription.Dock = DockStyle.StackBegin;
			this.primaryDescription.TextChanged += this.HandleTextChanged;
			this.primaryDescription.CursorChanged += this.HandleCursorChanged;
			this.primaryDescription.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryDescription.TabIndex = this.tabIndex++;
			this.primaryDescription.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryDescription = new TextFieldMulti(this.groupSecondaryDescription.GroupBox);
			this.secondaryDescription.AcceptsNullValue = true;
			this.secondaryDescription.PreferredHeight = 10+14*4;
			this.secondaryDescription.Dock = DockStyle.StackBegin;
			this.secondaryDescription.TextChanged += this.HandleTextChanged;
			this.secondaryDescription.CursorChanged += this.HandleCursorChanged;
			this.secondaryDescription.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryDescription.TabIndex = this.tabIndex++;
			this.secondaryDescription.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Icône.
			this.CreateBand(out leftContainer, Res.Strings.Viewers.Captions.Icon.Title, BandMode.MainView, GlyphShape.None, false, 0.3);

			this.groupPrimaryIcon = new MyWidgets.ResetBox(leftContainer.Container);
			this.groupPrimaryIcon.IsPatch = this.module.IsPatch;
			this.groupPrimaryIcon.Dock = DockStyle.Fill;
			this.groupPrimaryIcon.ResetButton.Clicked += this.HandleResetButtonClicked;

			StaticText label = new StaticText(this.groupPrimaryIcon.GroupBox);
			label.Text = Res.Strings.Viewers.Captions.Icon.Title;
			label.MinHeight = 30;  // attention, très important !
			label.PreferredHeight = 30;
			label.PreferredWidth = 30;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Margins = new Margins(0, 5, 0, 0);
			label.Dock = DockStyle.Left;

			this.primaryIcon = new IconButton(this.groupPrimaryIcon.GroupBox);
			this.primaryIcon.MinHeight = 30;  // attention, très important !
			this.primaryIcon.PreferredHeight = 30;
			this.primaryIcon.PreferredWidth = 30;
			this.primaryIcon.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryIcon.Dock = DockStyle.Left;
			this.primaryIcon.TabIndex = this.tabIndex++;
			this.primaryIcon.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.primaryIcon.Clicked += this.HandlePrimaryIconClicked;

			this.primaryIconInfo = new StaticText(this.groupPrimaryIcon.GroupBox);
			this.primaryIconInfo.PreferredHeight = 30;
			this.primaryIconInfo.PreferredWidth = 300;
			this.primaryIconInfo.Margins = new Margins(10, 0, 0, 0);
			this.primaryIconInfo.Dock = DockStyle.Left;

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
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.groupPrimaryLabels.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupSecondaryLabels.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupPrimaryDescription.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupSecondaryDescription.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupPrimaryIcon.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupPrimaryComment.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupSecondaryComment.ResetButton.Clicked -= this.HandleResetButtonClicked;
				
				this.primaryLabels.StringTextChanged -= this.HandleStringTextCollectionChanged;
				this.primaryLabels.StringFocusChanged -= this.HandleStringFocusCollectionChanged;

				this.secondaryLabels.StringTextChanged -= this.HandleStringTextCollectionChanged;
				this.secondaryLabels.StringFocusChanged -= this.HandleStringFocusCollectionChanged;

				this.primaryDescription.TextChanged -= this.HandleTextChanged;
				this.primaryDescription.CursorChanged -= this.HandleCursorChanged;
				this.primaryDescription.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryDescription.TextChanged -= this.HandleTextChanged;
				this.secondaryDescription.CursorChanged -= this.HandleCursorChanged;
				this.secondaryDescription.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.primaryIcon.Clicked -= this.HandlePrimaryIconClicked;

				this.primaryComment.TextChanged -= this.HandleTextChanged;
				this.primaryComment.CursorChanged -= this.HandleCursorChanged;
				this.primaryComment.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryComment.TextChanged -= this.HandleTextChanged;
				this.secondaryComment.CursorChanged -= this.HandleCursorChanged;
				this.secondaryComment.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			}

			base.Dispose(disposing);
		}


		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			base.UpdateEdit();

			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			bool usesOriginalData;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			CultureMapSource source = this.access.GetCultureMapSource(item);
			StructuredData data = null;

			if (item != null)
			{
				data = item.GetCultureData(this.GetTwoLetters(0));
				this.SetTextField(this.primaryLabels, data.GetValue(Support.Res.Fields.ResourceCaption.Labels, out usesOriginalData) as IList<string>);
				this.ColorizeResetBox(this.groupPrimaryLabels, source, usesOriginalData);
				this.primaryDescription.Text = data.GetValue(Support.Res.Fields.ResourceCaption.Description, out usesOriginalData) as string;
				this.ColorizeResetBox(this.groupPrimaryDescription, source, usesOriginalData);
				this.primaryComment.Text = data.GetValue(Support.Res.Fields.ResourceBase.Comment, out usesOriginalData) as string;
				this.ColorizeResetBox(this.groupPrimaryComment, source, usesOriginalData);
			}

			this.groupPrimaryLabels.Enable = !this.designerApplication.IsReadonly;
			this.groupPrimaryDescription.Enable = !this.designerApplication.IsReadonly;
			this.groupPrimaryComment.Enable = !this.designerApplication.IsReadonly;

			if (data == null || this.GetTwoLetters(1) == null || this.designerApplication.IsReadonly)
			{
				this.secondaryLabels.Collection = null;
				this.secondaryDescription.Text = "";
				this.secondaryComment.Text = "";
				this.groupSecondaryLabels.Enable = false;
				this.groupSecondaryDescription.Enable = false;
				this.groupSecondaryComment.Enable = false;
			}
			else
			{
				data = item.GetCultureData(this.GetTwoLetters(1));
				this.SetTextField(this.secondaryLabels, data.GetValue(Support.Res.Fields.ResourceCaption.Labels, out usesOriginalData) as IList<string>);
				this.ColorizeResetBox(this.groupSecondaryLabels, source, usesOriginalData);
				this.secondaryDescription.Text = data.GetValue(Support.Res.Fields.ResourceCaption.Description, out usesOriginalData) as string;
				this.ColorizeResetBox(this.groupSecondaryDescription, source, usesOriginalData);
				this.secondaryComment.Text = data.GetValue(Support.Res.Fields.ResourceBase.Comment, out usesOriginalData) as string;
				this.ColorizeResetBox(this.groupSecondaryComment, source, usesOriginalData);
				this.groupSecondaryLabels.Enable = true;
				this.groupSecondaryDescription.Enable = true;
				this.groupSecondaryComment.Enable = true;
			}

			this.UpdateIcon();

			this.ignoreChange = iic;
		}

		private void UpdateIcon()
		{
			this.groupPrimaryIcon.Enable = !this.designerApplication.IsReadonly;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			string icon = null;
			if (item != null)
			{
				CultureMapSource source = this.access.GetCultureMapSource(item);
				StructuredData data = item.GetCultureData("00");
				bool usesOriginalData;
				icon = data.GetValue(Support.Res.Fields.ResourceCaption.Icon, out usesOriginalData) as string;
				this.ColorizeResetBox(this.groupPrimaryIcon, source, usesOriginalData);
			}

			if (string.IsNullOrEmpty(icon))
			{
				this.primaryIcon.IconUri = null;
				this.primaryIconInfo.Text = Res.Strings.Dialog.Icon.None;
			}
			else
			{
				this.primaryIcon.IconUri = icon;

				string module, name;
				Misc.GetIconNames(icon, out module, out name);
				
				if (string.IsNullOrEmpty(name))
				{
					this.primaryIconInfo.Text = Res.Strings.Dialog.Icon.None;
				}
				else
				{
					this.primaryIconInfo.Text = string.Format("{0}<br/>{1}", module, name);
				}
			}

			this.primarySummaryIcon.IconUri = this.primaryIcon.IconUri;
			this.secondarySummaryIcon.IconUri = this.primaryIcon.IconUri;
		}


		protected override string GetSummary(string twoLettersCulture)
		{
			//	Retourne le texte résumé de la ressource sélectionnée.
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();

			if (twoLettersCulture == null)
			{
				buffer.Append(Misc.Italic(Res.Strings.Viewers.Captions.Summary.Undefined));
			}
			else
			{
				CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
				if (item == null)
				{
					buffer.Append(Misc.Italic(Res.Strings.Viewers.Captions.Summary.Empty));
				}
				else
				{
					StructuredData data = item.GetCultureData(twoLettersCulture);

					IList<string> list = data.GetValue(Support.Res.Fields.ResourceCaption.Labels) as IList<string>;
					string desc = data.GetValue(Support.Res.Fields.ResourceCaption.Description) as string;

					if ((list == null || list.Count == 0) && string.IsNullOrEmpty(desc))
					{
						buffer.Append(Misc.Italic(Res.Strings.Viewers.Captions.Summary.Undefined));
					}
					else
					{
						if (list != null && list.Count != 0)
						{
							for (int i=0; i<list.Count; i++)
							{
								buffer.Append(list[i]);
								if (i < list.Count-1)
								{
									buffer.Append(", ");
								}
							}
							buffer.Append("<br/>");
						}

						if (!string.IsNullOrEmpty(desc))
						{
							buffer.Append(desc);
						}
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
			label.Text = Res.Strings.Viewers.Captions.Labels.Title;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 0, 0);

			label = new StaticText(parent);
			label.PreferredWidth = 80;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Text = Res.Strings.Viewers.Captions.Description.Short;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 16, 0);

			label = new StaticText(parent);
			label.PreferredWidth = 80;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Text = Res.Strings.Viewers.Captions.About.Title;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 32, 0);

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
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryLabels);

			check = new CheckButton(parent);
			check.Name = "2";  // (*)
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 0, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += handler;
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryLabels);

			check = new CheckButton(parent);
			check.Name = "3";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 16, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += handler;
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryDescription);

			check = new CheckButton(parent);
			check.Name = "4";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 16, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += handler;
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryDescription);

			check = new CheckButton(parent);
			check.Name = "5";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 32, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += handler;
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryAbout);

			check = new CheckButton(parent);
			check.Name = "6";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 32, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += handler;
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryAbout);

			// (*)	Ce numéro correspond à field dans ResourceAccess.SearcherIndexToAccess !
		}

		protected override void TextFieldToIndex(AbstractTextField textField, out int field, out int subfield)
		{
			//	Cherche les index correspondant à un texte éditable.
			if (textField == this.labelEdit)
			{
				field = 0;
				subfield = 0;
				return;
			}

			subfield = this.primaryLabels.GetIndex(textField);
			if (subfield != -1)
			{
				field = 1;
				return;
			}

			if (textField == this.primaryDescription)
			{
				field = 3;
				subfield = 0;
				return;
			}

			if (textField == this.primaryComment)
			{
				field = 5;
				subfield = 0;
				return;
			}

			if (this.secondaryCulture != null)
			{
				subfield = this.secondaryLabels.GetIndex(textField);
				if (subfield != -1)
				{
					field = 2;
					return;
				}

				if (textField == this.secondaryDescription)
				{
					field = 4;
					subfield = 0;
					return;
				}

				if (textField == this.secondaryComment)
				{
					field = 6;
					subfield = 0;
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

					case 3:
						return this.primaryDescription;

					case 4:
						return this.secondaryDescription;

					case 5:
						return this.primaryComment;

					case 6:
						return this.secondaryComment;

				}
			}

			if (field == 1)
			{
				return this.primaryLabels.GetTextField(subfield);
			}

			if (field == 2)
			{
				return this.secondaryLabels.GetTextField(subfield);
			}

			return null;
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
				System.Text.StringBuilder buffer = new System.Text.StringBuilder();
				StructuredData data = item.GetCultureData(twoLettersCulture);

				IList<string> list = data.GetValue(Support.Res.Fields.ResourceCaption.Labels) as IList<string>;
				if (list != null && list.Count != 0)
				{
					foreach(string s in list)
					{
						if (buffer.Length != 0)
						{
							buffer.Append(", ");
						}
						buffer.Append(s);
					}
				}

				string desc = data.GetValue(Support.Res.Fields.ResourceCaption.Description) as string;
				if (!string.IsNullOrEmpty(desc))
				{
					if (buffer.Length != 0)
					{
						buffer.Append(", ");
					}
					buffer.Append(desc);
				}

				return buffer.ToString();
			}
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
			if (item == null)
			{
				return;
			}

			if (edit == this.primaryDescription)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				this.SetValue(item, data, Support.Res.Fields.ResourceCaption.Description, text, true);
			}

			if (edit == this.secondaryDescription)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(1));
				this.SetValue(item, data, Support.Res.Fields.ResourceCaption.Description, text, true);
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

		private void HandleStringTextCollectionChanged(object sender)
		{
			//	Une collection de textes a changé.
			if (this.ignoreChange)
			{
				return;
			}

			MyWidgets.StringCollection sc = sender as MyWidgets.StringCollection;
			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			if (item == null)
			{
				return;
			}

			if (sc == this.primaryLabels)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				this.SetValue(item, data, Support.Res.Fields.ResourceCaption.Labels, sc.Collection, true);
			}

			if (sc == this.secondaryLabels)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(1));
				this.SetValue(item, data, Support.Res.Fields.ResourceCaption.Labels, sc.Collection, true);
			}

			this.UpdateModificationsCulture();
		}

		private void HandleStringFocusCollectionChanged(object sender)
		{
			//	Le focus a changé dans une collection.
			if (this.ignoreChange)
			{
				return;
			}

			MyWidgets.StringCollection sc = sender as MyWidgets.StringCollection;
			this.currentTextField = sc.FocusedTextField;
		}

		private void HandlePrimaryIconClicked(object sender, MessageEventArgs e)
		{
			//	Le boutons pour choisir l'icône a été cliqué.
			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			if (item == null)
			{
				return;
			}

			StructuredData data = item.GetCultureData("00");
			if (data == null)
			{
				return;
			}

			string initialIcon = data.GetValue(Support.Res.Fields.ResourceCaption.Icon) as string;

			string icon = this.module.DesignerApplication.DlgIcon(this.module.ResourceManager, initialIcon);

			if (icon != initialIcon)
			{
				this.SetValue(item, data, Support.Res.Fields.ResourceCaption.Icon, icon, false);
				this.UpdateIcon();
			}
		}

		private void HandleResetButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;

			if (button == this.groupPrimaryLabels.ResetButton)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				this.access.Accessor.ResetToOriginalValue(item, data, Support.Res.Fields.ResourceCaption.Labels);
			}

			if (button == this.groupSecondaryLabels.ResetButton)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(1));
				this.access.Accessor.ResetToOriginalValue(item, data, Support.Res.Fields.ResourceCaption.Labels);
			}

			if (button == this.groupPrimaryDescription.ResetButton)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				this.access.Accessor.ResetToOriginalValue(item, data, Support.Res.Fields.ResourceCaption.Description);
			}

			if (button == this.groupSecondaryDescription.ResetButton)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(1));
				this.access.Accessor.ResetToOriginalValue(item, data, Support.Res.Fields.ResourceCaption.Description);
			}

			if (button == this.groupPrimaryIcon.ResetButton)
			{
				StructuredData data = item.GetCultureData(this.GetTwoLetters(0));
				this.access.Accessor.ResetToOriginalValue(item, data, Support.Res.Fields.ResourceCaption.Icon);
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


		private IconButton						primarySummaryIcon;
		private IconButton						secondarySummaryIcon;
		private MyWidgets.ResetBox				groupPrimaryLabels;
		private MyWidgets.StringCollection		primaryLabels;
		private MyWidgets.ResetBox				groupSecondaryLabels;
		private MyWidgets.StringCollection		secondaryLabels;
		private MyWidgets.ResetBox				groupPrimaryDescription;
		private TextFieldMulti					primaryDescription;
		private MyWidgets.ResetBox				groupSecondaryDescription;
		private TextFieldMulti					secondaryDescription;
		private MyWidgets.ResetBox				groupPrimaryIcon;
		private IconButton						primaryIcon;
		private StaticText						primaryIconInfo;
		private MyWidgets.ResetBox				groupPrimaryComment;
		private TextFieldMulti					primaryComment;
		private MyWidgets.ResetBox				groupSecondaryComment;
		private TextFieldMulti					secondaryComment;
	}
}
