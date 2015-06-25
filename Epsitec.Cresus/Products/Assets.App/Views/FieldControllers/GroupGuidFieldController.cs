//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	/// <summary>
	/// Contrôleur permettant de choisir un goupe (champ ObjectField.GroupParent) pour
	/// les groupes d'immobilisations.
	/// </summary>
	public class GroupGuidFieldController : AbstractFieldController
	{
		public GroupGuidFieldController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public DataAccessor						Accessor;
		public BaseType							BaseType;

		public Guid								Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value)
				{
					this.value = value;

					if (this.button != null)
					{
						this.button.Text = this.GuidToString (this.value);
					}
				}
			}
		}

		protected override void ClearValue()
		{
			this.Value = Guid.Empty;
			this.OnValueEdited (this.Field);
		}

		protected override void UpdatePropertyState()
		{
			base.UpdatePropertyState ();

			if (this.button != null)
			{
				var type = AbstractFieldController.GetFieldColorType (this.propertyState, isLocked: this.isReadOnly, isError: this.hasError);
				AbstractFieldController.UpdateButton (this.button, type, this.isReadOnly);
			}
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.button = new ColoredButton
			{
				Parent           = this.frameBox,
				HoverColor       = ColorManager.HoverColor,
				ContentAlignment = ContentAlignment.MiddleLeft,
				Dock             = DockStyle.Left,
				PreferredWidth   = this.EditWidth,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Margins          = new Margins (0, 10, 0, 0),
				TabIndex         = ++this.TabIndex,
				Text             = this.GuidToString (this.value),
			};

			//	Petit triangle "v" par-dessus la droite du bouton principal, sans fond
			//	afin de prendre la couleur du bouton principal.
			var arrowButton = new GlyphButton
			{
				Parent           = this.button,
				GlyphShape       = GlyphShape.TriangleDown,
				ButtonStyle      = ButtonStyle.ToolItem,
				Anchor           = AnchorStyles.Right,
				PreferredWidth   = AbstractFieldController.lineHeight,
				PreferredHeight  = AbstractFieldController.lineHeight,
			};

			this.UpdatePropertyState ();

			this.button.Clicked += delegate
			{
				this.ShowPopup ();
			};

			this.button.KeyboardFocusChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					this.SetFocus ();
				}
				else  // perdu le focus ?
				{
				}
			};

			arrowButton.Clicked += delegate
			{
				this.ShowPopup ();
			};
		}

		public override void SetFocus()
		{
			this.button.Focus ();

			base.SetFocus ();
		}


		private void ShowPopup()
		{
			GroupsPopup.Show (this.button, this.accessor, this.BaseType, this.Value, delegate (Guid selectedGuid)
			{
				this.Value = selectedGuid;
				this.OnValueEdited (this.Field);
			});
		}

		private string GuidToString(Guid guid)
		{
			if (this.BaseType == BaseType.Groups)
			{
				return GroupsLogic.GetFullName (this.Accessor, guid);
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Unsupported BaseType {0}", this.BaseType.ToString ()));
			}
		}


		private ColoredButton					button;
		private Guid							value;
	}
}
