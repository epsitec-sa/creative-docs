//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataType d�crit un type de donn�e.
	/// </summary>
	public class DataType
	{
		public DataType()
		{
		}
		
		public void Initialise(DataClass data_class, string binder_name)
		{
			this.data_class  = data_class;
			this.binder_name = binder_name;
		}
		
		
		public DataClass						DataClass
		{
			get { return this.data_class; }
		}
		
		public string							BinderName
		{
			get { return this.binder_name; }
		}
		
		
		
		protected string						binder_name;
		protected DataClass						data_class = DataClass.Null;
	}
	
	public enum DataClass
	{
		Unsupported,					//	type non support�
		Null,							//	type pas analysable, donn�e absente
		
		Complex,						//	type complexe (contenant d'autres types)
		
		Decimal,						//	tout ce qui est num�rique (bool�en, entier, r�el, temps, ...)
		String,							//	texte (Unicode)
		Date,							//	date, uniquement
		Time,							//	heure, uniquement
		DateTime,						//	date et heure, 64 bits (r�solution de 1ms ou mieux)
		ByteArray,						//	tableau de bytes
		Guid,							//	identificateur globalement unique, 128 bits
	}
}
