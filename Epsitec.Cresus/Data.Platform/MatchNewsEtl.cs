//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Epsitec.Data.Platform
{
	public sealed class MatchNewsEtl
	{

		/// <summary>
		/// Perform ETL job on Mat[CH]news CSV file
		/// based on a source line taken from MAT[CH]street Switzerland light.
		/// </summary>
		/// <param name="filePath">Mat[CH]sort CSV file path</param>
		private MatchNewsEtl()
		{
			
			this.places = new Dictionary<string,string[]>();
			this.streets = new List<MatchNEW_STR>();
			this.houses = new Dictionary<string, string[]> ();

			//Parse CSV and populate dictionnary
			foreach (var lineFields in File.ReadLines (this.filePath).Select (l => l.Split (';')))
			{
				switch (lineFields[0])
				{
					case "00":
						this.fileVersion = lineFields[1];
						break;

					case "01":
						this.places.Add (lineFields[1], lineFields.Skip (2).Take (14).ToArray ());
						break;

					case "04":
						this.streets.Add (new MatchNEW_STR (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6], lineFields[7], lineFields[8], lineFields[9], lineFields[10], lineFields[11]));
						break;

					case "06":
						this.houses.Add (lineFields[1], lineFields.Skip (2).Take (6).ToArray ());
						break;

				}
			}
		}

		public static readonly MatchNewsEtl Current = new MatchNewsEtl ();

		public IList<MatchNEW_STR> Streets
		{
			get
			{
				return this.streets.AsReadOnly ();
			}
		}


		private readonly string filePath = @"s:/MAT[CH]news.csv";
		private readonly string fileVersion;
		private readonly Dictionary<string,string[]> places;
		private readonly List<MatchNEW_STR> streets;
		private readonly Dictionary<string,string[]> houses;
	}

	public sealed class MatchNEW_STR
	{

		public MatchNEW_STR(string c1, string c2, string c3, string c4, string c5, string c6, string c7, string c8, string c9, string c10, string c11)
		{
			this.STR_ID = c1;
			this.ONRP = c2;
			this.STR_BEZ_K = c3;
			this.STR_BEZ_L = c4;
			this.STR_BEZ_2K= c5;
			this.STR_BEZ_2L= c6;
			this.STR_LOK_TYP = c7;
			this.STR_BEZ_SPC = c8;
			this.STR_BEZ_COFF = c9;
			this.STR_GANZFACH = c10;
			this.STR_FACH_ONRP = c11;
		}

		public readonly string		STR_ID;
		public readonly string		ONRP;
		public readonly string		STR_BEZ_K;
		public readonly string		STR_BEZ_L;
		public readonly string		STR_BEZ_2K;
		public readonly string		STR_BEZ_2L;
		public readonly string		STR_LOK_TYP;
		public readonly string		STR_BEZ_SPC;
		public readonly string		STR_BEZ_COFF;
		public readonly string		STR_GANZFACH;
		public readonly string		STR_FACH_ONRP;
	}

}
