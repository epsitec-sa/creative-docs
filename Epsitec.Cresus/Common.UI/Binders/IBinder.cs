//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Binders
{
	/// <summary>
	/// IBinder spécifie l'interface d'un "binder", élément qui permet de faire le
	/// lien entre une source de données et des consommateurs (par exemple, des
	/// adaptateurs).
	/// </summary>
	public interface IBinder
	{
		Adapters.IAdapter	Adapter		{ get; set; }
		string				Caption		{ get; }
		bool				IsValid		{ get; }
		
		bool ReadData(out object data);
		bool WriteData(object data);
		bool EqualsData(object data);
		
//		bool NotifyInvalidData();
		
		System.Type GetDataType();
	}
}
