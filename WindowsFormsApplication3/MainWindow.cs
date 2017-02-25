using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeifGWCalc;

namespace WindowsFormsApplication3
{
    public partial class LeifGWCalc : Form
    {
        public Button[] mainButtons;
        public Button[] extraButtons;

        public LeifGWCalc()
        {
            InitializeComponent();

            extraButtons = new Button[] {
                    button_cos, button_sin,  button_tan, button_leftpar, button_leftpar,
                    button_rightpar, button_power, button_degrees, button_root
            };

            mainButtons = new Button[] {
                button1,button2,button3,button4,button5,button6,button7,button8,button9,button10,
                button11,button12,button13,button14,button15,button16,button17
            };

            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\LeifGwCalc\data.txt";

            if (File.Exists(appdata))
            {
                string text = File.ReadAllText(appdata);
                if (text != "")
                {
                    if (text[0] == '<')
                    {
                        button18.Text = "<";
                        currentMode = WindowMode.extended;
                        UpdateWindow();
                    }
                    else if (text[0] == '>')
                    {
                        currentMode = WindowMode.normal;
                        UpdateWindow();
                    }
                    else
                    {
                        currentMode = WindowMode.minimal;
                        UpdateWindow();
                    }
                    text = text.Remove(0, 1);
                    button_degrees.Text = text;
                }
            }
            else
            {
                currentMode = WindowMode.normal;
                UpdateWindow();
            }
        }


        public enum WindowMode
        {
            normal,
            extended,
            minimal
        }


        public WindowMode currentMode = WindowMode.normal;
        public bool hasQuote = false;

        public void UpdateWindow()
        {
            resizeBox();

            int height = 20;
            if (hasQuote)
            {
                height += 40;
            }

            if (currentMode == WindowMode.minimal)
            {
                returnBox.Location = new Point(12, 40);
                returnBox.Height = height;
            }
            else
            {
                returnBox.Location = new Point(12, 182);
                returnBox.Height = height;
            }

            if (currentMode == WindowMode.normal)
            {
                foreach (Button a in mainButtons)
                { a.Visible = true; }
                foreach (Button a in extraButtons)
                { a.Visible = false; }
                button18.Text = ">";
            }

            else if (currentMode == WindowMode.extended)
            {
                foreach (Button a in mainButtons)
                { a.Visible = true; }
                foreach (Button a in extraButtons)
                { a.Visible = true; }
                button18.Text = "<";
            }
            else
            {
                foreach (Button a in mainButtons)
                { a.Visible = false; }
                foreach (Button a in extraButtons)
                { a.Visible = false; }
                button18.Text = "v";
            }
        }

        public void resizeBox(int width,int height)
        {
            this.MaximumSize = new Size(width, height);
            this.MinimumSize = new Size(width, height);
            this.Size        = new Size(width, height);
        }
        public void resizeBox()
        {
            int height = 0;
            if (hasQuote)
            { height += 40; }

            if (currentMode == WindowMode.normal)
            { resizeBox(218, 251+height); }
            else if (currentMode == WindowMode.extended)
            { resizeBox(322, 251+height); }
            else if (currentMode == WindowMode.minimal)
            { resizeBox(218, 105+height); }
        }

        public void inputBox_TextChanged(object sender, EventArgs e)
        {
            string testString = inputBox.Text.ToLower();
            if (testString == "tiny box" || testString == "small box" || testString == "small mode" || testString == "tiny mode" ||
                testString == "tiny window" || testString == "small window")
            {
                inputBox.Text = "";
                returnBox.Text = "type :\"Big box\" to return";
                currentMode = WindowMode.minimal;
                UpdateWindow();
            }
            else if (testString == "big box" || testString == "normal box" || testString == "big mode" || testString == "normal mode" ||
                     testString == "big window" || testString == "normal window")
            {
                inputBox.Text = "";
                returnBox.Text = "type :\"small box\" to return";
                currentMode = WindowMode.normal;
                UpdateWindow();
            }
            else
            {
                string output = Calculator.Calculate(inputBox.Text, button_degrees.Text == "Degrees");
                if (output == "Error")
                {
                    hasQuote = true;
                    resizeBox();
                    returnBox.Text = '"' + gwreturn() + '"';
                }
                else
                {
                    returnBox.Text = output;
                    hasQuote = false;
                    resizeBox();
                }
            }
        }

