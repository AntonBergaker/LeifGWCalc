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
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\LeifGwCalc");
                File.Create(appdata);
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
                //clean the input
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                provider.NegativeSign = "-";
                provider.NegativeInfinitySymbol = "Infinity";
                provider.PositiveInfinitySymbol = "Infinity";
                provider.NumberGroupSeparator = "";

                string mathback = "";
                bool repeat = true;

                string mainInput = (inputBox.Text);

                //clean the input

                //cleanup invalid signs and replace with valid signs
                mainInput = mainInput.
                ToLower().
                Replace(",", ".").
                Replace("x", "*").
                Replace("square root", "root").
                Replace(" ", "");


                //add extra paranthesis if needed
                int paraCount = 0;
                foreach (char a in mainInput)
                {
                    if (a == ')')
                    { paraCount++; }
                    else if (a == '(')
                    { paraCount--; }
                }

                if (paraCount < 0)
                {
                    for (int i = paraCount; i < 0; i++)
                    { mainInput += ")"; }
                }
                else if (paraCount > 0)
                {
                    for (int i = 0; i < paraCount; i++)
                    { mainInput = "(" + mainInput; }
                }

                int parstart;
                int parend;
                string mathtosend;
                do
                {
                    //find that parenthesis to send to MathDo
                    char[] maininputchar;
                    maininputchar = mainInput.ToCharArray();
                    parstart = -1;
                    parend = -1;
                    mathtosend = "";

                    for (int i = 0; maininputchar.Length > i; i++)  { //look for beginning parenthasis
                        if (maininputchar[i] == '(') //if found set it as active
                        { parstart = i; }
                        else if (maininputchar[i] == ')')    { //if a ) is found it mean parenthasis over(duh) but with this setup it will always grab the right one furthest in
                            if (parstart >= 0)
                                {
                                parend = i;
                                i = 9999;
                                for (var ii = parstart+1; (ii) <= (parend-1); ii++) //insert the par into the part to send to MathDo
                                    {
                                    mathtosend += maininputchar[ii]; 
                                    }
                                }
                            else //invalid number of ) means error
                            {
                                i = 9999;
                                mathtosend = "error";
                            }
                        }
                    }
                    if (mathtosend == "") //no paranthesis found or empty string
                        {
                            if (parstart == -1 && parend == -1)
                            {
                                mathtosend = new string(maininputchar);
                                repeat = false;
                            }
                            else
                            {
                                mathtosend = "error"; //honestly this always triggers
                            }
                        }
                    if (mathtosend != "error" && mathtosend != "Infinity" && mathtosend != "")
                    {
                        mathback = MathDo(mathtosend,provider); //do math
                        if ((mathback != "" && mathback != "error" && mathback != "INF" && mathback != "Infinity") && parstart >= 0 && parend >= 0) //if the return stiring was a () do some extra fun
                        {
                            if (parstart >= 1)
                            {
                                int operatorLength = 1;
                                bool didoperation = false;
                                double var_modifier;
                                if (button_degrees.Text == "Degrees")
                                { var_modifier = Math.PI / 180; }
                                else
                                { var_modifier = 1;}
                                if (parstart >= 3)
                                {
                                    //check if it was a sin/tan/cos/root paranthesis
                                    if (mainInput.Substring(parstart - 3, 3) == "sin")
                                    {
                                        mathback = Convert.ToString(Math.Sin(Convert.ToDouble(mathback, provider) * var_modifier), provider);
                                        didoperation = true;
                                        operatorLength = 3;
                                    }
                                    if (mainInput.Substring(parstart - 3, 3) == "cos")
                                    {
                                        mathback = Convert.ToString(Math.Cos(Convert.ToDouble(mathback, provider) * var_modifier), provider);
                                        didoperation = true;
                                        operatorLength = 3;
                                    }
                                    if (mainInput.Substring(parstart - 3, 3) == "tan")
                                    {
                                        mathback = Convert.ToString(Math.Tan(Convert.ToDouble(mathback, provider) * var_modifier), provider);
                                        didoperation = true;
                                        operatorLength = 3;
                                    }
                                }
                                if (parstart >=4)
                                {
                                    if (mainInput.Substring(parstart - 4, 4) == "root")
                                    {
                                        mathback = Convert.ToString(Math.Sqrt(Convert.ToDouble(mathback, provider)), provider);
                                        didoperation = true;
                                        operatorLength = 4;
                                    }
                                }
                                if (mainInput.Substring(parstart-1,1) == "√")
                                {
                                    mathback = Convert.ToString(Math.Sqrt(Convert.ToDouble(mathback, provider)), provider);
                                    didoperation = true;
                                    operatorLength = 1;
                                }

                            if (didoperation == true)
                            {
                                if (parstart >= operatorLength+1)
                                {
                                    if (isnumber(mainInput[parstart-operatorLength]) == true)
                                    {
                                        mainInput = mainInput.Insert(parstart - operatorLength, "*");
                                        parend++;
                                        parstart++;
                                    }
                                }

                                parstart -= operatorLength;
                                }
                            }
                            if (parend >= 0 && parend < (mainInput.Length-1)) //if operation was done with )( aka multiplication with paranthesis
                            {
                                if (mainInput[parend + 1] == '(')
                                {
                                    mainInput = mainInput.Insert(parend+1, "*");
                                }
                                else if (isnumber(mainInput[parend + 1]) == true)
                                {
                                    mainInput = mainInput.Insert(parend + 1, "*");
                                }
                            }
                            if (parstart >= 1)
                            {
                                if (isnumber(mainInput[parstart - 1]) == true)
                                {
                                    mainInput = mainInput.Insert(parstart, "*");
                                    parstart++;
                                    parend++;
                                }
                            }
                            mainInput = mainInput.Remove(parstart, 1+parend-parstart);
                            mainInput = mainInput.Insert(parstart, mathback);
                        }
                        else
                        {
                            mainInput = mathback;
                            repeat = false;
                        }
                    }
                    else  {
                        mathback = mathtosend;
                        repeat = false;
                    }
                } while (repeat == true);

                if (mathback == "Infinity" || mathback == "INF") //Give a snarky response if you are mathing too hard.
                { mathback = "Great, you broke it. Think smaller."; }
                else if (mathback == "error" || mathback == "") //return GW citat
                {
                    hasQuote = true;
                    resizeBox();
                    returnBox.Text = '"' + gwreturn() + '"';
                }
                else {
                    hasQuote = false;
                    resizeBox();
                    returnBox.Text = Convert.ToString(Convert.ToDouble(mathback, provider)); //Output the only remaining entry with the formatting of the users choice
                }
            }
        }
        private string MathDo(string inputstring, NumberFormatInfo provider)
        {
            NumberStyles style = new NumberStyles();
            style = NumberStyles.Any;

            char[] input;
            input = inputstring.ToCharArray();
            string before = "";
            bool error = false;
            string nextisnegative = "";

            //divide all the entries into a list with operators and number blocks
            //A list may look like this: 500,/,2,*,7
            List<string> entries = new List<string>();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '+' || input[i] == '-' || input[i] == '/' || input[i] == '*' || input[i] == '^') //check too see if the current char is a operator
                {   
                    if (before != "") //Check too see if something came before it, so we can find negative values
                    {
                        entries.Add(nextisnegative + before); //add the value and if it had a - ahead of it make it negative
                        before = "";
                        nextisnegative = "";
                        entries.Add(Convert.ToString(input[i])); //add what was before it, and itself too the list
                    }
                    else
                    {
                        if (entries.Count >= 1 && input[i] == '-') //if there's no numbers ahead of it it might be a negative value
                        {
                            if (entries[entries.Count - 1] == "/" || entries[entries.Count - 1] == "*" || entries[entries.Count - 1] == "^") //see if the negative had a *, ^ or / ahead of it.
                            {
                                nextisnegative = "-";
                            }
                            else //if neither, trow an error!
                            { error = true; }
                        }
                        else //if it's the first number check if it's a negative
                        {
                            if (input[i] == '-')
                            { nextisnegative = "-"; }
                            else //if the first character is not a number or a negative throw an error
                            { error = true; }
                        }
                    }
                }
                else
                {
                    before += input[i];
                }

            }
            //add the final entry
            if (before != "")
            {
                entries.Add(nextisnegative + before);
                before = "";
            }

            //check for errors
            if (inputBox.Text == "") //if the box is empty throw an error
            { error = true; }

            if (entries.Count % 2 == 0) //if there are as many operators as numbers throw an error
            { error = true; }

            double numbers;
            if (error == false) //if no errors so far, check if number blocks doesn't contain other than numbers
            {
                for (int i = 0; i < entries.Count; i+=2)
                {
                    if (Double.TryParse(entries[i], style, provider, out numbers) == false)
                    { error = true; }
                }
            }

            if (error == false) //if no errors, start counting
            {

                int index, index2;
                //find the operators in the right order. When an operator is detected run both entries to it's sides with the operator
                while (entries.Count > 1) //keep doing this until there is only one number remaining
                {
                    if (entries.Contains("^")) //first look for power off because of the order of operations
                    {
                        index = entries.IndexOf("^"); //find the dividors index
                        entries[index - 1] = Convert.ToString(Math.Pow(Convert.ToDouble(entries[index - 1], provider), Convert.ToDouble(entries[index + 1], provider)),provider); //perform dividing operation
                        entries.RemoveAt(index); //remove the old entries and replace one of the old with this brand new one that has been calculated
                        entries.RemoveAt(index);
                    }
                    else if (entries.Contains("/") || entries.Contains("*")) //find divide or multiply first
                    {
                        index = entries.IndexOf("*");
                        index2 = entries.IndexOf("/");
                        
                        if (index == -1 || (index2>=0 && index2 < index))
                        {
                            index = index2;
                            entries[index - 1] = Convert.ToString(Convert.ToDouble(entries[index - 1], provider) / Convert.ToDouble(entries[index + 1], provider),provider);
                        }
                        else
                        { entries[index - 1] = Convert.ToString(Convert.ToDouble(entries[index - 1], provider) * Convert.ToDouble(entries[index + 1], provider),provider); }
                        
                        entries.RemoveAt(index);
                        entries.RemoveAt(index);
                    }
                    else if (entries.Contains("+") || entries.Contains("-")) //find add or subtract
                    {
                        index = entries.IndexOf("+");
                        index2 = entries.IndexOf("-");
                        
                        if (index == -1 || (index2>=0 && index2 < index)) //if you should do subtract first
                        {
                            index = index2;
                            entries[index - 1] = Convert.ToString(Convert.ToDouble(entries[index - 1], provider) - Convert.ToDouble(entries[index + 1], provider),provider);
                        }
                        else //else add
                        { entries[index - 1] = Convert.ToString(Convert.ToDouble(entries[index - 1], provider) + Convert.ToDouble(entries[index + 1], provider), provider); }
                        
                        entries.RemoveAt(index);
                        entries.RemoveAt(index);
                    }
                }
                return Convert.ToString(entries[0],provider);
            }
            else
            {
                return "error";
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

        private bool isnumber(char c)
        {
            if (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9')
            { return true; }
            else
            { return false; }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            button_Click(button1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button_Click(button2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button_Click(button3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button_Click(button4);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button_Click(button5);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button_Click(button6);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button_Click(button7);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button_Click(button8);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            button_Click(button9);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            button_Click(button12);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            button_Click(button14);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            button_Click(button11);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            button_Click(button13);
        }

        private void button10_Click(object sender, EventArgs e) //backspace
        {
            if (inputBox.Text.Length > 0)
            { inputBox.Text = inputBox.Text.Remove(inputBox.Text.Length - 1, 1); }
            this.ActiveControl = inputBox;
            inputBox.Select(inputBox.Text.Length, inputBox.Text.Length);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            button_Click(button15);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            button_Click(button17);
        }

        private void button16_Click(object sender, EventArgs e) //clear
        {
            inputBox.Text = "";
            this.ActiveControl = inputBox;
            inputBox.Select(inputBox.Text.Length, inputBox.Text.Length);
        }

        private void returnBox_TextChanged(object sender, EventArgs e)
        {

        }
        private void CheckEnter(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                returnBox.BackColor = Color.Turquoise;
            }
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

        private void button_sin_Click(object sender, EventArgs e)
        {
            button_Click("sin(");
        }

        private void button_leftpar_Click(object sender, EventArgs e)
        {
            button_Click(button_leftpar);
        }

        private void button_rightpar_Click(object sender, EventArgs e)
        {
            button_Click(button_rightpar);
        }

        private void button_power_Click(object sender, EventArgs e)
        {
            button_Click(button_power);
        }

        private void button_cos_Click(object sender, EventArgs e)
        {
            button_Click("cos(");
        }

        private void button_tan_Click(object sender, EventArgs e)
        {
            button_Click("tan(");
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
            if (!File.Exists(appdata))
            { File.Create(appdata); }
            File.WriteAllText(appdata, text);
        }

        private void button_root_Click(object sender, EventArgs e)
        {
            button_Click("√(");
        }


    }
}
