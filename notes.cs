//this is a test
//  <form id="form1" runat="server">
//     <div>
//         <asp:PlaceHolder runat="server" ID="PlaceHolder1" />
//         <asp:Button runat="server" ID="Button1" OnClick="Button1_Click" Text="Submit" />
//         <asp:Label runat="server" ID="Label1" />
//     </div>
//     </form>

 protected void Button1_Click(object sender, EventArgs e)
        {
            LoadControls();
        }
        private void LoadControls()
        {


            string conString = ConfigurationManager.ConnectionStrings["aspnetdbConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from groupInfo order by group1", con);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            string strGroupName = string.Empty;
            DataTable dt = ds.Tables[0];
            int flag = 0;

            RadioButtonList rblGroup = new RadioButtonList();
            Label lblGroup;
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                while (strGroupName == Convert.ToString(dt.Rows[i][0]))
                {
                    i++;
                    goto Outer;
                }

                strGroupName = Convert.ToString(dt.Rows[i][0]);
                rblGroup = new RadioButtonList();
                lblGroup = new Label();
                lblGroup.Text = strGroupName;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if (strGroupName == Convert.ToString(dt.Rows[j][0]))
                    {
                        ListItem lblItem = new ListItem(Convert.ToString(dt.Rows[j][1]), Convert.ToString(dt.Rows[j][1]));
                        rblGroup.Items.Add(lblItem);
                    }
                }
                PlaceHolder1.Controls.Add(lblGroup);
                PlaceHolder1.Controls.Add(rblGroup);

            Outer:
                continue;
            }
        }