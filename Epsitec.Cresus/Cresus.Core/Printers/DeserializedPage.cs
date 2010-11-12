//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Printers
{
	public class DeserializedPage
	{
		public DeserializedPage(string title, string printerLogicalName, string printerPhysicalName, string printerPhysicalTray, Size pageSize, int pageRank)
		{
			this.title               = title;
			this.printerLogicalName  = printerLogicalName;
			this.printerPhysicalName = printerPhysicalName;
			this.printerPhysicalTray = printerPhysicalTray;
			this.pageSize            = pageSize;
			this.pageRank            = pageRank;
		}


		public string Title
		{
			get
			{
				return this.title;
			}
		}

		public string PrinterLogicalName
		{
			get
			{
				return this.printerLogicalName;
			}
		}

		public string PrinterPhysicalName
		{
			get
			{
				return this.printerPhysicalName;
			}
		}

		public string PrinterPhysicalTray
		{
			get
			{
				return this.printerPhysicalTray;
			}
		}

		public Size PageSize
		{
			get
			{
				return this.pageSize;
			}
		}

		public int PageRank
		{
			get
			{
				return this.pageRank;
			}
		}

		public string FullDescription
		{
			get
			{
				return string.Format ("● Titre: <b>{0}</b><br/>● Nom logique: {1}<br/>● Imprimante: {2}<br/>● Bac: {3}<br/>● Dimensions: {4}×{5} mm<br/>● N° page: {6}", this.title, this.printerLogicalName, this.printerPhysicalName, this.printerPhysicalTray, this.pageSize.Width, this.pageSize.Height, this.pageRank+1);
			}
		}

		public Bitmap Miniature
		{
			get;
			set;
		}


		private readonly string		title;
		private readonly string		printerLogicalName;
		private readonly string		printerPhysicalName;
		private readonly string		printerPhysicalTray;
		private readonly Size		pageSize;
		private readonly int		pageRank;
	}
}
