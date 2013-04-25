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
		/// https://match.post.ch/pdf/post-match-new-sort.pdf
		/// </summary>
		public MatchSortEtl()
		{
            NEW_HEA header;
            Dictionary<MatchSortCompositeKey<string, string>, NEW_PLZ1> placesNames = new Dictionary<MatchSortCompositeKey<string, string>, NEW_PLZ1> ();
		    Dictionary<MatchSortCompositeKey<string, string>, NEW_PLZ2> placesNamesAltLang = new Dictionary<MatchSortCompositeKey<string, string>, NEW_PLZ2> ();
            Dictionary<string, NEW_COM>  municipalities = new Dictionary<string, NEW_COM> ();
            Dictionary<MatchSortCompositeKey<string,string>,NEW_STR> streetNames = new Dictionary<MatchSortCompositeKey<string,string>,NEW_STR> ();
            Dictionary<MatchSortCompositeKey<string, string>, NEW_STRA> streetNamesAltLang = new Dictionary<MatchSortCompositeKey<string, string>, NEW_STRA> ();
            Dictionary<MatchSortCompositeKey<string, string>, NEW_GEB> houseNames = new Dictionary<MatchSortCompositeKey<string, string>, NEW_GEB> ();
            Dictionary<MatchSortCompositeKey<string, string>, NEW_GEBA> houseNamesAltLang = new Dictionary<MatchSortCompositeKey<string, string>, NEW_GEBA> ();
		    Dictionary<MatchSortCompositeKey<string, string>, NEW_BOT_B> deliveryInformations = new Dictionary<MatchSortCompositeKey<string, string>, NEW_BOT_B> ();

            this.Places = new Dictionary<string, List<MatchSortPlace>>();
            this.Streets = new Dictionary<string, List<MatchSortStreet>>();
            this.Houses = new Dictionary<string, List<MatchSortHouse>>();

			//Parse CSV and extract line fields
			foreach (var lineFields in File.ReadLines (this.filePath,Encoding.GetEncoding("Windows-1252")).Select (l => l.Split (';')))
			{
                //Map CSV line with the "Recordart"
				switch (lineFields[0])
				{
					case "00":
                        header = new NEW_HEA(lineFields[1],lineFields[2]);
						break;

					case "01":
						var compKey = new MatchSortCompositeKey<string,string>();
                        compKey.PK = lineFields[1];
                        compKey.FK = lineFields[2];
                        placesNames.Add(compKey,new NEW_PLZ1 (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6], lineFields[7], lineFields[8], lineFields[9], lineFields[10], lineFields[11], lineFields[12], lineFields[13], lineFields[14], lineFields[15]));
						break;

					case "02":
						compKey = new MatchSortCompositeKey<string, string> ();
						compKey.PK = Guid.NewGuid ().ToString ();
						compKey.FK = lineFields[1];
						placesNamesAltLang.Add(compKey,new NEW_PLZ2 (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6]));
						break;

                    case "03":
                        municipalities.Add(lineFields[1], new NEW_COM (lineFields[1], lineFields[2], lineFields[3], lineFields[4]));
                        break;

					case "04":
                        compKey = new MatchSortCompositeKey<string,string>();
                        compKey.PK = lineFields[1];
                        compKey.FK = lineFields[2];
						streetNames.Add (compKey,new NEW_STR (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6], lineFields[7], lineFields[8], lineFields[9], lineFields[10], lineFields[11]));
                        break;

                    case "05":
                        compKey = new MatchSortCompositeKey<string, string>();
                        compKey.PK = lineFields[1];
                        compKey.FK = lineFields[2];
                        streetNamesAltLang.Add(compKey, new NEW_STRA (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6], lineFields[7], lineFields[8], lineFields[9]));
                        break;

					case "06":
						compKey = new MatchSortCompositeKey<string, string>();
                        compKey.PK = lineFields[1];
                        compKey.FK = lineFields[2];
                        houseNames.Add(compKey, new NEW_GEB (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6], lineFields[7]));
						break;

                    case "07":
                        compKey = new MatchSortCompositeKey<string, string>();
                        compKey.PK = lineFields[1];
                        compKey.FK = lineFields[2];
                        houseNamesAltLang.Add(compKey, new NEW_GEBA(lineFields[1], lineFields[2], lineFields[3], lineFields[4]));
                        break;

                    case "08":
						compKey = new MatchSortCompositeKey<string, string>();
                        compKey.PK = Guid.NewGuid().ToString();
                        compKey.FK = lineFields[1];
						deliveryInformations.Add (compKey,new NEW_BOT_B (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6], lineFields[7]));
                        break;
				}
			}
            

            //Loading aggregates Enumerables from native models entity
            IEnumerable<MatchSortPlace> PlacesEnum = from p in placesNames
                                                 join pa in placesNamesAltLang on p.Key.PK equals pa.Key.FK into paj
                                                 from pa in paj.DefaultIfEmpty()
                                                 join m in municipalities on p.Key.FK equals m.Key
                                                 select new MatchSortPlace(p.Value.ONRP, m.Value.BFSNR, m.Value.GEMEINDENAME, m.Value.KANTON, m.Value.AGGLONR, p.Value.PLZ_TYP, p.Value.PLZ, p.Value.PLZ_ZZ, p.Value.GPLZ, p.Value.ORT_BEZ_18, p.Value.ORT_BEZ_27, p.Value.KANTON, p.Value.SPRACHCODE, p.Value.SPRACHCODE_ABW == "" ? "0" : p.Value.SPRACHCODE_ABW, p.Value.BRIEFZ_DURCH, p.Value.GILT_AB_DAT, p.Value.PLZ_BRIEFZUST, p.Value.PLZ_COFF, pa.Value == null ? "" : pa.Value.BEZ_TYP, pa.Value == null ? "" : pa.Value.ORT_BEZ_18, pa.Value == null ? "" : pa.Value.ORT_BEZ_27, pa.Value == null ? "0" : pa.Value.SPRACHCODE);

            IEnumerable<MatchSortStreet> StreetsEnum = from s in streetNames
                                                   join a in streetNamesAltLang on s.Key.PK equals a.Key.FK into aj
                                                   from a in aj.DefaultIfEmpty()
                                                   select new MatchSortStreet(s.Value.STR_ID, s.Value.ONRP, s.Value.STR_BEZ_K, a.Value == null ? "" : a.Value.STR_BEZ_AK, s.Value.STR_BEZ_L, a.Value == null ? "" : a.Value.STR_BEZ_AL, s.Value.STR_BEZ_2K, a.Value == null ? "" : a.Value.STR_BEZ_A2K, s.Value.STR_BEZ_2L, a.Value == null ? "" : a.Value.STR_BEZ_A2L, s.Value.STR_LOK_TYP, s.Value.STR_BEZ_SPC, s.Value.STR_BEZ_COFF);

            IEnumerable<MatchSortHouse> HousesEnum = from h in houseNames
                                                 join a in houseNamesAltLang on h.Key.PK equals a.Key.FK into aj
                                                 from a in aj.DefaultIfEmpty()
                                                 join d in deliveryInformations on h.Key.PK equals d.Key.FK into dj
                                                 from d in dj.DefaultIfEmpty()
                                                 select new MatchSortHouse(h.Value.STR_ID, h.Value.HNR, h.Value.HNR_A, h.Value.HNR_COFF, a.Value == null ? "" : a.Value.GEB_BEZ_ALT, a.Value == null ? "" : a.Value.GEB_TYP, d.Value == null ? "" : d.Value.BBZ_PLZ, d.Value == null ? "" : d.Value.BOTEN_BEZ, d.Value == null ? "" : d.Value.ETAPPEN_NR, d.Value == null ? "" : d.Value.LAUF_NR, d.Value == null ? "" : d.Value.NDEPOT);

            //Preparing final dictionnary

            List<MatchSortPlace> PlaceList;
            foreach (MatchSortPlace p in PlacesEnum)
            {
                if (!this.Places.TryGetValue(p.zip, out PlaceList))
                {
                    PlaceList = new List<MatchSortPlace>();
                    this.Places[p.zip] = PlaceList;
                }
                PlaceList.Add(p);
            }


            List<MatchSortStreet> StreetList;
            foreach(MatchSortStreet s in StreetsEnum)
            {
                if(!this.Streets.TryGetValue(s.placeId,out StreetList))
                {
                    StreetList = new List<MatchSortStreet>();
                    this.Streets[s.placeId] = StreetList;
                }
                StreetList.Add(s);

            }

            List<MatchSortHouse> HouseList;
            foreach (MatchSortHouse h in HousesEnum)
            {
                if (!this.Houses.TryGetValue(h.streetId, out HouseList))
                {
                    HouseList = new List<MatchSortHouse>();
                    this.Houses[h.streetId] = HouseList;
                }
                HouseList.Add(h);

            }
            placesNames = null;
            placesNamesAltLang = null;
            municipalities = null;
            streetNames = null;
            streetNamesAltLang = null;
            houseNames = null;
            houseNamesAltLang = null;
            deliveryInformations = null;

		}


       
        public string DeliveryMessengerNumber(string Zip,string ZipExtra, string StreetName,string HouseNumber)
        {
            //get the placeId from FullZip
            
            
            
            
            var placeList = from p in this.Places
                          where this.Places.ContainsKey(Zip+ZipExtra)
                          select p.Value.First();

            var placeId = from p in placeList
                          where p.zipExtraDigit == ZipExtra
                          select p.placeId;


            //get the street with placeId and StreetName selector
            var streetList = from sl in this.Streets
                             where this.Streets.ContainsKey(placeId.ToString())
                             select sl.Value.First();

            var streetId = from s in streetList
                         where s.streetName == StreetName
                         select s.streetId;

            var houseList = from h in this.Houses
                        where this.Houses.ContainsKey(streetId.ToString())
                        select h.Value.First();

            var houseAtNumber = from hn in houseList
                                where hn.houseNumber == HouseNumber
                                select hn;
                        
            return houseAtNumber.First().runningNumber;
        }

        #region Aggregate Getters
        /*
        /// <summary>
        /// Street Aggregate with alternative names
        /// </summary>
        public IEnumerable<MatchSortStreet> Streets
        {
            get
            {

                var query = from s in this.streetNames
                            join a in this.streetNamesAltLang on s.Key.PK equals a.Key.FK into aj
                            from a in aj.DefaultIfEmpty()
                            select new MatchSortStreet(s.Value.STR_ID, s.Value.ONRP, s.Value.STR_BEZ_K, a.Value == null ? "" : a.Value.STR_BEZ_AK, s.Value.STR_BEZ_L, a.Value == null ? "" : a.Value.STR_BEZ_AL, s.Value.STR_BEZ_2K, a.Value == null ? "" : a.Value.STR_BEZ_A2K, s.Value.STR_BEZ_2L, a.Value == null ? "" : a.Value.STR_BEZ_A2L, s.Value.STR_LOK_TYP, s.Value.STR_BEZ_SPC, s.Value.STR_BEZ_COFF);

                return query;
            }
        }


        /// <summary>
        /// Places aggregates with Zip codes, city addresses lines and alternative names
        /// </summary>
		public IEnumerable<MatchSortPlace> Places
        {
            get
            {
				var query = from p in this.placesNames
							join pa in this.placesNamesAltLang on p.Key.PK equals pa.Key.FK into paj
							from pa in paj.DefaultIfEmpty()
							join m in this.municipalities on p.Key.FK equals m.Key
							select new MatchSortPlace (p.Value.ONRP,m.Value.BFSNR, m.Value.GEMEINDENAME, m.Value.KANTON, m.Value.AGGLONR, p.Value.PLZ_TYP, p.Value.PLZ, p.Value.PLZ_ZZ, p.Value.GPLZ, p.Value.ORT_BEZ_18, p.Value.ORT_BEZ_27, p.Value.KANTON, p.Value.SPRACHCODE, p.Value.SPRACHCODE_ABW=="" ? "0" : p.Value.SPRACHCODE_ABW, p.Value.BRIEFZ_DURCH, p.Value.GILT_AB_DAT, p.Value.PLZ_BRIEFZUST, p.Value.PLZ_COFF, pa.Value==null ? "" : pa.Value.BEZ_TYP, pa.Value==null ? "" : pa.Value.ORT_BEZ_18, pa.Value==null ? "" : pa.Value.ORT_BEZ_27, pa.Value==null ? "0" : pa.Value.SPRACHCODE);
			
				return query;

            }
        }

		/// <summary>
		/// Places aggregates with Zip codes, city addresses lines and alternative names
		/// </summary>
		public IEnumerable<MatchSortPlace> placesFilteredByZip(string zipFilter)
		{		
			var query = from p in this.placesNames
						where p.Value.PLZ == zipFilter
						join pa in this.placesNamesAltLang on p.Key.PK equals pa.Key.FK into paj
						from pa in paj.DefaultIfEmpty ()
						join m in this.municipalities on p.Key.FK equals m.Key
						select new MatchSortPlace (p.Value.ONRP, m.Value.BFSNR, m.Value.GEMEINDENAME, m.Value.KANTON, m.Value.AGGLONR, p.Value.PLZ_TYP, p.Value.PLZ, p.Value.PLZ_ZZ, p.Value.GPLZ, p.Value.ORT_BEZ_18, p.Value.ORT_BEZ_27, p.Value.KANTON, p.Value.SPRACHCODE, p.Value.SPRACHCODE_ABW=="" ? "0" : p.Value.SPRACHCODE_ABW, p.Value.BRIEFZ_DURCH, p.Value.GILT_AB_DAT, p.Value.PLZ_BRIEFZUST, p.Value.PLZ_COFF, pa.Value==null ? "" : pa.Value.BEZ_TYP, pa.Value==null ? "" : pa.Value.ORT_BEZ_18, pa.Value==null ? "" : pa.Value.ORT_BEZ_27, pa.Value==null ? "0" : pa.Value.SPRACHCODE);
			return query;	
		}

        /// <summary>
        /// Houses aggregate with house numbers, alternative description and messanger delivery numbers
        /// </summary>
		public IEnumerable<MatchSortHouse> Houses
        {
            get
            {
				var query = from h in this.houseNames
                            join a in this.houseNamesAltLang on h.Key.PK equals a.Key.FK into aj
							from a in aj.DefaultIfEmpty()
                            join d in this.deliveryInformations on h.Key.PK equals d.Key.FK into dj
						    from d in dj.DefaultIfEmpty()
							select new MatchSortHouse (h.Value.STR_ID, h.Value.HNR, h.Value.HNR_A, h.Value.HNR_COFF, a.Value==null ? "" : a.Value.GEB_BEZ_ALT, a.Value==null ? "" : a.Value.GEB_TYP, d.Value==null ? "" : d.Value.BBZ_PLZ, d.Value==null ? "" :  d.Value.BOTEN_BEZ, d.Value==null ? "" :  d.Value.ETAPPEN_NR, d.Value==null ? "" :  d.Value.LAUF_NR, d.Value==null ? "" :  d.Value.NDEPOT);

				return query;
            }
        }
        */
        #endregion

        #region Custom Query
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="zip">place zip code</param>
        /// <param name="streetName">a street name like "rue du bassin"</param>
        /// <returns></returns>
        public IEnumerable<MatchSortHouse> HousesAtStreet(string zip, string streetName)
        {
            

            var streetList = from s in this.QuickStreets
                             where this.QuickStreets.ContainsKey(placeId)
                             select s.Value;
    

                        join s in this.streetNames on p.Key.PK equals s.Key.FK
                        where s.Value.STR_BEZ_2L.ToUpper() == streetName.ToUpper()
                        join h in this.houseNames on s.Key.PK equals h.Key.FK
                        join a in this.houseNamesAltLang on h.Key.PK equals a.Key.FK into aj
                        from a in aj.DefaultIfEmpty()
                        join d in this.deliveryInformations on h.Key.PK equals d.Key.FK into dj
                        from d in dj.DefaultIfEmpty()
                        select new MatchSortHouse(h.Value.STR_ID, h.Value.HNR, h.Value.HNR_A, h.Value.HNR_COFF, a.Value == null ? "" : a.Value.GEB_BEZ_ALT, a.Value == null ? "" : a.Value.GEB_TYP, d.Value == null ? "" : d.Value.BBZ_PLZ, d.Value == null ? "" : d.Value.BOTEN_BEZ, d.Value == null ? "" : d.Value.ETAPPEN_NR, d.Value == null ? "" : d.Value.LAUF_NR, d.Value == null ? "" : d.Value.NDEPOT);

            return query;
        }

        /// <summary>
        ///  Return a house from a query based on parameters
        /// </summary>
        /// <param name="zip">place zip code</param>
        /// <param name="streetName">a street name like "rue du bassin"</param>
        /// <param name="houseNumber">the house number</param>
        /// <returns></returns>
        public IEnumerable<MatchSortHouse> HouseAtStreet(string zip, string streetName, string houseNumber)
        {
            var query = from p in this.placesNames
                        where p.Value.PLZ == zip
                        join s in this.streetNames on p.Key.PK equals s.Key.FK
                        where s.Value.STR_BEZ_2L.ToUpper() == streetName.ToUpper()
                        join h in this.houseNames on s.Key.PK equals h.Key.FK
                        where h.Value.HNR == houseNumber //Check on numeric
                        join a in this.houseNamesAltLang on h.Key.PK equals a.Key.FK into aj
                        from a in aj.DefaultIfEmpty()
                        join d in this.deliveryInformations on h.Key.PK equals d.Key.FK into dj
                        from d in dj.DefaultIfEmpty()
                        select new MatchSortHouse(h.Value.STR_ID, h.Value.HNR, h.Value.HNR_A, h.Value.HNR_COFF, a.Value == null ? "" : a.Value.GEB_BEZ_ALT, a.Value == null ? "" : a.Value.GEB_TYP, d.Value == null ? "" : d.Value.BBZ_PLZ, d.Value == null ? "" : d.Value.BOTEN_BEZ, d.Value == null ? "" : d.Value.ETAPPEN_NR, d.Value == null ? "" : d.Value.LAUF_NR, d.Value == null ? "" : d.Value.NDEPOT);

            return query;
        }
        #endregion
        
        #region Piece By Piece Query Building
        /// <summary>
        /// Build a query piece by piece on place's data, using parameters as filters if used
        /// </summary>
        /// <param name="zipFilter">zip filter applied if not null</param>
        /// <param name="nameFilter">name filter applied if not null</param>
        /// <returns></returns>
        public IEnumerable<MatchSortPlace> placesFilteredBy(string zipFilter, string nameFilter)
        {
            var query = from p in this.placesNames select p;

            if (nameFilter != null)
            {
                query = this.filterPlaceNameQueryByName(query, nameFilter);
            }
            if (zipFilter != null)
            {
                query = this.filterPlaceNameQueryByZip(query, zipFilter);
            }

            var result = from p in query
                         join pa in this.placesNamesAltLang on p.Key.PK equals pa.Key.FK into paj
                         from pa in paj.DefaultIfEmpty()
                         join m in this.municipalities on p.Key.FK equals m.Key
                         select new MatchSortPlace(p.Value.ONRP, m.Value.BFSNR, m.Value.GEMEINDENAME, m.Value.KANTON, m.Value.AGGLONR, p.Value.PLZ_TYP, p.Value.PLZ, p.Value.PLZ_ZZ, p.Value.GPLZ, p.Value.ORT_BEZ_18, p.Value.ORT_BEZ_27, p.Value.KANTON, p.Value.SPRACHCODE, p.Value.SPRACHCODE_ABW == "" ? "0" : p.Value.SPRACHCODE_ABW, p.Value.BRIEFZ_DURCH, p.Value.GILT_AB_DAT, p.Value.PLZ_BRIEFZUST, p.Value.PLZ_COFF, pa.Value == null ? "" : pa.Value.BEZ_TYP, pa.Value == null ? "" : pa.Value.ORT_BEZ_18, pa.Value == null ? "" : pa.Value.ORT_BEZ_27, pa.Value == null ? "0" : pa.Value.SPRACHCODE);
            return result;
        }


        private IEnumerable<KeyValuePair<MatchSortCompositeKey<string, string>, NEW_PLZ1>> filterPlaceNameQueryByName(IEnumerable<KeyValuePair<MatchSortCompositeKey<string, string>, NEW_PLZ1>> placeNameQuery, string name)
        {
            return from p in placeNameQuery
                   where p.Value.ORT_BEZ_27.ToUpper() == name.ToUpper()
                   select p;
        }

        private IEnumerable<KeyValuePair<MatchSortCompositeKey<string, string>, NEW_PLZ1>> filterPlaceNameQueryByZip(IEnumerable<KeyValuePair<MatchSortCompositeKey<string, string>, NEW_PLZ1>> placeNameQuery, string zip)
        {
            return from p in placeNameQuery
                   where p.Value.PLZ == zip
                   select p;
        }
        */
        #endregion
        


		private readonly string filePath = @"s:/MAT[CH]news.csv";

        //Final Dictionnary
        private readonly Dictionary<string, List<MatchSortPlace>> Places;
        private readonly Dictionary<string, List<MatchSortStreet>> Streets;
        private readonly Dictionary<string, List<MatchSortHouse>> Houses;

	}

    


    #region MAT[CH] DATA MODEL
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
