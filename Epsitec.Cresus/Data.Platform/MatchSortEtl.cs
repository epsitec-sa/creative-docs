//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Epsitec.Data.Platform
{
	public sealed class MatchSortEtl
	{

		/// <summary>
		/// Perform ETL job on Mat[CH]sort CSV file
		/// </summary>
		private MatchSortEtl()
		{
            this.placesNames = new Dictionary<CompositeKey<string, string>, NEW_PLZ1>();
            this.placesNamesAltLang = new Dictionary<string, NEW_PLZ2>();
            this.municipalities = new Dictionary<string, NEW_COM>();
            this.streetNames = new Dictionary<CompositeKey<string,string>,NEW_STR>();
            this.streetNamesAltLang = new Dictionary<CompositeKey<string, string>, NEW_STRA>();
            this.houseNames = new Dictionary<CompositeKey<string, string>, NEW_GEB>();
            this.houseNamesAltLang = new Dictionary<CompositeKey<string, string>, NEW_GEBA>();
            this.deliveryInformations = new Dictionary<string, NEW_BOT_B>();

			//Parse CSV and extract line fields
			foreach (var lineFields in File.ReadLines (this.filePath).Select (l => l.Split (';')))
			{
                //Map CSV line with the "Recordart"
				switch (lineFields[0])
				{
					case "00":
                        this.header = new NEW_HEA(lineFields[1],lineFields[2]);
						break;

					case "01":
						var compKey = new CompositeKey<string,string>();
                        compKey.PK = lineFields[1];
                        compKey.FK = lineFields[2];
                        this.placesNames.Add(compKey, new NEW_PLZ1 (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6], lineFields[7], lineFields[8], lineFields[9], lineFields[10], lineFields[11], lineFields[12], lineFields[13], lineFields[14], lineFields[15]));
						break;

                    case "02":
                        this.placesNamesAltLang.Add(lineFields[1], new NEW_PLZ2 (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6]));
                        break;

                    case "03":
                        this.municipalities.Add(lineFields[1], new NEW_COM (lineFields[1], lineFields[2], lineFields[3], lineFields[4]));
                        break;

					case "04":
                        compKey = new CompositeKey<string,string>();
                        compKey.PK = lineFields[1];
                        compKey.FK = lineFields[2];
						this.streetNames.Add (compKey,new NEW_STR (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6], lineFields[7], lineFields[8], lineFields[9], lineFields[10], lineFields[11]));
						break;

                    case "05":
                        compKey = new CompositeKey<string, string>();
                        compKey.PK = lineFields[1];
                        compKey.FK = lineFields[2];
                        this.streetNamesAltLang.Add(compKey, new NEW_STRA (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6], lineFields[7], lineFields[8], lineFields[9]));
                        break;

					case "06":
						compKey = new CompositeKey<string, string>();
                        compKey.PK = lineFields[1];
                        compKey.FK = lineFields[2];
                        this.houseNames.Add(compKey, new NEW_GEB (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6], lineFields[7]));
						break;

                    case "07":
                        compKey = new CompositeKey<string, string>();
                        compKey.PK = lineFields[1];
                        compKey.FK = lineFields[2];
                        this.houseNamesAltLang.Add(compKey, new NEW_GEBA(lineFields[1], lineFields[2], lineFields[3], lineFields[4]));
                        break;

                    case "08":
                        this.deliveryInformations.Add(lineFields[1], new NEW_BOT_B(lineFields[1], lineFields[2], lineFields[3], lineFields[4],lineFields[5], lineFields[6], lineFields[7]));
                        break;
				}
			}
		}

		public static readonly MatchSortEtl Current = new MatchSortEtl ();
        
        /// <summary>
        /// Street Aggregate with alternative names
        /// </summary>
		public IEnumerable<dynamic> Streets
		{
			get
			{
                var query = from s in this.streetNames
                            join a in this.streetNamesAltLang on s.Key.PK equals a.Key.FK
                            select new
                            {
                                publishedStreetNameAbbreviated = s.Value.STR_BEZ_K,
                                publishedStreetNameAbbreviatedAlternative = a.Value.STR_BEZ_AK,
                                publishedStreetName = s.Value.STR_BEZ_L,
                                publishedStreetNameAlternative = a.Value.STR_BEZ_AL,
                                streetNameAbbreviated = s.Value.STR_BEZ_2K,
                                streetNameAbbreviatedAlternative =  a.Value.STR_BEZ_A2K,
                                streetName = s.Value.STR_BEZ_2L,
                                streetNameAlternative = a.Value.STR_BEZ_A2L,
                                streetType = s.Value.STR_LOK_TYP,
                                streetLanguage = s.Value.STR_BEZ_SPC,
                                isOfficialDesignation = s.Value.STR_BEZ_COFF
                            };

                return query;
			}
		}
        /// <summary>
        /// Places aggregates with Zip codes, city addresses lines and alternative names
        /// </summary>
        public IEnumerable<dynamic> Places
        {
            get
            {
                var query = from p in this.placesNames
                            join a in this.placesNamesAltLang on p.Key.PK equals a.Key
                            join m in this.municipalities on p.Key.PK equals m.Key
                            select new
                            {
                                bfsNumber = m.Value.BFSNR,
                                officialCommunityName = m.Value.GEMEINDENAME,
                                cantonAbbreviation = m.Value.KANTON,
                                agglomerationNumber = m.Value.AGGLONR,
                                zipType = p.Value.PLZ_TYP,                              
                                zip = p.Value.PLZ,
                                zipExtraDigit = p.Value.PLZ_ZZ,
                                rootZip = p.Value.GPLZ,
                                cityLine18 = p.Value.ORT_BEZ_18,
                                cityLine27 = p.Value.ORT_BEZ_27,
                                cityLineAlternativeType = a.Value.BEZ_TYP,
                                cityLine18Alternative = a.Value.ORT_BEZ_18,
                                cityLine27Alternative = a.Value.ORT_BEZ_27,
                                officialLicensePlate = p.Value.KANTON,
                                primaryLanguage = p.Value.SPRACHCODE,
                                primaryLanguageAlternative = a.Value.SPRACHCODE,
                                secondLanguage = p.Value.SPRACHCODE_ABW,
                                briefsBy = p.Value.BRIEFZ_DURCH,
                                validFromDate = p.Value.GILT_AB_DAT,
                                barCodeLabel =p.Value.PLZ_BRIEFZUST,
                                isOfficialZip = p.Value.PLZ_COFF

                            };

                return query;

            }
        }

        /// <summary>
        /// Houses aggregate with house numbers, alternative description and messanger delivery numbers
        /// </summary>
        public IEnumerable<dynamic> Houses
        {
            get
            {
                var query = from h in this.houseNames
                            join a in this.houseNamesAltLang on h.Key.PK equals a.Key.FK
                            join d in this.deliveryInformations on h.Key.PK equals d.Key
                            select new 
                            {
                                houseNumber = h.Value.HNR,
                                houseNumberAlphaNum = h.Value.HNR_A,
                                isOfficialHouseNumber = h.Value.HNR_COFF,
                                additionalDescription = a.Value.GEB_BEZ_ALT,
                                houseAlternativeType = a.Value.GEB_TYP,
                                zipDistrictMessenger  = d.Value.BBZ_PLZ,
                                messengerDistrictNumber = d.Value.BOTEN_BEZ,
                                stageNumber = d.Value.ETAPPEN_NR,
                                runningNumber = d.Value.LAUF_NR,
                                depotNumber = d.Value.NDEPOT
                            };

                return query;
            }
        }

        /// <summary>
        /// Fully aggregated data model's
        /// </summary>
        public IEnumerable<dynamic> PlacesStreetsAndHouses
        {
            get
            {

                var query = from p in this.placesNames
                            join pa in this.placesNamesAltLang on p.Key.PK equals pa.Key
                            join m in this.municipalities on p.Key.PK equals m.Key
                            join s in this.streetNames on p.Key.PK equals s.Key.FK
                            join sa in this.streetNamesAltLang on s.Key.PK equals sa.Key.FK
                            join h in this.houseNames on s.Key.PK equals h.Key.FK
                            join ha in this.houseNamesAltLang on h.Key.PK equals ha.Key.FK
                            join d in this.deliveryInformations on h.Key.PK equals d.Key                         
                            select new
                            {
                                bfsNumber = m.Value.BFSNR,
                                officialCommunityName = m.Value.GEMEINDENAME,
                                cantonAbbreviation = m.Value.KANTON,
                                agglomerationNumber = m.Value.AGGLONR,
                                zipType = p.Value.PLZ_TYP,
                                zip = p.Value.PLZ,
                                zipExtraDigit = p.Value.PLZ_ZZ,
                                rootZip = p.Value.GPLZ,
                                cityLine18 = p.Value.ORT_BEZ_18,
                                cityLine27 = p.Value.ORT_BEZ_27,
                                cityLineAlternativeType = pa.Value.BEZ_TYP,
                                cityLine18Alternative = pa.Value.ORT_BEZ_18,
                                cityLine27Alternative = pa.Value.ORT_BEZ_27,
                                officialLicensePlate = p.Value.KANTON,
                                primaryLanguage = p.Value.SPRACHCODE,
                                primaryLanguageAlternative = pa.Value.SPRACHCODE,
                                secondLanguage = p.Value.SPRACHCODE_ABW,
                                briefsBy = p.Value.BRIEFZ_DURCH,
                                validFromDate = p.Value.GILT_AB_DAT,
                                barCodeLabel = p.Value.PLZ_BRIEFZUST,
                                isOfficialZip = p.Value.PLZ_COFF,

                                publishedStreetNameAbbreviated = s.Value.STR_BEZ_K,
                                publishedStreetNameAbbreviatedAlternative = sa.Value.STR_BEZ_AK,
                                publishedStreetName = s.Value.STR_BEZ_L,
                                publishedStreetNameAlternative = sa.Value.STR_BEZ_AL,
                                streetNameAbbreviated = s.Value.STR_BEZ_2K,
                                streetNameAbbreviatedAlternative =  sa.Value.STR_BEZ_A2K,
                                streetName = s.Value.STR_BEZ_2L,
                                streetNameAlternative = sa.Value.STR_BEZ_A2L,
                                streetType = s.Value.STR_LOK_TYP,
                                streetLanguage = s.Value.STR_BEZ_SPC,
                                isOfficialDesignation = s.Value.STR_BEZ_COFF,

                                houseNumber = h.Value.HNR,
                                houseNumberAlphaNum = h.Value.HNR_A,
                                isOfficialHouseNumber = h.Value.HNR_COFF,
                                additionalDescription = ha.Value.GEB_BEZ_ALT,
                                houseAlternativeType = ha.Value.GEB_TYP,
                                zipDistrictMessenger = d.Value.BBZ_PLZ,
                                messengerDistrictNumber = d.Value.BOTEN_BEZ,
                                stageNumber = d.Value.ETAPPEN_NR,
                                runningNumber = d.Value.LAUF_NR,
                                depotNumber = d.Value.NDEPOT

                            };

                return query;
            }
            
        }

		private readonly string filePath = @"s:/MAT[CH]news.csv";
        private readonly NEW_HEA header;
        private readonly Dictionary<CompositeKey<string, string>, NEW_PLZ1> placesNames;
        private readonly Dictionary<string, NEW_PLZ2> placesNamesAltLang;
        private readonly Dictionary<string, NEW_COM>  municipalities;
        private readonly Dictionary<CompositeKey<string,string>,NEW_STR> streetNames;
        private readonly Dictionary<CompositeKey<string, string>, NEW_STRA> streetNamesAltLang;
        private readonly Dictionary<CompositeKey<string, string>, NEW_GEB> houseNames;
        private readonly Dictionary<CompositeKey<string, string>, NEW_GEBA> houseNamesAltLang;
        private readonly Dictionary<string, NEW_BOT_B> deliveryInformations;
	}

    


    #region MAT[CH] DATA MODEL
    struct CompositeKey<T1, T2>
    {
        public T1 PK;
        public T2 FK;
    }

    /// <summary>
    /// Contains the version date and a unique random code
    /// </summary>
    public sealed class NEW_HEA
    {
        public NEW_HEA(string c1, string c2)
        {
            this.VDAT = c1;
            this.ZCODE = c2;
        }

        public readonly string VDAT;
        public readonly string ZCODE;
    }


    /// <summary>
    ///  Contains all valid zip codes for addressing the Switzerland and the Principality of Liechtenstein.
    /// </summary>
    public sealed class NEW_PLZ1
    {

        public NEW_PLZ1(string c1, string c2, string c3, string c4, string c5, string c6, string c7, string c8, string c9, string c10, string c11,string c12,string c13,string c14,string c15)
        {
            this.ONRP = c1;
            this.BFSNR = c2;
            this.PLZ_TYP = c3;
            this.PLZ = c4;
            this.PLZ_ZZ = c5;
            this.GPLZ = c6;
            this.ORT_BEZ_18 = c7;
            this.ORT_BEZ_27 = c8;
            this.KANTON = c9;
            this.SPRACHCODE = c10;
            this.SPRACHCODE_ABW = c11;
            this.BRIEFZ_DURCH = c12;
            this.GILT_AB_DAT = c13;
            this.PLZ_BRIEFZUST = c14;
            this.PLZ_COFF = c15;
        }
    
        public readonly string ONRP;
        public readonly string BFSNR;
        public readonly string PLZ_TYP;
        public readonly string PLZ;
        public readonly string PLZ_ZZ;
        public readonly string GPLZ;
        public readonly string ORT_BEZ_18;
        public readonly string ORT_BEZ_27;
        public readonly string KANTON;
        public readonly string SPRACHCODE;
        public readonly string SPRACHCODE_ABW;
        public readonly string BRIEFZ_DURCH;
        public readonly string GILT_AB_DAT;
        public readonly string PLZ_BRIEFZUST;
        public readonly string PLZ_COFF;
    }

    /// <summary>
    /// Contains alternate place names and Area designations for each postcode
    /// </summary>
    public sealed class NEW_PLZ2
    {

        public NEW_PLZ2(string c1, string c2, string c3, string c4, string c5, string c6)
        {
            this.ONRP = c1;
            this.LAUFNUMMER = c2;
            this.BEZ_TYP = c3;
            this.SPRACHCODE = c4;
            this.ORT_BEZ_18 = c5;
            this.ORT_BEZ_27 = c6;         
        }

        public readonly string ONRP;
        public readonly string LAUFNUMMER;
        public readonly string BEZ_TYP;
        public readonly string SPRACHCODE;
        public readonly string ORT_BEZ_18;
        public readonly string ORT_BEZ_27;
    }

    /// <summary>
    /// Contains the municipalities in Switzerland and the Principality of Liechtenstein. These data come from the official list of BfS
    /// </summary>
    public sealed class NEW_COM
    {

        public NEW_COM(string c1, string c2, string c3, string c4)
        {
            this.BFSNR = c1;
            this.GEMEINDENAME = c2;
            this.KANTON = c3;
            this.AGGLONR = c4;
        }

        public readonly string BFSNR;
        public readonly string GEMEINDENAME;
        public readonly string KANTON;
        public readonly string AGGLONR;
    }

    /// <summary>
    /// Street names of all the towns of Switzerland and the Principality of Liechtenstein
    /// </summary>
	public sealed class NEW_STR
	{

		public NEW_STR(string c1, string c2, string c3, string c4, string c5, string c6, string c7, string c8, string c9, string c10, string c11)
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

		public readonly string STR_ID;
		public readonly string ONRP;
		public readonly string STR_BEZ_K;
		public readonly string STR_BEZ_L;
		public readonly string STR_BEZ_2K;
		public readonly string STR_BEZ_2L;
		public readonly string STR_LOK_TYP;
		public readonly string STR_BEZ_SPC;
		public readonly string STR_BEZ_COFF;
		public readonly string STR_GANZFACH;
		public readonly string STR_FACH_ONRP;
	}

    /// <summary>
    /// Alternative or foreign-language street name
    /// </summary>
    public sealed class NEW_STRA
    {

        public NEW_STRA(string c1, string c2, string c3, string c4, string c5, string c6, string c7, string c8, string c9)
        {
            this.STR_ID_ALT = c1;
            this.STR_ID = c2;
            this.STR_TYP = c3;
            this.STR_BEZ_AK = c4;
            this.STR_BEZ_AL = c5;
            this.STR_BEZ_A2K = c6;
            this.STR_BEZ_A2L = c7;
            this.STR_LOK_TYP = c8;
            this.STR_BEZ_SPC = c9;
        }

        public readonly string STR_ID_ALT;
        public readonly string STR_ID;
        public readonly string STR_TYP;
        public readonly string STR_BEZ_AK;
        public readonly string STR_BEZ_AL;
        public readonly string STR_BEZ_A2K;
        public readonly string STR_BEZ_A2L;   
        public readonly string STR_LOK_TYP;
        public readonly string STR_BEZ_SPC;

    }

    /// <summary>
    /// House number and Hauskey
    /// </summary>
    public sealed class NEW_GEB
    {

        public NEW_GEB(string c1, string c2, string c3, string c4, string c5, string c6, string c7)
        {
            this.HAUSKEY = c1;
            this.STR_ID = c2;
            this.HNR = c3;
            this.HNR_A = c4;
            this.HNR_COFF = c5;
            this.GANZFACH = c6;
            this.FACH_ONRP = c7;
        }

        public readonly string HAUSKEY;
        public readonly string STR_ID;
        public readonly string HNR;
        public readonly string HNR_A;
        public readonly string HNR_COFF;
        public readonly string GANZFACH;
        public readonly string FACH_ONRP;

    }

    /// <summary>
    /// alternative building name and alternative Hauskey
    /// </summary>
    public sealed class NEW_GEBA
    {

        public NEW_GEBA(string c1, string c2, string c3, string c4)
        {
            this.HAUSKEY_ALT = c1;
            this.HAUSKEY = c2;
            this.GEB_BEZ_ALT = c3;
            this.GEB_TYP = c4;
        }

        public readonly string HAUSKEY_ALT;
        public readonly string HAUSKEY;
        public readonly string GEB_BEZ_ALT;
        public readonly string GEB_TYP;

    }
    /// <summary>
    /// Offered information on house number level (postal delivery)
    /// </summary>
    public sealed class NEW_BOT_B
    {

        public NEW_BOT_B(string c1, string c2, string c3, string c4,string c5, string c6, string c7)
        {
            this.HAUSKEY = c1;
            this.A_PLZ = c2;
            this.BBZ_PLZ = c3;
            this.BOTEN_BEZ = c4;
            this.ETAPPEN_NR = c5;
            this.LAUF_NR = c6;
            this.NDEPOT = c7;
        }

        public readonly string HAUSKEY;
        public readonly string A_PLZ;
        public readonly string BBZ_PLZ;
        public readonly string BOTEN_BEZ;
        public readonly string ETAPPEN_NR;
        public readonly string LAUF_NR;
        public readonly string NDEPOT;

    }
    #endregion
}
