//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataType décrit un type de donnée.
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
		Unsupported,					//	type non supporté
		Null,							//	type pas analysable, donnée absente
		
		Complex,						//	type complexe (contenant d'autres types)
		
		Decimal,						//	tout ce qui est numérique (booléen, entier, réel, temps, ...)
		String,							//	texte (Unicode)
		Date,							//	date, uniquement
		Time,							//	heure, uniquement
		DateTime,						//	date et heure, 64 bits (résolution de 1ms ou mieux)
		ByteArray,						//	tableau de bytes
		Guid,							//	identificateur globalement unique, 128 bits
	}
}
