//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Documents.Verbose;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Print.Controllers
{
	public class PageTypesController
	{
		public PageTypesController(DocumentPrintingUnitsEntity documentPrintingUnitsEntity)
		{
			this.documentPrintingUnitsEntity = documentPrintingUnitsEntity;

			this.pageTypes = this.documentPrintingUnitsEntity.GetPageTypes ();
			this.allPageTypes = VerbosePageType.GetAll ().ToList ();
			this.checkButtons = new List<CheckButton> ();
		}


		public void CreateUI(Widget parent)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, 10, 5, 5),
			};

			new StaticText
			{
				Parent = box,
				Text = "Pages imprimables par cette unité :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 10),  // TODO: '10' devrait être UIBuilder.RightMargin, mais on ne peut pas !
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

			var footer = new FrameBox
			{
				Parent = box,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 10, 0),
			};

			this.noneButton = new Button
			{
				Parent = footer,
				Text = "Aucune",
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 2, 0, 0),
			};

			this.allButton = new Button
			{
				Parent = footer,
				Text = "Toutes",
				Dock = DockStyle.Fill,
				Margins = new Margins (2, 0, 0, 0),
			};

			//	Conexion des événements.
			this.noneButton.Clicked += delegate
			{
				this.ActionAll (false);
			};

			this.allButton.Clicked += delegate
			{
				this.ActionAll (true);
			};

			this.UpdateWidgets ();
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
			this.Save ();
		}

		private void ActionAll(bool value)
		{
			this.pageTypes.Clear ();

			if (value)
			{
				foreach (var pageType in this.allPageTypes)
				{
					this.pageTypes.Add (pageType.Type);
				}
			}

			this.UpdateWidgets ();
			this.Save ();
		}

		private void Save()
		{
			this.documentPrintingUnitsEntity.SetPageTypes (this.pageTypes);
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

			this.noneButton.Enable = (this.pageTypes.Count != 0);
			this.allButton.Enable  = (this.pageTypes.Count != this.allPageTypes.Count);
		}


		private readonly DocumentPrintingUnitsEntity		documentPrintingUnitsEntity;
		private readonly List<PageType>						pageTypes;
		private readonly List<VerbosePageType>				allPageTypes;
		private readonly List<CheckButton>					checkButtons;

		private Button										noneButton;
		private Button										allButton;
	}
}