        private string gwreturn()
        {
            //enter citat from our lord and saviour Leif Gw Persson if there is an error.
            Random rnd1 = new Random();
            int random = rnd1.Next(0, 21);
            string citat = "";
            switch (random)
            {
                case 0:
                    citat = "Mitt yngsta barnbarn är rödfnasigt och ser ut some en ung Winston Churchhill. Minus cigarren.";
                    break;
                case 1:
                    citat = "Du har väldigt bastanta små händer. Har du tänkt på det?";
                    break;
                case 2:
                    citat = "Till skillnad från dig så har jag faktiskt gått igenom de där handlingarna, så jag är inte lika imponerad.";
                    break;
                case 3:
                    citat = "Det finns riktigt bra hembränt på några ställen och det brukar jag då inte tacka nej till.";
                    break;
                case 4:
                    citat = "Jakt är en jävla udda sysselsättning - att hålla på och skjuta på djur. Ett utmärkt sätt att förhärda en människa på.";
                    break;
                case 5:
                    citat = "Det är inte bra att ge kommunisterna allt de pekar på.";
                    break;
                case 6:
                    citat = "Jag går på krogen och får massa idéer. Ju fullare desto bättre idéer.";
                    break;
                case 7:
                    citat = "Det är ganska trevligt att vara populär.";
                    break;
                case 8:
                    citat = "Jag betalar åtminstånde ett par daghem varje år på mina skatter.";
                    break;
                case 9:
                    citat = "En gång var det en tomte som gjorde en ändring utan att fråga. Honom blev jag galen på.";
                    break;
                case 10:
                    citat = "Om jag äter sill kan jag ta en öl. Men bara några klunkar för att skölja ner vodkan.";
                    break;
                case 11:
                    citat = "Andra författare väljer platser där de själva har bott. De lata jävlarna. De behöver inte göra sin research.";
                    break;
                case 12:
                    citat = "Jag hade fel om Fridolin.";
                    break;
                case 13:
                    citat = "Polisens mordvapenarkiv, det är ett sånt ställe jag skulle kunna åldras och då på.";
                    break;
                case 14:
                    citat = "Mamma tog mina aktier.";
                    break;
                case 15:
                    citat = "Paradise Hotel. Det är ett antal ungdomspsykopater som för säkerhets skull är på fyllan.";
                    break;
                case 16:
                    citat ="Jag är ju ganska gammal och behöver tid att tänka. Så jag har den här framtoningen för att ge mig själv mer tid";
                    break;
                case 17:
                    citat = "Ge barnen pengar och säg åt dem att inte dränka sig själva.";
                    break;
                case 18:
                    citat = "Normalt hoppar jag omkring. Men då är jag noga med att bara mina närmaste kan se mig.";
                    break;
                case 19:
                    citat = "Jag vet inte varför mina böcker säljer så dåligt i Norge. För lite bilder eller för små bokstäver kanske.";
                    break;
                case 20:
                    citat = "Om jag har blivit inbjuden till Nobelfesten? Ja, men jag tror inte jag har svarat. Den där inbjudan ligger någonstans.";
                    break;
                case 21:
                    citat = "Jag har kallat folk för massa fula saker. Men inget som jag ångrar direkt, det vill jag inte påstå.";
                    break;
            }
            return citat;
        }

        private void button_Click(Button button) //when pressing a button, add the buttons text too the equation, and select the window again
        {
            button_Click(button.Text);
        }
        private void button_Click(string addText)
        {
            inputBox.Text += addText;
            this.ActiveControl = inputBox;
            inputBox.Select(inputBox.Text.Length, inputBox.Text.Length);
        }

        private void ButtonClick(object sender, EventArgs e)
        { button_Click((Button)sender); }

        private void button10_Click(object sender, EventArgs e) //backspace
        {
            if (inputBox.Text.Length > 0)
            { inputBox.Text = inputBox.Text.Remove(inputBox.Text.Length - 1, 1); }
            this.ActiveControl = inputBox;
            inputBox.Select(inputBox.Text.Length, inputBox.Text.Length);
        }

        private void button16_Click(object sender, EventArgs e) //clear
        {
            inputBox.Text = "";
            this.ActiveControl = inputBox;
            inputBox.Select(inputBox.Text.Length, inputBox.Text.Length);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (button18.Text == "<")
            {
                currentMode = WindowMode.normal;
            }
            else if (button18.Text == ">")
            {
                currentMode = WindowMode.extended;
            }
            UpdateWindow();
            
        }

        private void button_degrees_Click(object sender, EventArgs e)
        {
            if (button_degrees.Text == "Degrees")
            { button_degrees.Text = "Radians";}
            else
            { button_degrees.Text = "Degrees"; }
            inputBox_TextChanged(inputBox,e);
        }

        private void LeifGWCalc_FormClosing(object sender, FormClosingEventArgs e)
        {
            string text = button18.Text + button_degrees.Text;
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\LeifGwCalc\data.txt";
            File.WriteAllText(appdata, text);
        }

    }
}
