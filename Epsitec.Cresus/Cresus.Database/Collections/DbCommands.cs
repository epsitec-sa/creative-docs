//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Collections
{
	using IDbCommand = System.Data.IDbCommand;
	
	/// <summary>
	/// La classe Collections.DbCommands encapsule une collection d'instances de type IDbCommand.
	/// </summary>
	public class DbCommands : AbstractList
	{
		public DbCommands()
		{
		}
		
		
		public virtual IDbCommand			this[int index]
		{
			get
			{
				return this.List[index] as IDbCommand;
			}
		}
		
		
		public virtual void Add(IDbCommand command)
		{
			this.InternalAdd (command);
		}
		
		public virtual void AddRange(IDbCommand[] commands)
		{
			this.InternalAddRange (commands);
		}
		
		public virtual void Remove(IDbCommand command)
		{
			this.InternalRemove (command);
		}
		
		
		public virtual bool Contains(IDbCommand command)
		{
			return this.List.Contains (command);
		}
		
		public virtual int IndexOf(IDbCommand command)
		{
			return this.List.IndexOf (command);
		}
		
	}
}
