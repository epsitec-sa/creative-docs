//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Print;
using Epsitec.Cresus.Core.Print.Serialization;
using Epsitec.Cresus.Core.Print.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Epsitec.Common.Types.Collections;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour choisir les options d'impression ainsi que les pages à imprimer.
	/// </summary>
	public sealed class PrintOptionsDialog : CoreDialog
	{
		public PrintOptionsDialog(IBusinessContext businessContext, IEnumerable<EntityToPrint> entitiesToPrint, bool isPreview)
			: base (businessContext.Data.Host)
		{
			this.businessContext = businessContext;
			this.entitiesToPrint = entitiesToPrint.ToList ();
			this.entitiesPageControllers = new List<PageController> ();
			this.isPreview = isPreview;
		}


		public IList<DeserializedJob> GetJobs()
		{
			var list = new List<DeserializedJob> ();

			foreach (var page in this.entitiesPageControllers)
			{
				list.AddRange (page.GetJobs ());
			}

			return list;
		}


		protected override void SetupWindow(Window window)
		{
			window.Text = "Choix des options d'impression";
			window.ClientSize = new Size (this.isPreview ? 940 : 350, this.isPreview ? 550 : 500);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !
		}

		protected override void SetupWidgets(Window window)
		{
			int tabIndex = 1;

			var mainFrame = new FrameBox
			{
				Parent = window.Root,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 10),
				TabIndex = tabIndex++,
			};

			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 0, 10),
				TabIndex = tabIndex++,
			};

			this.CreateTabBook (mainFrame);
			this.CreateFooterButtons (footer);
		}

		
		private void CreateTabBook(FrameBox container)
		{
			//	Remplit les onglets.
			var book = new TabBook
			{
				Parent = container,
				Dock = DockStyle.Fill,
			};

			this.entitiesToPrint.ForEach (entityToPrint => this.CreatePageControllers (book, entityToPrint));

			//	Active par défaut le premier onglet:
			book.ActivePageIndex = 0;
		}

		private void CreateFooterButtons(FrameBox container)
		{
			//	Remplit le pied de page.
			int tabIndex = 1;
			new Button (Epsitec.Common.Dialogs.Res.Commands.Dialog.Generic.Cancel)
			{
				Parent = container,
//				Text = "Annuler",
//				ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
				TabIndex = tabIndex++,
			};

			new Button (Epsitec.Common.Widgets.Res.Commands.Print)
			{
				Parent = container,
//				Text = "Imprimer",
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
				Dock = DockStyle.Right,
				Margins = new Margins (20, 0, 0, 0),
				TabIndex = tabIndex++,
			};
		}
		
		private void CreatePageControllers(TabBook book, EntityToPrint entityToPrint)
		{
			var tabPage = new TabPage ()
			{
				TabTitle = entityToPrint.Title
			};

			book.Items.Add (tabPage);

			var pageController = new PageController (this.businessContext, entityToPrint, tabPage, this.isPreview);

			this.entitiesPageControllers.Add (pageController);
		}

		
		[Command (Epsitec.Common.Dialogs.Res.CommandIds.Dialog.Generic.Cancel)]
		private void ProcessCancel()
		{
			this.Result = DialogResult.Cancel;
			this.CloseDialog ();
		}

		[Command (Epsitec.Common.Widgets.Res.CommandIds.Print)]
		private void ProcessPrint()
		{
			for (int i = 0; i < this.entitiesToPrint.Count; i++)
			{
				this.entitiesPageControllers[i].ApplyFinalOptions (this.entitiesToPrint[i]);
				
			}

			this.Result = DialogResult.Accept;
			this.CloseDialog ();
		}


		#region PageController Class

		private sealed class PageController
		{
			public PageController(IBusinessContext businessContext, EntityToPrint entityToPrint, Widget container, bool isPreview)
			{
				this.businessContext = businessContext;
				this.entityToPrint   = entityToPrint;
				this.isPreview       = isPreview;

				this.categoryOptions = new PrintingOptionDictionary ();
				this.modifiedOptions = new PrintingOptionDictionary ();
				this.finalOptions    = new PrintingOptionDictionary ();

				this.CreateUI (container);
			}

			public void ApplyFinalOptions(EntityToPrint entityToPrint)
			{
				entityToPrint.Options.MergeWith (this.finalOptions);
			}

			public IList<DeserializedJob> GetJobs()
			{
				if (this.jobs == null)
				{
					this.UpdateJobs ();
				}

				return this.jobs;
			}

			private void CreateUI(Widget parent)
			{
				int tabIndex = 1;

				var mainFrame = new FrameBox
				{
					Parent = parent,
					Dock = DockStyle.Fill,
					Margins = new Margins (10, 10, 10, 10),
					TabIndex = tabIndex++,
				};

				var footer = new FrameBox
				{
					Parent = parent,
					Visibility = this.isPreview,
					PreferredHeight = 20,
					Dock = DockStyle.Bottom,
					Margins = new Margins (10, 10, 0, 10),
					TabIndex = tabIndex++,
				};

				//	Crée les 2 colonnes.
				this.leftFrame = new FrameBox
				{
					Parent = mainFrame,
					Dock = this.isPreview ? DockStyle.Left : DockStyle.Fill,
					PreferredWidth = 300,
					Margins = new Margins (0, this.isPreview ? 10 : 0, 0, 0),
				};

				this.rightFrame = new FrameBox
				{
					Parent = mainFrame,
					Visibility = this.isPreview,
					Dock = DockStyle.Fill,
					Margins = new Margins (0, 0, 0, 0),
				};

				//	Crée le bouton '>' par-dessus le reste.
				this.showOptionsButton = new GlyphButton
				{
					Parent = mainFrame,
					ButtonStyle = ButtonStyle.Slider,
					PreferredSize = new Size (20, 20),
					Anchor = AnchorStyles.TopLeft,
					Margins = new Margins (0, 0, 0, 0),
					Visibility = this.isPreview,
				};
				ToolTip.Default.SetToolTip (this.showOptionsButton, "Montre ou cache les options d'impression");

				//	Rempli la colonne de gauche.
				var columnTitle2 = new StaticText (this.leftFrame);
				columnTitle2.SetColumnTitle ("Options d'impression");

				if (this.isPreview)
				{
					columnTitle2.Margins = new Margins (30, columnTitle2.Margins.Right, columnTitle2.Margins.Top, columnTitle2.Margins.Bottom);
				}

				this.optionsFrame = new Scrollable
				{
					Parent = this.leftFrame,
					Dock = DockStyle.Fill,
					HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
					VerticalScrollerMode = ScrollableScrollerMode.Auto,
					PaintViewportFrame = true,
					TabIndex = tabIndex++,
				};
				this.optionsFrame.Viewport.IsAutoFitting = true;
				this.optionsFrame.ViewportPadding = new Margins (1);

				//	Rempli la colonne de droite.
				this.previewFrame = new FrameBox
				{
					Parent = this.rightFrame,
					Dock = DockStyle.Fill,
					TabIndex = tabIndex++,
				};

				//	Rempli le pied de page.
				this.toolbarFrame = new FrameBox
				{
					Parent = footer,
					Dock = DockStyle.Bottom,
					Margins = new Margins (0, 0, 0, 0),
					TabIndex = tabIndex++,
				};

				//	Connexion des événements.
				this.showOptionsButton.Clicked += delegate
				{
					this.showOptions = !this.showOptions;
					this.UpdateWidgets ();
				};

				this.UpdateOptions ();
				this.UpdatePreview ();
				this.UpdateWidgets ();
			}

			private void UpdateOptions()
			{
				var category = this.DocumentCategoryEntityToUse;

				this.categoryOptions.Clear ();
				this.categoryOptions.MergeWith (this.entityToPrint.Options);

				if (category != null && category.DocumentOptions != null)
				{
					foreach (var documentOptions in category.DocumentOptions)
					{
						var option = documentOptions.GetOptions ();
						this.categoryOptions.MergeWith (option);
					}
				}

				this.optionsFrame.Viewport.Children.Clear ();

				if (category != null)
				{
					this.modifiedOptions.Clear ();
					this.modifiedOptions.MergeWith (this.categoryOptions);
					this.modifiedOptions.Remove (this.GetForcingOptions (this.entityToPrint.PrintingUnits));

					var controller = new DocumentOptionsEditor.OptionsController (this.entityToPrint.Entity, this.modifiedOptions);
					controller.CreateUI (this.optionsFrame.Viewport, this.OnOptionsChanged);
				}
			}

			private void OnOptionsChanged()
			{
				this.UpdatePreview ();
			}

			private void UpdatePreview()
			{
				if (this.isPreview)
				{
					this.previewFrame.Children.Clear ();
					this.toolbarFrame.Children.Clear ();

					this.UpdateJobs ();

					var controller = new XmlPreviewerController (this.businessContext, this.jobs, showCheckButtons: true);
					controller.CreateUI (this.previewFrame, this.toolbarFrame);
				}
			}

			private void UpdateJobs()
			{
				var category = this.DocumentCategoryEntityToUse;
				
				if (category == null)
				{
					this.jobs = EmptyList<DeserializedJob>.Instance;
				}
				else
				{
					this.finalOptions.Clear ();
					this.finalOptions.MergeWith (this.categoryOptions);
					this.finalOptions.MergeWith (this.modifiedOptions);

					var xml = PrintEngine.MakePrintingData (this.businessContext, this.entityToPrint.Entity, this.finalOptions, this.entityToPrint.PrintingUnits);
					this.jobs = SerializationEngine.DeserializeJobs (this.businessContext, xml);
				}
			}

			private void UpdateWidgets()
			{
				this.showOptionsButton.GlyphShape = this.showOptions ? GlyphShape.TriangleLeft : GlyphShape.TriangleRight;
				this.leftFrame.Visibility = this.showOptions || !this.isPreview;
				this.rightFrame.Margins = new Margins (this.showOptions ? 0 : 24, 0, 0, 0);
			}


			private void UseCategoryEntity(DocumentCategoryEntity category)
			{
				this.UpdateOptions ();
				this.UpdatePreview ();
				this.UpdateWidgets ();
			}

			private DocumentCategoryEntity DocumentCategoryEntityToUse
			{
				//	Retourne le DocumentCategoryEntity à utiliser pour imprimer l'entité dans this.entityToPrint.Entity.
				get
				{
					return PrintEngine.GetDocumentCategoryEntity (this.businessContext, this.entityToPrint.Entity);
				}
			}


			private Druid GetPrintableEntityId(AbstractEntity entity)
			{
				if (entity != null)
				{
					return EntityInfo.GetTypeId (entity.GetType ());
				}
				else
				{
					return Druid.Empty;
				}
			}


			private PrintingOptionDictionary GetForcingOptions(PrintingUnitDictionary printingUnits)
			{
				//	Retourne toutes les options forcées par un ensemble d'unités d'impressiom.
				var options = new PrintingOptionDictionary ();

				foreach (var pair in printingUnits.ContentPair)
				{
					var printingUnit = Print.Common.GetPrintingUnit (this.businessContext.Data.Host, pair.Value);

					if (printingUnit != null)
					{
						options.MergeWith (printingUnit.Options);
					}
				}

				return options;
			}


			private readonly IBusinessContext						businessContext;
			private readonly EntityToPrint							entityToPrint;
			private readonly PrintingOptionDictionary				categoryOptions;
			private readonly PrintingOptionDictionary				modifiedOptions;
			private readonly PrintingOptionDictionary				finalOptions;
			private readonly bool									isPreview;

			private bool											showOptions;
			private IList<DeserializedJob>							jobs;

			private GlyphButton										showOptionsButton;
			private FrameBox										leftFrame;
			private FrameBox										rightFrame;
			private Scrollable										optionsFrame;
			private FrameBox										previewFrame;
			private FrameBox										toolbarFrame;
		}

		#endregion


		private readonly IBusinessContext						businessContext;
		private readonly List<EntityToPrint>					entitiesToPrint;
		private readonly List<PageController>					entitiesPageControllers;
		private readonly bool									isPreview;
	}
}
