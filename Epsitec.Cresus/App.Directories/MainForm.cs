using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Epsitec.Data.Platform.Directories;
using Epsitec.Data.Platform.Directories.Entity;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Media.Imaging;
using System.Windows.Controls;


namespace App.Directories
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent ();
		}

		public DirectoriesWebService ws = null;
		public SignIn SignIn = new SignIn ();
		
		private void MainForm_Load(object sender, EventArgs e)
		{
			//BINGS MAP
			this.Map.CredentialsProvider = new ApplicationIdCredentialsProvider ("AvjzXlyB0Pj-_c0qJHxpfOTJ3vIFchlb4ggWs5zaSar7Xh63v9zHtefyrdZUGJwo");
			this.Map.ZoomLevel = 10;
            
		}


		private void cmd_search_Click(object sender, EventArgs e)
		{

			if (this.ws==null)
			{
				this.ws = new DirectoriesWebService ();
			}
			DirectoriesSearchAddressResult result = null;
			if (this.opt_phone.Checked || this.opt_email.Checked || this.opt_web.Checked)
			{
				var ValueList = new List<string> ();
				var Values = this.txt_value.Text.Split (',');
				foreach (string Value in Values)
				{
					ValueList.Add (Value);
				}
				if (this.opt_phone.Checked)
				{
					result = ws.SearchService (ValueList, DirectoriesServiceCode.TelCode);
				}
				if (this.opt_email.Checked)
				{
					result = ws.SearchService (ValueList, DirectoriesServiceCode.EmailCode);
				}
				if (this.opt_web.Checked)
				{
					result = ws.SearchService (ValueList, DirectoriesServiceCode.WebCode);
				}
				
			}
			else
			{
				string[] Args = this.txt_value.Text.Split(' ');
					
				switch(Args.Length)
				{
					case 0: result = ws.SearchAddressByPhone(this.txt_value.Text);
					break;

					case 1: result = ws.SearchAddressByFirstName(this.txt_value.Text, 1, 100);
					break;

					case 2: result = ws.SearchAddressByFullName(Args[0], Args[1], 1, 100);
					break;
				}
			}
				

			//Clear Tree
			this.result_tree.Nodes.Clear();
			if (result.Info.MatchedEntries < 2)
			{
				this.result_tree.Nodes.Add (String.Format ("Query has {0} Result.", result.Info.MatchedEntries)); 
			}
			else
			{
				this.result_tree.Nodes.Add (String.Format ("Query has {0} Results.", result.Info.MatchedEntries)); 
			}
			
		   

			foreach (DirectoriesEntryAdd add in result.GetEntries())
			{
				var node = this.result_tree.Nodes.Add (String.Format ("{0} {1}, {2}", add.FirstName, add.LastName, add.StateCode));
				node.Tag = add.Zip + "/" +add.FirstName + " " + add.LastName;
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
			
					
			
			this.result_tree.ExpandAll();
			this.chk_deploy_tree.CheckState = CheckState.Unchecked;
			
			
			
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


        private Image GetGooglePlusImage(string FullName)
        {
            if (this.SignIn.AccessToken != null)
            {

                UriTemplate Template = new UriTemplate("people?access_token={t}&maxResults=1&query={q}");
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
                            string ImageUri = Root.Elements("items").ToArray()[0].Element("item").Element("image").Element("url").Value;
                            Image image = new Image();
                            image.Height = 50;
                            //Define the URI location of the image
                            BitmapImage myBitmapImage = new BitmapImage();
                            myBitmapImage.BeginInit();
                            myBitmapImage.UriSource = new Uri(ImageUri);
                            myBitmapImage.DecodePixelHeight = 50;
                            myBitmapImage.EndInit();
                            image.Source = myBitmapImage;
                            image.Opacity = 0.8;
                            image.Stretch = System.Windows.Media.Stretch.None;
                            return image;
                        }
                        else
                        {
                            return null;
                        }
                        
                    }


                }
                catch (WebException we)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
        }
		private Location GetCoordinates(string Zip)
		{


			UriTemplate Template = new UriTemplate ("Locations?countryRegion={c}&postalCode={z}&maxResults={n}&key={k}");
			Uri Prefix = new Uri ("http://dev.virtualearth.net/REST/v1/");
			NameValueCollection Parameters = new NameValueCollection ();
			Parameters.Add ("c", "CH");
			Parameters.Add ("z", Zip);
			Parameters.Add ("n", "1");
			Parameters.Add ("k", "AvjzXlyB0Pj-_c0qJHxpfOTJ3vIFchlb4ggWs5zaSar7Xh63v9zHtefyrdZUGJwo");

			Uri ForgedUri = Template.BindByName (Prefix, Parameters);

			HttpWebRequest Request = WebRequest.Create (ForgedUri) as HttpWebRequest;
			Request.Method = WebRequestMethods.Http.Get;


			try
			{
				using (HttpWebResponse Response = Request.GetResponse () as HttpWebResponse)
				{
					XmlReader JsonReader = JsonReaderWriterFactory.CreateJsonReader (Response.GetResponseStream (), new XmlDictionaryReaderQuotas ());
					XElement Root = XElement.Load (JsonReader);


					XElement [] Coordinates = Root.Elements ("resourceSets").ToArray ()[0].Element ("item").Elements ("resources").ToArray ()[0].Element ("item").Element ("point").Element ("coordinates").Elements ("item").ToArray ();
					string MapX = Coordinates[0].Value;
					string MapY = Coordinates[1].Value;


					return new Location (Convert.ToDouble (MapX), Convert.ToDouble (MapY));
				}


			}
			catch (WebException we)
			{
				return null;
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
                    var LocationByZip = this.GetCoordinates(TagArgs[0]);

                    Pushpin pin = new Pushpin()
                    {
                        Location = LocationByZip,
                        PositionOrigin = PositionOrigin.BottomCenter,
                    };

                    this.Map.Children.Add(pin);
                    this.Map.Center = pin.Location;


                    

                    Image image = this.GetGooglePlusImage(TagArgs[1]);
                    if (image != null)
                    {
                        MapLayer imageLayer = new MapLayer();
                        //The map location to place the image at
                        Location location = new Location(LocationByZip);
                        //Center the image around the location specified
                        PositionOrigin position = PositionOrigin.BottomRight;

                        //Add the image to the defined map layer
                        imageLayer.AddChild(image, location, position);
                        //Add the image layer to the map
                        this.Map.Children.Add(imageLayer);
                    }
                    

                }


				
			}
		}

        private void img_google_signin_Click(object sender, EventArgs e)
        {
            this.img_google_signin.Visible = false;
            this.SignIn.Show(this);
            
        }
	}
}
