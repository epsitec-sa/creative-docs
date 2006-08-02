using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Types : AbstractCaptions
	{
		public Types(Module module, PanelsContext context, ResourceAccess access) : base(module, context, access)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		protected override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Types;
			}
		}

		public override ResourceAccess.Type BundleType
		{
			get
			{
				return ResourceAccess.Type.Types;
			}
		}


		public override void Update()
		{
			//	Met à jour le contenu du Viewer.
			base.Update();
		}

		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			base.UpdateEdit();
		}


		protected override void TextFieldToIndex(AbstractTextField textField, out int field, out int subfield)
		{
			//	Cherche les index correspondant à un texte éditable.
			base.TextFieldToIndex(textField, out field, out subfield);
		}

		protected override AbstractTextField IndexToTextField(int field, int subfield)
		{
			//	Cherche le TextField permettant d'éditer des index.
			return base.IndexToTextField(field, subfield);
		}

		public static void SearchCreateFilterGroup(AbstractGroup parent, EventHandler handler)
		{
			StaticText label;
			CheckButton check;

			label = new StaticText(parent);
			label.PreferredWidth = 80;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Text = Res.Strings.Viewers.Captions.Labels;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 0, 0);

			label = new StaticText(parent);
			label.PreferredWidth = 80;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Text = Res.Strings.Viewers.Captions.ShortDescription;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 16, 0);

			label = new StaticText(parent);
			label.PreferredWidth = 80;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Text = Res.Strings.Viewers.Captions.About;
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			label.Margins = new Margins(0, 0, 32, 0);

			check = new CheckButton(parent);
			check.Name = "0";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*0, 0, 0, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.Label);

			check = new CheckButton(parent);
			check.Name = "1";  // (*)
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 0, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryLabels);

			check = new CheckButton(parent);
			check.Name = "2";  // (*)
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 0, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryLabels);

			check = new CheckButton(parent);
			check.Name = "3";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 16, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryDescription);

			check = new CheckButton(parent);
			check.Name = "4";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 16, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryDescription);

			check = new CheckButton(parent);
			check.Name = "5";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*1, 0, 32, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.PrimaryAbout);

			check = new CheckButton(parent);
			check.Name = "6";  // (*)
			check.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			check.PreferredWidth = check.PreferredHeight;
			check.Margins = new Margins(90+20*2, 0, 32, 0);
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.ActiveStateChanged += new EventHandler(handler);
			ToolTip.Default.SetToolTip(check, Res.Strings.Dialog.Search.Check.SecondaryAbout);

			// (*)	Ce numéro correspond à field dans ResourceAccess.SearcherIndexToAccess !
		}

		

	}
}
