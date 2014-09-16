//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public enum ToolbarCommand
	{
		Unknown,

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
