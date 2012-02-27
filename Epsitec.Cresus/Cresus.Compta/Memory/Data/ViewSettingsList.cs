//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.ViewSettings.Data
{
	/// <summary>
	/// Liste des paramètres des recherches, du filtre et des options, c'est-à-dire de l'ensemble des paramètres
	/// liés à une présentation (traduit en français par "réglage de présentation").
	/// </summary>
	public class ViewSettingsList : ISettingsData
	{
		public ViewSettingsList()
		{
			this.list = new List<ViewSettingsData> ();
		}


		public List<ViewSettingsData> List
		{
			get
			{
				return this.list;
			}
		}


		public int SelectedIndex
		{
			get
			{
				return this.list.IndexOf (this.selected);
			}
			set
			{
				if (value >= 0 && value < this.list.Count)
				{
					this.selected = this.list[value];
				}
				else
				{
					this.selected = null;
				}
			}
		}

		public ViewSettingsData Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				this.selected = value;
			}
		}


		private readonly List<ViewSettingsData>	list;
		private ViewSettingsData					selected;
	}
}