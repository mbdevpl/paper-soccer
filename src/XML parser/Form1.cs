using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                OpenFileDialog ofd = new OpenFileDialog();
                Stream stream = null;
                string xml = null;
                if (ofd.ShowDialog() == DialogResult.OK)
                    if ((stream = ofd.OpenFile()) != null)
                        using (stream)
                        {
                            StreamReader reader = new StreamReader(stream);
                            xml = reader.ReadToEnd();
                        }

                Parseit(xml);

            }
        }

        private static void Parseit(string xml)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            Console.Write("Reading " + reader.Name);
                            reader.MoveToAttribute("type");
                            Console.WriteLine(" of type: " + reader.Value);

                            #region playerLogin handling

                            if (reader.Value == "playerLogin")
                            {
                                string nick, gametype;
                                reader.ReadToFollowing("playerLogin");
                                Console.WriteLine("Obtained a player login request:");
                                reader.MoveToAttribute("nick");
                                nick = reader.Value;
                                Console.WriteLine("Nick is : " + nick);
                                reader.MoveToAttribute("gameType");
                                Console.WriteLine("GameType  is : " + reader.Value);

                            }
                                #endregion
                                #region gameMasterLogin handling

                            else if (reader.Value == "gameMasterLogin")
                            {
                                int min = 0, max = 0;
                                reader.ReadToFollowing("gameMasterLogin");
                                Console.WriteLine("Obtained a gameMaster login request:");
                                reader.MoveToAttribute("id");
                                Console.WriteLine("Id is : " + reader.Value);
                                reader.MoveToAttribute("gameType");
                                Console.WriteLine("gameType is : " + reader.Value);
                                reader.MoveToAttribute("playersMin");
                                Console.WriteLine("playersMin is : " + reader.Value);
                                //ToDo
                                //min = Int16.Parse(reader.Value);
                                reader.MoveToAttribute("playersMax");
                                Console.WriteLine("playersMax is : " + reader.Value);
                                //max = Int16.Parse(reader.Value);
                                /*
                                     * if (min < 1 || max < min)
                                        throw new ArgumentException("Incorrect min/max values at gamemasterlogin message parsing");
                                    */
                            }
                                #endregion
                                #region loginresponse handling

                            else if (reader.Value == "loginResponse")
                            {
                                Boolean value;
                                reader.ReadToFollowing("response");
                                Console.WriteLine("Obtained a loginResponse:");
                                reader.MoveToAttribute("accept");
                                Console.WriteLine("accept is : " + reader.Value);
                                if (reader.Value.Equals("yes"))
                                    value = true;
                                else if (reader.Value.Equals("no"))
                                    value = false;
                                else throw new ArgumentException("Incorrect accept value at loginResponse message parsing");
                                if (!value)
                                {
                                    reader.ReadToFollowing("error");
                                    reader.MoveToAttribute("id");
                                    int i = Int32.Parse(reader.Value);
                                    switch (i)
                                    {
                                        case 1:
                                            Console.WriteLine("wrong nick");
                                            break;
                                        case 2:
                                            Console.WriteLine("improper game type");
                                            break;
                                        case 3:
                                            Console.WriteLine("players pool overflow ");
                                            break;
                                        case 4:
                                            Console.WriteLine("master for this game already registered");
                                            break;
                                        case 5:
                                            Console.WriteLine("wrong game type description data ");
                                            break;
                                    }
                                }
                            }

                            #endregion

                            break;
                    }
                }
            }
        }
    }
}
