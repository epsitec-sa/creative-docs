//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class MCH2SummaryReportPopup : StackedPopup
	{
		public MCH2SummaryReportPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Paramètres du tableau des immobilisations MCH2";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = "Etat initial au",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = "Durée en mois",
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = "Etat final au",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = "Effectuer un groupement",
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.GroupGuid,
				Label                 = "",
				Width                 = 200 + (int) AbstractScroller.DefaultBreadth,
				Height                = 150,
			});

			list.Add (new StackedControllerDescription  // 5
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = "Jusqu'au niveau",
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = "Voir";
			this.defaultControllerRankFocus = 0;
		}


		public System.DateTime?					InitialDate
		{
			get
			{
				return this.InitialDateController.Value;
			}
			set
			{
				this.InitialDateController.Value = value;

				var date = this.ComputeFinalDate ();
				if (this.FinalDate != date)
				{
					this.FinalDate = date;
				}
			}
		}

		private int?							MonthCount
		{
			get
			{
				return this.MonthCountController.Value;
			}
			set
			{
				this.MonthCountController.Value = value;

				var date = this.ComputeFinalDate ();
				if (this.FinalDate != date)
				{
					this.FinalDate = date;
				}
			}
		}

		public System.DateTime?					FinalDate
		{
			get
			{
				return this.FinalDateController.Value;
			}
			set
			{
				this.FinalDateController.Value = value;

				var month = this.ComputeMonthCount ();
				if (this.MonthCount != month)
				{
					this.MonthCount = month;
				}
			}
		}

		private bool							GroupEnable
		{
			get
			{
				return this.GroupEnableController.Value;
			}
			set
			{
				this.GroupEnableController.Value = value;
			}
		}

		public Guid								GroupGuid
		{
			get
			{
				if (this.GroupEnable)
				{
					return this.GroupGuidController.Value;
				}
				else
				{
					return Guid.Empty;
				}
			}
			set
			{
				this.GroupGuidController.Value = value;
				this.GroupEnable = !value.IsEmpty;
			}
		}

		public int?								Level
		{
			get
			{
				return this.LevelController.Value;
			}
			set
			{
				this.LevelController.Value = value;
			}
		}


		public override void CreateUI()
		{
			base.CreateUI ();

			{
				this.LevelController.Value = 1;
			}
		}

		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			int rank = this.GetRank (description);

			if (rank == MCH2SummaryReportPopup.InitialDateRank)  // modification de la date initiale ?
			{
				this.FinalDateController.Value = this.ComputeFinalDate ();
			}
			else if (rank == MCH2SummaryReportPopup.MonthCountRank)  // modification de la durée en mois ?
			{
				this.FinalDateController.Value = this.ComputeFinalDate ();
			}
			else if (rank == MCH2SummaryReportPopup.FinalDateRank)  // modification de la date finale ?
			{
				this.MonthCountController.Value = this.ComputeMonthCount ();
			}

			this.SetVisibility (MCH2SummaryReportPopup.GroupGuidRank, this.GroupEnable);
			this.SetVisibility (MCH2SummaryReportPopup.LevelRank,     this.GroupEnable);

			this.okButton.Enable = this.InitialDate.HasValue
								&& this.MonthCount.HasValue
								&& this.MonthCount.Value > 0
								&& this.FinalDate.HasValue
								&& this.InitialDate < this.FinalDate
								&& (!this.GroupEnable || !this.GroupGuid.IsEmpty)
								&& !this.HasError;
		}


		private DateStackedController InitialDateController
		{
			get
			{
				var controller = this.GetController (MCH2SummaryReportPopup.InitialDateRank) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller;
			}
		}

		private IntStackedController MonthCountController
		{
			get
			{
				var controller = this.GetController (MCH2SummaryReportPopup.MonthCountRank) as IntStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller;
			}
		}

		private DateStackedController FinalDateController
		{
			get
			{
				var controller = this.GetController (MCH2SummaryReportPopup.FinalDateRank) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller;
			}
		}

		private BoolStackedController GroupEnableController
		{
			get
			{
				var controller = this.GetController (MCH2SummaryReportPopup.GroupEnableRank) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller;
			}
		}

		private GroupGuidStackedController GroupGuidController
		{
			get
			{
				var controller = this.GetController (MCH2SummaryReportPopup.GroupGuidRank) as GroupGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller;
			}
		}

		private IntStackedController LevelController
		{
			get
			{
				var controller = this.GetController (MCH2SummaryReportPopup.LevelRank) as IntStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller;
			}
		}


		private System.DateTime? ComputeFinalDate()
		{
			//	Calcule la date finale, à partir de la date initiale et du nombre de mois.
			//	Du 01.01.2014 + 12 mois retourne 31.12.2014.
			var i = this.InitialDate;
			var m = this.MonthCount;

			if (i.HasValue && m.HasValue)
			{
				return i.Value.AddMonths (m.Value).AddDays (-1);  // dernier jour du mois
			}
			else
			{
				return null;
			}
		}

		private int? ComputeMonthCount()
		{
			//	Calcule le nombre de mois, à partir des dates initiale et finale.
			//	Du 01.01.2014 au 31.12.2014 retourne 12.
			var i = this.InitialDate;
			var f = this.FinalDate;

			if (i.HasValue && f.HasValue)
			{
				return MCH2SummaryReportPopup.GetMonth (f.Value)
					 - MCH2SummaryReportPopup.GetMonth (i.Value)
					 + 1;
			}
			else
			{
				return null;
			}
		}

		private static int GetMonth(System.DateTime date)
		{
			return date.Year*12 + date.Month;
		}


		private const int InitialDateRank = 0;
		private const int MonthCountRank  = 1;
		private const int FinalDateRank   = 2;
		private const int GroupEnableRank = 3;
		private const int GroupGuidRank   = 4;
		private const int LevelRank       = 5;
	}
}