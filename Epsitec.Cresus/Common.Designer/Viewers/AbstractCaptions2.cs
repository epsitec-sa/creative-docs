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
	public abstract class AbstractCaptions2 : Abstract2
	{
		public AbstractCaptions2(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			this.itemViewFactory = new ItemViewFactory(this);
			
			//	Résumé des captions.
			MyWidgets.StackedPanel leftContainer, rightContainer;

			this.buttonMainExtend = this.CreateBand(out leftContainer, out rightContainer, "Résumé", BandMode.MainSummary, GlyphShape.ArrowDown, false, 0.2);
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
			this.buttonMainCompact = this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Labels.Title, BandMode.MainView, GlyphShape.ArrowUp, false, 0.2);
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
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.Description.Title, BandMode.MainView, GlyphShape.None, false, 0.2);

			this.primaryDescription = new TextFieldMulti(leftContainer.Container);
			this.primaryDescription.PreferredHeight = 10+14*4;
			this.primaryDescription.Dock = DockStyle.StackBegin;
			this.primaryDescription.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryDescription.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryDescription.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryDescription.TabIndex = this.tabIndex++;
			this.primaryDescription.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryDescription = new TextFieldMulti(rightContainer.Container);
			this.secondaryDescription.PreferredHeight = 10+14*4;
			this.secondaryDescription.Dock = DockStyle.StackBegin;
			this.secondaryDescription.TextChanged += new EventHandler(this.HandleTextChanged);
			this.secondaryDescription.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.secondaryDescription.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.secondaryDescription.TabIndex = this.tabIndex++;
			this.secondaryDescription.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Icône.
			this.CreateBand(out leftContainer, Res.Strings.Viewers.Captions.Icon.Title, BandMode.MainView, GlyphShape.None, false, 0.2);

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
			this.CreateBand(out leftContainer, out rightContainer, Res.Strings.Viewers.Captions.About.Title, BandMode.MainView, GlyphShape.None, false, 0.2);

			this.primaryComment = new TextFieldMulti(leftContainer.Container);
			this.primaryComment.PreferredHeight = 10+14*4;
			this.primaryComment.Dock = DockStyle.StackBegin;
			this.primaryComment.TextChanged += new EventHandler(this.HandleTextChanged);
			this.primaryComment.CursorChanged += new EventHandler(this.HandleCursorChanged);
			this.primaryComment.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryComment.TabIndex = this.tabIndex++;
			this.primaryComment.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.secondaryComment = new TextFieldMulti(rightContainer.Container);
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

			if (data == null || this.GetTwoLetters(1) == null)
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
			CultureMap item = this.access.CollectionView.CurrentItem as CultureMap;
			string icon = null;
			if (item != null)
			{
				StructuredData data = item.GetCultureData("00");
				icon = data.GetValue(Support.Res.Fields.ResourceCaption.Icon) as string;
			}

			if (string.IsNullOrEmpty(icon))
			{
				this.primaryIcon.Enable = false;
				this.primaryIcon.IconName = null;

				this.primaryIconInfo.Text = Res.Strings.Dialog.Icon.None;
			}
			else
			{
				this.primaryIcon.Enable = true;
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

			this.primarySummaryIcon.Enable = this.primaryIcon.Enable;
			this.primarySummaryIcon.IconName = this.primaryIcon.IconName;

			this.secondarySummaryIcon.Enable = this.primaryIcon.Enable;
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
			StructuredData data = item.GetCultureData("00");
			string initialIcon = data.GetValue(Support.Res.Fields.ResourceCaption.Icon) as string;

			string icon = this.module.MainWindow.DlgIcon(this.module.ResourceManager, initialIcon);

			if (icon != initialIcon)
			{
				this.SetValue(item, data, Support.Res.Fields.ResourceCaption.Icon, icon, false);
				this.UpdateIcon();
			}
		}


		protected override UI.IItemViewFactory ItemViewFactoryGetter(UI.ItemView itemView)
		{
			//	Retourne le "factory" a utiliser pour les éléments représentés dans cet ItemTable/ItemPanel.
			if (itemView.Item == null)
			{
				return null;
			}
			else
			{
				return this.itemViewFactory;
			}
		}


		private class ItemViewFactory : UI.AbstractItemViewFactory
		{
			//	Cette classe peuple les 3 colonnes du tableau.
			public ItemViewFactory(AbstractCaptions2 owner)
			{
				this.owner = owner;
			}

			protected override Widget CreateElement(string name, UI.ItemPanel panel, UI.ItemView view, UI.ItemViewShape shape)
			{
				CultureMap item = view.Item as CultureMap;

				switch (name)
				{
					case "Name":
						return this.CreateName(item);

					case "Primary":
						return this.CreatePrimary(item);

					case "Secondary":
						return this.CreateSecondary(item);
				}

				return null;
			}

			private Widget CreateName(CultureMap item)
			{
				StaticText widget = new StaticText();
				string text = item.ToString();

				StructuredData data = item.GetCultureData(Support.Resources.DefaultTwoLetterISOLanguageName);
				object typeCodeValue = data.GetValue(Support.Res.Fields.ResourceBaseType.TypeCode);

				//	Si c'est un type que l'on veut représenter, alors on ajoute encore la
				//	description du type de base (TypeCode) pour permettre à l'utilisateur
				//	de s'y retrouver plus facilement :
				if ((!UndefinedValue.IsUndefinedValue(typeCodeValue)) &&
					(!UnknownValue.IsUnknownValue(typeCodeValue)))
				{
					text = string.Concat(text, " (", typeCodeValue, ")");
				}

				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(text);
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				widget.PreferredSize = widget.GetBestFitSize();

				return widget;
			}

			private Widget CreatePrimary(CultureMap item)
			{
				return this.CreateContent(item, this.owner.GetTwoLetters(0));
			}

			private Widget CreateSecondary(CultureMap item)
			{
				return this.CreateContent(item, this.owner.GetTwoLetters(1));
			}
			
			private Widget CreateContent(CultureMap item, string twoLettersCulture)
			{
				//	Crée le contenu pour une colonne primaire ou secondaire.
				//	Par optimisation, un seul widget est créé s'il n'y a pas de couleur de fond.
				StaticText main, text;
				ResourceAccess.ModificationState state = this.owner.access.GetModification(item, twoLettersCulture);

				if (state == ResourceAccess.ModificationState.Normal)
				{
					main = text = new StaticText();
				}
				else
				{
					main = new StaticText();
					main.BackColor = Abstract.GetBackgroundColor(state, 0.7);

					text = new StaticText(main);
					text.Dock = DockStyle.Fill;
				}

				string value = "";
				if (twoLettersCulture != null)
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

					value = buffer.ToString();
				}

				text.Margins = new Margins(5, 5, 0, 0);
				//?text.Text = TextLayout.ConvertToTaggedText(value);
				text.Text = value;
				text.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split;
				text.PreferredSize = text.GetBestFitSize();

				return main;
			}
			

			AbstractCaptions2 owner;
		}


		private ItemViewFactory					itemViewFactory;

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
