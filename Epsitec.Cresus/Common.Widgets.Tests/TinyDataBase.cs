namespace Epsitec.Common
{
	/// <summary>
	/// La classe TinyDataBase impl�mente une base de donn�es tr�s simple.
	/// </summary>
	public class TinyDataBase : System.Object
	{
		public enum FieldType
		{
			None,			// sans type
			String,			// cha�ne
			Tel,			// num�ros de t�l�phone
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
			public FieldRequire				require;	// pr�sence
			public int						rank;		// rang d'�dition
			public Drawing.ContentAlignment	alignment;	// justification
			public int						max;		// nb max de caract�res
			public int						lines;		// nb de lignes
			public bool						link;		// li�e � la suivante
			public int						width;		// largeur dans liste
			public string					combo;		// contenu �ventuel combo
		}

		// Classe d'une fiche.
		public class Record : System.Collections.ArrayList
		{
		}


		// Constructeur.
		public TinyDataBase()
		{
			this.sortField = new int[this.maxSort];
			this.sortMode = new Widgets.SortMode[this.maxSort];
			for ( int i=0 ; i<this.maxSort ; i++ )
			{
				this.sortField[i] = -1;  // pas de tri
				this.sortMode[i] = Widgets.SortMode.None;
			}
		}

		// Titre de la base.
		public string Title
		{
			get
			{
				return this.title;
			}

			set
			{
				this.title = value;
			}
		}


		// Retourne le nombre total de rubriques.
		public int TotalField
		{
			get
			{
				return this.fields.Count;
			}
		}

		// Cr�e une nouvelle d�finition de rubrique vide.
		public void CreateEmptyFieldDesc(out FieldDesc desc)
		{
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

		// Cr�e une nouvelle rubrique et retourne l'identificateur de la rubrique.
		// Une rubrique ne peut jamais �tre d�truite.
		// D�s que la base contient une ou plusieurs fiches, il n'est plus possible
		// d'ajouter des rubriques.
		public int CreateFieldDesc(FieldDesc desc)
		{
			System.Diagnostics.Debug.Assert(!this.inUse);
			return this.fields.Add(desc);
		}

		// Donne les d�finitions d'une rubrique.
		public FieldDesc RetFieldDesc(int fieldID)
		{
			System.Diagnostics.Debug.Assert( fieldID >= 0 && fieldID < this.TotalField );
			return (FieldDesc)this.fields[fieldID];
		}

		// Retourne l'identificateur de rubrique correspondant � un rang donn�.
		public int RetFieldID(int rank)
		{
			for ( int fieldID=0 ; fieldID<this.TotalField ; fieldID++ )
			{
				FieldDesc fieldDesc = RetFieldDesc(fieldID);
				if ( rank == fieldDesc.rank )  return fieldID;
			}
			return -1;
		}


		// Cr�e une fiche vide.
		public void CreateEmptyRecord(out Record record)
		{
			record = new Record();
			for ( int i=0 ; i<this.TotalField ; i++ )
			{
				record.Add("");
			}
		}

		// Cr�e une copie d'une fiche.
		public void CreateCopyRecord(out Record record, Record original)
		{
			record = new Record();
			for ( int i=0 ; i<this.TotalField ; i++ )
			{
				record.Add(original[i]);
			}
		}

		// Met une rubrique dans une fiche.
		public bool SetFieldInRecord(Record record, int fieldID, string text)
		{
			if ( fieldID < 0 || fieldID >= this.TotalField )  return false;

			record[fieldID] = text;
			return true;
		}

		// Rend une rubrique d'une fiche.
		public string RetFieldInRecord(Record record, int fieldID)
		{
			if ( fieldID < 0 || fieldID >= this.TotalField )  return "";
			if ( fieldID >= record.Count )  return "";
			return (string)record[fieldID];
		}

		// V�rifie si une fiche est correcte.
		// Retourne l'identificateur de la rubrique fausse, ou -1 si tout est ok.
		public int CheckRecord(Record record)
		{
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

		// V�rifie si une cha�ne est une adresse e-mail.
		protected bool CheckMail(string text)
		{
			return true;
		}

		// V�rifie si une cha�ne est une adreese web.
		protected bool CheckWeb(string text)
		{
			return true;
		}

		// V�rifie si une cha�ne est un prix.
		protected bool CheckPrix(string text)
		{
			return true;
		}

		// V�rifie si une cha�ne est une date.
		protected bool CheckDate(string text)
		{
			return true;
		}


		// Efface toutes les fiches de la base.
		public void FlushDataBase()
		{
			this.records.Clear();
			this.inUse = false;
		}

		// Retourne le nombre total de fiches.
		public int TotalRecord
		{
			get
			{
				return this.records.Count;
			}
		}

		// Ajoute une nouvelle fiche et retourne son index.
		public int CreateRecord(Record record)
		{
			int i = this.TotalRecord;
			this.records.Add(record);
			this.inUse = true;
			Sort();
			return InternalToSort(i);
		}

		// Modifie une fiche existante et retourne son index.
		public int SetRecord(int rank, Record record)
		{
			if ( rank < 0 || rank >= this.TotalRecord )  return -1;
			if ( this.index == null || this.index.Length == 0 )  return -1;

			int i = this.index[rank];
			this.records[i] = record;
			this.inUse = true;
			Sort();
			return InternalToSort(i);
		}

		// Rend une fiche existante.
		public Record RetRecord(int rank)
		{
			if ( rank < 0 || rank >= this.TotalRecord )  return null;
			if ( this.index == null || this.index.Length == 0 )  return null;

			int i = this.index[rank];
			return (Record)this.records[i];
		}

		// Supprime une fiche.
		public bool DeleteRecord(int rank)
		{
			if ( rank < 0 || rank >= this.TotalRecord )  return false;
			if ( this.index == null || this.index.Length == 0 )  return false;

			int i = this.index[rank];
			this.records.RemoveAt(i);
			this.inUse = true;
			Sort();
			return true;
		}


		// Sp�cifie l'identificateur d'une rubrique de tri.
		// Si rankSort=0 -> crit�re principal
		// Si rankSort=1 -> crit�re secondaire (si �galit� avec crit�re principal)
		// Si renkSort=2 -> etc.
		// Si mode= 1 -> tri croissant
		// Si mode=-1 -> tri d�croissant
		public bool SetSortField(int rankSort, int fieldID, Widgets.SortMode mode)
		{
			System.Diagnostics.Debug.Assert( fieldID >= 0 && fieldID < this.TotalField );
			if ( rankSort < 0 || rankSort >= this.maxSort )  return false;
			this.sortField[rankSort] = fieldID;
			this.sortMode[rankSort] = mode;
			Sort();
			return true;
		}

		// Retourne l'identificateur d'une rubrique et le mode de tri.
		public bool GetSortField(int rankSort, out int fieldID, out Widgets.SortMode mode)
		{
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

		// Cr�e l'index de tri.
		public void Sort()
		{
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

		// Ins�re un rang dans l'index.
		protected void InsertIndex(int i, int ii, int lenIndex)
		{
			int		j;

			for ( j=lenIndex ; j>ii ; j-- )
			{
				this.index[j] = this.index[j-1];
			}
			this.index[ii] = i;
		}

		// Cherche � quelle place ins�rer un item dans l'index.
		protected int SearchIndexPlace(int i, int lenIndex)
		{
			int		ii;

			for ( ii=0 ; ii<lenIndex ; ii++ )
			{
				if ( CompareItem(i, this.index[ii]) < 0 )  return ii;
			}
			return lenIndex;
		}

		// Compare deux items.
		protected int CompareItem(int i1, int i2)
		{
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
				
				n = (int)n * (int)this.sortMode[i];  // tri croissant ou d�croissant
				if ( n != 0 )  return n;
			}
			return 0;
		}

		// Conversion d'un index interne en index tri�.
		public int InternalToSort(int i)
		{
			int		max, ii;

			if ( this.index.Length == 0 )  return -1;

			max = this.TotalRecord;
			for ( ii=0 ; ii<max ; ii++ )
			{
				if ( i == this.index[ii] )  return ii;
			}
			return -1;
		}

		// Conversion d'un index tri� en index interne.
		public int SortToInternal(int i)
		{
			if ( this.index.Length == 0 )  return -1;
			if ( i == -1 )  return -1;
			return this.index[i];
		}


		// Cherche un crit�re dans toutes les rubriques, � partir de la
		// rubrique sp�cifi�e par rank (non comprise).
		public bool SearchCritere(ref int rank, out int fieldID, int dir, string crit, out string complete)
		{
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

		// V�rifie si une cha�ne est contenue dans une autre.
		protected int IndexSubstring(string text, string subText)
		{
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
