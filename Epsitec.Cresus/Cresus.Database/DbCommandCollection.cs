//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 01/12/2003

namespace Epsitec.Cresus.Database
{
	using IDbCommand = System.Data.IDbCommand;
	
	/// <summary>
	/// La classe IDbCommandCollection encapsule une collection d'instances de type IDbCommand.
	/// </summary>
	public class DbCommandCollection : InternalCollectionList
	{
		public DbCommandCollection()
		{
		}
		
		
		public virtual void Add(IDbCommand command)
		{
			this.List.Add (command);
		}

		public virtual void AddRange(IDbCommand[] commands)
		{
			if (commands == null)
			{
				return;
			}
			
			this.List.AddRange (commands);
		}
		
		public virtual void Remove(IDbCommand command)
		{
			this.List.Remove (command);
		}
		
		
		public virtual bool Contains(IDbCommand command)
		{
			return this.List.Contains (command);
		}
		
		public virtual int IndexOf(IDbCommand command)
		{
			return this.List.IndexOf (command);
		}
		
		
		
		public virtual IDbCommand			this[int index]
		{
			get
			{
				return this.List[index] as IDbCommand;
			}
		}
	}
}
