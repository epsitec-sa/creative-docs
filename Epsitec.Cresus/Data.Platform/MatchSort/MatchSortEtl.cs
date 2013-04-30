//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Epsitec.Data.Platform.MatchSort
{
	public sealed class MatchSortEtl : IDisposable
	{

		public void Dispose()
		{
			if (Conn != null)
			{
				this.Command.Dispose ();
				this.Command = null;
				this.Conn.Dispose ();
				Conn=null;

			}
		}
		/// <summary>
		/// Perform ETL job on Mat[CH]sort CSV file and load content for querying in SQLite
		/// https://match.post.ch/pdf/post-match-new-sort.pdf
		/// </summary>
		public MatchSortEtl(string CsvFilePath)
		{
           
			try
			{
                var DatabaseDirectoryPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                if (!File.Exists(DatabaseDirectoryPath + "\\MatchSort.sqlite"))
                {
                    //CASE NO DATABASE
                    SQLiteConnection.CreateFile("MatchSort.sqlite");
                    this.OpenDatabase();
                    this.CreateTableIfNeededAndResetDb();
                    this.LoadFromDatabaseCsv(CsvFilePath);
                    this.IndexAndAnalyzeDatabase();
                }
                else
                {
                    //CASE CHECK FOR UPDATE
                    this.OpenDatabase();

                    var VersionCsv = this.GetHeaderFromCsv(CsvFilePath);
                    var VersionDb = this.GetHeaderFromDatabase();

                    if (VersionCsv[0] != VersionDb[0] || VersionCsv[1] != VersionDb[1])
                    {
                        this.CreateTableIfNeededAndResetDb();
                        this.LoadFromDatabaseCsv(CsvFilePath);
                        this.IndexAndAnalyzeDatabase();
                    }
                }

			}
			catch (SQLiteException ex)
			{
				throw new Exception ("Erreur SQL dans Data.Platform Module MatchSort: "+ ex.Message.ToString());
			}
			
		}
		private const string CreateTableHeader="create table if not exists new_hea (vdat number(8), zcode number(6)); delete from new_hea";
		
		private const string CreateTablePlace1="create table if not exists new_plz1 (onrp number(5) primary key, bfsnr number(5), plz_typ number(2),plz number(4),plz_zz varchar(2), gplz number(4),ort_bez_18 varchar(18),ort_bez_27 varchar(27),kanton varchar(2),sprachcode number(1),sprachcode_abw number(1),briefz_durch number(5),gilt_ab_dat date(8),plz_briefzust number(6),plz_coff varchar(1)); delete from new_plz1";
		
		private const string CreateTablePlace2="create table if not exists new_plz2 (onrp number(5),laufnummer number(3),bez_typ number(1),sprachcode number(1),ort_bez_18 varchar(18),ort_bez_27 varchar(27)); delete from new_plz2";
		
		private const string CreateTableCommun="create table if not exists new_com (bfsnr number(5) primary key,gemeindename varchar(30),kanton varchar(2),agglonr number(5)); delete from new_com";
		
		private const string CreateTableStreet="create table if not exists new_str (str_id number(10) primary key,onrp number(5),str_bez_k varchar(25),str_bez_l varchar(60),str_bez_2k varchar(25),str_bez_2l varchar(60),str_lok_typ number(1),str_bez_spc number(1),str_bez_coff varchar(1),str_ganzfach varchar(1),str_fach_onrp number(5)); delete from new_str";
		
		//todo new_stra
		
		private const string CreateTableHouse1="create table if not exists new_geb (hauskey number(13) primary key,str_id number(10),hnr number(4),hnr_a varchar(6),hnr_coff varchar(1),ganzfach varchar(1),fach_onrp number(5)); delete from new_geb";
		//todo new_geba
		private const string CreateTableDeliver="create table if not exists new_bot_b (hauskey number(13),a_plz number(6),bbz_plz number(6),boten_bez number(4),etappen_nr number(3),lauf_nr number(6),ndepot varchar(60)); delete from new_bot_b";

		private const string IndexAll= "create index if not exists idx_zip on new_plz1(plz);create index if not exists idx_street on new_str(str_bez_2l collate nocase); create index if not exists idx_hnr on new_geb(hnr); create index if not exists idx_fhk on new_bot_b(hauskey)";
		private const string AnalyseAll= "analyze new_plz1;analyze new_str;analyze new_geb;analyze new_bot_b";

        private const string SelectHeader = "select vdat,zcode from new_hea";
        private const string SelectMessenger = "select b.etappen_nr from new_plz1 as p join new_str s on s.onrp = p.onrp join new_geb as g on g.str_id = s.str_id join new_bot_b as b on b.hauskey = g.hauskey where p.plz = @zip and s.str_bez_2l = @street collate nocase and g.hnr = @house";
		
		private SQLiteConnection Conn;
		private SQLiteCommand Command;
		private SQLiteTransaction Transaction;

        private void OpenDatabase()
        {
            this.Conn = new SQLiteConnection("Data Source=MatchSort.sqlite;Version=3;");
            this.Conn.Open();

            //SET Journal mode in WAL
            this.Command = new SQLiteCommand(this.Conn);
            this.Command.CommandText = "PRAGMA journal_mode=WAL";
            this.Command.ExecuteNonQuery();
        }

        private void CreateTableIfNeededAndResetDb()
        {
            this.Transaction = this.Conn.BeginTransaction();

            this.Command.CommandText = MatchSortEtl.CreateTableHeader;
            this.Command.ExecuteNonQuery();

            this.Command.CommandText = MatchSortEtl.CreateTablePlace1;
            this.Command.ExecuteNonQuery();

            this.Command.CommandText = MatchSortEtl.CreateTablePlace2;
            this.Command.ExecuteNonQuery();

            this.Command.CommandText = MatchSortEtl.CreateTableCommun;
            this.Command.ExecuteNonQuery();

            this.Command.CommandText = MatchSortEtl.CreateTableStreet;
            this.Command.ExecuteNonQuery();

            this.Command.CommandText = MatchSortEtl.CreateTableHouse1;
            this.Command.ExecuteNonQuery();

            this.Command.CommandText = MatchSortEtl.CreateTableDeliver;
            this.Command.ExecuteNonQuery();

            this.Transaction.Commit();
        }

        private void LoadFromDatabaseCsv(string CsvFilePath)
        {
            this.Transaction = this.Conn.BeginTransaction();
            var CommitIndex = 0;

            //Parse CSV and extract line fields -> INSERT
            foreach (var lineFields in File.ReadLines(CsvFilePath, Encoding.GetEncoding("Windows-1252")).Select(l => l.Split(';')))
            {
                switch (lineFields[0])
                {
                    case "00":

                        this.Command.CommandText = "insert into new_hea (vdat,zcode) values (@1,@2)";
                        this.Command.Parameters.AddWithValue("@1", lineFields[1]);
                        this.Command.Parameters.AddWithValue("@2", lineFields[2]);
                        this.Command.ExecuteNonQuery();
                        break;

                    case "01":
                        this.Command.CommandText = "insert into new_plz1 (onrp,bfsnr,plz_typ,plz,plz_zz,gplz,ort_bez_18,ort_bez_27,kanton,sprachcode,sprachcode_abw,briefz_durch,gilt_ab_dat,plz_briefzust,plz_coff) values (@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15)";
                        this.Command.Parameters.AddWithValue("@1", lineFields[1]);
                        this.Command.Parameters.AddWithValue("@2", lineFields[2]);
                        this.Command.Parameters.AddWithValue("@3", lineFields[3]);
                        this.Command.Parameters.AddWithValue("@4", lineFields[4]);
                        this.Command.Parameters.AddWithValue("@5", lineFields[5]);
                        this.Command.Parameters.AddWithValue("@6", lineFields[6]);
                        this.Command.Parameters.AddWithValue("@7", lineFields[7]);
                        this.Command.Parameters.AddWithValue("@8", lineFields[8]);
                        this.Command.Parameters.AddWithValue("@9", lineFields[9]);
                        this.Command.Parameters.AddWithValue("@10", lineFields[10]);
                        this.Command.Parameters.AddWithValue("@11", lineFields[11]);
                        this.Command.Parameters.AddWithValue("@12", lineFields[12]);
                        this.Command.Parameters.AddWithValue("@13", lineFields[13]);
                        this.Command.Parameters.AddWithValue("@14", lineFields[14]);
                        this.Command.Parameters.AddWithValue("@15", lineFields[15]);
                        this.Command.ExecuteNonQuery();
                        break;

                    case "02":
                        this.Command.CommandText = "insert into new_plz2 (onrp,laufnummer,bez_typ,sprachcode,ort_bez_18,ort_bez_27) values (@1,@2,@3,@4,@5,@6)";
                        this.Command.Parameters.AddWithValue("@1", lineFields[1]);
                        this.Command.Parameters.AddWithValue("@2", lineFields[2]);
                        this.Command.Parameters.AddWithValue("@3", lineFields[3]);
                        this.Command.Parameters.AddWithValue("@4", lineFields[4]);
                        this.Command.Parameters.AddWithValue("@5", lineFields[5]);
                        this.Command.Parameters.AddWithValue("@6", lineFields[6]);
                        this.Command.ExecuteNonQuery();
                        break;

                    case "03":
                        this.Command.CommandText = "insert into new_com (bfsnr,gemeindename,kanton,agglonr) values (@1,@2,@3,@4)";
                        this.Command.Parameters.AddWithValue("@1", lineFields[1]);
                        this.Command.Parameters.AddWithValue("@2", lineFields[2]);
                        this.Command.Parameters.AddWithValue("@3", lineFields[3]);
                        this.Command.Parameters.AddWithValue("@4", lineFields[4]);
                        this.Command.ExecuteNonQuery();
                        break;

                    case "04":
                        this.Command.CommandText = "insert into new_str (str_id,onrp,str_bez_k,str_bez_l,str_bez_2k,str_bez_2l,str_lok_typ,str_bez_spc,str_bez_coff,str_ganzfach,str_fach_onrp) values (@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11)";
                        this.Command.Parameters.AddWithValue("@1", lineFields[1]);
                        this.Command.Parameters.AddWithValue("@2", lineFields[2]);
                        this.Command.Parameters.AddWithValue("@3", lineFields[3]);
                        this.Command.Parameters.AddWithValue("@4", lineFields[4]);
                        this.Command.Parameters.AddWithValue("@5", lineFields[5]);
                        this.Command.Parameters.AddWithValue("@6", lineFields[6]);
                        this.Command.Parameters.AddWithValue("@7", lineFields[7]);
                        this.Command.Parameters.AddWithValue("@8", lineFields[8]);
                        this.Command.Parameters.AddWithValue("@9", lineFields[9]);
                        this.Command.Parameters.AddWithValue("@10", lineFields[10]);
                        this.Command.Parameters.AddWithValue("@11", lineFields[11]);
                        this.Command.ExecuteNonQuery();
                        break;

                    case "05":
                        //this.streetNamesAltLang.Add(compKey, new NEW_STRA (lineFields[1], lineFields[2], lineFields[3], lineFields[4], lineFields[5], lineFields[6], lineFields[7], lineFields[8], lineFields[9]));
                        break;

                    case "06":
                        this.Command.CommandText = "insert into new_geb (hauskey,str_id,hnr,hnr_a,hnr_coff,ganzfach,fach_onrp) values (@1,@2,@3,@4,@5,@6,@7)";
                        this.Command.Parameters.AddWithValue("@1", lineFields[1]);
                        this.Command.Parameters.AddWithValue("@2", lineFields[2]);
                        this.Command.Parameters.AddWithValue("@3", lineFields[3]);
                        this.Command.Parameters.AddWithValue("@4", lineFields[4]);
                        this.Command.Parameters.AddWithValue("@5", lineFields[5]);
                        this.Command.Parameters.AddWithValue("@6", lineFields[6]);
                        this.Command.Parameters.AddWithValue("@7", lineFields[7]);
                        this.Command.ExecuteNonQuery();

                        break;

                    case "07":
                        //this.houseNamesAltLang.Add(compKey, new NEW_GEBA(lineFields[1], lineFields[2], lineFields[3], lineFields[4]));
                        break;

                    case "08":
                        this.Command.CommandText = "insert into new_bot_b (hauskey,a_plz,bbz_plz,boten_bez,etappen_nr,lauf_nr,ndepot) values  (@1,@2,@3,@4,@5,@6,@7)";
                        this.Command.Parameters.AddWithValue("@1", lineFields[1]);
                        this.Command.Parameters.AddWithValue("@2", lineFields[2]);
                        this.Command.Parameters.AddWithValue("@3", lineFields[3]);
                        this.Command.Parameters.AddWithValue("@4", lineFields[4]);
                        this.Command.Parameters.AddWithValue("@5", lineFields[5]);
                        this.Command.Parameters.AddWithValue("@6", lineFields[6]);
                        this.Command.Parameters.AddWithValue("@7", lineFields[7]);
                        this.Command.ExecuteNonQuery();
                        break;
                }
                CommitIndex++;
                if (CommitIndex % 15000 == 0)
                {
                    this.Transaction.Commit();
                    this.Transaction = this.Conn.BeginTransaction();
                }
            }

            this.Transaction.Commit();
        }

        private void IndexAndAnalyzeDatabase()
        {
            this.Transaction = this.Conn.BeginTransaction();
            this.Command.CommandText = MatchSortEtl.IndexAll;
            this.Command.ExecuteNonQuery();
            this.Command.CommandText = MatchSortEtl.AnalyseAll;
            this.Command.ExecuteNonQuery();
            this.Transaction.Commit();
        }

        private string[] GetHeaderFromCsv(string CsvFilePath)
        {
            string[] result = new string[2];

            using(StreamReader reader = new StreamReader(CsvFilePath, Encoding.GetEncoding("Windows-1252"))) 
            {
                var lineFields = reader.ReadLine().Split(';');
                if (lineFields[0] == "00")
                {
                    result[0] = lineFields[1];
                    result[1] = lineFields[2];
                    return result;
                }
                else
                {
                    return null;
                }   
            }          
        }

        private string [] GetHeaderFromDatabase()
        {
            string[] result = new string[2];
            this.Command.CommandText = MatchSortEtl.SelectHeader;
            using (SQLiteDataReader dr = this.Command.ExecuteReader())
            {
                dr.Read();
                if (dr.HasRows)
                {
                    result[0] = dr.GetValue(0).ToString();
                    result[1] = dr.GetValue(1).ToString();
                    return result;
                }
                else
                {
                    return null;
                }

            }
        }


		public List<string> CustomQuery(string sql)
		{
			List<string> result = new List<string> ();
			this.Command.CommandText = sql;
			try
			{
				using (SQLiteDataReader dr = this.Command.ExecuteReader ())
				{
					var i = 0;
					while (dr.Read ())
					{
						var row = "";
						foreach (string k in dr.GetValues ().AllKeys)
						{
							row += String.Format ("{0}: {1}, ", k, dr.GetValues ().Get (k));
						}
						result.Add (row);
						i++;
					}


				}

			}
			catch (SQLiteException ex)
			{
				throw new Exception ("Erreur SQL: " + ex.Message.ToString ());
			}
			
			return result;

		}

		public string GetMessenger(string zip,string street,string house)
		{
			this.Command.CommandText = MatchSortEtl.SelectMessenger;
			this.Command.Parameters.AddWithValue ("@zip", zip);
			this.Command.Parameters.AddWithValue ("@street", street);
			this.Command.Parameters.AddWithValue ("@house", house);
			using (SQLiteDataReader dr = this.Command.ExecuteReader ())
			{
				dr.Read ();
				if (dr.HasRows)
				{
					return dr.GetValue (0).ToString ();
				}
				else
				{
					return null;
				}
				
			}
		}
	}
}
