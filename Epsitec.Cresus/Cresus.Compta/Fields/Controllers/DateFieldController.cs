﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Fields.Controllers
{
	/// <summary>
	/// Contrôleur générique permettant l'édition d'un champ "date".
	/// </summary>
	public class DateFieldController : AbstractFieldController
	{
		public DateFieldController(AbstractController controller, int line, ColumnMapper columnMapper, System.Action<int, ColumnType> setFocusAction = null, System.Action contentChangedAction = null)
			: base (controller, line, columnMapper, setFocusAction, contentChangedAction)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateLabelUI (parent);

			this.box = new FrameBox
			{
				Parent          = parent,
				DrawFullFrame   = true,
				PreferredHeight = 20,
				Dock            = this.HasRightEditor ? DockStyle.Top : DockStyle.Left,
				Margins         = this.BoxMargins,
			};

			this.container = new FrameBox
			{
				Parent          = this.box,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				TabIndex        = 1,
			};

			this.editWidget = new TextField
			{
				Parent          = this.container,
				TextDisplayMode = TextFieldDisplayMode.ActiveHint,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				TabIndex        = 1,
			};

			this.editWidget.TextChanged += new EventHandler (this.HandleTextChanged);
			this.editWidget.IsFocusedChanged += new EventHandler<DependencyPropertyChangedEventArgs> (this.HandleIsFocusedChanged);
		}


		public override bool IsReadOnly
		{
			get
			{
				return this.InternalField.IsReadOnly;
			}
			protected set
			{
				this.InternalField.IsReadOnly = value;
				this.InternalField.Invalidate ();  // pour contourner un bug !
			}
		}

		public override void SetFocus()
		{
			this.InternalField.SelectAll ();
			this.InternalField.Focus ();
		}

		public override void EditionDataToWidget()
		{
			if (this.editionData != null)
			{
				using (this.ignoreChanges.Enter ())
				{
					this.InternalField.FormattedText = this.editionData.Text;
					this.IsReadOnly = !this.editionData.Enable || !this.columnMapper.Enable;
				}
			}
		}

		public override void WidgetToEditionData()
		{
			if (this.editionData != null)
			{
				this.editionData.Text = this.InternalField.FormattedText;
				this.editionData.Validate ();
			}
		}

		public override void Validate()
		{
			if (this.editionData == null)
			{
				return;
			}

			this.WidgetToEditionData ();

			this.InternalField.HintText = this.AdjustHintDate (this.InternalField.FormattedText, this.editionData.Text);

			this.editWidget.SetError (this.editionData.HasError);

			if (this.editionData.HasError)
			{
				ToolTip.Default.SetToolTip (this.editWidget, this.editionData.Error);
			}
			else
			{
				if (this.columnMapper != null)
				{
					ToolTip.Default.SetToolTip (this.editWidget, this.columnMapper.Tooltip);
				}
			}
		}


		private void HandleTextChanged(object sender)
		{
			if (this.ignoreChanges.IsNotZero || this.editionData == null)
			{
				return;
			}

			this.Validate ();
			this.ContentChangedAction ();
		}

		private void HandleIsFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (this.editionData == null)
			{
				return;
			}

			this.hasFocus = (bool) e.NewValue;

			if (this.hasFocus)  // prise du focus ?
			{
				this.SetFocusAction ();
			}
			else  // perte du focus ?
			{
				var text = this.editionData.Text;

				if (this.InternalField.FormattedText != text)
				{
					this.InternalField.FormattedText = text;
					this.InternalField.HintText = null;
					//?this.ContentChangedAction ();  // TODO: Normalement inutile ?
				}
			}
		}


		private string AdjustHintDate(FormattedText entered, FormattedText hint)
		{
			//	Ajuste le texte 'hint' en fonction du texte entré, pour une date.
			//
			//	entered = "5"     hint = "05.03.2012" out = "5.03.2012"
			//	entered = "5."    hint = "05.03.2012" out = "5.03.2012"
			//	entered = "5.3"   hint = "05.03.2012" out = "5.3.2012"
			//	entered = "5.3."  hint = "05.03.2012" out = "5.3.2012"
			//	entered = "5.3.2" hint = "05.03.2012" out = "5.3.2"
			//	entered = "5 3"   hint = "05.03.2012" out = "5 3.2012"

			if (entered.IsNullOrEmpty || hint.IsNullOrEmpty)
			{
				return hint.ToSimpleText ();
			}

			//	Décompose le texte 'entered', en mots et en séparateurs.
			var brut = entered.ToSimpleText ();

			var we = new List<string> ();
			var se = new List<string> ();

			int j = 0;
			bool n = true;
			for (int i = 0; i <= brut.Length; i++)
			{
				bool isDigit;

				if (i < brut.Length)
				{
					isDigit = brut[i] >= '0' && brut[i] <= '9';
				}
				else
				{
					isDigit = !n;
				}

				if (n && !isDigit)
				{
					we.Add (brut.Substring (j, i-j));
					j = i;
					n = false;
				}

				if (!n && isDigit)
				{
					se.Add (brut.Substring (j, i-j));
					j = i;
					n = true;
				}
			}

			//	Décompose le texte 'hint', en mots.
			string sep = Converters.SettingsEnumToChar (this.controller.SettingsList.GetEnum (SettingsType.DateSeparator));
			var wh = hint.ToSimpleText ().Split (sep[0]);

			int count = System.Math.Min (we.Count, wh.Length);
			for (int i = 0; i < count; i++)
			{
				if (!string.IsNullOrEmpty (we[i]))
				{
					wh[i] = we[i];
				}
			}

			//	Recompose la chaîne finale.
			var builder = new System.Text.StringBuilder ();

			for (int i = 0; i < wh.Length; i++)
			{
				builder.Append (wh[i]);

				if (i < wh.Length-1)
				{
					if (i < se.Count)
					{
						builder.Append (se[i]);
					}
					else
					{
						builder.Append (sep);
					}
				}
			}

			return builder.ToString ();
		}



		private TextField InternalField
		{
			get
			{
				return this.editWidget as TextField;
			}
		}
	}
}
