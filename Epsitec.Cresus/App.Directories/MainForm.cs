using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Epsitec.Data.Platform.MatchSort;
using Epsitec.Data.Platform.Directories;
using Epsitec.Data.Platform.Directories.Entity;
using Epsitec.Data.Platform.Bings;
using Microsoft.Maps.MapControl.WPF;
using System.Drawing;
using System.IO;
using System.Reflection;



namespace App.Directories
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent ();
		}

		public DirectoriesWebService dws = null;
		public BingsWebService bws = null;
		public MatchSortEtl etl = null;
		public SignIn SignIn = new SignIn ();
		public int PageSize = 10;
		public int FromPage = 1;
		public int ToPage = 10;
		
		private void MainForm_Load(object sender, EventArgs e)
		{
			//BINGS MAP
			this.Map.CredentialsProvider = new ApplicationIdCredentialsProvider ("AvjzXlyB0Pj-_c0qJHxpfOTJ3vIFchlb4ggWs5zaSar7Xh63v9zHtefyrdZUGJwo");
			this.Map.ZoomLevel = 10;
			
			//BINGS SEARCH ON VIRTUAL EARTH
			this.bws = new BingsWebService ("AvjzXlyB0Pj-_c0qJHxpfOTJ3vIFchlb4ggWs5zaSar7Xh63v9zHtefyrdZUGJwo");

			//Paginators
			this.FromPage = 1;
			this.ToPage = this.PageSize;
		}


		private void cmd_search_Click(object sender, EventArgs e)
		{

			if (this.dws==null)
			{
				this.dws = new DirectoriesWebService ();
			}
			DirectoriesSearchAddressResult result = null;

			var ValueList = new List<string> ();
			var Values = this.txt_value.Text.Split (',');
			foreach (string Value in Values)
			{
				ValueList.Add (Value);
			}
			if (this.opt_phone.Checked)
			{
				result = dws.SearchService (ValueList, DirectoriesServiceCode.TelCode);
			}
			if (this.opt_email.Checked)
			{
				result = dws.SearchService (ValueList, DirectoriesServiceCode.EmailCode);
			}
			if (this.opt_web.Checked)
			{
				result = dws.SearchService (ValueList, DirectoriesServiceCode.WebCode);
			}
			if (this.opt_name.Checked)
			{
				string[] Args = this.txt_value.Text.Split(' ');

				switch (Args.Length)
				{
					case 1: result = dws.SearchAddressByLastName(this.txt_value.Text, this.FromPage, this.ToPage);
						break;

					case 2: result = dws.SearchAddressByFullName(Args[0], Args[1], this.FromPage, this.ToPage);
						break;
				}
			}
					

			//Clear Tree
			this.result_tree.Nodes.Clear();

			int TotalPage = (result.Info.MatchedEntries / this.PageSize) + 1;
			int PageNumber = this.ToPage / this.PageSize;

			if (PageNumber == TotalPage)
			{
				this.cmd_next.Visible = false;
			}
			else
			{
				this.cmd_next.Visible = true;
			}
			if (PageNumber == 1)
			{
				this.cmd_back.Visible = false;
			}
			else
			{
				this.cmd_back.Visible = true;
			}

			if (result.Info.MatchedEntries < 2)
			{
				this.result_tree.Nodes.Add(String.Format("Query has {0} Result.", result.Info.MatchedEntries));
			}
			else
			{
				this.result_tree.Nodes.Add(String.Format("Query has {0} Results. Page {1} of {2}", result.Info.MatchedEntries,PageNumber,TotalPage));
			}
			foreach (DirectoriesEntryAdd add in result.GetEntries())
			{
				var node = this.result_tree.Nodes.Add (String.Format ("{0} {1}, {2}", add.FirstName, add.LastName, add.StateCode));
				node.Tag = String.Format ("{0}/{1} {2}/{3}/{4}", add.Zip, add.FirstName.Split (' ')[0], add.LastName, add.StreetName, add.HouseNo);
				if (add.Profession!="")
				{
					node.Nodes.Add ("Profession: " + add.Profession);
				}
				node.Nodes.Add (String.Format ("Address: {0} {1}, {2} {3}", add.StreetName, add.HouseNo, add.Zip, add.LocaPostName));
				foreach (DirectoriesEntryAddService ser in add.Services)
				{
					node.Nodes.Add(String.Format ("{0}: {1}", ser.TypeGrpCode,ser.Value));
				}
			}    
			
					
			
			
			this.chk_deploy_tree.CheckState = CheckState.Checked;
			
			
			
		}

		private void chk_deploy_tree_CheckedChanged(object sender, EventArgs e)
		{
			if (this.chk_deploy_tree.Checked)
			{
				this.result_tree.CollapseAll ();
			}
			else
			{
				this.result_tree.ExpandAll ();
			}
		}

		private void txt_value_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.cmd_search_Click (this, null);
			}
		}


		private void GetGooglePlusImage(string FullName,ListView list)
		{
			list.Clear ();
			if (this.SignIn.AccessToken != null)
			{

				UriTemplate Template = new UriTemplate("people?access_token={t}&maxResults=20&query={q}");
				Uri Prefix = new Uri("https://www.googleapis.com/plus/v1/");
				NameValueCollection Parameters = new NameValueCollection();
				Parameters.Add("t", SignIn.AccessToken);
				Parameters.Add("q", FullName);
				Uri ForgedUri = Template.BindByName(Prefix, Parameters);

				HttpWebRequest Request = WebRequest.Create(ForgedUri) as HttpWebRequest;
				Request.Method = WebRequestMethods.Http.Get;
				try
				{
					using (HttpWebResponse Response = Request.GetResponse() as HttpWebResponse)
					{
						XmlReader JsonReader = JsonReaderWriterFactory.CreateJsonReader(Response.GetResponseStream(), new XmlDictionaryReaderQuotas());
						XElement Root = XElement.Load(JsonReader);
						if (Root.Elements("items").ToArray()[0].Element("item") != null)
						{
						   
							var urls = Root.XPathSelectElements ("//image/url");
							var names = Root.XPathSelectElements ("//displayName");

							ListView.ListViewItemCollection Items = new ListView.ListViewItemCollection(list);
							
							for (int i = 0; i < names.Count(); i++)
							{
								WebRequest request = WebRequest.Create (urls.ElementAt(i).Value);

								WebResponse response = request.GetResponse ();
								Stream responseStream = response.GetResponseStream ();

								Bitmap bmp = new Bitmap (responseStream);

								responseStream.Dispose ();
								this.google_plus_image_list.Images.Add(names.ElementAt(i).Value,bmp);
								Items.Add (new ListViewItem(names.ElementAt(i).Value,names.ElementAt(i).Value));
							}

						}
						
					}


				}
				catch (WebException we)
				{
					throw new Exception (we.Message);
				}
			}
			
		}

		private void result_tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{

			var node = e.Node;

			
			if (node.Tag!=null)
			{
				var TagArgs = node.Tag.ToString().Split('/');

				if (TagArgs[0] != "")
				{
					var LocationByZip = bws.GetCoordinatesBySwissZip(TagArgs[0]);

					Pushpin pin = new Pushpin()
					{
						Location = new Location (LocationByZip.Item1,LocationByZip.Item2),
						PositionOrigin = PositionOrigin.BottomCenter,
					};

					this.Map.Children.Add(pin);
					this.Map.Center = pin.Location;
					this.GetGooglePlusImage(TagArgs[1],this.lst_head);
				}

				
				
			}
		}

		private void img_google_signin_Click(object sender, EventArgs e)
		{
			this.img_google_signin.Visible = false;
			this.SignIn.Show(this);
			
		}

		private void cmd_back_Click(object sender, EventArgs e)
		{
			this.FromPage -= this.PageSize;
			this.ToPage -= this.PageSize;
			this.cmd_search_Click(this, null);
		}

		private void cmd_next_Click(object sender, EventArgs e)
		{
			this.FromPage += this.PageSize;
			this.ToPage += this.PageSize;
			this.cmd_search_Click(this, null);
		}

		private void txt_value_MouseClick(object sender, MouseEventArgs e)
		{
			this.txt_value.Text = "";
			this.FromPage = 1;
			this.ToPage = this.PageSize;
			
		}

		private void cmd_enable_match_sort_Click(object sender, EventArgs e)
		{
			var t = System.Diagnostics.Stopwatch.StartNew();
			this.etl = new MatchSortEtl (@"s:/MAT[CH]news.csv");
			t.Stop();
			this.lbl_time_enable_match.Text =  t.Elapsed.Minutes + "min " + t.Elapsed.Seconds + "s " +t.Elapsed.Milliseconds + "ms";
			this.cmd_enable_match_sort.Visible = false;
			this.cmd_dispose_matchsort.Visible = true;
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (this.etl!=null)
			{
				this.etl.Dispose ();
			}
			
		}

		private void cmd_test_match_Click(object sender, EventArgs e)
		{
			if (this.etl != null)
			{
				this.result_tree.Nodes.Clear ();

				foreach (string s in this.etl.CustomQuery (this.txt_match_sql.Text))
				{
					this.result_tree.Nodes.Add (s);
				}
				
			}
		}

		private void cmd_getmessengerprepared_Click(object sender, EventArgs e)
		{
			if (this.etl != null)
			{
				var t = System.Diagnostics.Stopwatch.StartNew();
				
				var test = this.etl.GetDistrictNumber(this.txt_zip.Text, this.txt_zip_add.Text, this.txt_street.Text, this.txt_house.Text, this.txt_house_a.Text);
				t.Stop();
				this.lbl_time_get_messengerprepared.Text = "Messenger="+ test + " / " + t.Elapsed.Milliseconds + "ms";
			}
		}

		private void cmd_dispose_matchsort_Click(object sender, EventArgs e)
		{
			if (this.etl != null)
			{
				this.etl.Dispose();
				this.cmd_dispose_matchsort.Visible = false;
				this.cmd_enable_match_sort.Visible = true;
			}
		}

		private void cmd_gethouses_at_street_Click(object sender, EventArgs e)
		{
			if (this.etl != null)
			{
				var t = System.Diagnostics.Stopwatch.StartNew();

				var test = this.etl.GetHousesAtStreet(this.txt_zip.Text, this.txt_zip_add.Text, this.txt_street.Text);
				t.Stop();
				this.lbl_time_gethousesatstreet.Text = t.Elapsed.Milliseconds + "ms";
				
				this.result_tree.Nodes.Clear();
				var streetNode = this.result_tree.Nodes.Add(this.txt_street.Text + "@" + this.txt_zip.Text);
				foreach (string s in test)
				{
					streetNode.Nodes.Add(s);
				}
			}
		}
	}
}
