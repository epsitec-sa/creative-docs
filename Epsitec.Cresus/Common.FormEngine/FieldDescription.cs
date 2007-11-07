using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Décrit un champ ou un séparateur dans un masque de saisie.
	/// </summary>
	public class FieldDescription
	{
		public enum FieldType
		{
			Node	= 0,	// noeud
			Field	= 1,	// champ
			Line    = 2,	// séparateur trait horizontal
			Title   = 3,	// séparateur titre automatique
		}

		public enum SeparatorType
		{
			Normal	= 0,	// les champs sont proches
			Compact	= 1,	// les champs se touchent (chevauchement d'un pixel)
			Extend	= 2,	// les champs sont très espacés
			Append	= 3,	// le champ suivant sera sur la même ligne
		}


		public FieldDescription(FieldType type)
		{
			//	Constructeur. Le type est toujours déterminé à la création. Il ne pourra plus changer ensuite.
			this.type = type;
			this.backColor = Color.Empty;
			this.separator = SeparatorType.Normal;
			this.columnsRequired = 10;
			this.rowsRequired = 1;
		}

		public FieldDescription(FieldDescription model)
		{
			//	Constructeur copiant une instance existante.
			this.type = model.type;
			this.backColor = model.backColor;
			this.separator = model.separator;
			this.columnsRequired = model.columnsRequired;
			this.rowsRequired = model.rowsRequired;
			this.container = model.container;
			this.nodeDescription = model.nodeDescription;
			this.fieldIds = model.fieldIds;
		}


		public FieldType Type
		{
			//	Retourne le type immuable de cet élément.
			get
			{
				return this.type;
			}
		}


		public void SetNode(List<FieldDescription> descriptions)
		{
			//	Donne la liste des descriptions du noeud.
			System.Diagnostics.Debug.Assert(this.type == FieldType.Node);

			this.nodeDescription = new List<FieldDescription>();
			foreach (FieldDescription description in descriptions)
			{
				this.nodeDescription.Add(description);
			}
		}

		public List<FieldDescription> NodeDescription
		{
			//	Retourne la liste des descriptions du noeud.
			get
			{
				return this.nodeDescription;
			}
		}


		public void SetFields(string listDruids)
		{
			//	Donne d'une liste de Druids séparés par des points.
			//	Par exemple: listDruids = "[630B2].[630S2]"
			System.Diagnostics.Debug.Assert(this.type == FieldType.Field);
			this.fieldIds = new List<Druid>();
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
			if (this.type == FieldType.Field)
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();
				builder.Append(prefix);

				foreach (Druid druid in this.fieldIds)
				{
					builder.Append(".");
					builder.Append(druid);
				}

				return builder.ToString();
			}
			else
			{
				return null;
			}
		}


		public SeparatorType Separator
		{
			//	Type de séparation après le champ suivant.
			get
			{
				return this.separator;
			}
			set
			{
				this.separator = value;
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

		public string Container
		{
			//	Nom du conteneur (sorte de colonne).
			get
			{
				return this.container;
			}
			set
			{
				this.container = value;
			}
		}


		public void CopyTo(FieldDescription dst)
		{
			//	Copie la description courante vers une destination quelconque.
			dst.type = this.type;
			dst.backColor = this.backColor;
			dst.separator = this.separator;
			dst.columnsRequired = this.columnsRequired;
			dst.rowsRequired = this.rowsRequired;
			dst.container = this.container;

#if false
			if (this.nodeDescription == null)
			{
				dst.nodeDescription = null;
			}
			else
			{
				dst.nodeDescription = new List<FieldDescription>();
				foreach (FieldDescription field in this.nodeDescription)
				{
					dst.nodeDescription.Add(field);
				}
			}

			if (this.fieldIds == null)
			{
				dst.fieldIds = null;
			}
			else
			{
				dst.fieldIds = new List<Druid>();
				foreach (Druid druid in this.fieldIds)
				{
					dst.fieldIds.Add(druid);
				}
			}
#else
			dst.nodeDescription = this.nodeDescription;
			dst.fieldIds = this.fieldIds;
#endif
		}


		protected FieldType type;
		protected List<FieldDescription> nodeDescription;
		protected List<Druid> fieldIds;
		protected Color backColor;
		protected SeparatorType separator;
		protected int columnsRequired;
		protected int rowsRequired;
		protected string container;
	}
}
