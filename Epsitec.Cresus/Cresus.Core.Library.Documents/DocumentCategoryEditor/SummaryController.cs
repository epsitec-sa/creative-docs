﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		}


		public void CreateUI(Widget parent)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.summaryFrame = new Scrollable
			{
				Parent = box,
				Dock = DockStyle.Fill,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.Auto,
				PaintViewportFrame = true,
				Margins = new Margins (0, 0, 10, 0),
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


		private void CreateSummary()
		{
			var parent = this.summaryFrame.Viewport;
			parent.Children.Clear ();

			if (this.documentOptionsController.RequiredDocumentOptionsCount == 0)
			{
				return;
			}

			var frame = this.CreateColorizedFrameBox (parent, Color.FromBrightness (1));
			this.CreateTitle (frame, "Résumé des options");

			var options = this.GetOptions ();

			foreach (var verboseOption in this.verboseDocumentOptions)
			{
				FormattedText icon = null, description = null, value = null;

				if (options.Options.Contains (verboseOption.Option))
				{
					options.GetOptionDescription (verboseOption, false, out description, out value);
					icon = DocumentOptionsController.normalBullet;

					if (this.documentOptionsController.ErrorOptions.Contains (verboseOption.Option))
					{
						icon = DocumentOptionsController.errorBullet;

						description = description.ApplyFontColor (DocumentOptionsController.errorColor);
						value       = value.ApplyFontColor (DocumentOptionsController.errorColor);
					}

					if (!this.documentOptionsController.RequiredDocumentOptionsContains (verboseOption.Option))
					{
						icon = DocumentOptionsController.uselessBullet;

						description = description.ApplyFontColor (DocumentOptionsController.uselessColor);
						value       = value.ApplyFontColor (DocumentOptionsController.uselessColor);
					}
				}
				else
				{
					if (this.documentOptionsController.RequiredDocumentOptionsContains (verboseOption.Option))
					{
						options.GetOptionDescription (verboseOption, true, out description, out value);

						icon = DocumentOptionsController.missingBullet;

						description = description.ApplyFontColor (DocumentOptionsController.missingColor);
						value       = value.ApplyFontColor (DocumentOptionsController.missingColor);
					}
				}

				if (icon != null)
				{
					this.CreateLine (frame, icon, description, value);
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
				Margins = new Margins (0, 0, 0, 0),
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
				Margins = new Margins (0, 0, 0, 5),
			};
		}

		private void CreateLine(Widget parent, FormattedText icon, FormattedText description, FormattedText value)
		{
			var line = new FrameBox
			{
				Parent = parent,
				PreferredHeight = this.documentCategoryController.lineHeight,
				Dock = DockStyle.Top,
			};

			new StaticText
			{
				Parent = line,
				FormattedText = icon,
				PreferredWidth = DocumentOptionsController.errorBulletWidth,
				ContentAlignment = ContentAlignment.MiddleLeft,
				Dock = DockStyle.Left,
			};

			var staticDescription = new StaticText
			{
				Parent = line,
				FormattedText = description,
				ContentAlignment = ContentAlignment.MiddleLeft,
				TextBreakMode = TextBreakMode.SingleLine | TextBreakMode.Ellipsis | TextBreakMode.Split,
				Margins = new Margins (5, 0, 0, 0),
				Dock = DockStyle.Fill,
			};

			ToolTip.Default.SetToolTip (staticDescription, description);

			var staticValue = new StaticText
			{
				Parent = line,
				FormattedText = value,
				PreferredWidth = 70,
				ContentAlignment = ContentAlignment.MiddleRight,
				TextBreakMode = TextBreakMode.SingleLine | TextBreakMode.Ellipsis | TextBreakMode.Split,
				Margins = new Margins (5, 0, 0, 0),
				Dock = DockStyle.Right,
			};

			ToolTip.Default.SetToolTip (staticValue, value);
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

		private Scrollable									summaryFrame;
	}
}
