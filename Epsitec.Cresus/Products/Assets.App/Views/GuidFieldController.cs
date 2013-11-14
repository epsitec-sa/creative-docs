//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class GuidFieldController : AbstractFieldController
	{
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
						this.button.Text = this.GuidToString (value);
					}
				}
			}
		}

		protected override void ClearValue()
		{
			this.Value = Guid.Empty;
			this.OnValueEdited ();
		}

		protected override void UpdatePropertyState()
		{
			base.UpdatePropertyState ();

			if (this.button != null)
			{
				if (this.propertyState == PropertyState.Readonly)
				{
					this.button.NormalColor = ColorManager.ReadonlyFieldColor;
				}
				else
				{
					this.button.NormalColor = this.BackgroundColor;
				}
			}
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.button = new ColoredButton
			{
				Parent          = this.frameBox,
				HoverColor      = ColorManager.HoverColor,
				Dock            = DockStyle.Left,
				PreferredWidth  = 160,
				PreferredHeight = AbstractFieldController.lineHeight,
				Margins         = new Margins (0, 10, 0, 0),
				TabIndex        = this.TabIndex,
				Text            = this.value.ToString (),
			};

			this.button.Clicked += delegate
			{
				this.ShowPopup ();
			};
		}


		private void ShowPopup()
		{
			var popup = new ObjectsPopup (this.Accessor, this.Value, true);

			popup.Create (this.button, leftOrRight: false);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				this.Value = guid;
				this.OnValueEdited ();
			};
		}

		private string GuidToString(Guid guid)
		{
			if (!guid.IsEmpty)
			{
				var obj = this.Accessor.GetObject (this.BaseType, guid);
				if (obj != null)
				{
					return ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Nom);
				}
			}

			return null;
		}


		private ColoredButton					button;
		private Guid							value;
	}
}
