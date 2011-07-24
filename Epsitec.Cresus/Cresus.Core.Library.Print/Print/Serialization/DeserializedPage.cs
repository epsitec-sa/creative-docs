//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using System.Xml.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Print.Serialization
{
	public class DeserializedPage
	{
		public DeserializedPage(DeserializedSection parentSection, int pageRank, XElement xRoot)
		{
			this.parentSection = parentSection;
			this.pageRank      = pageRank;
			this.xRoot         = xRoot;

			this.IsPrintable = true;
		}

		public DeserializedPage(FormattedText error)
		{
			this.error = error;

			this.IsPrintable = false;
		}


		public Size ParentSectionPageSize
		{
			get
			{
				if (this.IsOK)
				{
					return this.parentSection.PageSize;
				}
				else
				{
					return new Size (210, 210);  // format carré arbitraire permettant de montrer une erreur
				}
			}
		}

		public int PageRank
		{
			get
			{
				return this.pageRank;
			}
		}

		public bool IsPrintable
		{
			get;
			set;
		}

		public XElement XRoot
		{
			get
			{
				return this.xRoot;
			}
		}


		public bool IsOK
		{
			get
			{
				return this.error.IsNullOrEmpty;
			}
		}

		public bool IsError
		{
			get
			{
				return !this.error.IsNullOrEmpty;
			}
		}

		public FormattedText Error
		{
			get
			{
				return this.error;
			}
		}


		public string ShortDescription
		{
			get
			{
				if (this.IsOK)
				{
					return string.Format ("<b>{0}</b> ({1} {2}×{3})", this.parentSection.ParentJob.JobFullName, this.parentSection.DocumentPrintingUnitName, this.parentSection.PageSize.Width, this.parentSection.PageSize.Height);
				}
				else
				{
					return "Erreur";
				}
			}
		}

		public string FullDescription
		{
			get
			{
				if (this.IsOK)
				{
					string s1 = string.Concat ("● Titre: <b>",           this.parentSection.ParentJob.JobFullName, "</b>");
					string s2 = string.Concat ("● Unité d'impression: ", this.parentSection.DocumentPrintingUnitName);
					string s3 = string.Concat ("● Imprimante: ",         this.parentSection.ParentJob.PrinterPhysicalName);
					string s4 = string.Concat ("● Bac: ",                this.parentSection.PrinterPhysicalTray);
					string s5 = string.Concat ("● Dimensions: ",         this.parentSection.PageSize.Width, "×", this.parentSection.PageSize.Height, " mm");
					string s6 = string.Concat ("● Numéro de la page: ",  this.pageRank+1);

					return string.Join ("<br/>", s1, s2, s3, s4, s5, s6);
				}
				else
				{
					return "Erreur";
				}
			}
		}

		public Bitmap Miniature
		{
			get;
			set;
		}


		private readonly DeserializedSection	parentSection;
		private readonly int					pageRank;
		private readonly XElement				xRoot;
		private readonly FormattedText			error;
	}
}
