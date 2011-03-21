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
			window.ClientSize = new Size (this.isPreview ? 940 : 650, this.isPreview ? 550 : 500);
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

				this.documentCategoryEntities = this.GetDocumentCategoryEntities ();
				this.confirmationButtons = new List<ConfirmationButton> ();
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

				//	Crée les 3 colonnes.
				var leftFrame = new FrameBox
				{
					Parent = mainFrame,
					Dock = DockStyle.Left,
					PreferredWidth = 300,
					Margins = new Margins (0, 0, 0, 0),
				};

				this.centerFrame = new FrameBox
				{
					Parent = mainFrame,
					Dock = this.isPreview ? DockStyle.Left : DockStyle.Fill,
					PreferredWidth = 300,
					Margins = new Margins (10, 0, 0, 0),
				};

				var rightFrame = new FrameBox
				{
					Parent = mainFrame,
					Visibility = this.isPreview,
					Dock = DockStyle.Fill,
					Margins = new Margins (10, 0, 0, 0),
				};

				//	Crée le bouton '>' par-dessus le reste.
				this.showOptionsButton = new GlyphButton
				{
					Parent = mainFrame,
					ButtonStyle = ButtonStyle.Slider,
					PreferredSize = new Size (20, 20),
					Anchor = AnchorStyles.TopLeft,
					Margins = new Margins (300-20, 0, 0, 0),
				};
				ToolTip.Default.SetToolTip (this.showOptionsButton, "Montre ou cache les options d'impression");

				//	Rempli la première colonne (à gauche).
				var columnTitle1 = new StaticText (leftFrame);
				columnTitle1.SetColumnTitle ("Catégories de documents");

				this.categoriesFrame = new Scrollable
				{
					Parent = leftFrame,
					Dock = DockStyle.Fill,
					HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
					VerticalScrollerMode = ScrollableScrollerMode.Auto,
					PaintViewportFrame = true,
					TabIndex = tabIndex++,
				};
				this.categoriesFrame.Viewport.IsAutoFitting = true;

				//	Rempli la deuxième colonne (au centre).
				var columnTitle2 = new StaticText (this.centerFrame);
				columnTitle2.SetColumnTitle ("Options d'impression");

				this.optionsFrame = new Scrollable
				{
					Parent = this.centerFrame,
					Dock = DockStyle.Fill,
					HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
					VerticalScrollerMode = ScrollableScrollerMode.Auto,
					PaintViewportFrame = true,
					TabIndex = tabIndex++,
				};
				this.optionsFrame.Viewport.IsAutoFitting = true;
				this.optionsFrame.ViewportPadding = new Margins (1);

				//	Rempli la troisième colonne (à droite).
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

				this.UpdateCategories ();
				this.UpdateOptions ();
				this.UpdatePreview ();
				this.UpdateWidgets ();
			}


			private void UpdateCategories()
			{
				this.categoriesFrame.Viewport.Children.Clear ();
				this.confirmationButtons.Clear ();

				int index = 0;
				foreach (var category in this.documentCategoryEntities)
				{
					var button = new ConfirmationButton
					{
						Parent = this.categoriesFrame.Viewport,
						Index = index++,
						Dock = DockStyle.Top,
					};

					button.FormattedText = ConfirmationButton.FormatContent (category.Name, category.Description);

					button.Clicked += delegate
					{
						var entity = this.documentCategoryEntities.ElementAt (button.Index);
						this.UseCategoryEntity (entity);
					};

					this.confirmationButtons.Add (button);
				}
			}

			private void UpdateOptions()
			{
				var category = this.SelectedDocumentCategoryEntity;

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
				var category = this.SelectedDocumentCategoryEntity;
				
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
				var category = this.SelectedDocumentCategoryEntity;
				string code = (category == null) ? "_none_" : category.Code;

				for (int i = 0; i < this.confirmationButtons.Count; i++)
				{
					bool selected = (this.documentCategoryEntities.ElementAt (i).Code == code);
					this.confirmationButtons[i].SelectConfirmationButton (selected);
				}

				//?this.printButton.Enable = (category != null);

				this.showOptionsButton.GlyphShape = this.showOptions ? GlyphShape.TriangleLeft : GlyphShape.TriangleRight;
				this.centerFrame.Visibility = this.showOptions;
			}


			private void UseCategoryEntity(DocumentCategoryEntity category)
			{
				this.SelectedDocumentCategoryCode = category.Code;

				this.UpdateOptions ();
				this.UpdatePreview ();
				this.UpdateWidgets ();
			}

			private DocumentCategoryEntity SelectedDocumentCategoryEntity
			{
				get
				{
					string code = this.SelectedDocumentCategoryCode;

					if (code == "_none_")
					{
						return null;
					}
					else
					{
						return this.documentCategoryEntities.Where (x => x.Code == code).FirstOrDefault ();
					}
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


			private IEnumerable<DocumentCategoryEntity> GetDocumentCategoryEntities()
			{
				//	Retourne toutes les entités DocumentCategoryEntity correspondant à l'entité à imprimer.
				//	En principe, il ne devrait y avoir qu'une seule entité DocumentCategoryMappingEntity,
				//	mais s'il y en a plusieurs, on cumule toutes les DocumentCategoryEntity trouvés.
				var list = new List<DocumentCategoryEntity> ();

				var none = new DocumentCategoryEntity
				{
					Code        = "_none_",
					Name        = "Aucun",
					Description = "N'imprime pas ce document.",
				};
				list.Add (none);

				foreach (var mapping in this.GetMappingEntities ())
				{
					foreach (var documentCategory in mapping.DocumentCategories)
					{
						if (!list.Contains (documentCategory))
						{
							list.Add (documentCategory);
						}
					}
				}

				return list;
			}

			private IEnumerable<DocumentCategoryMappingEntity> GetMappingEntities()
			{
				var example = new DocumentCategoryMappingEntity ();
				
				example.PrintableEntity = this.GetPrintableEntityId (this.entityToPrint.Entity).ToString ();
				
				return this.businessContext.DataContext.GetByExample (example);
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
			private readonly PrintingOptionDictionary						categoryOptions;
			private readonly PrintingOptionDictionary						modifiedOptions;
			private readonly PrintingOptionDictionary						finalOptions;
			private readonly bool									isPreview;
			private readonly IEnumerable<DocumentCategoryEntity>	documentCategoryEntities;
			private readonly List<ConfirmationButton>				confirmationButtons;

			private bool											showOptions;
			private List<DeserializedJob>							deserializeJobs;

			private Scrollable										categoriesFrame;
			private GlyphButton										showOptionsButton;
			private FrameBox										centerFrame;
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
