//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 23/04/2004

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface INameCaption permet de regrouper sous un m�me toit
	/// les diverses classes qui exportent un nom, un titre et une
	/// �ventuelle description.
	/// </summary>
	public interface INameCaption
	{
		string	Name		{ get; }
		string	Caption		{ get; }
		string	Description	{ get; }
	}
}
