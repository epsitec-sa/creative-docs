//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script
{
	/// <summary>
	/// Summary description for IEditorEngine.
	/// </summary>
	public interface IEditorEngine
	{
		object CreateDocument(ScriptWrapper script);
		void ShowMethod(object document, string name);
	}
}
