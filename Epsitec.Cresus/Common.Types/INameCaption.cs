//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface INameCaption permet de regrouper sous un m�me toit
	/// les diverses classes qui exportent un nom, un titre et une
	/// �ventuelle description.
	/// </summary>
	public interface INameCaption : IName
	{
//		string	Name		{ get; }
		string	Caption		{ get; }
		string	Description	{ get; }
	}
}
