//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Memory.Data
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


		public void Update(AbstractController controller, Command command)
		{
			//	Met à jour les données de la présentation active dans l'historique de la navigation.
			if (this.index != -1 && this.history[this.index].Command == command)
			{
				this.history[this.index] = this.CreateNavigatorData (controller, command);
			}
		}

		public void Put(AbstractController controller, Command command)
		{
			//	Ajoute les données de la présentation active au somment de l'historique de la navigation.
			//	Toutes les données "en avant" sont supprimées.
			var data = this.CreateNavigatorData (controller, command);
			this.history.Insert (++this.index, data);

			int overflow = this.history.Count-this.index-1;
			for (int i = 0; i < overflow; i++)
			{
				this.history.RemoveAt (this.index+1);
			}
		}

		private NavigatorData CreateNavigatorData(AbstractController controller, Command command)
		{
			//	Présentation active -> NavigatorData.
			SearchData      search     = null;
			SearchData      filter     = null;
			AbstractOptions options    = null;

			if (controller == null)
			{
				return new NavigatorData (command, FormattedText.Empty, null, search, filter, options, null);
			}
			else
			{
				if (controller.DataAccessor != null && controller.DataAccessor.SearchData != null)
				{
					search = controller.DataAccessor.SearchData.CopyFrom ();
				}

				if (controller.DataAccessor != null && controller.DataAccessor.FilterData != null)
				{
					filter = controller.DataAccessor.FilterData.CopyFrom ();
				}

				if (controller.DataAccessor != null && controller.DataAccessor.Options != null)
				{
					options = controller.DataAccessor.Options.NavigatorCopyFrom ();
				}

				if (controller.MemoryList == null)
				{
					return new NavigatorData (command, controller.MixTitle, null, search, filter, options, controller.SelectedArrayLine);
				}
				else
				{
					return new NavigatorData (command, controller.MixTitle, controller.MemoryList.Selected, search, filter, options, controller.SelectedArrayLine);
				}
			}
		}


		public Command Any(int index)
		{
			//	Retourne l'index permettant de revenir à une position quelconque dans l'historique de la navigation.
			this.index = index;
			return this.history[this.index].Command;
		}

		public Command Back
		{
			//	Retourne l'index permettant de revenir en arrière dans l'historique de la navigation.
			get
			{
				return this.history[--this.index].Command;
			}
		}

		public Command Forward
		{
			//	Retourne l'index permettant de revenir en avant dans l'historique de la navigation.
			get
			{
				return this.history[++this.index].Command;
			}
		}

		public void Restore(AbstractController controller)
		{
			//	Restitue le NavigatorData obtenu par Any/Back/Forward.
			//	NavigatorData -> présetation active.
			var data = this.history[this.index];

			if (controller != null)
			{
				if (controller.MemoryList != null && data.Memory != null)
				{
					controller.MemoryList.Selected = data.Memory;
				}

				if (controller.DataAccessor != null && controller.DataAccessor.SearchData != null)
				{
					data.Search.CopyTo (controller.DataAccessor.SearchData);
				}

				if (controller.DataAccessor != null && controller.DataAccessor.FilterData != null)
				{
					data.Filter.CopyTo (controller.DataAccessor.FilterData);
				}

				if (controller.DataAccessor != null && controller.DataAccessor.Options != null)
				{
					data.Options.NavigatorCopyTo (controller.DataAccessor.Options);
				}
			}
		}

		public void RestoreArrayController(AbstractController controller)
		{
			//	Termine le travail de Restore, pour sélectionner la bonne ligne dans le tableau.
			var data = this.history[this.index];

			if (controller != null)
			{
				controller.SelectedArrayLine = data.ArrayIndex.Value;
			}
		}


		private readonly List<NavigatorData>	history;
		private int								index;
	}
}