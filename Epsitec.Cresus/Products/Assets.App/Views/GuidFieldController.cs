//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
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

					if (this.textField != null)
					{
						if (this.ignoreChanges.IsZero)
						{
							using (this.ignoreChanges.Enter ())
							{
								this.textField.Text = this.GuidToString (value);
								this.textField.SelectAll ();
							}
						}
					}
				}
			}
		}

		private void UpdateValue()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.textField.Text = this.GuidToString (this.value);
				this.textField.SelectAll ();
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

			AbstractFieldController.UpdateBackColor (this.textField, this.BackgroundColor);
			this.UpdateTextField (this.textField);
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.textField = new TextField
			{
				Parent          = this.frameBox,
				Dock            = DockStyle.Left,
				PreferredWidth  = this.EditWidth,
				PreferredHeight = AbstractFieldController.lineHeight,
				Margins         = new Margins (0, 10, 0, 0),
				TabIndex        = this.TabIndex,
				Text            = this.value.ToString (),
			};

			this.textField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.Value = this.StringToGuid (this.textField.Text);
						this.OnValueEdited ();
					}
				}
			};

			this.textField.KeyboardFocusChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					this.SetFocus ();
				}
				else  // perdu le focus ?
				{
					this.UpdateValue ();
				}
			};
		}

		public override void SetFocus()
		{
			this.textField.SelectAll ();
			this.textField.Focus ();
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

		private Guid StringToGuid(string text)
		{
			var getter = this.Accessor.GetNodesGetter (this.BaseType);

			foreach (var node in getter.Nodes)
			{
				var obj = this.Accessor.GetObject (this.BaseType, node.Guid);
				var nom = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Nom);

				if (nom == text)
				{
					return node.Guid;
				}
			}

			return Guid.Empty;
		}


		private TextField						textField;
		private Guid							value;
	}
}
