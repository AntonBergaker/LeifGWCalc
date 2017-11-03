using System;
using System.Drawing;
using System.Windows.Forms;

namespace LeifGWCalc
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

            string text = (string)Properties.Settings.Default["WindowMode"];
            if (text == "<")
            {
                button18.Text = "<";
                currentMode = WindowMode.extended;
                UpdateWindow();
            }
            else if (text == ">")
            {
                currentMode = WindowMode.normal;
                UpdateWindow();
            }
            else
            {
                currentMode = WindowMode.minimal;
                UpdateWindow();
            }

            button_degrees.Text = (string)Properties.Settings.Default["AngleMode"];
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

            Properties.Settings.Default["WindowMode"] = button18.Text;
            Properties.Settings.Default.Save();
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
                    returnBox.Text = '"' + GetGWQuote() + '"';
                }
                else
                {
                    returnBox.Text = output;
                    hasQuote = false;
                    resizeBox();
                }
            }
        }

        private string GetGWQuote()
        {
            //enter citat from our lord and saviour Leif Gw Persson if there is an error.
            Random random = new Random();
            string[] quotes = new string[] {
                "Mitt yngsta barnbarn är rödfnasigt och ser ut some en ung Winston Churchhill. Minus cigarren.",
                "Du har väldigt bastanta små händer. Har du tänkt på det?",
                "Till skillnad från dig så har jag faktiskt gått igenom de där handlingarna, så jag är inte lika imponerad.",
                "Det finns riktigt bra hembränt på några ställen och det brukar jag då inte tacka nej till.",
                "Jakt är en jävla udda sysselsättning - att hålla på och skjuta på djur. Ett utmärkt sätt att förhärda en människa på.",
                "Det är inte bra att ge kommunisterna allt de pekar på.",
                "Jag går på krogen och får massa idéer. Ju fullare desto bättre idéer.",
                "Det är ganska trevligt att vara populär.",
                "Jag betalar åtminstånde ett par daghem varje år på mina skatter.",
                "En gång var det en tomte som gjorde en ändring utan att fråga. Honom blev jag galen på.",
                "Om jag äter sill kan jag ta en öl. Men bara några klunkar för att skölja ner vodkan.",
                "Andra författare väljer platser där de själva har bott. De lata jävlarna. De behöver inte göra sin research.",
                "Jag hade fel om Fridolin.",
                "Polisens mordvapenarkiv, det är ett sånt ställe jag skulle kunna åldras och då på.",
                "Mamma tog mina aktier.",
                "Paradise Hotel. Det är ett antal ungdomspsykopater som för säkerhets skull är på fyllan.",
                "Jag är ju ganska gammal och behöver tid att tänka. Så jag har den här framtoningen för att ge mig själv mer tid",
                "Ge barnen pengar och säg åt dem att inte dränka sig själva.",
                "Normalt hoppar jag omkring. Men då är jag noga med att bara mina närmaste kan se mig.",
                "Jag vet inte varför mina böcker säljer så dåligt i Norge. För lite bilder eller för små bokstäver kanske.",
                "Om jag har blivit inbjuden till Nobelfesten? Ja, men jag tror inte jag har svarat. Den där inbjudan ligger någonstans.",
                "Jag har kallat folk för massa fula saker. Men inget som jag ångrar direkt, det vill jag inte påstå."
            };
            return (quotes[random.Next(0,quotes.Length)]);

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

            Properties.Settings.Default["AngleMode"] = button_degrees.Text;
            Properties.Settings.Default.Save();
        }
    }
}
