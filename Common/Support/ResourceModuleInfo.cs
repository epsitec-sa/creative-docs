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


using System.Collections.Generic;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>ResourceModuleInfo</c> class stores the module identity and
    /// associated properties.
    /// </summary>
    public sealed class ResourceModuleInfo : Epsitec.Common.Types.IReadOnly
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceModuleInfo"/> class.
        /// </summary>
        public ResourceModuleInfo() { }

        /// <summary>
        /// Gets or sets the resource module id for this module.
        /// </summary>
        /// <value>The resource module id.</value>
        public ResourceModuleId FullId
        {
            get { return this.fullId; }
            set
            {
                this.VerifyWritable("FullId");
                this.fullId = value;
            }
        }

        /// <summary>
        /// Gets or sets the path of the reference module.
        /// </summary>
        /// <value>The path of the reference module.</value>
        public string ReferenceModulePath
        {
            get { return this.referenceModulePath; }
            set
            {
                this.VerifyWritable("ReferenceModulePath");
                this.referenceModulePath = value;
            }
        }

        /// <summary>
        /// Gets or sets the patch depth for this module. Zero means that this
        /// is a root reference module, any other value that this module is
        /// the result of a (possibly multi-level) merge.
        /// </summary>
        /// <value>The patch depth.</value>
        public int PatchDepth
        {
            get { return this.patchDepth; }
            set
            {
                this.VerifyWritable("PatchDepth");
                this.patchDepth = value;
            }
        }

        /// <summary>
        /// Gets or sets the source default namespace.
        /// </summary>
        /// <value>The source namespace.</value>
        public string SourceNamespaceDefault
        {
            get { return this.sourceNamespaceDefault; }
            set
            {
                this.VerifyWritable("SourceNamespaceDefault");
                this.sourceNamespaceDefault = value;
            }
        }

        /// <summary>
        /// Gets or sets the source namespace used when generating associated
        /// code ("Res.cs").
        /// </summary>
        /// <value>The source namespace.</value>
        public string SourceNamespaceRes
        {
            get { return this.sourceNamespaceRes ?? this.sourceNamespaceDefault; }
            set
            {
                this.VerifyWritable("SourceNamespaceRes");
                this.sourceNamespaceRes = value;
            }
        }

        /// <summary>
        /// Gets or sets the source namespace used when generating associated
        /// code ("Entities.cs").
        /// </summary>
        /// <value>The source namespace.</value>
        public string SourceNamespaceEntities
        {
            get { return this.sourceNamespaceEntities ?? this.sourceNamespaceDefault; }
            set
            {
                this.VerifyWritable("SourceNamespaceEntities");
                this.sourceNamespaceEntities = value;
            }
        }

        /// <summary>
        /// Gets or sets the source namespace used when generating associated
        /// code ("Forms.cs").
        /// </summary>
        /// <value>The source namespace.</value>
        public string SourceNamespaceForms
        {
            get { return this.sourceNamespaceForms ?? this.sourceNamespaceDefault; }
            set
            {
                this.VerifyWritable("SourceNamespaceForms");
                this.sourceNamespaceForms = value;
            }
        }

        /// <summary>
        /// Gets or sets the assemblies related to this module.
        /// </summary>
        /// <value>
        /// The assemblies.
        /// </value>
        public string Assemblies
        {
            get { return this.assemblies; }
            set
            {
                this.VerifyWritable("Assemblies");
                this.assemblies = value;
            }
        }

        /// <summary>
        /// Gets or sets the text mode used when generating associated code.
        /// </summary>
        /// <value>The text mode.</value>
        public ResourceTextMode TextMode
        {
            get { return this.textMode; }
            set
            {
                this.VerifyWritable("TextMode");
                this.textMode = value;
            }
        }

        /// <summary>
        /// Gets the versions for this module information.
        /// </summary>
        /// <value>A list of versions. The collection might be read only
        /// if this instance was frozen.</value>
        public IList<ResourceModuleVersion> Versions
        {
            get
            {
                if (this.isFrozen)
                {
                    if (this.versions == null)
                    {
                        return Types.Collections.EmptyList<ResourceModuleVersion>.Instance;
                    }
                    else
                    {
                        return new Types.Collections.ReadOnlyList<ResourceModuleVersion>(
                            this.versions
                        );
                    }
                }

                if (this.versions == null)
                {
                    this.versions = new List<ResourceModuleVersion>();
                }

                return this.versions;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has any versions
        /// associated to it.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has any versions; otherwise, <c>false</c>.
        /// </value>
        public bool HasVersions
        {
            get { return (this.versions == null) || (this.versions.Count == 0) ? false : true; }
        }

        /// <summary>
        /// Gets a value indicating whether this is a patch module.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this is a patch module; otherwise, <c>false</c>.
        /// </value>
        public bool IsPatchModule
        {
            get { return string.IsNullOrEmpty(this.referenceModulePath) ? false : true; }
        }

        /// <summary>
        /// Updates the version stored in the versions property. If the version
        /// is newer (the comparison is date based), it will replace the one
        /// already found in the versions property.
        /// </summary>
        /// <param name="version">The version.</param>
        public void UpdateVersion(ResourceModuleVersion version)
        {
            if (this.versions == null)
            {
                this.versions = new List<ResourceModuleVersion>();
                this.versions.Add(version.Clone());
            }
            else
            {
                ResourceModuleVersion existing = this.versions.Find(
                    delegate(ResourceModuleVersion candidate)
                    {
                        return candidate.DeveloperId == version.DeveloperId;
                    }
                );

                if (existing != null)
                {
                    if (version.BuildDate > existing.BuildDate)
                    {
                        this.versions.Remove(existing);
                        this.versions.Add(version.Clone());
                    }
                }
                else
                {
                    this.versions.Add(version.Clone());
                }
            }
        }

        /// <summary>
        /// Updates the version stored in the versions property. If the version
        /// is newer (the comparison is date based), it will replace the one
        /// already found in the versions property.
        /// </summary>
        /// <param name="versions">The versions.</param>
        public void UpdateVersions(IEnumerable<ResourceModuleVersion> versions)
        {
            foreach (ResourceModuleVersion version in versions)
            {
                this.UpdateVersion(version);
            }
        }

        #region Interface IReadOnly

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get { return this.isFrozen; }
        }

        #endregion

        /// <summary>
        /// Freezes this instance. This makes the instance read only. No further
        /// modification will be possible. Any attempt to modify the properties
        /// will throw a <see cref="System.InvalidOperationException"/>.
        /// </summary>
        public void Freeze()
        {
            this.isFrozen = true;
        }

        /// <summary>
        /// Returns a copy of this instance. The copy is modifiable.
        /// </summary>
        /// <returns>The copy of this instance.</returns>
        public ResourceModuleInfo Clone()
        {
            ResourceModuleInfo copy = new ResourceModuleInfo();

            copy.fullId = this.fullId;

            copy.referenceModulePath = this.referenceModulePath;

            copy.sourceNamespaceDefault = this.sourceNamespaceDefault;
            copy.sourceNamespaceRes = this.sourceNamespaceRes;
            copy.sourceNamespaceEntities = this.sourceNamespaceEntities;
            copy.sourceNamespaceForms = this.sourceNamespaceForms;

            copy.versions =
                this.versions == null ? null : new List<ResourceModuleVersion>(this.versions);
            copy.textMode = this.textMode;
            copy.assemblies = this.assemblies;

            return copy;
        }

        private void VerifyWritable(string property)
        {
            if (this.isFrozen)
            {
                throw new System.InvalidOperationException(
                    string.Format("Property {0} is not writable", property)
                );
            }
        }

        private bool isFrozen;
        private ResourceModuleId fullId;
        private string referenceModulePath;

        private string sourceNamespaceDefault;
        private string sourceNamespaceRes;
        private string sourceNamespaceEntities;
        private string sourceNamespaceForms;
        private string assemblies;
        private ResourceTextMode textMode;
        private List<ResourceModuleVersion> versions;
        private int patchDepth;
    }
}
