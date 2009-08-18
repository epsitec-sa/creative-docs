//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Document;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	using Document = Epsitec.Common.Document.Document;

	public abstract class AbstractFileSave : AbstractFile
	{
		public AbstractFileSave(DocumentEditor editor)
			: base (editor)
		{
		}

		public Document.FontIncludeMode FontIncludeMode
		{
			//	Mode d'inclusion des polices.
			get
			{
				return this.fontIncludeMode;
			}
			set
			{
				if (this.fontIncludeMode != value)
				{
					this.fontIncludeMode = value;
					this.UpdateFontIncludeMode ();
				}
			}
		}

		public Document.ImageIncludeMode ImageIncludeMode
		{
			//	Mode d'inclusion des images.
			get
			{
				return this.imageIncludeMode;
			}
			set
			{
				if (this.imageIncludeMode != value)
				{
					this.imageIncludeMode = value;
					this.UpdateImageIncludeMode ();
				}
			}
		}

		protected override void CreateOptionsUserInterface()
		{
			//	Crée le panneau facultatif pour les options d'enregistrement.
			this.optionsContainer = new Widget (this.window.Root);
			this.optionsContainer.Margins = new Margins (0, 0, 8, 0);
			this.optionsContainer.Dock = DockStyle.Bottom;
			this.optionsContainer.TabNavigationMode = TabNavigationMode.None;
			this.optionsContainer.Visibility = false;
			this.optionsContainer.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.optionsContainer.Name = "OptionsContainer";

			//	Options pour les polices.
			GroupBox groupFont = new GroupBox (this.optionsContainer);
			groupFont.Text = Res.Strings.Dialog.Save.Include.Font.Title;
			groupFont.PreferredWidth = 180;
			groupFont.Padding = new Margins (4, 0, 0, 3);
			groupFont.Dock = DockStyle.StackEnd;
			groupFont.Margins = new Margins (0, 8, 0, 0);
			groupFont.Name = "FontOptions";

			this.optionsFontNone = new RadioButton (groupFont);
			this.optionsFontNone.Text = Res.Strings.Dialog.Save.Include.Font.None;
			this.optionsFontNone.Dock = DockStyle.Top;
			this.optionsFontNone.Clicked += this.HandleOptionsFontClicked;

			this.optionsFontUsed = new RadioButton (groupFont);
			this.optionsFontUsed.Text = Res.Strings.Dialog.Save.Include.Font.Used;
			this.optionsFontUsed.Dock = DockStyle.Top;
			this.optionsFontUsed.Clicked += this.HandleOptionsFontClicked;

			this.optionsFontAll = new RadioButton (groupFont);
			this.optionsFontAll.Text = Res.Strings.Dialog.Save.Include.Font.All;
			this.optionsFontAll.Dock = DockStyle.Top;
			this.optionsFontAll.Clicked += this.HandleOptionsFontClicked;

			//	Options pour les images.
			GroupBox groupImage = new GroupBox (this.optionsContainer);
			groupImage.Text = Res.Strings.Dialog.Save.Include.Image.Title;
			groupImage.PreferredWidth = 180;
			groupImage.Padding = new Margins (4, 0, 0, 3);
			groupImage.Dock = DockStyle.StackEnd;
			groupImage.Margins = new Margins (0, 8, 0, 0);
			groupImage.Name = "ImageOptions";

			this.optionsImageNone = new RadioButton (groupImage);
			this.optionsImageNone.Text = Res.Strings.Dialog.Save.Include.Image.None;
			this.optionsImageNone.Dock = DockStyle.Top;
			this.optionsImageNone.Clicked += this.HandleOptionsImageClicked;

			this.optionsImageDefined = new RadioButton (groupImage);
			this.optionsImageDefined.Text = Res.Strings.Dialog.Save.Include.Image.Defined;
			this.optionsImageDefined.Dock = DockStyle.Top;
			this.optionsImageDefined.Clicked += this.HandleOptionsImageClicked;

			this.optionsImageAll = new RadioButton (groupImage);
			this.optionsImageAll.Text = Res.Strings.Dialog.Save.Include.Image.All;
			this.optionsImageAll.Dock = DockStyle.Top;
			this.optionsImageAll.Clicked += this.HandleOptionsImageClicked;
		}

		protected override void CreateFooterOptions(Widget footer)
		{
			this.optionsExtend = new GlyphButton (footer);
			this.optionsExtend.PreferredWidth = 16;
			this.optionsExtend.ButtonStyle = ButtonStyle.Slider;
			this.optionsExtend.AutoFocus = false;
			this.optionsExtend.TabNavigationMode = TabNavigationMode.None;
			this.optionsExtend.Dock = DockStyle.Left;
			this.optionsExtend.Margins = new Margins (0, 8, 3, 3);
			this.optionsExtend.Clicked += this.HandleOptionsExtendClicked;
			ToolTip.Default.SetToolTip (this.optionsExtend, Res.Strings.Dialog.File.Tooltip.ExtendInclude);
		}

		protected override void UpdateOptions()
		{
			base.UpdateOptions ();
			
			if (this.optionsExtend != null)
			{
				this.optionsExtend.GlyphShape = this.optionsContainer.Visibility ? GlyphShape.ArrowDown : GlyphShape.ArrowUp;
			}

			
			this.UpdateFontIncludeMode ();
			this.UpdateImageIncludeMode ();
		}

		protected void UpdateFontIncludeMode()
		{
			//	Met à jour le mode d'inclusion des polices.
			if (this.optionsFontNone != null)
			{
				this.optionsFontNone.ActiveState = (this.fontIncludeMode == Document.FontIncludeMode.None) ? ActiveState.Yes : ActiveState.No;
				this.optionsFontUsed.ActiveState = (this.fontIncludeMode == Document.FontIncludeMode.Used) ? ActiveState.Yes : ActiveState.No;
				this.optionsFontAll.ActiveState = (this.fontIncludeMode == Document.FontIncludeMode.All) ? ActiveState.Yes : ActiveState.No;
			}
		}

		protected void UpdateImageIncludeMode()
		{
			//	Met à jour le mode d'inclusion des images.
			if (this.optionsImageNone != null)
			{
				this.optionsImageNone.ActiveState = (this.imageIncludeMode == Document.ImageIncludeMode.None) ? ActiveState.Yes : ActiveState.No;
				this.optionsImageDefined.ActiveState = (this.imageIncludeMode == Document.ImageIncludeMode.Defined) ? ActiveState.Yes : ActiveState.No;
				this.optionsImageAll.ActiveState = (this.imageIncludeMode == Document.ImageIncludeMode.All) ? ActiveState.Yes : ActiveState.No;
			}
		}

		private void HandleOptionsExtendClicked(object sender, MessageEventArgs e)
		{
			this.optionsContainer.Visibility = !this.optionsContainer.Visibility;
			this.UpdateOptions();
		}

		private void HandleOptionsFontClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton radio pour le mode d'inclusion des polices a été cliqué.
			if (sender == this.optionsFontNone)
			{
				this.FontIncludeMode = Document.FontIncludeMode.None;
			}

			if (sender == this.optionsFontUsed)
			{
				this.FontIncludeMode = Document.FontIncludeMode.Used;
			}

			if (sender == this.optionsFontAll)
			{
				this.FontIncludeMode = Document.FontIncludeMode.All;
			}
		}

		private void HandleOptionsImageClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton radio pour le mode d'inclusion des images a été cliqué.
			if (sender == this.optionsImageNone)
			{
				this.ImageIncludeMode = Document.ImageIncludeMode.None;
			}

			if (sender == this.optionsImageDefined)
			{
				this.ImageIncludeMode = Document.ImageIncludeMode.Defined;
			}

			if (sender == this.optionsImageAll)
			{
				this.ImageIncludeMode = Document.ImageIncludeMode.All;
			}
		}

		protected Document.FontIncludeMode fontIncludeMode;
		protected Document.ImageIncludeMode imageIncludeMode;
		protected GlyphButton optionsExtend;
		protected Widget optionsContainer;
		protected RadioButton optionsFontNone;
		protected RadioButton optionsFontUsed;
		protected RadioButton optionsFontAll;
		protected RadioButton optionsImageNone;
		protected RadioButton optionsImageDefined;
		protected RadioButton optionsImageAll;
	}
}
