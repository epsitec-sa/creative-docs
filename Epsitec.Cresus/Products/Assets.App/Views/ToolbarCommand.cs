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
		Now,
		Date,
		CompactAll,
		ExpandAll,
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

		ViewTypeObjects,
		ViewTypeAmortizations,
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
		AmortizationsUnpreview,
		AmortizationsDelete,
		AmortizationsInfo,
	}
}
