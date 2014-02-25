//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views
{
	public enum ToolbarCommand
	{
		Open,
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
		ExpandAll,
		Narrow,
		Wide,
		Edit,
		WeeksOfYear,
		DaysOfWeek,
		Graph,
		Labels,
		Simulation,
		Accept,
		Cancel,
		Filter,

		NavigateBack,
		NavigateForward,
		NavigateMenu,

		ViewTypeAssets,
		ViewTypeAmortizations,
		ViewTypeEcritures,
		ViewTypeCategories,
		ViewTypeGroups,
		ViewTypePersons,
		ViewTypeEvents,
		ViewTypeReports,
		ViewTypeSettings,

		ViewModeSingle,
		ViewModeEvent,
		ViewModeMultiple,

		AmortizationsPreview,
		AmortizationsFix,
		AmortizationsToExtra,
		AmortizationsUnpreview,
		AmortizationsDelete,
		AmortizationsInfo,

		SettingsGeneral,
		SettingsAssetsView,
		SettingsPersonsView,
		SettingsPlanComptable,
	}
}
