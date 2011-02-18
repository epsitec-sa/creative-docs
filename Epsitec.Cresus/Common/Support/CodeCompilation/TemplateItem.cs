//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.CodeCompilation
{
	/// <summary>
	/// The <c>TemplateItem</c> enumeration lists all items which can be
	/// customized in the project file template.
	/// </summary>
	public enum TemplateItem
	{
		/// <summary>
		/// The project GUID.
		/// </summary>
		ProjectGuid,
		
		/// <summary>
		/// The root namespace for the project (optional).
		/// </summary>
		RootNamespace,
		
		/// <summary>
		/// The output assembly name (without extension).
		/// </summary>
		AssemblyName,
		
		/// <summary>
		/// The debug build output directory (usually "bin").
		/// </summary>
		DebugOutputDirectory,
		
		/// <summary>
		/// The debug build temporary directory (usually "obj").
		/// </summary>
		DebugTemporaryDirectory,
		
		/// <summary>
		/// The debug build output XML documentation file name (optional).
		/// </summary>
		DebugDocumentationFile,
		
		/// <summary>
		/// The release build output directory (usually "bin").
		/// </summary>
		ReleaseOutputDirectory,
		
		/// <summary>
		/// The release build temporary directory (usually "obj").
		/// </summary>
		ReleaseTemporaryDirectory,
		
		/// <summary>
		/// The release build output XML documentation file name (optional).
		/// </summary>
		ReleaseDocumentationFile,
		
		/// <summary>
		/// The reference node list.
		/// </summary>
		ReferenceInsertionPoint,
		
		/// <summary>
		/// The compile node list.
		/// </summary>
		CompileInsertionPoint,
	}
}