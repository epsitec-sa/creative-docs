//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 12/11/2003

using Epsitec.Common.Support;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataRecord sert de base pour DataSet, DataField et DataTable.
	/// </summary>
	public abstract class DataRecord : AbstractRecord, IDataAttributesHost
	{
		protected DataRecord()
		{
		}
		
		
		public DataType							DataType
		{
			get { return this.type; }
		}
		
		
		public string							UserLabel
		{
			get { return this.Attributes.GetAttribute (Tags.Caption, ResourceLevel.Merged); }
		}
		
		public string							UserDescription
		{
			get { return this.Attributes.GetAttribute (Tags.Description, ResourceLevel.Merged); }
		}
		
		
		#region IDataAttributesHost Members
		public DataAttributes					Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new DataAttributes ();
				}
				
				return this.attributes;
			}
		}
		#endregion
		
		public DataRecord FindRecord(string path)
		{
			return this.FindRecord (path, DataVersion.Active);
		}
		
		public virtual DataRecord FindRecord(string path, DataVersion version)
		{
			return null;
		}
		
		
		internal virtual DataRecord RecursiveFindRecord(string path, DataVersion version)
		{
			switch (version)
			{
				case DataVersion.Original:
					
					//	On demande une version originale: il faut donc éviter de
					//	continuer la recherche si on se rend compte que le record
					//	a été rajouté (il ne fait pas partie des données d'origine).
					
					if (this.State != DataState.Added)
					{
						return this.FindRecord (path, version);
					}
					break;
				
				case DataVersion.Active:
					
					//	On demande une version active: il faut donc éviter de
					//	continuer la recherche si on se rend compte que le record
					//	a été supprimé. Dans tous les autres cas, le record est à
					//	prendre en considération.
					
					if ((this.State != DataState.Removed) &&
						(this.State != DataState.Invalid))
					{
						return this.FindRecord (path, version);
					}
					break;
				
				case DataVersion.ActiveOrDead:
					return this.FindRecord (path, version);
			}
			
			return null;
		}
		
		
		
		
		internal virtual void MarkAsModified()
		{
			switch (this.State)
			{
				case DataState.Unchanged:
					this.SetState (DataState.Modified);
					break;
				
				case DataState.Added:
				case DataState.Modified:
					break;
				
				default:
					throw new DataException (string.Format ("Illegal state {0}", this.State.ToString ()));
			}
		}
		
		internal virtual void MarkAsUnchanged()
		{
			switch (this.State)
			{
				case DataState.Unchanged:
					break;
				
				case DataState.Added:
				case DataState.Modified:
					this.SetState (DataState.Unchanged);
					break;
				
				case DataState.Invalid:
				case DataState.Removed:
				default:
					
					//	Ni le data set non initialisé, ni le data set supprimé ne peuvent
					//	être "validés"...
				
					throw new DataException (string.Format ("Illegal state {0}", this.State.ToString ()));
			}
		}
		
		internal virtual void MarkAsAdded()
		{
			switch (this.State)
			{
				case DataState.Unchanged:
				case DataState.Modified:
					this.SetState (DataState.Added);
					break;
				
				case DataState.Added:
					break;
				
				default:
					throw new DataException (string.Format ("Illegal state {0}", this.State.ToString ()));
			}
		}
		
		internal virtual void MarkAsRemoved()
		{
			switch (this.State)
			{
				case DataState.Unchanged:
				case DataState.Modified:
					this.SetState (DataState.Removed);
					break;
				
				case DataState.Added:
					this.SetState (DataState.Invalid);
					break;
				
				default:
					throw new DataException (string.Format ("Illegal state {0}", this.State.ToString ()));
			}
		}
		
		
		
		
		protected static string SplitPath(string path, out string path_remaining)
		{
			System.Diagnostics.Debug.Assert (path != null);
			
			int pos = path.IndexOf ('.');
			
			if (pos < 0)
			{
				path_remaining = null;
				return path;
			}
			
			path_remaining = path.Substring (pos+1);
			return path.Substring (0, pos);
		}
		
		
		
		protected DataType						type;
		protected DataAttributes				attributes;
	}
}
