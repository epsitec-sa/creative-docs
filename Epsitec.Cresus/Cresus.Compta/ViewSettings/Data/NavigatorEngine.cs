//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Accessors;
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
	/// Gestion de la navigation.
	/// </summary>
	public class NavigatorEngine
	{
		public NavigatorEngine()
		{
			this.history = new List<NavigatorData> ();
			this.Clear ();
		}

		public void Clear()
		{
			//	Efface l'historique de la navigation.
			this.history.Clear ();
			this.index = -1;
		}


		public int Count
		{
			//	Retourne le nombre d'éléments contenus dans l'historique de la navigation.
			get
			{
				return this.history.Count;
			}
		}

		public int Index
		{
			//	Index dans l'historique de la navigation.
			get
			{
				return this.index;
			}
		}

		public NavigatorData GetNavigatorData(int index)
		{
			//	Retourne un élément contenu dans l'historique de la navigation.
			return this.history[index];
		}


		public bool PrevEnable
		{
			//	Indique si la commande "en arrière" est active.
			get
			{
				return this.index > 0;
			}
		}

		public bool NextEnable
		{
			//	Indique si la commande "en avant" est active.
			get
			{
				return this.index < this.history.Count-1;
			}
		}


		public void Update(AbstractController controller, ControllerType controllerType)
		{
			//	Met à jour les données de la présentation active dans l'historique de la navigation.
			if (this.index != -1 && this.history[this.index].ControllerType == controllerType)
			{
				this.history[this.index] = this.CreateNavigatorData (controller, controllerType, this.history[this.index].ViewSettingsName);
			}
		}

		public void Put(AbstractController controller, ControllerType controllerType)
		{
			//	Ajoute les données de la présentation active au somment de l'historique de la navigation.
			//	Toutes les données "en avant" sont supprimées.
			var data = this.CreateNavigatorData (controller, controllerType, FormattedText.Null);
			this.history.Insert (++this.index, data);

			int overflow = this.history.Count-this.index-1;
			for (int i = 0; i < overflow; i++)
			{
				this.history.RemoveAt (this.index+1);
			}
		}

		private NavigatorData CreateNavigatorData(AbstractController controller, ControllerType controllerType, FormattedText viewSettingsName)
		{
			//	Présentation active -> NavigatorData.
			SearchData         search     = null;
			SearchData         filter     = null;
			AbstractOptions    options    = null;
			AbstractPermanents permanents = null;

			if (controller == null)
			{
				return new NavigatorData (controllerType, FormattedText.Empty, null, search, filter, options, permanents, null);
			}
			else
			{
				if (controller.DataAccessor != null)
				{
					if (controller.DataAccessor.SearchData != null)
					{
						search = controller.DataAccessor.SearchData.CopyFrom ();
					}

					if (controller.DataAccessor.FilterData != null)
					{
						filter = controller.DataAccessor.FilterData.CopyFrom ();
					}

					if (controller.DataAccessor.Options != null)
					{
						options = controller.DataAccessor.Options.CopyFrom ();
					}

					if (controller.DataAccessor.Permanents != null)
					{
						permanents = controller.DataAccessor.Permanents.CopyFrom ();
					}
				}

				if (viewSettingsName.IsNullOrEmpty && controller.ViewSettingsList != null && controller.ViewSettingsList.Selected != null)
				{
					viewSettingsName = controller.ViewSettingsList.Selected.Name;
				}

				return new NavigatorData (controllerType, controller.MixTitle, viewSettingsName, search, filter, options, permanents, controller.SelectedArrayLine);
			}
		}


		public ControllerType Any(int index)
		{
			//	Retourne l'index permettant de revenir à une position quelconque dans l'historique de la navigation.
			this.index = index;
			return this.history[this.index].ControllerType;
		}

		public ControllerType Back
		{
			//	Retourne l'index permettant de revenir en arrière dans l'historique de la navigation.
			get
			{
				return this.history[--this.index].ControllerType;
			}
		}

		public ControllerType Forward
		{
			//	Retourne l'index permettant de revenir en avant dans l'historique de la navigation.
			get
			{
				return this.history[++this.index].ControllerType;
			}
		}

		public void Restore(MainWindowController mainWindowController)
		{
			//	Restitue le NavigatorData obtenu par Any/Back/Forward.
			//	NavigatorData -> présetation active.
			var data = this.history[this.index];

			{
				string key = string.Concat ("Présentation." + Présentations.ControllerTypeToString (data.ControllerType) + ".Search");
				var searchData = mainWindowController.GetSettingsSearchData ("Présentation." + key + ".Search");

				if (data.Search != null && searchData != null)
				{
					data.Search.CopyTo (searchData);
				}
			}

			{
				var type = Présentations.GetGroupControllerType (data.ControllerType);
				string key = string.Concat ("Présentation." + Présentations.ControllerTypeToString (type) + ".ViewSettings");
				var list = mainWindowController.GetViewSettingsList (key);
				System.Diagnostics.Debug.Assert (list != null);

				//	On cherche le ViewSettingsData qui avait le même nom. Si on ne le trouve pas, on cherche
				//	un ViewSettingsData qui a le même type. Le dernier est préférable !
				var viewSettingsData = list.List.Where (x => x.Name == data.ViewSettingsName).LastOrDefault ();

				if (viewSettingsData == null)
				{
					viewSettingsData = list.List.Where (x => x.ControllerType == data.ControllerType).LastOrDefault ();
				}

				//	On sélectionne le ViewSettingsData trouvé.
				System.Diagnostics.Debug.Assert (viewSettingsData != null);
				list.Selected = viewSettingsData;

				if (viewSettingsData.CurrentFilter != null && data.Filter != null)
				{
					data.Filter.CopyTo (viewSettingsData.CurrentFilter);
				}

				if (viewSettingsData.CurrentOptions != null && data.Options != null)
				{
					data.Options.CopyTo (viewSettingsData.CurrentOptions);
				}
			}

#if false
				if (controller.ViewSettingsList != null && data.ViewSettings != null)
				{
					controller.ViewSettingsList.Selected = data.ViewSettings;
				}

				if (controller.DataAccessor != null)
				{
					if (controller.DataAccessor.SearchData != null)
					{
						data.Search.CopyTo (controller.DataAccessor.SearchData);
					}

					if (controller.DataAccessor.FilterData != null)
					{
						data.Filter.CopyTo (controller.DataAccessor.FilterData);
					}

					if (controller.DataAccessor.Options != null)
					{
						data.Options.CopyTo (controller.DataAccessor.Options);
					}

					if (controller.DataAccessor.Permanents != null)
					{
						data.Permanents.CopyTo (controller.DataAccessor.Permanents);
					}
				}
#endif
		}

		public void RestoreArrayController(AbstractController controller)
		{
			//	Termine le travail de Restore, pour sélectionner la bonne ligne dans le tableau.
			var data = this.history[this.index];

			if (controller != null)
			{
				controller.SelectedArrayLine = data.ArrayIndex.Value;

				if (controller.EditorController != null)
				{
					controller.EditorController.EditorSelect (0, 0);
				}
			}
		}


		private readonly List<NavigatorData>	history;
		private int								index;
	}
}