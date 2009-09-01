//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.DynamicData
{
	/// <summary>
	/// La classe DynamicFieldCollection définit quels champs d'une table
	/// sont calculés de manière dynamique.
	/// </summary>
	public class DynamicFieldCollection : System.Collections.ICollection, Types.INotifyChanged
	{
		public DynamicFieldCollection()
		{
			this.list = new System.Collections.ArrayList ();
		}
		
		
		public IDynamicField					this[int index]
		{
			get
			{
				return this.list[index] as IDynamicField;
			}
		}
		
		
		public void Add(IDynamicField field)
		{
			if (this.list.Contains (field))
			{
				throw new System.InvalidOperationException ("Field already in collection.");
			}
			
			this.list.Add (field);
			this.OnChanged ();
		}
		
		public void Clear()
		{
			if (this.list.Count > 0)
			{
				this.list.Clear ();
				this.OnChanged ();
			}
		}
		
		
		public IDynamicField FindDynamicField(System.Data.DataTable table, int rowIndex, int columnIndex)
		{
			System.Data.DataRow    row    = table.Rows[rowIndex];
			System.Data.DataColumn column = table.Columns[columnIndex];
			
			//	TODO: gérer un cache
			
			foreach (IDynamicField field in this.list)
			{
				if (field.Match (row, column))
				{
					return field;
				}
			}
			
			return null;
		}
		
		public CellIndex[] FindDynamicCells(System.Data.DataTable table)
		{
			//	Construit et retourne la table contenant les index des cellules pour
			//	lesquelles il y a des champs dynamiques.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			FieldMatchResult[] matchCols = this.AnalyseColumns (table);
			FieldMatchResult[] matchRows = this.AnalyseRows (table);
			
			int[] analyseRows = this.FindPossibleIndexes (matchRows);
			int[] analyseCols = this.FindPossibleIndexes (matchCols);
			
			bool refresh;
			
			foreach (int ir in analyseRows)
			{
				switch (matchRows[ir])
				{
					case FieldMatchResult.One:
						
						//	Il y a juste un champ dans cette ligne; on peut donc s'arrêter dès que l'on
						//	a trouvé une colonne avec un champ dynamique :
						
						this.FindDynamicCellsRow (table, list, ir, ref analyseCols, matchCols, 1);
						break;
					
					case FieldMatchResult.Some:
						
						//	Il y a 0..n champs dans cette ligne; il faut donc vérifier toutes les
						//	colonnes, une à une :
						
						this.FindDynamicCellsRow (table, list, ir, ref analyseCols, matchCols, analyseCols.Length);
						break;
					
					case FieldMatchResult.All:
						
						//	Toutes les colonnes de cette ligne contiennent des champs dynamiques; on
						//	peut donc toutes les inclure dans la liste.
						
						System.Diagnostics.Debug.Assert (analyseCols.Length == matchCols.Length);
						
						refresh = false;
						
						for (int ic = 0; ic < matchCols.Length; ic++)
						{
							list.Add (new CellIndex (ir, ic));
							
							System.Diagnostics.Debug.Assert (analyseCols[ic] == ic);
							
							//	Si une colonne spécifiait un champ dans une unique ligne, alors on
							//	pourra supprimer la colonne en question pour ne pas la reconsidérer
							//	ultérieurement :
							
							if (matchCols[ic] == FieldMatchResult.One)
							{
								matchCols[ic] = FieldMatchResult.Zero;
								refresh = true;
							}
						}
						
						if (refresh)
						{
							analyseCols = this.FindPossibleIndexes (matchCols);
						}
						break;
				}
			}
			
			CellIndex[] cells = new CellIndex[list.Count];
			list.CopyTo (cells);
			
			return cells;
		}
		
		
		public FieldMatchResult[] AnalyseColumns(System.Data.DataTable table)
		{
			System.Data.DataColumnCollection columns = table.Columns;
			
			int n = columns.Count;
			
			FieldMatchResult[] results = new FieldMatchResult[n];
			
			foreach (IDynamicField field in this.list)
			{
				for (int i = 0; i < n; i++)
				{
					if (results[i] != FieldMatchResult.All)
					{
						results[i] = DynamicFieldCollection.Combine (results[i], field.Match (columns[i]));
					}
				}
			}
			
			return results;
		}
		
		public FieldMatchResult[] AnalyseRows(System.Data.DataTable table)
		{
			System.Data.DataRowCollection rows = table.Rows;
			
			int n = rows.Count;
			
			FieldMatchResult[] results = new FieldMatchResult[n];
			
			foreach (IDynamicField field in this.list)
			{
				for (int i = 0; i < n; i++)
				{
					if (results[i] != FieldMatchResult.All)
					{
						results[i] = DynamicFieldCollection.Combine (results[i], field.Match (rows[i]));
					}
				}
			}
			
			return results;
		}
		
		
		public static DynamicFieldCollection GetDynamicFiels(System.Data.DataTable table)
		{
			return table.ExtendedProperties[DynamicFieldCollection.DynamicFieldsName] as DynamicFieldCollection;
		}
		
		public static void SetDynamicFiels(System.Data.DataTable table, DynamicFieldCollection dynamicFields)
		{
			if (dynamicFields == null)
			{
				if (table.ExtendedProperties.Contains (DynamicFieldCollection.DynamicFieldsName))
				{
					table.ExtendedProperties.Remove (DynamicFieldCollection.DynamicFieldsName);
				}
			}
			else
			{
				table.ExtendedProperties[DynamicFieldCollection.DynamicFieldsName] = dynamicFields;
			}
		}
		
		public static FieldMatchResult Combine(FieldMatchResult a, FieldMatchResult b)
		{
			switch (a)
			{
				case FieldMatchResult.Zero:	return b;
				case FieldMatchResult.Some:	return (b == FieldMatchResult.All) ? FieldMatchResult.All : FieldMatchResult.Some;
				case FieldMatchResult.All:	return FieldMatchResult.All;
				
				case FieldMatchResult.One:
					switch (b)
					{
						case FieldMatchResult.Zero:	return FieldMatchResult.One;
						case FieldMatchResult.One:	return FieldMatchResult.Some;
						case FieldMatchResult.Some:	return FieldMatchResult.Some;
						case FieldMatchResult.All:	return FieldMatchResult.All;
					}
					break;
			}
			
			throw new System.ArgumentException (string.Format ("Cannot combine {0} and {1}.", a, b));
		}
		
		
		#region ICollection Members
		public int								Count
		{
			get
			{
				return this.list.Count;
			}
		}
		
		public bool								IsSynchronized
		{
			get
			{
				return false;
			}
		}
		
		public object							SyncRoot
		{
			get
			{
				return null;
			}
		}
		
		public void CopyTo(System.Array array, int index)
		{
			this.list.CopyTo (array, index);
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion
		
		private void FindDynamicCellsRow(System.Data.DataTable table, System.Collections.ArrayList list, int ir, ref int[] analyseCols, FieldMatchResult[] matchCols, int maxMatch)
		{
			//	Passe en revue toutes les colonnes qui sont susceptibles de contenir un
			//	champ dynamique; en fonction du type de match pour la colonne en cours,
			//	on peut simplifier la recherche.
			
			//	Analyse au plus maxMatch colonnes.
			
			int n = analyseCols.Length;
			
			for (int c = 0; (c < n) && (maxMatch > 0); c++)
			{
				int ic = analyseCols[c];
				
				FieldMatchResult match = matchCols[ic];
				
				if (match == FieldMatchResult.One)
				{
					if (this.FindDynamicField (table, ir, ic) != null)
					{
						//	Trouvé une cellule contenant un champ dynamique.
						
						list.Add (new CellIndex (ir, ic));
						maxMatch--;
						
						//	La colonne contient exactement un champ et on vient de le trouver;
						//	on peut donc supprimer la colonne et éviter des analyses ultérieures
						//	inutiles de celle-ci.
						
						int[] fixCols = new int[analyseCols.Length-1];
						
						System.Array.Copy (analyseCols, 0,   fixCols, 0, c);
						System.Array.Copy (analyseCols, c+1, fixCols, c, n-c-1);
						
						analyseCols = fixCols;
						
						//	Reprend là où on en était resté (comme on a enlevé la définition de
						//	la colonne courante) :
						
						n -= 1;
						c -= 1;
					}
				}
				else if (match == FieldMatchResult.All)
				{
					//	Toutes les lignes de la colonne contiennent des champs, on n'a pas
					//	besoin de vérifier si cette ligne particulière correspond.
					
					list.Add (new CellIndex (ir, ic));
					maxMatch--;
				}
				else
				{
					System.Diagnostics.Debug.Assert (match == FieldMatchResult.Some);
					
					//	Vérifions si la combinaison ligne/colonne correspond bel et bien à
					//	une cellule contenant un champ dynamique :
					
					if (this.FindDynamicField (table, ir, ic) != null)
					{
						list.Add (new CellIndex (ir, ic));
						maxMatch--;
					}
				}
			}
		}
		
		private int[] FindPossibleIndexes(FieldMatchResult[] results)
		{
			int n = 0;
			
			for (int i = 0; i < results.Length; i++)
			{
				if (results[i] != FieldMatchResult.Zero)
				{
					n++;
				}
			}
			
			int[] array = new int[n];
			n = 0;
			
			for (int i = 0; i < results.Length; i++)
			{
				if (results[i] != FieldMatchResult.Zero)
				{
					array[n++] = i;
				}
			}
			
			return array;
		}
		
		
		protected void InvalidateCache()
		{
			//	TODO: gérer un cache
		}
		
		protected virtual void OnChanged()
		{
			this.InvalidateCache ();
			
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		
		#region INotifyChanged Members
		public event Support.EventHandler		Changed;
		#endregion
		
		private const string					DynamicFieldsName = "DynamicFields";
		private System.Collections.ArrayList	list;
	}
}
