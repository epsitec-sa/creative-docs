//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Epsitec.Data.Platform
{
	public sealed class MatchSortEtlBidouille
	{

		/// <summary>
		/// Perform ETL job on Mat[CH]sort CSV file
		/// https://match.post.ch/pdf/post-match-new-sort.pdf
		/// </summary>
		public MatchSortEtlBidouille()
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

            this.Places = new Dictionary<string, MatchSortPlace>();
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

            
			foreach (MatchSortPlace p in PlacesEnum)
			{

				this.Places[p.zip+p.zipExtraDigit] = p;

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
		}

		public string GetPlaceId(string FullZip)
		{
			var place = from p in this.Places
						  where this.Places.ContainsKey (FullZip)
						  select p.Value;



			return place.First().placeId;
		}

		public string GetStreetId(string PlaceId,string StreetName)
		{
			var streetList = from sl in this.Streets
							 where this.Streets.ContainsKey (PlaceId)
							 select sl.Value;

			var streetId = from s in streetList as List<MatchSortStreet>
						   where s.streetName == StreetName
						   select s.streetId;


			return streetId.ToString();
		}
       
        public string MessengerNumber(string StreetId,string HouseNumber)
        {

            //get the street with placeId and StreetName selector
            

            var houseList = from h in this.Houses
                        where this.Houses.ContainsKey(StreetId)
                        select h.Value;

			var houseAtNumber = from hn in houseList as List<MatchSortHouse>
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
        private readonly Dictionary<string, MatchSortPlace> Places;
        private readonly Dictionary<string, List<MatchSortStreet>> Streets;
        private readonly Dictionary<string, List<MatchSortHouse>> Houses;

	}
}
