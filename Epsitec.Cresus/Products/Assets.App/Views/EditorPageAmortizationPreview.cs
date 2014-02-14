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
	/// <summary>
	/// Ce paneau n'est pas éditable, contrairement à ses frères. Il montre comment
	/// a été calculé un amortissement ordinaire.
	/// </summary>
	public class EditorPageAmortizationPreview : AbstractEditorPage
	{
		public EditorPageAmortizationPreview(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.line1a = this.CreateFrame (parent, false);
			this.line1b = this.CreateFrame (parent, false);
			this.line2a = this.CreateFrame (parent, true);
			this.line2b = this.CreateFrame (parent, false);
			this.line3a = this.CreateFrame (parent, true);
			this.line3b = this.CreateFrame (parent, false);
		}


		public override void SetObject(Guid objectGuid, Timestamp timestamp)
		{
			this.line1a.Children.Clear ();
			this.line1b.Children.Clear ();
			this.line2a.Children.Clear ();
			this.line2b.Children.Clear ();
			this.line3a.Children.Clear ();
			this.line3b.Children.Clear ();

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
					this.CreateTitle (this.line1a, "Paramètres", "Paramètres de l'amortissement, définis dans l'objet");
					this.CreateText  (this.line1b, 600, ad.Def.GetFullName ());
				}

				this.CreateTitle (this.line2a, "Calcul", "Calcul effectué pour obtenir la valeur finale amortie");

				if (ad.ForcedValue.HasValue)
				{
					this.CreateAmount (this.line2b, ad.FinalValue, "Valeur finale amortie");
					this.CreateOper   (this.line2b, 20, "=");
					this.CreateAmount (this.line2b, ad.ForcedValue, "Valeur résiduelle");
				}
				else
				{
					var rnd = ad.FinalValue.GetValueOrDefault () - (ad.InitialValue.GetValueOrDefault () - (ad.BaseValue.GetValueOrDefault () * ad.Def.EffectiveRate * ad.Prorata.Quotient.GetValueOrDefault (1.0m)));

					this.CreateAmount (this.line2b, ad.FinalValue, "Valeur finale amortie");
					this.CreateOper   (this.line2b, 20, "=");
					this.CreateAmount (this.line2b, ad.InitialValue, "Valeur précédente");
					this.CreateOper   (this.line2b, 30, "− (");
					this.CreateAmount (this.line2b, ad.BaseValue, "Valeur de base");
					this.CreateOper   (this.line2b, 20, "×");
					this.CreateRate   (this.line2b, ad.Def.EffectiveRate, "Taux adapté selon la périodicité");
					this.CreateOper   (this.line2b, 20, "×");
					this.CreateRate   (this.line2b, ad.Prorata.Quotient, "Facteur correctif si \"au prorata\"");
					this.CreateOper   (this.line2b, 30, rnd >= 0 ? ") +" : ") −");
					this.CreateAmount (this.line2b, System.Math.Abs (rnd), "Arrondi");
				}

				if (!ad.Prorata.IsFullPeriod)
				{
					this.CreateTitle   (this.line3a, "Prorata", "Réduction de l'amortissement si la date d'entrée est en cours de période");

					this.CreateOper    (this.line3b, 20, "a");
					this.CreateDecimal (this.line3b, ad.Prorata.Numerator, "Nombre effectif");
					this.CreateOper    (this.line3b, 20, "b");
					this.CreateDecimal (this.line3b, ad.Prorata.Denominator, "Nombre total");
					this.CreateOper    (this.line3b, 60, "1-(a/b)");
					this.CreateRate    (this.line3b, ad.Prorata.Quotient, "Facteur correctif");
				}
			}
		}


		private FrameBox CreateFrame(Widget parent, bool topGap)
		{
			return new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = 30,
				Margins         = new Margins (0, 0, topGap ? 20 : 0, 10),
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
				Margins          = new Margins (0, 0, 5, 5),
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
				Margins          = new Margins (0, 0, 5, 5),
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
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = 60,
				IsReadOnly       = true,
				Text             = TypeConverters.DecimalToString (value),
				Margins          = new Margins (0, 0, 5, 5),
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (field, tooltip);
			}
		}

		private void CreateTitle(Widget parent, string text, string tooltip = null)
		{
			var label = new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = 600,
				Text             = text,
				ContentAlignment = ContentAlignment.BottomLeft,
				Margins          = new Margins (0, 0, 10, 0),
			};

			//?label.TextLayout.DefaultFontSize = 12.0;

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (label, tooltip);
			}
		}

		private void CreateText(Widget parent, int width, string text)
		{
			new TextField
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = width,
				IsReadOnly       = true,
				Text             = text,
				Margins          = new Margins (0, 0, 5, 5),
			};
		}

		private void CreateOper(Widget parent, int width, string text)
		{
			var label = new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = width,
				Text             = text,
				ContentAlignment = ContentAlignment.TopCenter,
				Margins          = new Margins (0, 0, 3, 0),
			};

			label.TextLayout.DefaultFontSize = 16.0;
		}


		private FrameBox line1a;
		private FrameBox line1b;
		private FrameBox line2a;
		private FrameBox line2b;
		private FrameBox line3a;
		private FrameBox line3b;
	}
}
