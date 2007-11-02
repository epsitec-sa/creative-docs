using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// D�crit un champ dans un masque de saisie.
	/// </summary>
	public class FieldDescription
	{
		public FieldDescription()
		{
			//	Constructeur.
			this.fieldIds = new List<Druid>();
		}

		public FieldDescription(string listDruids) : this()
		{
			//	Constructeur sur la base d'une liste de Druids s�par�s par des points.
			//	Par exemple: "[630B2].[630S2]"
			string[] druids = listDruids.Split('.');
			foreach (string druid in druids)
			{
				Druid id = Druid.Parse(druid);
				this.fieldIds.Add(id);
			}
		}

		public List<Druid> FieldIds
		{
			//	Liste des Druids qui repr�sentent le champ.
			get
			{
				return this.fieldIds;
			}
		}

		public string GetPath(string prefix)
		{
			//	Retourne le chemin permettant d'acc�der au champ.
			//	Par exemple: "Data.[630B2].[630S2]"
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append(prefix);

			foreach (Druid druid in this.fieldIds)
			{
				builder.Append(".");
				builder.Append(druid);
			}

			return builder.ToString();
		}

		public bool Separator
		{
			get
			{
				return this.separator;
			}
			set
			{
				this.separator = value;
			}
		}


		protected List<Druid> fieldIds;
		protected bool separator;
	}
}
