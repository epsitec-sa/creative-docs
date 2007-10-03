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
		public AbstractCaptions(Module module, PanelsContext context, ResourceAccess access, DesignerApplication designerApplication) : base(module, context, access, designerApplication)
		{
			//	Résumé des captions.
			MyWidgets.StackedPanel leftContainer, rightContainer;

			this.buttonMainExtend = this.CreateBand(out leftContainer, out rightContainer, "Résumé", BandMode.MainSummary, GlyphShape.ArrowDown, false, 0.3);
			this.buttonMainExtend.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

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
			this.buttonMainCompact = this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Labels.Title, BandMode.MainView, GlyphShape.ArrowUp, false, 0.3);
			this.buttonMainCompact.Clicked += new MessageEventHandler(this.HandleButtonCompactOrExtendClicked);

			this.primaryLabels = new MyWidgets.StringCollection(leftContainer.Container);
			this.primaryLabels.Dock = DockStyle.StackBegin;
			this.primaryLabels.StringTextChanged += new EventHandler(this.HandleStringTextCollectionChanged);
			this.primaryLabels.StringFocusChanged += new EventHandler(this.HandleStringFocusCollectionChanged);
			this.primaryLabels.TabIndex = this.tabIndex++;
			this.primaryLabels.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.secondaryLabels = new MyWidgets.StringCollection(rightContainer.Container);
			this.secondaryLabels.Dock = DockStyle.StackBegin;
			this.secondaryLabels.StringTextChanged += new EventHandler(this.HandleStringTextCollectionChanged);
			this.secondaryLabels.StringFocusChanged += new EventHandler(this.HandleStringFocusCollectionChanged);
			this.secondaryLabels.TabIndex = this.tabIndex++;
			this.secondaryLabels.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			//	Description.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Description.Title, BandMode.MainView, GlyphShape.None, false, 0.3);

			this.primaryDescription = new TextFieldMulti(leftContainer.Container);
			this.primaryDescription.AcceptsNullValue = true;
			this.primaryDescription.PreferredHeight = 10+14*4;
			this.primaryDescription.Dock = DockStyle.StackBegin;
			this.primaryDescription.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryDescription.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryDescription.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryDescription.TabIndex = this.tabIndex++;
			this.primaryDescription.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryDescription = new TextFieldMulti(rightContainer.Container);
			this.secondaryDescription.AcceptsNullValue = true;
			this.secondaryDescription.PreferredHeight = 10+14*4;
			this.secondaryDescription.Dock = DockStyle.StackBegin;
			this.secondaryDescription.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryDescription.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryDescription.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryDescription.TabIndex = this.tabIndex++;
			this.secondaryDescription.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Icône.
			this.CreateBand(out leftContainer, Res.Strings.Viewers.Captions.Icon.Title, BandMode.MainView, GlyphShape.None, false, 0.3);

			StaticText label = new StaticText(leftContainer.Container);
			label.Text = Res.Strings.Viewers.Captions.Icon.Title;
			label.MinHeight = 30;  // attention, très important !
			label.PreferredHeight = 30;
			label.PreferredWidth = 30;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Margins = new Margins(0, 5, 0, 0);
			label.Dock = DockStyle.Left;

			this.primaryIcon = new IconButton(leftContainer.Container);
			this.primaryIcon.MinHeight = 30;  // attention, très important !
			this.primaryIcon.PreferredHeight = 30;
			this.primaryIcon.PreferredWidth = 30;
			this.primaryIcon.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryIcon.Dock = DockStyle.Left;
			this.primaryIcon.TabIndex = this.tabIndex++;
			this.primaryIcon.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.primaryIcon.Clicked += new MessageEventHandler(this.HandlePrimaryIconClicked);

			this.primaryIconInfo = new StaticText(leftContainer.Container);
			this.primaryIconInfo.PreferredHeight = 30;
			this.primaryIconInfo.PreferredWidth = 300;
			this.primaryIconInfo.Margins = new Margins(10, 0, 0, 0);
			this.primaryIconInfo.Dock = DockStyle.Left;

			//	Commentaires.
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.About.Title, BandMode.MainView, GlyphShape.None, false, 0.3);

			this.primaryComment = new TextFieldMulti(leftContainer.Container);
			this.primaryComment.AcceptsNullValue = true;
			this.primaryComment.PreferredHeight = 10+14*4;
			this.primaryComment.Dock = DockStyle.StackBegin;
			this.primaryComment.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryComment.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryComment.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryComment.TabIndex = this.tabIndex++;
			this.primaryComment.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryComment = new TextFieldMulti(rightContainer.Container);
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
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.primaryLabels.StringTextChanged -= new EventHandler(this.HandleStringTextCollectionChanged);
				this.primaryLabels.StringFocusChanged -= new EventHandler(this.HandleStringFocusCollectionChanged);

				this.secondaryLabels.StringTextChanged -= new EventHandler(this.HandleStringTextCollectionChanged);
				this.secondaryLabels.StringFocusChanged -= new EventHandler(this.HandleStringFocusCollectionChanged);

				this.primaryDescription.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.primaryDescription.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.primaryDescription.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryDescription.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.secondaryDescription.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.secondaryDescription.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.primaryIcon.Clicked -= new MessageEventHandler(this.HandlePrimaryIconClicked);

				this.primaryComment.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.primaryComment.CursorChanged -= new EventHandler(this.HandleCursorChanged);
				this.primaryComment.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);

				this.secondaryComment.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.secondaryComment.CursorChanged -= new EventHandler(this.HandleCursorChanged);
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

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			StructuredData data = null;

			if (item != null)
			{
				data = item.GetCultureData(this.GetTwoLetters(0));
				this.SetTextField(this.primaryLabels, data.GetValue(Support.Res.Fields.ResourceCaption.Labels) as IList<string>);
				this.primaryDescription.Text = data.GetValue(Support.Res.Fields.ResourceCaption.Description) as string;
				this.primaryComment.Text = data.GetValue(Support.Res.Fields.ResourceBase.Comment) as string;
			}

			this.primaryLabels.Enable = !this.designerApplication.IsReadonly;
			this.primaryDescription.Enable = !this.designerApplication.IsReadonly;
			this.primaryComment.Enable = !this.designerApplication.IsReadonly;

			if (data == null || this.GetTwoLetters(1) == null || this.designerApplication.IsReadonly)
			{
				this.secondaryLabels.Collection = null;
				this.secondaryDescription.Text = "";
				this.secondaryComment.Text = "";
				this.secondaryLabels.Enable = false;
				this.secondaryDescription.Enable = false;
				this.secondaryComment.Enable = false;
			}
			else
			{
				data = item.GetCultureData(this.GetTwoLetters(1));
				this.SetTextField(this.secondaryLabels, data.GetValue(Support.Res.Fields.ResourceCaption.Labels) as IList<string>);
				this.secondaryDescription.Text = data.GetValue(Support.Res.Fields.ResourceCaption.Description) as string;
				this.secondaryComment.Text = data.GetValue(Support.Res.Fields.ResourceBase.Comment) as string;
				this.secondaryDescription.Enable = true;
				this.secondaryComment.Enable = true;
			}

			this.UpdateIcon();

			this.ignoreChange = iic;
		}

		protected void UpdateIcon()
		{
			this.primaryIcon.Enable = !this.designerApplication.IsReadonly;

			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			string icon = null;
			if (item != null)
			{
				StructuredData data = item.GetCultureData("00");
				icon = data.GetValue(Support.Res.Fields.ResourceCaption.Icon) as string;
			}

			if (string.IsNullOrEmpty(icon))
			{
				this.primaryIcon.IconName = null;

				this.primaryIconInfo.Text = Res.Strings.Dialog.Icon.None;
			}
			else
			{
				this.primaryIcon.IconName = icon;

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

			this.primarySummaryIcon.IconName = this.primaryIcon.IconName;
			this.secondarySummaryIcon.IconName = this.primaryIcon.IconName;
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
					buffer.Append(Misc.Italic("(vide)"));
				}
				else
				{
					StructuredData data = item.GetCultureData(twoLettersCulture);

					IList<string> list = data.GetValue(Support.Res.Fields.ResourceCaption.Labels) as IList<string>;
					string desc = data.GetValue(Support.Res.Fields.ResourceCaption.Description) as string;

					if ((list == null || list.Count == 0) && string.IsNullOrEmpty(desc))
					{
						buffer.Append(Misc.Italic("(indéfini)"));
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
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryLabels);

			check = new CheckButton(parent);
			check.Name = "2";  // (*)
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 0, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryLabels);

			check = new CheckButton(parent);
			check.Name = "3";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 16, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryDescription);

			check = new CheckButton(parent);
			check.Name = "4";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 16, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryDescription);

			check = new CheckButton(parent);
			check.Name = "5";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 32, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryAbout);

			check = new CheckButton(parent);
			check.Name = "6";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 32, 0);
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
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


		protected IconButton					primarySummaryIcon;
		protected IconButton					secondarySummaryIcon;
		protected MyWidgets.StringCollection	primaryLabels;
		protected MyWidgets.StringCollection	secondaryLabels;
		protected TextFieldMulti				primaryDescription;
		protected TextFieldMulti				secondaryDescription;
		protected IconButton					primaryIcon;
		protected StaticText					primaryIconInfo;
		protected TextFieldMulti				primaryComment;
		protected TextFieldMulti				secondaryComment;
	}
}
