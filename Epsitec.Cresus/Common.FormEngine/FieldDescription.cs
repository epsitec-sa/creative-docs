using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Décrit un champ dans un masque de saisie.
	/// </summary>
	public class FieldDescription
	{
		public enum SeparatorType
		{
			Normal,
			Compact,
			Extend,
		}


		public FieldDescription()
		{
			//	Constructeur.
			this.fieldIds = new List<Druid>();
			this.backColor = Color.Empty;
			this.bottomSeparator = SeparatorType.Normal;
		}

		public FieldDescription(string listDruids) : this()
		{
			//	Constructeur sur la base d'une liste de Druids séparés par des points.
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
			//	Liste des Druids qui représentent le champ.
			get
			{
				return this.fieldIds;
			}
		}

		public string GetPath(string prefix)
		{
			//	Retourne le chemin permettant d'accéder au champ.
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

		public SeparatorType BottomSeparator
		{
			get
			{
				return this.bottomSeparator;
			}
			set
			{
				this.bottomSeparator = value;
			}
		}

		public Color BackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
			}
		}


		protected List<Druid> fieldIds;
		protected Color backColor;
		protected SeparatorType bottomSeparator;
	}
}
