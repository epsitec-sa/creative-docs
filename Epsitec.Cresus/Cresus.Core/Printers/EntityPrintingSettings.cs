//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public class EntityPrintingSettings
	{
		public EntityPrintingSettings()
		{
			this.DocumentTypeSelected = DocumentType.None;

			this.documentOptionsNameSelected = new List<string> ();
		}


		public DocumentType DocumentTypeSelected
		{
			//	Indique le type du document sélectionné.
			get;
			set;
		}

		public List<string> DocumentOptionsSelected
		{
			//	Liste des options cochées.
			get
			{
				return this.documentOptionsNameSelected;
			}
		}

		public bool HasDocumentOption(string name)
		{
			//	Retourne true si une option est cochée.
			return this.documentOptionsNameSelected.Contains (name);
		}


		private readonly List<string>						documentOptionsNameSelected;
	}
}
