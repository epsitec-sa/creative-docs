//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class MCH2SummaryReportPopup : AbstractStackedPopup
	{
		public MCH2SummaryReportPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.MCH2SummaryReport.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.Report.CustomTitle.ToString (),
				Width                 = GroupGuidStackedController.ControllerWidth,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = Res.Strings.Popup.MCH2SummaryReport.InitialDate.ToString (),
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = Res.Strings.Popup.MCH2SummaryReport.MonthCount.ToString (),
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = Res.Strings.Popup.MCH2SummaryReport.FinalDate.ToString (),
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.Popup.MCH2SummaryReport.GroupEnable.ToString (),
			});

			list.Add (new StackedControllerDescription  // 5
			{
				StackedControllerType = StackedControllerType.GroupGuid,
				Label                 = "",
				Width                 = GroupGuidStackedController.ControllerWidth,
				Height                = 150,
			});

			list.Add (new StackedControllerDescription  // 6
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = Res.Strings.Popup.MCH2SummaryReport.Level.ToString (),
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 7
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.Popup.MCH2SummaryReport.FilterEnable.ToString (),
			});

			list.Add (new StackedControllerDescription  // 8
			{
				StackedControllerType = StackedControllerType.GroupGuid,
				Label                 = "",
				Width                 = GroupGuidStackedController.ControllerWidth,
				Height                = 150,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Show.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		public MCH2SummaryParams				MCH2SummaryParams
		{
			get
			{
				return new MCH2SummaryParams (this.CustomTitle, this.DateRange, this.GroupGuid, this.Level, this.FilterGuid);
			}
			set
			{
				this.initialCustomTitle = value.CustomTitle;

				this.CustomTitle = value.CustomTitle;
				this.DateRange   = value.DateRange;
				this.GroupGuid   = value.RootGuid;
				this.FilterGuid  = value.FilterGuid;
				this.Level       = value.Level;
			}
		}

		private DateRange						DateRange
		{
			//	Donne la période. Comme les dates spécifiées par l'utilisateur vont
			//	habituellement du 1 janvier au 31 décembre, il faut adapter la date
			//	de fin. En effet, un DateRange a une date de fin exclue.
			get
			{
				if (this.InitialDate.HasValue && this.FinalDate.HasValue)
				{
					return new DateRange (
						this.InitialDate.Value,             // du 1 janvier
						this.FinalDate.Value.AddDays (1));  // au 1 janvier de l'année suivante (exlu)
				}
				else
				{
					return DateRange.Empty;
				}
			}
			set
			{
				if (value.IsEmpty)
				{
					this.InitialDate = null;
					this.FinalDate   = null;
				}
				else
				{
					this.InitialDate = value.IncludeFrom;
					this.FinalDate   = value.ExcludeTo.AddDays (-1);
					this.MonthCount  = this.ComputeMonthCount ();
				}
			}
		}

		private string							CustomTitle
		{
			get
			{
				return this.CustomTitleController.Value;
			}
			set
			{
				this.CustomTitleController.Value = value;
			}
		}

		private System.DateTime?				InitialDate
		{
			get
			{
				return this.InitialDateController.Value;
			}
			set
			{
				this.InitialDateController.Value = value;
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
			}
		}

		private System.DateTime?				FinalDate
		{
			get
			{
				return this.FinalDateController.Value;
			}
			set
			{
				this.FinalDateController.Value = value;
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

		private Guid							GroupGuid
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

		private int?							Level
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

		private bool							FilterEnable
		{
			get
			{
				return this.FilterEnableController.Value;
			}
			set
			{
				this.FilterEnableController.Value = value;
			}
		}

		private Guid							FilterGuid
		{
			get
			{
				if (this.FilterEnable)
				{
					return this.FilterGuidController.Value;
				}
				else
				{
					return Guid.Empty;
				}
			}
			set
			{
				this.FilterGuidController.Value = value;
				this.FilterEnable = !value.IsEmpty;
			}
		}


		protected override void CreateUI()
		{
			base.CreateUI ();

			{
				this.GroupGuidController.Level = 1;
			}
		}

		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			int rank = this.GetRank (description);

			if (rank == MCH2SummaryReportPopup.InitialDateRank)  // modification de la date initiale ?
			{
				this.FinalDate = this.ComputeFinalDate ();
			}
			else if (rank == MCH2SummaryReportPopup.MonthCountRank)  // modification de la durée en mois ?
			{
				this.FinalDate = this.ComputeFinalDate ();
			}
			else if (rank == MCH2SummaryReportPopup.FinalDateRank)  // modification de la date finale ?
			{
				this.MonthCount = this.ComputeMonthCount ();
			}

			this.SetEnable     (MCH2SummaryReportPopup.GroupGuidRank,  this.GroupEnable);
			this.SetVisibility (MCH2SummaryReportPopup.LevelRank,      this.GroupEnable);
			this.SetEnable     (MCH2SummaryReportPopup.FilterGuidRank, this.FilterEnable);

			this.okButton.Enable = this.IsCustomTitleCorrect
								&& this.InitialDate.HasValue
								&& this.MonthCount.HasValue
								&& this.MonthCount.Value > 0
								&& this.FinalDate.HasValue
								&& this.InitialDate < this.FinalDate
								&& (!this.GroupEnable || !this.GroupGuid.IsEmpty)
								&& !this.HasError;
		}


		private bool IsCustomTitleCorrect
		{
			get
			{
				if (this.CustomTitle == this.initialCustomTitle)
				{
					return true;
				}
				else
				{
					var savedParams = ReportParamsHelper.Search (this.accessor, this.CustomTitle);
					return savedParams == null;
				}
			}
		}


		private TextStackedController CustomTitleController
		{
			get
			{
				var controller = this.GetController (MCH2SummaryReportPopup.CustomTitleRank) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller;
			}
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

		private BoolStackedController FilterEnableController
		{
			get
			{
				var controller = this.GetController (MCH2SummaryReportPopup.FilterEnableRank) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller;
			}
		}

		private GroupGuidStackedController FilterGuidController
		{
			get
			{
				var controller = this.GetController (MCH2SummaryReportPopup.FilterGuidRank) as GroupGuidStackedController;
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
				return DateTime.Months (f.Value, i.Value) + 1;
			}
			else
			{
				return null;
			}
		}


		private const int CustomTitleRank  = 0;
		private const int InitialDateRank  = 1;
		private const int MonthCountRank   = 2;
		private const int FinalDateRank    = 3;
		private const int GroupEnableRank  = 4;
		private const int GroupGuidRank    = 5;
		private const int LevelRank        = 6;
		private const int FilterEnableRank = 7;
		private const int FilterGuidRank   = 8;


		private string initialCustomTitle;
	}
}