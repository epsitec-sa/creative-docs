//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Permanents;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.ViewSettings.Data
{
	/// <summary>
	/// Mémorise les paramètres d'un contrôleur, pour permettre de le recréer dans le même état.
	/// </summary>
	public class NavigatorData
	{
		public NavigatorData(ControllerType controllerType, FormattedText description, ViewSettingsData viewSettings, SearchData search, SearchData filter, AbstractOptions options, AbstractPermanents permanents, int? arrayIndex)
		{
			this.ControllerType = controllerType;
			this.Description    = description;
			this.ViewSettings   = viewSettings;
			this.Search         = search;
			this.Filter         = filter;
			this.Options        = options;
			this.Permanents     = permanents;
			this.ArrayIndex     = arrayIndex;
		}


		public ControllerType ControllerType
		{
			get;
			private set;
		}

		public FormattedText Description
		{
			get;
			private set;
		}


		public ViewSettingsData ViewSettings
		{
			get;
			private set;
		}

		public SearchData Search
		{
			get;
			private set;
		}

		public SearchData Filter
		{
			get;
			private set;
		}

		public AbstractOptions Options
		{
			get;
			private set;
		}

		public AbstractPermanents Permanents
		{
			get;
			private set;
		}

		public int? ArrayIndex
		{
			get;
			private set;
		}
	}
}