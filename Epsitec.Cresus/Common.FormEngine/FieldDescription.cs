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
			Normal  = 0,	// petit séparateur standard
			Compact = 1,	// les champs se touchent (chevauchement d'un pixel)
			Extend  = 2,	// grand séparateur
			Line    = 3,	// grand séparateur avec trait horizontal
			Append  = 4,	// le champ suivant sera sur la même ligne
		}


		public FieldDescription()
		{
			//	Constructeur.
			this.fieldIds = new List<Druid>();
			this.backColor = Color.Empty;
			this.bottomSeparator = SeparatorType.Normal;
			this.columnsRequired = 10;
			this.rowsRequired = 1;
		}

		public FieldDescription(string listDruids) : this()
		{
			//	Constructeur sur la base d'une liste de Druids séparés par des points.
			//	Par exemple: listDruids = "[630B2].[630S2]"
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
			//	Par exemple, si prefix = "Data": retourne "Data.[630B2].[630S2]"
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
			//	Type du séparateur après le champ (donc en dessous).
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
			//	Couleur de fond pour le champ.
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
			}
		}

		public int ColumnsRequired
		{
			//	Nombre de colonnes requises [1..10].
			get
			{
				return this.columnsRequired;
			}
			set
			{
				value = System.Math.Max(value, 1);
				value = System.Math.Min(value, FormEngine.MaxColumnsRequired);
				this.columnsRequired = value;
			}
		}

		public int RowsRequired
		{
			//	Nombre de lignes requises [1..20].
			get
			{
				return this.rowsRequired;
			}
			set
			{
				value = System.Math.Max(value, 1);
				value = System.Math.Min(value, FormEngine.MaxRowsRequired);
				this.rowsRequired = value;
			}
		}


		protected List<Druid> fieldIds;
		protected Color backColor;
		protected SeparatorType bottomSeparator;
		protected int columnsRequired;
		protected int rowsRequired;
	}
}
