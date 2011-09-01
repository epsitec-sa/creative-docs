//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Documents.Verbose;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.PlugIns;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentCategoryController
{
	public sealed class SummaryController
	{
		public SummaryController(DocumentCategoryController documentCategoryController)
		{
			this.documentCategoryController = documentCategoryController;
			this.businessContext            = this.documentCategoryController.BusinessContext;
			this.documentCategoryEntity     = this.documentCategoryController.DocumentCategoryEntity;
			this.documentOptionsController  = this.documentCategoryController.DocumentOptionsController;

			this.verboseDocumentOptions = VerboseDocumentOption.GetAll ().Where (x => x.Option != DocumentOption.None);
			this.detailTexts = new List<FormattedText> ();
		}


		public void CreateUI(Widget parent)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				BackColor = TileColors.SurfaceDefaultColors.First (),
			};

			this.summaryFrame = new Scrollable
			{
				Parent = box,
				Dock = DockStyle.Fill,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.Auto,
				PaintViewportFrame = true,
			};
			this.summaryFrame.Viewport.IsAutoFitting = true;
			this.summaryFrame.ViewportPadding = new Margins (-1);

			this.CreateSummary ();
		}

		public void UpdateAfterDocumentTypeChanged()
		{
			this.CreateSummary ();
		}

		public void UpdateAfterOptionChanged()
		{
			this.CreateSummary ();
		}

		public List<FormattedText> DetailTexts
		{
			get
			{
				return this.detailTexts;
			}
		}


		private void CreateSummary()
		{
			var parent = this.summaryFrame.Viewport;
			parent.Children.Clear ();

			if (this.documentOptionsController.RequiredDocumentOptionsCount == 0)
			{
				return;
			}

			if (this.detailTexts.Count == 0)
			{
				var frame = this.CreateColorizedFrameBox (parent, Color.FromBrightness (1));

				this.CreateTitle (frame, "Résumé des options");

				var options = this.GetOptions ();

				foreach (var verboseOption in this.verboseDocumentOptions)
				{
					FormattedText icon = null, iconTooltip = null, description = null, value = null, valueTooltip = null;
					Color color = Color.FromBrightness (0);

					if (options.Options.Contains (verboseOption.Option))
					{
						options.GetOptionDescription (verboseOption, false, out description, out value);
						icon = DocumentCategoryController.normalBullet;

						if (this.documentOptionsController.ErrorOptions.Contains (verboseOption.Option))
						{
							icon = DocumentCategoryController.errorBullet;
							iconTooltip = "Option définie plusieurs fois dont <b>la valeur est aléatoire</b>";
							valueTooltip = "<b>valeur aléatoire</b>";
							color = DocumentCategoryController.errorColor;
						}

						if (!this.documentOptionsController.RequiredDocumentOptionsContains (verboseOption.Option))
						{
							icon = DocumentCategoryController.uselessBullet;
							iconTooltip = "Option définie inutilement";
							valueTooltip = "valeur inutile";
							color = DocumentCategoryController.uselessColor;
						}
					}
					else
					{
						if (this.documentOptionsController.RequiredDocumentOptionsContains (verboseOption.Option))
						{
							options.GetOptionDescription (verboseOption, true, out description, out value);

							icon = DocumentCategoryController.missingBullet;
							iconTooltip = "Option indéfinie qui prend la valeur par défaut";
							valueTooltip = "valeur par défaut";
							color = DocumentCategoryController.missingColor;
						}
					}

					if (icon != null)
					{
						this.CreateLine (frame, icon, iconTooltip, description, value, valueTooltip, color);
					}
				}
			}
			else
			{
				var frame = this.CreateColorizedFrameBox (parent, Widgets.Tiles.TileColors.SurfaceSelectedContainerColors.First ());
				frame.Padding = new Margins (0, 0, 5, 5);

				foreach (var line in this.detailTexts)
				{
					if (line.IsNullOrEmpty)
					{
						new Separator
						{
							Parent = frame,
							PreferredHeight = 1,
							IsHorizontalLine = true,
							Dock = DockStyle.Top,
							Margins = new Margins (0, 0, 5, 5),
						};
					}
					else
					{
						double h = DocumentCategoryController.lineHeight;

						int additionnalLines = line.Split ("<br/>").Count () - 1;
						h += additionnalLines*DocumentCategoryController.lineHeight;

						if (line.ToString ().Contains ("<font size="))
						{
							h = System.Math.Floor (h*1.5);
						}

						new StaticText
						{
							Parent = frame,
							FormattedText = line,
							PreferredHeight = h,
							Dock = DockStyle.Top,
							Margins = new Margins (5, 5, 0, 0),
						};
					}
				}
			}
		}

		private FrameBox CreateColorizedFrameBox(Widget parent, Color color)
		{
			var box = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = color,
				Dock = DockStyle.Top,
				Padding = new Margins (5),
			};

			return box;
		}

		private void CreateTitle(Widget parent, FormattedText title)
		{
			new StaticText
			{
				Parent = parent,
				FormattedText = FormattedText.Concat (title, " :"),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, Library.UI.Constants.TileInternalPadding, 5),
			};
		}

		private void CreateLine(Widget parent, FormattedText icon, FormattedText iconTooltip, FormattedText description, FormattedText value, FormattedText valueTooltip, Color color)
		{
			var line = new FrameBox
			{
				Parent = parent,
				PreferredHeight = DocumentCategoryController.lineHeight,
				Dock = DockStyle.Top,
			};

			var staticIcon = new StaticText
			{
				Parent = line,
				FormattedText = icon,
				PreferredWidth = DocumentCategoryController.errorBulletWidth,
				ContentAlignment = ContentAlignment.MiddleLeft,
				Dock = DockStyle.Left,
			};

			if (!iconTooltip.IsNullOrEmpty)
			{
				ToolTip.Default.SetToolTip (staticIcon, iconTooltip);
			}

			var staticDescription = new StaticText
			{
				Parent = line,
				FormattedText = description.ApplyFontColor (color),
				ContentAlignment = ContentAlignment.MiddleLeft,
				TextBreakMode = TextBreakMode.SingleLine | TextBreakMode.Ellipsis | TextBreakMode.Split,
				Margins = new Margins (5, 0, 0, 0),
				Dock = DockStyle.Fill,
			};

			ToolTip.Default.SetToolTip (staticDescription, description);

			var staticValue = new StaticText
			{
				Parent = line,
				FormattedText = value.ApplyFontColor (color),
				PreferredWidth = 70,
				ContentAlignment = ContentAlignment.MiddleRight,
				TextBreakMode = TextBreakMode.SingleLine | TextBreakMode.Ellipsis | TextBreakMode.Split,
				Margins = new Margins (5, 0, 0, 0),
				Dock = DockStyle.Right,
			};

			if (valueTooltip.IsNullOrEmpty)
			{
				ToolTip.Default.SetToolTip (staticValue, value);
			}
			else
			{
				ToolTip.Default.SetToolTip (staticValue, FormattedText.Concat (value, " (", valueTooltip, ")"));
			}
		}


		private PrintingOptionDictionary GetOptions()
		{
			//	Retourne les options à utiliser, comme le ferait le moteur d'impression, notament si
			//	des options sont définies à double.
			var result = new PrintingOptionDictionary ();

			foreach (var documentOptionEntity in this.documentCategoryEntity.DocumentOptions)
			{
				foreach (var documentOptions in this.documentCategoryEntity.DocumentOptions)
				{
					var option = documentOptions.GetOptions ();
					result.MergeWith (option);
				}
			}

			return result;
		}


		private readonly IBusinessContext					businessContext;
		private readonly DocumentCategoryEntity				documentCategoryEntity;
		private readonly DocumentCategoryController			documentCategoryController;
		private readonly DocumentOptionsController			documentOptionsController;
		private readonly IEnumerable<VerboseDocumentOption>	verboseDocumentOptions;
		private readonly List<FormattedText>				detailTexts;

		private Scrollable									summaryFrame;
	}
}
