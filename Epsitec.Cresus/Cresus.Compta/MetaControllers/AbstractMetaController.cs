//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Settings.Data;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Search.Controllers;
using Epsitec.Cresus.Compta.Options.Controllers;
using Epsitec.Cresus.Compta.ViewSettings.Data;
using Epsitec.Cresus.Compta.ViewSettings.Controllers;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Graph;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.MetaControllers
{
	public abstract class AbstractMetaController
	{
		public AbstractMetaController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
		{
			this.app                  = app;
			this.businessContext      = businessContext;
			this.mainWindowController = mainWindowController;
			this.compta               = this.mainWindowController.Compta;
			this.période              = this.mainWindowController.Période;

			this.controllerTypes = new List<ControllerType> ();
			this.InitMetas ();
		}


		public List<ControllerType> ControllerTypes
		{
			get
			{
				return this.controllerTypes;
			}
		}

		public ControllerType CurrentControllerType
		{
			get
			{
				return this.currentControllerType;
			}
		}

		public AbstractController Controller
		{
			get
			{
				return this.controller;
			}
		}

		public void CreateController(FrameBox parent, ControllerType type)
		{
			this.parent = parent;
			this.currentControllerType = type;

			switch (this.currentControllerType)
			{
				case ControllerType.Open:
					this.controller = new OpenController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Save:
					this.controller = new SaveController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Print:
					this.controller = new PrintController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Login:
					this.controller = new LoginController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Périodes:
					this.controller = new PériodesController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Modèles:
					this.controller = new ModèlesController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Libellés:
					this.controller = new LibellésController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Journaux:
					this.controller = new JournauxController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Journal:
					this.controller = new JournalController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.PlanComptable:
					this.controller = new PlanComptableController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Balance:
					this.controller = new BalanceController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Extrait:
					this.controller = new ExtraitDeCompteController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Bilan:
					this.controller = new BilanController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.PP:
					this.controller = new PPController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Exploitation:
					this.controller = new ExploitationController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Budgets:
					this.controller = new BudgetsController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.RésuméTVA:
					this.controller = new RésuméTVAController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.RésuméPériodique:
					this.controller = new RésuméPériodiqueController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Soldes:
					this.controller = new SoldesController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.PiècesGenerator:
					this.controller = new PiècesGeneratorController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.CodesTVA:
					this.controller = new CodesTVAController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.ListeTVA:
					this.controller = new ListesTVAController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Monnaies:
					this.controller = new MonnaiesController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Utilisateurs:
					this.controller = new UtilisateursController (this.app, this.businessContext, this.mainWindowController);
					break;

				case ControllerType.Réglages:
					this.controller = new RéglagesController (this.app, this.businessContext, this.mainWindowController);
					break;
			}

			if (this.controller != null)
			{
				this.controller.CreateUI (this.parent);
				//?controller.SetVariousParameters (parentWindow, command);
			}
		}

		public void DisposeController()
		{
			if (this.controller != null)
			{
				this.controller.Dispose ();
				this.controller = null;
			}

			if (this.parent != null)
			{
				this.parent.Children.Clear ();
				this.parent = null;
			}
		}


		protected virtual MetaControllerType MetaControllerType
		{
			get
			{
				return MetaControllerType.Unknown;
			}
		}

		private void InitMetas()
		{
#if false
			var types = this.mainWindowController.Metas[this.MetaControllerType];

			foreach (var type in types)
			{
				this.controllerTypes.Add (type);
			}

			this.currentControllerType = this.controllerTypes.First ();
#endif
		}



		protected readonly ComptaApplication					app;
		protected readonly BusinessContext						businessContext;
		protected readonly MainWindowController					mainWindowController;
		protected readonly ComptaEntity							compta;
		protected readonly ComptaPériodeEntity					période;
		protected readonly List<ControllerType>					controllerTypes;

		protected ControllerType								currentControllerType;
		protected FrameBox										parent;
		protected AbstractController							controller;
	}
}
