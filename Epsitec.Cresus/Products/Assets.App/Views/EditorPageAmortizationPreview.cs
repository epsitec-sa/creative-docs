//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageAmortizationPreview : AbstractEditorPage
	{
		public EditorPageAmortizationPreview(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		public override void CreateUI(Widget parent)
		{
			new StaticText
			{
				Parent  = parent,
				Text    = "Comment amortir:",
				Dock    = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 20),
			};

			this.line1 = this.CreateFrame (parent);

			new StaticText
			{
				Parent  = parent,
				Text    = "Calcul effectué:",
				Dock    = DockStyle.Top,
				Margins = new Margins (0, 0, 30, 20),
			};

			this.line2 = this.CreateFrame (parent);

			new StaticText
			{
				Parent  = parent,
				Text    = "Prorata éventuel:",
				Dock    = DockStyle.Top,
				Margins = new Margins (0, 0, 30, 20),
			};

			this.line3 = this.CreateFrame (parent);
		}


		public override void SetObject(Guid objectGuid, Timestamp timestamp)
		{
			this.line1.Children.Clear ();
			this.line2.Children.Clear ();
			this.line3.Children.Clear ();

			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);
			if (obj == null)
			{
				return;
			}

			var a = new Amortizations (this.accessor);
			var ad = a.GetAmortizationDetails (obj, timestamp.Date);

			if(!ad.IsEmpty)
			{
				if (!ad.Def.IsEmpty)
				{
					this.CreateText (this.line1, 600, ad.Def.GetFullName ());
				}

				//-var v = ObjectCalculator.GetObjectPropertyComputedAmount (obj, timestamp, ObjectField.MainValue, synthetic: false);
				//-decimal? d = v.HasValue ? v.Value.FinalAmount : null;
				//-this.CreateAmount (this.line2, d);

				if (ad.ForcedValue.HasValue)
				{
					this.CreateAmount (this.line2, ad.FinalValue, "Valeur finale");
					this.CreateLabel  (this.line2, 20, "=");
					this.CreateAmount (this.line2, ad.ForcedValue, "Valeur résiduelle");
				}
				else
				{
					var rnd = ad.FinalValue.GetValueOrDefault () - (ad.InitialValue.GetValueOrDefault () - (ad.BaseValue.GetValueOrDefault () * ad.Def.EffectiveRate * ad.Prorata.Quotient.GetValueOrDefault (1.0m)));

					this.CreateAmount (this.line2, ad.FinalValue, "Valeur finale");
					this.CreateLabel  (this.line2, 20, "=");
					this.CreateAmount (this.line2, ad.InitialValue, "Valeur précédente");
					this.CreateLabel  (this.line2, 30, "− (");
					this.CreateAmount (this.line2, ad.BaseValue, "Valeur de base");
					this.CreateLabel  (this.line2, 20, "×");
					this.CreateRate   (this.line2, ad.Def.EffectiveRate, "Taux adapté à la périodicité");
					this.CreateLabel  (this.line2, 20, "×");
					this.CreateRate   (this.line2, ad.Prorata.Quotient, "Facteur correctif si \"au prorata\"");
					this.CreateLabel  (this.line2, 30, rnd >= 0 ? ") +" : ") −");
					this.CreateAmount (this.line2, System.Math.Abs (rnd), "Arrondi");
				}

				if (!ad.Prorata.IsEmpty)
				{
					this.CreateLabel   (this.line3, 20, "a");
					this.CreateDecimal (this.line3, ad.Prorata.Numerator, "Nombre effectif");
					this.CreateLabel   (this.line3, 20, "b");
					this.CreateDecimal (this.line3, ad.Prorata.Denominator, "Nombre total");
					this.CreateLabel   (this.line3, 50, "1-(a/b)");
					this.CreateRate    (this.line3, ad.Prorata.Quotient, "Facteur correctif");
				}
			}
		}


		private FrameBox CreateFrame(Widget parent)
		{
			return new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = 20,
				Margins         = new Margins (0, 0, 0, 10),
			};
		}

		private void CreateAmount(Widget parent, decimal? value, string tooltip = null)
		{
			var field = new TextField
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = 90,
				IsReadOnly       = true,
				Text             = TypeConverters.AmountToString (value),
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (field, tooltip);
			}
		}

		private void CreateRate(Widget parent, decimal? value, string tooltip = null)
		{
			var field = new TextField
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = 50,
				IsReadOnly       = true,
				Text             = TypeConverters.RateToString (value),
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (field, tooltip);
			}
		}

		private void CreateDecimal(Widget parent, decimal? value, string tooltip = null)
		{
			var field = new TextField
			{
				Parent          = parent,
				Dock            = DockStyle.Left,
				PreferredWidth  = 60,
				IsReadOnly      = true,
				Text            = TypeConverters.DecimalToString (value),
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (field, tooltip);
			}
		}

		private void CreateText(Widget parent, int width, string text)
		{
			new TextField
			{
				Parent          = parent,
				Dock            = DockStyle.Left,
				PreferredWidth  = width,
				IsReadOnly      = true,
				Text            = text,
			};
		}

		private void CreateLabel(Widget parent, int width, string text)
		{
			new StaticText
			{
				Parent          = parent,
				Dock            = DockStyle.Left,
				PreferredWidth  = width,
				Text            = text,
				ContentAlignment = ContentAlignment.MiddleCenter,
			};
		}


		private FrameBox line1;
		private FrameBox line2;
		private FrameBox line3;
	}
}
