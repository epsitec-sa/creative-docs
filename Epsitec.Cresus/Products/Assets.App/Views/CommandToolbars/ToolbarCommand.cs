//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public enum ToolbarCommand
	{
		Unknown,

		NewMandat,
		OpenMandat,
		SaveMandat,

		New,
		Delete,
		Deselect,
		First,
		Prev,
		Next,
		Last,
		MoveTop,
		MoveUp,
		MoveDown,
		MoveBottom,
		Now,
		Date,
		CompactAll,
		CompactOne,
		ExpandOne,
		ExpandAll,
		Narrow,
		Wide,
		WeeksOfYear,
		DaysOfWeek,
		Graph,
		Labels,
		Simulation,
		Locked,
		Graphic,
		Filter,
		DateRange,
		Copy,
		Paste,
		Export,
		Import,
		Goto,

		ReportParams,
		ReportExport,
		ReportPrevPeriod,
		ReportNextPeriod,
		ReportAddFavorite,
		ReportRemoveFavorite,
		ReportClose,

		NavigateBack,
		NavigateForward,
		NavigateMenu,

		AmortizationsPreview,
		AmortizationsFix,
		AmortizationsToExtra,
		AmortizationsUnpreview,
		AmortizationsDelete,
		AmortizationsInfo,

		SettingsAssetsView,
		SettingsPersonsView,
		SettingsAccounts,
	}
}
