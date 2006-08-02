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
	public class Commands : AbstractCaptions
	{
		public Commands(Module module, PanelsContext context, ResourceAccess access) : base(module, context, access)
		{
			MyWidgets.StackedPanel leftContainer, rightContainer;

			//	Statefull.
			this.CreateBand(out leftContainer, "Type", 0.1);

			this.primaryStatefull = new CheckButton(leftContainer.Container);
			this.primaryStatefull.Text = "Reflète un état activé ou désactivé";
			this.primaryStatefull.Dock = DockStyle.StackBegin;
			this.primaryStatefull.Pressed += new MessageEventHandler(this.HandleStatefullPressed);
			this.primaryStatefull.TabIndex = this.tabIndex++;
			this.primaryStatefull.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Shortcuts.
			this.CreateBand(out leftContainer, out rightContainer, "Raccourcis clavier", 0.3);

			//	Group.
			this.CreateBand(out leftContainer, "Groupe", 0.5);

			this.primaryGroup = new TextField(leftContainer.Container);
			this.primaryGroup.PreferredWidth = 200;
			this.primaryGroup.Dock = DockStyle.StackBegin;
			this.primaryGroup.TextChanged += new EventHandler(this.HandleGroupTextChanged);
			this.primaryGroup.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.primaryGroup.TabIndex = this.tabIndex++;
			this.primaryGroup.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.UpdateEdit();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.primaryStatefull.Pressed -= new MessageEventHandler(this.HandleStatefullPressed);

				this.primaryGroup.TextChanged -= new EventHandler(this.HandleGroupTextChanged);
				this.primaryGroup.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Commands;
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
			if (this.primaryStatefull == null)
			{
				return;
			}

			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			int sel = this.access.AccessIndex;

			if (sel >= this.access.AccessCount)
			{
				sel = -1;
			}

			if (sel == -1)
			{
				this.primaryStatefull.Enable = false;
				this.primaryStatefull.ActiveState = ActiveState.No;

				this.SetTextField(this.primaryGroup, 0, null, null);
			}
			else
			{
				int index = this.access.AccessIndex;

				ResourceAccess.Field field = this.access.GetField(index, null, "Statefull");
				bool statefull = field.Bool;

				this.primaryStatefull.Enable = true;
				this.primaryStatefull.ActiveState = statefull ? ActiveState.Yes : ActiveState.No;

				this.SetTextField(this.primaryGroup, index, null, "Group");
			}

			this.ignoreChange = iic;

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


		void HandleStatefullPressed(object sender, MessageEventArgs e)
		{
			//	Bouton à cocher 'Statefull' pressé.
			if (this.ignoreChange)
			{
				return;
			}

			bool statefull = (this.primaryStatefull.ActiveState == ActiveState.No);
			int sel = this.access.AccessIndex;
			this.access.SetField(sel, null, "Statefull", new ResourceAccess.Field(statefull));

			this.UpdateColor();
		}

		void HandleGroupTextChanged(object sender)
		{
			//	Le texte éditable pour le groupe a changé.
			if (this.ignoreChange)
			{
				return;
			}

			AbstractTextField edit = sender as AbstractTextField;
			string text = edit.Text;
			int sel = this.access.AccessIndex;

			if (edit == this.primaryGroup)
			{
				this.access.SetField(sel, null, "Group", new ResourceAccess.Field(text));
			}

			this.UpdateColor();
		}
		

		protected CheckButton					primaryStatefull;
		protected TextField						primaryGroup;
	}
}
