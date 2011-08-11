//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.PlugIns;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentCategoryController
{
	public sealed class PageTypesController
	{
		public PageTypesController(DocumentCategoryController documentCategoryController)
		{
			this.documentCategoryController = documentCategoryController;
			this.businessContext            = this.documentCategoryController.BusinessContext;
			this.documentCategoryEntity     = this.documentCategoryController.DocumentCategoryEntity;

			this.pageTypeInformations = new List<PageTypeInformation> ();
		}


		public void CreateUI(Widget parent)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.checkButtonsFrame = new Scrollable
			{
				Parent = box,
				Dock = DockStyle.Fill,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.Auto,
				PaintViewportFrame = true,
				Margins = new Margins (0, 0, 10, 0),
			};
			this.checkButtonsFrame.Viewport.IsAutoFitting = true;
			this.checkButtonsFrame.ViewportPadding = new Margins (-1);

			this.CreateCheckButtons ();

			this.CreateError (box);
			this.UpdateError ();
		}

		public void UpdateAfterDocumentTypeChanged()
		{
			this.CreateCheckButtons ();
			this.UpdateError ();
		}


		private void CreateError(Widget parent)
		{
			this.errorFrame = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = Color.FromBrightness (1),
				PreferredHeight = this.documentCategoryController.errorHeight,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, -1, 0),
			};

			this.errorText = new StaticText
			{
				Parent = this.errorFrame,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleLeft,
				Dock = DockStyle.Fill,
				Margins = new Margins (5+12, 0, 0, 0),
			};
		}

		private void CreateCheckButtons()
		{
			this.UpdateData ();

			var parent = this.checkButtonsFrame.Viewport;
			parent.Children.Clear ();

			this.firstGroup = true;
			this.CreateGroup (parent, this.pageTypeInformations.Where (x =>  x.Match), "Pages imprimables adaptées",   this.documentCategoryController.acceptedColor);  // vert clair
			this.CreateGroup (parent, this.pageTypeInformations.Where (x => !x.Match), "Pages imprimables inadaptées", this.documentCategoryController.rejectedColor);  // rouge clair
		}

		private void CreateGroup(Widget parent, IEnumerable<PageTypeInformation> pageTypeInformations, FormattedText title, Color color)
		{
			if (pageTypeInformations.Any ())
			{
				var frame = this.CreateColorizedFrameBox (parent, color);
				this.CreateTitle (frame, title);

				foreach (var pageTypeInformation in pageTypeInformations)
				{
					this.CreateCheckButton (frame, pageTypeInformation);
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
				Margins = new Margins (0, 0, this.firstGroup ? 0 : -1, 0),
				Padding = new Margins (5),
			};

			this.firstGroup = false;

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

		private void CreateCheckButton(Widget parent, PageTypeInformation info)
		{
			int index = this.pageTypeInformations.IndexOf (info);

			var entity = info.Entity;
			bool check = this.documentCategoryEntity.DocumentPrintingUnits.Contains (entity);

			var button = new CheckButton
			{
				Parent = parent,
				FormattedText = entity.Name,
				Name = index.ToString (),
				ActiveState = check ? ActiveState.Yes : ActiveState.No,
				Dock = DockStyle.Top,
			};

			button.ActiveStateChanged += delegate
			{
				this.ButtonClicked (button);
			};
		}


		private void ButtonClicked(AbstractButton button)
		{
			int index = int.Parse (button.Name);
			var entity = this.pageTypeInformations[index].Entity;

			if (button.ActiveState == ActiveState.Yes)
			{
				if (!this.documentCategoryEntity.DocumentPrintingUnits.Contains (entity))
				{
					this.documentCategoryEntity.DocumentPrintingUnits.Add (entity);
				}
			}
			else
			{
				if (this.documentCategoryEntity.DocumentPrintingUnits.Contains (entity))
				{
					this.documentCategoryEntity.DocumentPrintingUnits.Remove (entity);
				}
			}

			this.UpdateError ();
		}


		private void UpdateError()
		{
			FormattedText errorMessage = null;

			int accepted = this.pageTypeInformations.Where (x =>  x.Match).Where (x => this.documentCategoryEntity.DocumentPrintingUnits.Contains (x.Entity)).Count ();
			int rejected = this.pageTypeInformations.Where (x => !x.Match).Where (x => this.documentCategoryEntity.DocumentPrintingUnits.Contains (x.Entity)).Count ();

			if (rejected != 0)
			{
				errorMessage = "Il y a une page imprimable inadaptée";
			}
			else if (accepted == 0)
			{
				errorMessage = "Aucune page imprimable n'est choisie";
			}

			if (errorMessage.IsNullOrEmpty)
			{
				this.errorFrame.Visibility = false;
			}
			else
			{
				this.errorFrame.Visibility = true;
				this.errorText.FormattedText = errorMessage;
			}
		}

		private void UpdateData()
		{
			this.requiredPageTypes = Epsitec.Cresus.Core.Documents.External.CresusCore.GetRequiredPageTypes (this.documentCategoryEntity.DocumentType);

			this.pageTypeInformations.Clear ();

			var printingUnitEntities = this.businessContext.GetAllEntities<DocumentPrintingUnitsEntity> ().OrderBy (x => x.Name);
			foreach (var printingUnitEntity in printingUnitEntities)
			{
				this.pageTypeInformations.Add (this.GetPageType (printingUnitEntity));
			}
		}

		private PageTypeInformation GetPageType(DocumentPrintingUnitsEntity printingUnitEntity)
		{
			var pageTypes = printingUnitEntity.GetPageTypes ();

			bool match = false;

			if (this.requiredPageTypes != null)
			{
				foreach (var pageType in pageTypes)
				{
					foreach (var requiredPageType in this.requiredPageTypes)
					{
						if (PageTypeHelper.IsPrinterAndPageMatching (pageType, requiredPageType))
						{
							match = true;
							goto end;
						}
					}
				}
			}

			end:
			return new PageTypeInformation (printingUnitEntity, pageTypes, match);
		}


		private class PageTypeInformation
		{
			public PageTypeInformation(DocumentPrintingUnitsEntity entity, IEnumerable<PageType> pageTypes, bool match)
			{
				this.Entity    = entity;
				this.pageTypes = pageTypes;
				this.Match     = match;
			}

			public DocumentPrintingUnitsEntity Entity
			{
				get;
				private set;
			}

			public bool Match
			{
				get;
				private set;
			}

			public IEnumerable<PageType> PageTypes
			{
				get
				{
					return this.pageTypes;
				}
			}

			private readonly IEnumerable<PageType> pageTypes;
		}


		private readonly IBusinessContext					businessContext;
		private readonly DocumentCategoryEntity				documentCategoryEntity;
		private readonly DocumentCategoryController			documentCategoryController;
		private readonly List<PageTypeInformation>			pageTypeInformations;

		private Scrollable									checkButtonsFrame;
		private bool										firstGroup;
		private IEnumerable<PageType>						requiredPageTypes;
		private FrameBox									errorFrame;
		private StaticText									errorText;
	}
}
