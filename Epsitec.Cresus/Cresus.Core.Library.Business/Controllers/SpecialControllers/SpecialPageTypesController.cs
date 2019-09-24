//	Copyright © 2010-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Documents.Verbose;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SpecialControllers
{
	public class SpecialPageTypesController : IEntitySpecialController
	{
		public SpecialPageTypesController(TileContainer tileContainer, DocumentPrintingUnitsEntity documentPrintingUnitsEntity)
		{
			this.tileContainer = tileContainer;
			this.documentPrintingUnitsEntity = documentPrintingUnitsEntity;

			this.pageTypes = this.documentPrintingUnitsEntity.GetPageTypes ();
			this.allPageTypes = VerbosePageType.GetAll ().ToList ();
			this.checkButtons = new List<CheckButton> ();
		}


		public void CreateUI(Widget parent, UIBuilder builder, bool isReadOnly)
		{
			this.isReadOnly = isReadOnly;

			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, 10, 0, 0),
			};

			new StaticText
			{
				Parent = box,
				Text = "Pages imprimables par cette unité :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
			};

			foreach (var pageType in this.allPageTypes)
			{
				var button = new CheckButton
				{
					Parent = box,
					Text = pageType.ShortDescription,
					Name = pageType.Type.ToString (),
					ActiveState = (this.pageTypes.Contains (pageType.Type)) ? ActiveState.Yes : ActiveState.No,
					AutoToggle = false,
					Dock = DockStyle.Top,
				};

				button.Clicked += delegate
				{
					PageType type;
					if (System.Enum.TryParse (button.Name, out type))
					{
						this.ActionToggle (type);
					}
				};

				this.checkButtons.Add (button);
			}

			new FrameBox
			{
				Parent = box,
				PreferredHeight = 5,
				Dock = DockStyle.Top,
			};

			//	Bandes pour les exemples.
			this.sample1 = this.CreateUISampleBand (box, "Document d'une seule page avec BV intégré :");
			this.sample2 = this.CreateUISampleBand (box, "Document de 3 pages avec BV intégré :");
			this.sample3 = this.CreateUISampleBand (box, "Document de 3 pages avec BV séparé :");

			this.UpdateWidgets ();
			this.UpdateSamples ();
		}

		private FrameBox CreateUISampleBand(Widget parent, string title)
		{
			new Separator
			{
				Parent = parent,
				PreferredHeight = 1,
				IsHorizontalLine = true,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 2, 2),
			};

			new StaticText
			{
				Parent = parent,
				Text = title,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 4),
			};

			return new FrameBox
			{
				Parent = parent,
				PreferredHeight = 72,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Top,
			};
		}


		private void ActionToggle(PageType pageType)
		{
			if (this.pageTypes.Contains (pageType))
			{
				this.pageTypes.Remove (pageType);
			}
			else
			{
				this.pageTypes.Add (pageType);
			}

			this.UpdateWidgets ();
			this.UpdateSamples ();
			this.Save ();
		}

		private void Save()
		{
			this.documentPrintingUnitsEntity.SetPageTypes (this.pageTypes);
		}


		private void UpdateSamples()
		{
			int t11 = 0;  // page seule avec BV
			int t21 = 0;  // première page avec BV
			int t2n = 0;  // page suivante avec BV
			int t31 = 0;  // premère page sans BV
			int t3n = 0;  // page suivante sans BV
			int t3b = 0;  // BV seul

			if (this.pageTypes.Contains (PageType.All))
			{
				t11++;
				t21++;
				t2n++;
				t31++;
				t3n++;
			}

			if (this.pageTypes.Contains (PageType.Copy))
			{
				t11++;
				t21++;
				t2n++;
				t31++;
				t3n++;
			}

			if (this.pageTypes.Contains (PageType.Single))
			{
				t11++;
			}

			if (this.pageTypes.Contains (PageType.First))
			{
				t21++;
				t31++;
			}

			if (this.pageTypes.Contains (PageType.Following))
			{
				t2n++;
				t3n++;
			}

			if (this.pageTypes.Contains (PageType.Isr))
			{
				t3b++;
			}

			this.sample1.Children.Clear ();
			this.CreateSample (this.sample1, SamplePage.PageTypeEnum.WithIsr,    "p1", t11);

			this.sample2.Children.Clear ();
			this.CreateSample (this.sample2, SamplePage.PageTypeEnum.WithIsr,    "p1", t21);
			this.CreateSample (this.sample2, SamplePage.PageTypeEnum.WithIsr,    "p2", t2n);
			this.CreateSample (this.sample2, SamplePage.PageTypeEnum.WithIsr,    "p3", t2n);

			this.sample3.Children.Clear ();
			this.CreateSample (this.sample3, SamplePage.PageTypeEnum.WithoutIsr, "p1", t31);
			this.CreateSample (this.sample3, SamplePage.PageTypeEnum.WithoutIsr, "p2", t3n);
			this.CreateSample (this.sample3, SamplePage.PageTypeEnum.WithoutIsr, "p3", t3n);
			this.CreateSample (this.sample3, SamplePage.PageTypeEnum.SingleIsr,  "BV", t3b);
		}

		private void CreateSample(Widget parent, SamplePage.PageTypeEnum type, string text, int copies)
		{
			string nx = "";

			if (copies > 0)
			{
				nx = string.Format ("{0}×", copies.ToString ());
			}

			new SamplePage
			{
				Parent = parent,
				PreferredWidth = 40,
				PageType = type,
				PageText = text,
				PageBottomLabel = nx,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 2, 0, 0),
			};
		}

		private void UpdateWidgets()
		{
			foreach (var button in this.checkButtons)
			{
				PageType type;
				if (System.Enum.TryParse (button.Name, out type))
				{
					button.ActiveState = (this.pageTypes.Contains (type)) ? ActiveState.Yes : ActiveState.No;
				}
			}
		}


		private class Factory : DefaultEntitySpecialControllerFactory<DocumentPrintingUnitsEntity>
		{
			protected override IEntitySpecialController Create(TileContainer container, DocumentPrintingUnitsEntity entity, ViewId mode)
			{
				return new SpecialPageTypesController (container, entity);
			}
		}


		private readonly TileContainer						tileContainer;
		private readonly DocumentPrintingUnitsEntity		documentPrintingUnitsEntity;
		private readonly List<PageType>						pageTypes;
		private readonly List<VerbosePageType>				allPageTypes;
		private readonly List<CheckButton>					checkButtons;

		private bool										isReadOnly;
		private FrameBox									sample1;
		private FrameBox									sample2;
		private FrameBox									sample3;
	}
}
