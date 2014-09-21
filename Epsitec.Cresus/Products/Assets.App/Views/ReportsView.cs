//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ReportsView : AbstractView, System.IDisposable
	{
		public ReportsView(DataAccessor accessor, CommandContext commandContext, MainToolbar toolbar, ViewType viewType, List<AbstractViewState> historyViewStates)
			: base (accessor, commandContext, toolbar, viewType)
		{
			this.historyViewStates = historyViewStates;

			this.toolbar = new ReportsToolbar (this.accessor, this.commandContext);
			this.reportChoiceController = new ReportChoiceController (this.accessor);
		}

		public override void Dispose()
		{
			this.toolbar.Dispose ();
			this.reportChoiceController.Dispose ();
			this.DeleteTreeTable ();

			base.Dispose ();
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.topTitle = new TopTitle
			{
				Parent = parent,
			};

			this.toolbar.CreateUI (parent);

			this.mainFrame = new FrameBox
			{
				Parent              = parent,
				Dock                = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			this.choiceFrame = new FrameBox
			{
				Parent              = parent,
				Dock                = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			this.reportChoiceController.CreateUI (this.choiceFrame);

			this.reportChoiceController.ReportSelected += delegate (object sender, AbstractReportParams reportParams)
			{
				this.UpdateUI (reportParams);
			};

			this.UpdateReport (null);
		}

		public override void UpdateUI()
		{
			this.UpdateWarningsRedDot ();

			this.OnViewStateChanged (this.ViewState);
		}

		private void UpdateUI(AbstractReportParams reportParams)
		{
			this.UpdateReport (reportParams);
			this.OnViewStateChanged (this.ViewState);
		}


		[Command (Res.CommandIds.Reports.Params)]
		private void OnParams(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	Affiche le Popup pour choisir les paramètres d'un rapport.
			var target = this.toolbar.GetTarget (e);
			this.report.ShowParamsPopup (target);
		}

		[Command (Res.CommandIds.Reports.AddFavorite)]
		private void OnAddFavorite(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	Cherche s'il existe déjà des paramètres avec le même nom.
			//	Si oui, on les supprime, pour les rajouter juste après, ce qui
			//	équivaut à une mise à jour.
			var existingParams = ReportParamsHelper.Search (this.accessor, this.report.ReportParams.CustomTitle);
			if (existingParams != null)
			{
				var target = this.toolbar.GetTarget (e);
				AddFavoritePopup.Show (target, this.accessor, createOperation =>
				{
					if (createOperation)  // crée un nouveau favori ?
					{
						//	Modifie le nom "Toto" en "Toto (copie)", car il ne peut pas y avoir
						//	2 rapports avec le même nom (sous peine de comportements futurs
						//	erratiques). On répète avec "Toto (copie) (copie)..." autant de fois
						//	que nécessaire, jusqu'à trouver un nom inexistant.
						do
						{
							var name = DataClipboard.GetCopyName (this.report.ReportParams.CustomTitle, this.accessor.GlobalSettings.CopyNameStrategy);
							this.report.ReportParams = this.report.ReportParams.ChangeCustomTitle (name);

							existingParams = ReportParamsHelper.Search (this.accessor, name);
						}
						while (existingParams != null);

						this.AddFavorite (null);
					}
					else  // met à jour le favosi existant ?
					{
						this.AddFavorite (existingParams);
					}
				});
			}
			else
			{
				this.AddFavorite (null);
			}
		}

		[Command (Res.CommandIds.Reports.RemoveFavorite)]
		private void OnRemoveFavorite(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.accessor.Mandat.Reports.Remove (this.report.ReportParams);

			this.reportChoiceController.Update ();
			this.UpdateToolbars ();
		}

		[Command (Res.CommandIds.Reports.CompactAll)]
		private void OnCompactAll(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.report.OnCompactAll ();
		}

		[Command (Res.CommandIds.Reports.CompactOne)]
		private void OnCompactOne(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.report.OnCompactOne ();
		}

		[Command (Res.CommandIds.Reports.ExpandOne)]
		private void OnExpandOne(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.report.OnExpandOne ();
		}

		[Command (Res.CommandIds.Reports.ExpandAll)]
		private void OnExpandAll(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.report.OnExpandAll ();
		}

		[Command (Res.CommandIds.Reports.Period.Prev)]
		private void OnPeriodPrev(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.report.ReportParams = this.report.ReportParams.ChangePeriod (-1);
			this.report.UpdateParams ();
		}

		[Command (Res.CommandIds.Reports.Period.Next)]
		private void OnPeriodNext(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.report.ReportParams = this.report.ReportParams.ChangePeriod (1);
			this.report.UpdateParams ();
		}

		[Command (Res.CommandIds.Reports.Export)]
		private void OnExport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	Affiche le Popup pour choisir comment exporter le rapport.
			var target = this.toolbar.GetTarget (e);
			this.report.ShowExportPopup (target);
		}

		[Command (Res.CommandIds.Reports.Close)]
		private void OnClose(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	Ferme le rapport.
			this.UpdateUI (null);
		}


		private void AddFavorite(AbstractReportParams paramsToRemove)
		{
			if (paramsToRemove != null)
			{
				this.accessor.Mandat.Reports.Remove (paramsToRemove);
			}

			this.accessor.Mandat.Reports.Add (this.report.ReportParams);

			this.reportChoiceController.Update ();
			this.UpdateToolbars ();
		}


		private void UpdateReport(AbstractReportParams reportParams)
		{
			if (this.report != null)
			{
				this.DeleteTreeTable ();

				this.report.ParamsChanged  -= this.HandleParamsChanged;
				this.report.UpdateCommands -= this.HandleUpdateCommands;
				this.report.Dispose ();
				this.report = null;
			}

			this.report = ReportParamsHelper.CreateReport (this.accessor, reportParams);

			if (this.report != null)
			{
				this.CreateTreeTable ();

				this.report.ParamsChanged  += this.HandleParamsChanged;
				this.report.UpdateCommands += this.HandleUpdateCommands;
			}

			this.UpdateTitle ();

			this.mainFrame  .Visibility = (this.report != null);
			this.choiceFrame.Visibility = (this.report == null);

			this.reportChoiceController.ClearSelection ();

			this.UpdateToolbars ();
		}

		private void CreateTreeTable()
		{
			this.treeTableController = new NavigationTreeTableController ();
			this.treeTableController.CreateUI (this.mainFrame, footerHeight: 0);

			this.report.Initialize (this.treeTableController);
		}

		private void DeleteTreeTable()
		{
			if (this.treeTableController != null)
			{
				this.treeTableController.Dispose ();
				this.treeTableController = null;
			}

			this.mainFrame.Children.Clear ();
		}

		private void UpdateTitle()
		{
			if (this.report == null)
			{
				this.topTitle.SetTitle (this.GetViewTitle (ViewType.Reports));
			}
			else
			{
				this.topTitle.SetTitle (this.report.Title);
			}
		}

		private void HandleParamsChanged(object sender)
		{
			this.UpdateTitle ();
			this.OnViewStateChanged (this.ViewState);
		}

		private void HandleUpdateCommands(object sender)
		{
			this.UpdateToolbars ();
		}


		public override AbstractViewState ViewState
		{
			get
			{
				return new ReportsViewState
				{
					ViewType     = ViewType.Reports,
					ReportParams = this.report == null ? null : this.report.ReportParams,
				};
			}
			set
			{
				var viewState = value as ReportsViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

				this.UpdateUI (viewState.ReportParams);
			}
		}


		private void UpdateToolbars()
		{
			this.toolbar.Visibility = (this.report != null);

			bool isCompactEnable    = this.IsCompactEnable;
			bool isExpandEnable     = this.IsExpandEnable;
			bool changePeriodEnable = this.ChangePeriodEnable;
			bool insideFavorites    = this.HasParamsInsideFavorites;

			this.commandContext.GetCommandState (Res.Commands.Reports.Params        ).Enable = this.HasParams;
			this.commandContext.GetCommandState (Res.Commands.Reports.AddFavorite   ).Enable = this.HasParams && this.report != null && !insideFavorites;
			this.commandContext.GetCommandState (Res.Commands.Reports.RemoveFavorite).Enable = this.HasParams && this.report != null &&  insideFavorites && !this.IsLastParamsType;

			this.commandContext.GetCommandState (Res.Commands.Reports.CompactAll).Enable = isCompactEnable;
			this.commandContext.GetCommandState (Res.Commands.Reports.CompactOne).Enable = isCompactEnable;
			this.commandContext.GetCommandState (Res.Commands.Reports.ExpandOne ).Enable = isExpandEnable;
			this.commandContext.GetCommandState (Res.Commands.Reports.ExpandAll ).Enable = isExpandEnable;

			this.commandContext.GetCommandState (Res.Commands.Reports.Period.Prev).Enable = changePeriodEnable;
			this.commandContext.GetCommandState (Res.Commands.Reports.Period.Next).Enable = changePeriodEnable;

			this.commandContext.GetCommandState (Res.Commands.Reports.Export).Enable = this.report != null;

			this.commandContext.GetCommandState (Res.Commands.Reports.Close).Enable = this.report != null;
		}

		private bool HasParamsInsideFavorites
		{
			//	Retourne true si les paramètres en cours sont contenus dans les favoris.
			get
			{
				if (this.report == null)
				{
					return false;
				}
				else
				{
					return this.accessor.Mandat.Reports
						.Where (x => x == this.report.ReportParams)
						.Any ();
				}
			}
		}

		private bool IsLastParamsType
		{
			//	Retourne true si les paramètres en cours sont les derniers de ce type.
			//	Il doit toujours exister au moins un rapport de chaque type.
			get
			{
				if (this.report == null)
				{
					return false;
				}
				else
				{
					return this.accessor.Mandat.Reports
						.Where (x => x.GetType () == this.report.ReportParams.GetType ())
						.Count () <= 1;
				}
			}
		}

		private bool IsCompactEnable
		{
			get
			{
				return this.report != null && this.report.IsCompactEnable;
			}
		}

		private bool IsExpandEnable
		{
			get
			{
				return this.report != null && this.report.IsExpandEnable;
			}
		}

		private bool ChangePeriodEnable
		{
			get
			{
				return this.report != null
					&& this.report.ReportParams.ChangePeriod (1) != null;
			}
		}

		private bool HasParams
		{
			get
			{
				return this.report != null
					&& this.report.ReportParams.HasParams;
			}
		}


		private readonly List<AbstractViewState>	historyViewStates;
		private readonly ReportsToolbar				toolbar;
		private readonly ReportChoiceController		reportChoiceController;

		private TopTitle							topTitle;
		private FrameBox							choiceFrame;
		private FrameBox							mainFrame;
		private NavigationTreeTableController		treeTableController;
		private AbstractReport						report;
	}
}
