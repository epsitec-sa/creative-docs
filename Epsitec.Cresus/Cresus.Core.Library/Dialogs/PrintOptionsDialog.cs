//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Print;
using Epsitec.Cresus.Core.Print.Serialization;
using Epsitec.Cresus.Core.Print.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Resolvers;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour choisir les options d'impression ainsi que les pages à imprimer.
	/// </summary>
	public sealed class PrintOptionsDialog : CoreDialog
	{
		public PrintOptionsDialog(BusinessContext businessContext, IEnumerable<EntityToPrint> entitiesToPrint, bool isPreview)
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

		public IList<string> GetXmlSources()
		{
			var list = new List<string> ();

			foreach (var page in this.entitiesPageControllers)
			{
				list.Add (page.GetXmlSource ());
			}

			return list;
		}


		protected override void SetupWindow(Window window)
		{
			window.Text = "Aperçu avant impression";
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
				this.entitiesPageControllers[i].SaveModifiedOptions ();
			}

			this.Result = DialogResult.Accept;
			this.CloseDialog ();
		}


		#region PageController Class

		private sealed class PageController
		{
			public PageController(BusinessContext businessContext, EntityToPrint entityToPrint, Widget container, bool isPreview)
			{
				this.businessContext = businessContext;
				this.settingsManager = businessContext.Data.Host.SettingsManager;
				this.entityToPrint   = entityToPrint;
				this.isPreview       = isPreview;

				this.categoryOptions = new PrintingOptionDictionary ();
				this.baseOptions     = new PrintingOptionDictionary ();
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

			public string GetXmlSource()
			{
				return this.xmlSource;
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
					Dock = DockStyle.Left,
					PreferredWidth = DocumentOptionsController.ValuesController.DocumentOptionsWidth,
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
					PaintViewportFrame = false,
					TabIndex = tabIndex++,
				};
				this.optionsFrame.Viewport.IsAutoFitting = true;

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
					this.ShowOptions = !this.ShowOptions;
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

					this.baseOptions.Clear ();
					this.baseOptions.MergeWith (this.modifiedOptions);

					this.RestoreModifiedOptions ();

					//	Supprime les options inutiles pour ce type de document.
					var visibleOptions = EntityPrinterFactoryResolver.FindRequiredDocumentOptions (this.entityToPrint.Entity);
					this.modifiedOptions.Keep (visibleOptions);

					var controller = new DocumentOptionsController.OptionsController (this.entityToPrint.Entity, this.modifiedOptions);
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

					this.xmlSource = PrintEngine.MakePrintingData (this.businessContext, this.entityToPrint.Entity, this.finalOptions, this.entityToPrint.PrintingUnits);
					this.jobs = SerializationEngine.DeserializeJobs (this.businessContext, this.xmlSource);
				}
			}

			private void UpdateWidgets()
			{
				bool showOptions = this.ShowOptions;

				this.showOptionsButton.GlyphShape = showOptions ? GlyphShape.TriangleLeft : GlyphShape.TriangleRight;
				this.leftFrame.Visibility = showOptions || !this.isPreview;
				this.rightFrame.Margins = new Margins (showOptions ? 0 : 24, 0, 0, 0);
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


			#region Properties using the SettingsManager
			private bool ShowOptions
			{
				get
				{
					var s = this.settingsManager.GetSettings ("PrintOptionsDialog.ShowOptions");
					return s == "true";
				}
				set
				{
					this.settingsManager.SetSettings ("PrintOptionsDialog.ShowOptions", value ? "true" : "false");
				}
			}

			private void RestoreModifiedOptions()
			{
				var temp = new PrintingOptionDictionary ();
				temp.SetSerializedData (this.ModifiedOptions);

				this.modifiedOptions.MergeWith (temp);  // ajoute les options modifiées par l'utilisateur
			}

			public void SaveModifiedOptions()
			{
				this.modifiedOptions.RemoveSame (this.baseOptions);  // ne garde que les options modifiées par l'utilisateur
				this.ModifiedOptions = this.modifiedOptions.GetSerializedData ();
			}

			private string ModifiedOptions
			{
				get
				{
					var key = this.ModifiedOptionsKey;

					if (string.IsNullOrEmpty (key))
					{
						return null;
					}
					else
					{
						return this.settingsManager.GetSettings (key);
					}
				}
				set
				{
					var key = this.ModifiedOptionsKey;

					if (!string.IsNullOrEmpty (key))
					{
						this.settingsManager.SetSettings (key, value);
					}
				}
			}

			private string ModifiedOptionsKey
			{
				get
				{
					string name = this.DocumentTypeName;

					if (string.IsNullOrEmpty (name))
					{
						return null;
					}
					else
					{
						return string.Concat ("PrintOptionsDialog.ModifiedOptions.", name);
					}
				}
			}

			private string DocumentTypeName
			{
				//	Retourne un nom unique décrivant le type du document imprimé.
				get
				{
					var category = this.DocumentCategoryEntityToUse;

					if (category != null)
					{
						return category.DocumentType.ToString ();
					}

					return null;
				}
			}
			#endregion


			private readonly BusinessContext						businessContext;
			private readonly SettingsManager						settingsManager;
			private readonly EntityToPrint							entityToPrint;
			private readonly PrintingOptionDictionary				categoryOptions;
			private readonly PrintingOptionDictionary				baseOptions;
			private readonly PrintingOptionDictionary				modifiedOptions;
			private readonly PrintingOptionDictionary				finalOptions;
			private readonly bool									isPreview;

			private IList<DeserializedJob>							jobs;
			private string											xmlSource;

			private GlyphButton										showOptionsButton;
			private FrameBox										leftFrame;
			private FrameBox										rightFrame;
			private Scrollable										optionsFrame;
			private FrameBox										previewFrame;
			private FrameBox										toolbarFrame;
		}

		#endregion


		private readonly BusinessContext						businessContext;
		private readonly List<EntityToPrint>					entitiesToPrint;
		private readonly List<PageController>					entitiesPageControllers;
		private readonly bool									isPreview;
	}
}
