//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour choisir les options d'impression ainsi que les pages à imprimer.
	/// </summary>
	public class PrintOptionsDialog : AbstractDialog
	{
		public PrintOptionsDialog(IBusinessContext businessContext, IEnumerable<EntityToPrint> entitiesToPrint, bool isPreview)
		{
			this.IsApplicationWindow = true;  // pour avoir les boutons Minimize/Maximize/Close !

			this.businessContext = businessContext;
			this.application     = this.businessContext.Data.Host;
			this.entitiesToPrint = entitiesToPrint.ToList ();
			this.isPreview       = isPreview;

			this.pages = new List<Page> ();
		}


		public List<DeserializedJob> DeserializeJobs
		{
			get
			{
				var list = new List<DeserializedJob> ();

				for (int i = 0; i < this.pages.Count; i++)
				{
					var jobs = this.pages[i].DeserializeJobs;

					if (jobs != null)
					{
						list.AddRange (jobs);
					}
				}

				return list;
			}
		}


		protected override Window CreateWindow()
		{
			Window window = new Window ();

			this.SetupWindow (window);
			this.SetupWidgets (window);

			window.AdjustWindowSize ();

			return window;
		}

		protected void SetupWindow(Window window)
		{
			this.OwnerWindow = this.application.Window;
			window.Icon = this.application.Window.Icon;
			window.Text = "Choix des options d'impression";
			window.ClientSize = new Size (this.isPreview ? 940 : 350, this.isPreview ? 550 : 500);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		protected void SetupWidgets(Window window)
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

			//	Rempli les onglets.
			var book = new TabBook
			{
				Parent = mainFrame,
				Dock = DockStyle.Fill,
			};

			for (int i = 0; i < this.entitiesToPrint.Count; i++)
			{
				var tabPage = new TabPage ();
				tabPage.TabTitle = this.entitiesToPrint[i].Title;

				book.Items.Add (tabPage);

				if (i == 0)
				{
					book.ActivePage = tabPage;
				}

				var page = new Page (this.businessContext, this.entitiesToPrint[i], this.isPreview);
				page.CreateUI (tabPage);

				this.pages.Add (page);
			}

			//	Rempli le pied de page.
			{
				this.cancelButton = new Button
				{
					Parent = footer,
					Text = "Annuler",
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
					Dock = DockStyle.Right,
					Margins = new Margins (10, 0, 0, 0),
					TabIndex = tabIndex++,
				};

				this.printButton = new Button
				{
					Parent = footer,
					Text = "Imprimer",
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
					Dock = DockStyle.Right,
					Margins = new Margins (20, 0, 0, 0),
					TabIndex = tabIndex++,
				};

			}
			
			//	Connexion des événements.
			this.printButton.Clicked += delegate
			{
				this.CloseAction (cancel: false);
			};

			this.cancelButton.Clicked += delegate
			{
				this.CloseAction (cancel: true);
			};

		}


		private void CloseAction(bool cancel)
		{
			if (cancel)
			{
				this.Result = DialogResult.Cancel;
			}
			else
			{
				for (int i = 0; i < this.entitiesToPrint.Count; i++)
				{
					this.entitiesToPrint[i].Options.MergeWith (this.pages[i].FinalOptions);
				}

				this.Result = DialogResult.Accept;
			}

			this.CloseDialog ();
		}

		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();
		}


		private class Page
		{
			public Page(IBusinessContext businessContext, EntityToPrint entityToPrint, bool isPreview)
			{
				this.businessContext = businessContext;
				this.entityToPrint   = entityToPrint;
				this.isPreview       = isPreview;

				this.categoryOptions = new PrintingOptionDictionary ();
				this.modifiedOptions = new PrintingOptionDictionary ();
				this.finalOptions    = new PrintingOptionDictionary ();
			}

			public List<DeserializedJob> DeserializeJobs
			{
				get
				{
					if (this.deserializeJobs == null)
					{
						this.UpdateDeserializeJobs ();
					}

					return this.deserializeJobs;
				}
			}

			public PrintingOptionDictionary FinalOptions
			{
				get
				{
					return this.finalOptions;
				}
			}

			public void CreateUI(Widget parent)
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

				var rightFrame = new FrameBox
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
					Parent = rightFrame,
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

					var controller = new DocumentOptionsEditor.OptionsController (this.modifiedOptions);
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

					this.UpdateDeserializeJobs ();

					if (this.deserializeJobs != null)
					{
						var controller = new XmlPreviewerController (this.businessContext, this.deserializeJobs, showCheckButtons: true);
						controller.CreateUI (this.previewFrame, this.toolbarFrame);
					}
				}
			}

			private void UpdateDeserializeJobs()
			{
				var category = this.DocumentCategoryEntityToUse;
				
				if (category == null)
				{
					this.deserializeJobs = null;
				}
				else
				{
					this.finalOptions.Clear ();
					this.finalOptions.MergeWith (this.categoryOptions);
					this.finalOptions.MergeWith (this.modifiedOptions);

					var xml = PrintEngine.MakePrintingData (this.businessContext, this.entityToPrint.Entity, this.finalOptions, this.entityToPrint.PrintingUnits);
					this.deserializeJobs = SerializationEngine.DeserializeJobs (this.businessContext, xml);
				}
			}

			private void UpdateWidgets()
			{
				this.showOptionsButton.GlyphShape = this.showOptions ? GlyphShape.TriangleLeft : GlyphShape.TriangleRight;
				this.leftFrame.Visibility = this.showOptions || !this.isPreview;
			}


			private void UseCategoryEntity(DocumentCategoryEntity category)
			{
				this.SelectedDocumentCategoryCode = category.Code;

				this.UpdateOptions ();
				this.UpdatePreview ();
				this.UpdateWidgets ();
			}

			private DocumentCategoryEntity DocumentCategoryEntityToUse
			{
				//	Retourne le DocumentCategoryEntity à utiliser pour imprimer l'entité dans this.entityToPrint.Entity.
				get
				{
					if (this.entityToPrint.Entity is DocumentMetadataEntity)
					{
						var documentMetadata = this.entityToPrint.Entity as DocumentMetadataEntity;

						if (documentMetadata.DocumentCategory != null)
						{
							return documentMetadata.DocumentCategory;
						}
					}

					var mapping = this.MappingEntity;

					if (mapping != null)
					{
						return mapping.DocumentCategories.FirstOrDefault ();
					}

					return null;
				}
			}


			private string SelectedDocumentCategoryCode
			{
				get
				{
					var type = this.GetPrintableEntityId (this.entityToPrint.Entity);

					if (PrintOptionsDialog.selectedDocumentCategoryCode.ContainsKey (type))
					{
						return PrintOptionsDialog.selectedDocumentCategoryCode[type];
					}
					else
					{
						return null;
					}
				}
				set
				{
					var type = this.GetPrintableEntityId (this.entityToPrint.Entity);
					PrintOptionsDialog.selectedDocumentCategoryCode[type] = value;
				}
			}


			private DocumentCategoryMappingEntity MappingEntity
			{
				get
				{
					var example = new DocumentCategoryMappingEntity ();

					example.PrintableEntity = this.GetPrintableEntityId (this.entityToPrint.Entity).ToString ();

					return this.businessContext.DataContext.GetByExample (example).FirstOrDefault ();
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
			private List<DeserializedJob>							deserializeJobs;

			private GlyphButton										showOptionsButton;
			private FrameBox										leftFrame;
			private Scrollable										optionsFrame;
			private FrameBox										previewFrame;
			private FrameBox										toolbarFrame;
		}


		private static Dictionary<Druid, string> selectedDocumentCategoryCode = new Dictionary<Druid, string> ();

		private readonly Application							application;
		private readonly IBusinessContext						businessContext;
		private readonly List<EntityToPrint>					entitiesToPrint;
		private readonly bool									isPreview;
		private readonly List<Page>								pages;

		private Button											printButton;
		private Button											cancelButton;
	}
}
