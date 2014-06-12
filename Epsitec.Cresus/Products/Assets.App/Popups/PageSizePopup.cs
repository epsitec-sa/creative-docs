//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir le format des pages en millimètres.
	/// </summary>
	public class PageSizePopup : StackedPopup
	{
		public PageSizePopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Format des pages";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = "Format",
				MultiLabels           = PageSizePopup.Labels,
				Width                 = 200,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = "Portrait<br/>Paysage",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Largeur",
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Hauteur",
			});

			this.SetDescriptions (list);
		}


		public Size							Value
		{
			get
			{
				double		width;
				double		height;

				{
					var controller = this.GetController (2) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					width = (double) controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (3) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					height = (double) controller.Value.GetValueOrDefault ();
				}

				return new Size (width, height);
			}
			set
			{
				{
					var controller = this.GetController (0) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = PageSizePopup.GetFormat (value);
				}

				{
					var controller = this.GetController (1) as RadioStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = PageSizePopup.GetOrientation (value);
				}

				{
					var controller = this.GetController (2) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = (decimal) value.Width;
				}

				{
					var controller = this.GetController (3) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = (decimal) value.Height;
				}
			}
		}

		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			int rank = this.GetRank (description);

			if (rank == 0)  // modification du format ?
			{
				var controller = this.GetController (0) as ComboStackedController;
				System.Diagnostics.Debug.Assert (controller != null);

				var format = PageSizePopup.RankToFormat (controller.Value.GetValueOrDefault (-1));
				this.Value = new Size (format.Width, format.Height);
			}
			else if (rank == 1)  // modification de l'orientation ?
			{
				var controller = this.GetController (1) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);

				int o = controller.Value.GetValueOrDefault (-1);

				if (o == 0)  // portait ?
				{
					this.Value = new Size (System.Math.Min (this.Value.Width, this.Value.Height),
										   System.Math.Max (this.Value.Width, this.Value.Height));
				}
				else if (o == 1)  // paysage ?
				{
					this.Value = new Size (System.Math.Max (this.Value.Width, this.Value.Height),
										   System.Math.Min (this.Value.Width, this.Value.Height));
				}
			}
			else  // modification d'une dimension (largeur ou hauteur) ?
			{
				this.Value = this.Value;
			}
		}


		#region Formats
		public static string GetDescription(Size size)
		{
			//	Retourne un description claire d'un format de page, par exemple "A4 paysage",
			//	ou null si les dimensions ne correspondent à aucun format connu.
			var rankFormat      = PageSizePopup.GetFormat      (size);
			var rankOrientation = PageSizePopup.GetOrientation (size);

			if (rankFormat != -1 && rankOrientation != -1)
			{
				var format = PageSizePopup.RankToFormat (rankFormat);
				var name = PageSizePopup.FormatToString (format);

				var orientation = rankOrientation == 0 ? "portrait" : "paysage";

				return string.Format ("{0} {1}", name, orientation);
			}
			else
			{
				var w = TypeConverters.DecimalToString ((decimal) size.Width);
				var h = TypeConverters.DecimalToString ((decimal) size.Height);

				return string.Format ("{0} × {1} mm", w, h);
			}
		}

		private static string Labels
		{
			get
			{
				return string.Join ("<br/>", PageSizePopup.Formats.Select (x => PageSizePopup.FormatToString (x)));
			}
		}

		private static int GetFormat(Size size)
		{
			int rank = 0;

			foreach (var f in PageSizePopup.Formats)
			{
				if ((f.Width == size.Width  && f.Height == size.Height) ||  // en portrait ?
					(f.Width == size.Height && f.Height == size.Width ))    // en paysage ?
				{
					return rank;
				}

				rank++;
			}

			return -1;
		}

		private static int GetOrientation(Size size)
		{
			if (size.Width < size.Height)
			{
				return 0;  // portrait
			}
			else if (size.Width > size.Height)
			{
				return 1;  // paysage
			}
			else
			{
				return -1;
			}
		}

		private static Format RankToFormat(int rank)
		{
			var a = PageSizePopup.Formats.ToArray ();

			if (rank >= 0 && rank < a.Length)
			{
				return a[rank];
			}

			return Format.Empty;
		}

		private static string FormatToString(Format format)
		{
			switch (format.Type)
			{
				case FormatType.A2:
					return "A2";

				case FormatType.A3:
					return "A3";

				case FormatType.A4:
					return "A4";

				case FormatType.A5:
					return "A5";

				case FormatType.Letter:
					return "Letter";

				case FormatType.Legal:
					return "Legal";

				default:
					return "Sur mesure";
			}
		}

		private static IEnumerable<Format> Formats
		{
			get
			{
				yield return new Format (FormatType.A2,     420.0, 594.0);
				yield return new Format (FormatType.A3,     297.0, 420.0);
				yield return new Format (FormatType.A4,     210.0, 297.0);
				yield return new Format (FormatType.A5,     148.0, 210.0);
				yield return new Format (FormatType.Letter, 279.4, 215.9);
				yield return new Format (FormatType.Legal,  355.6, 216.0);
			}
		}

		private struct Format
		{
			public Format(FormatType type, double width, double height)
			{
				this.Type   = type;
				this.Width  = width;
				this.Height = height;
			}

			public bool IsEmpty
			{
				get
				{
					return this.Type == FormatType.Unknown;
				}
			}

			public static Format Empty = new Format (FormatType.Unknown, 0.0, 0.0);

			public readonly FormatType			Type;
			public readonly double				Width;
			public readonly double				Height;
		}

		private enum FormatType
		{
			Unknown,
			A2,
			A3,
			A4,
			A5,
			Letter,
			Legal,
		}
		#endregion
	}
}