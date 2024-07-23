/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


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
