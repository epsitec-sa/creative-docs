//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataType décrit un type de donnée.
	/// </summary>
	public class DataType : IDataAttributesHost
	{
		public DataType() : this (null)
		{
		}
		
		public DataType(string binder_engine)
		{
			this.Initialise (binder_engine);
		}
		
		
		protected void Initialise(string binder_engine)
		{
			this.binder_engine = binder_engine;
		}
		
		
		public string							BinderEngine
		{
			get { return this.binder_engine; }
		}
		
		
		#region IDataAttributesHost Members
		public DataAttributes					DataAttributes
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
		
		public override bool Equals(object obj)
		{
			//	ATTENTION: L'égalité ne prend pas en compte la valeur des attributs.
			
			DataType that = obj as DataType;
			
			if (that == null)
			{
				return false;
			}
			
			return (this.binder_engine == that.binder_engine);
		}
		
		public override int GetHashCode()
		{
			return (this.binder_engine == null) ? 0 : this.binder_engine.GetHashCode ();
		}


		
		
		protected string						binder_engine;
		protected DataAttributes				attributes;
	}
}
