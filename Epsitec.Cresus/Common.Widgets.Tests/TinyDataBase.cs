namespace Epsitec.Common
{
	/// <summary>
	/// La classe TinyDataBase implémente une base de données très simple.
	/// </summary>
	public class TinyDataBase : System.Object
	{
		public enum FieldType
		{
			None,			// sans type
			String,			// chaîne
			Tel,			// numéros de téléphone
			Mail,			// adresse e-mail
			Web,			// url
			Date,			// date
			Prix,			// prix
			Checks,			// liste de coches
		}

		public enum FieldRequire
		{
			Optional,		// rubrique optionnelle
			Obligatory,		// cette rubrique doit exister
			Or,				// au moins une doit exister
		}

		public struct FieldDesc
		{
			public string					name;		// nom
			public FieldType				type;		// type
			public FieldRequire				require;	// présence
			public int						rank;		// rang d'édition
			public Drawing.ContentAlignment	alignment;	// justification
			public int						max;		// nb max de caractères
			public int						lines;		// nb de lignes
			public bool						link;		// liée à la suivante
			public int						width;		// largeur dans liste
			public string					combo;		// contenu éventuel combo
		}

		//	Classe d'une fiche.
		public class Record : System.Collections.ArrayList
		{
		}


		public TinyDataBase()
		{
			//	Constructeur.
			this.sortField = new int[this.maxSort];
			this.sortMode = new Widgets.SortMode[this.maxSort];
			for ( int i=0 ; i<this.maxSort ; i++ )
			{
				this.sortField[i] = -1;  // pas de tri
				this.sortMode[i] = Widgets.SortMode.None;
			}
		}

		public string Title
		{
			//	Titre de la base.
			get
			{
				return this.title;
			}

			set
			{
				this.title = value;
			}
		}


		public int TotalField
		{
			//	Retourne le nombre total de rubriques.
			get
			{
				return this.fields.Count;
			}
		}

		public void CreateEmptyFieldDesc(out FieldDesc desc)
		{
			//	Crée une nouvelle définition de rubrique vide.
			desc.name      = "";
			desc.type      = FieldType.String;
			desc.require   = FieldRequire.Optional;
			desc.rank      = -1;
			desc.alignment = Drawing.ContentAlignment.TopLeft;
			desc.max       = 100;
			desc.lines     = 1;
			desc.link      = false;
			desc.width     = 100;
			desc.combo     = "";
		}

		public int CreateFieldDesc(FieldDesc desc)
		{
			//	Crée une nouvelle rubrique et retourne l'identificateur de la rubrique.
			//	Une rubrique ne peut jamais être détruite.
			//	Dès que la base contient une ou plusieurs fiches, il n'est plus possible
			//	d'ajouter des rubriques.
			System.Diagnostics.Debug.Assert(!this.inUse);
			return this.fields.Add(desc);
		}

		public FieldDesc RetFieldDesc(int fieldID)
		{
			//	Donne les définitions d'une rubrique.
			System.Diagnostics.Debug.Assert( fieldID >= 0 && fieldID < this.TotalField );
			return (FieldDesc)this.fields[fieldID];
		}

		public int RetFieldID(int rank)
		{
			//	Retourne l'identificateur de rubrique correspondant à un rang donné.
			for ( int fieldID=0 ; fieldID<this.TotalField ; fieldID++ )
			{
				FieldDesc fieldDesc = RetFieldDesc(fieldID);
				if ( rank == fieldDesc.rank )  return fieldID;
			}
			return -1;
		}


		public void CreateEmptyRecord(out Record record)
		{
			//	Crée une fiche vide.
			record = new Record();
			for ( int i=0 ; i<this.TotalField ; i++ )
			{
				record.Add("");
			}
		}

		public void CreateCopyRecord(out Record record, Record original)
		{
			//	Crée une copie d'une fiche.
			record = new Record();
			for ( int i=0 ; i<this.TotalField ; i++ )
			{
				record.Add(original[i]);
			}
		}

		public bool SetFieldInRecord(Record record, int fieldID, string text)
		{
			//	Met une rubrique dans une fiche.
			if ( fieldID < 0 || fieldID >= this.TotalField )  return false;

			record[fieldID] = text;
			return true;
		}

		public string RetFieldInRecord(Record record, int fieldID)
		{
			//	Rend une rubrique d'une fiche.
			if ( fieldID < 0 || fieldID >= this.TotalField )  return "";
			if ( fieldID >= record.Count )  return "";
			return (string)record[fieldID];
		}

		public int CheckRecord(Record record)
		{
			//	Vérifie si une fiche est correcte.
			//	Retourne l'identificateur de la rubrique fausse, ou -1 si tout est ok.
			FieldDesc	field;
			int			max, i, reqTotal, reqExist, reqField;
			string		content;

			reqTotal = reqExist = 0;
			reqField = -1;
			max = this.TotalField;
			for ( i=0 ; i<max ; i++ )
			{
				field = this.RetFieldDesc(i);
				content = this.RetFieldInRecord(record, i);

				if ( content.Length > field.max )  return i;

				if ( field.require == FieldRequire.Obligatory )
				{
					if ( content.Length == 0 )  return i;
				}
				if ( field.require == FieldRequire.Or )
				{
					if ( content.Length != 0 )  reqExist ++;
					reqTotal ++;
					if ( reqField == -1 )  reqField = i;
				}

				switch ( field.type )
				{
					case FieldType.Mail:
						if ( !CheckMail(content) )  return i;
						break;
					case FieldType.Web:
						if ( !CheckWeb(content) )  return i;
						break;
					case FieldType.Prix:
						if ( !CheckPrix(content) )  return i;
						break;
					case FieldType.Date:
						if ( !CheckDate(content) )  return i;
						break;
				}
			}

			if ( reqTotal > 0 && reqExist == 0 )  return reqField;

			return -1;  // ok
		}

		protected bool CheckMail(string text)
		{
			//	Vérifie si une chaîne est une adresse e-mail.
			return true;
		}

		protected bool CheckWeb(string text)
		{
			//	Vérifie si une chaîne est une adreese web.
			return true;
		}

		protected bool CheckPrix(string text)
		{
			//	Vérifie si une chaîne est un prix.
			return true;
		}

		protected bool CheckDate(string text)
		{
			//	Vérifie si une chaîne est une date.
			return true;
		}


		public void FlushDataBase()
		{
			//	Efface toutes les fiches de la base.
			this.records.Clear();
			this.inUse = false;
		}

		public int TotalRecord
		{
			//	Retourne le nombre total de fiches.
			get
			{
				return this.records.Count;
			}
		}

		public int CreateRecord(Record record)
		{
			//	Ajoute une nouvelle fiche et retourne son index.
			int i = this.TotalRecord;
			this.records.Add(record);
			this.inUse = true;
			Sort();
			return InternalToSort(i);
		}

		public int SetRecord(int rank, Record record)
		{
			//	Modifie une fiche existante et retourne son index.
			if ( rank < 0 || rank >= this.TotalRecord )  return -1;
			if ( this.index == null || this.index.Length == 0 )  return -1;

			int i = this.index[rank];
			this.records[i] = record;
			this.inUse = true;
			Sort();
			return InternalToSort(i);
		}

		public Record RetRecord(int rank)
		{
			//	Rend une fiche existante.
			if ( rank < 0 || rank >= this.TotalRecord )  return null;
			if ( this.index == null || this.index.Length == 0 )  return null;

			int i = this.index[rank];
			return (Record)this.records[i];
		}

		public bool DeleteRecord(int rank)
		{
			//	Supprime une fiche.
			if ( rank < 0 || rank >= this.TotalRecord )  return false;
			if ( this.index == null || this.index.Length == 0 )  return false;

			int i = this.index[rank];
			this.records.RemoveAt(i);
			this.inUse = true;
			Sort();
			return true;
		}


		public bool SetSortField(int rankSort, int fieldID, Widgets.SortMode mode)
		{
			//	Spécifie l'identificateur d'une rubrique de tri.
			//	Si rankSort=0 -> critère principal
			//	Si rankSort=1 -> critère secondaire (si égalité avec critère principal)
			//	Si renkSort=2 -> etc.
			//	Si mode= 1 -> tri croissant
			//	Si mode=-1 -> tri décroissant
			System.Diagnostics.Debug.Assert( fieldID >= 0 && fieldID < this.TotalField );
			if ( rankSort < 0 || rankSort >= this.maxSort )  return false;
			this.sortField[rankSort] = fieldID;
			this.sortMode[rankSort] = mode;
			Sort();
			return true;
		}

		public bool GetSortField(int rankSort, out int fieldID, out Widgets.SortMode mode)
		{
			//	Retourne l'identificateur d'une rubrique et le mode de tri.
			if ( rankSort < 0 || rankSort >= this.maxSort )
			{
				fieldID = -1;
				mode = 0;
				return false;
			}
			fieldID = this.sortField[rankSort];
			mode = this.sortMode[rankSort];
			return true;
		}

		public void Sort()
		{
			//	Crée l'index de tri.
			int		max, i, ii, len;

			max = this.TotalRecord;
			if ( max == 0 )
			{
				this.index = null;
				return;
			}
			this.index = new int[max];

			if ( this.sortField[0] == -1 )
			{
				for ( i=0 ; i<max ; i++ )
				{
					this.index[i] = i;
				}
				return;
			}

			len = 0;
			for ( i=0 ; i<max ; i++ )
			{
				ii = SearchIndexPlace(i, len);
				InsertIndex(i, ii, len);
				len ++;
			}
		}

		protected void InsertIndex(int i, int ii, int lenIndex)
		{
			//	Insère un rang dans l'index.
			int		j;

			for ( j=lenIndex ; j>ii ; j-- )
			{
				this.index[j] = this.index[j-1];
			}
			this.index[ii] = i;
		}

		protected int SearchIndexPlace(int i, int lenIndex)
		{
			//	Cherche à quelle place insérer un item dans l'index.
			int		ii;

			for ( ii=0 ; ii<lenIndex ; ii++ )
			{
				if ( CompareItem(i, this.index[ii]) < 0 )  return ii;
			}
			return lenIndex;
		}

		protected int CompareItem(int i1, int i2)
		{
			//	Compare deux items.
			Record	r1, r2;
			string	s1, s2;
			int		i, n;

			r1 = (Record)this.records[i1];
			r2 = (Record)this.records[i2];

			for ( i=0 ; i<this.maxSort ; i++ )
			{
				if ( this.sortField[i] == -1 )  break;

				s1 = (string)r1[this.sortField[i]];
				s2 = (string)r2[this.sortField[i]];
				n = string.Compare(s1, s2);
				
				n = (int)n * (int)this.sortMode[i];  // tri croissant ou décroissant
				if ( n != 0 )  return n;
			}
			return 0;
		}

		public int InternalToSort(int i)
		{
			//	Conversion d'un index interne en index trié.
			int		max, ii;

			if ( this.index.Length == 0 )  return -1;

			max = this.TotalRecord;
			for ( ii=0 ; ii<max ; ii++ )
			{
				if ( i == this.index[ii] )  return ii;
			}
			return -1;
		}

		public int SortToInternal(int i)
		{
			//	Conversion d'un index trié en index interne.
			if ( this.index.Length == 0 )  return -1;
			if ( i == -1 )  return -1;
			return this.index[i];
		}


		public bool SearchCritere(ref int rank, out int fieldID, int dir, string crit, out string complete)
		{
			//	Cherche un critère dans toutes les rubriques, à partir de la
			//	rubrique spécifiée par rank (non comprise).
			Record	record;
			int		maxRecord, maxField, i, index;
			string	text, upper;

			crit = crit.ToUpper();
			maxRecord = this.TotalRecord;
			for ( i=0 ; i<maxRecord ; i++ )
			{
				if ( dir > 0 )
				{
					rank ++;
					if ( rank >= maxRecord )  rank = 0;
				}
				else
				{
					rank --;
					if ( rank < 0 )  rank = maxRecord-1;
				}
				record = RetRecord(rank);

				maxField = this.TotalField;
				for ( fieldID=0 ; fieldID<maxField ; fieldID++ )
				{
					text = RetFieldInRecord(record, fieldID);
					upper = text.ToUpper();
					index = IndexSubstring(upper, crit);
					if ( index >= 0 )
					{
						complete = text.Substring(index);
						return true;
					}
				}
			}

			fieldID = -1;
			complete = "";
			return false;
		}

		protected int IndexSubstring(string text, string subText)
		{
			//	Vérifie si une chaîne est contenue dans une autre.
			return text.IndexOf(subText);
		}


		protected bool							inUse = false;
		protected string						title = "";
		protected System.Collections.ArrayList	fields = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	records = new System.Collections.ArrayList();
		protected readonly int					maxSort = 10;
		protected int[]							sortField;
		protected Widgets.SortMode[]			sortMode;
		protected int[]							index;
	}
}
