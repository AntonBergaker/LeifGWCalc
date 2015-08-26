﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class LeifGWCalc : Form
    {
        public LeifGWCalc()
        {
            InitializeComponent();
        }

        private void inputBox_TextChanged(object sender, EventArgs e)
        {
            string mathback = "";
            bool repeat = true;

            string maininput = (inputBox.Text);
            int parstart;
            int parend;
            string mathtosend;
            do
            {
                //MessageBox.Show(maininput);
                //find that parenthesis to send to MathDo
                char[] maininputchar;
                maininputchar = maininput.ToCharArray();
                parstart = -1;
                parend = -1;
                mathtosend = "";
                for (int i = 0; maininputchar.Length > i; i++)  {
                    if (maininputchar[i] == '(')
                    { parstart = i; }
                    else if (maininputchar[i] == ')')    {
                        if (parstart >= 0)
                            {
                            parend = i;
                            i = 9999;
                            for (var ii = parstart+1; (ii) <= (parend-1); ii++)
                                {
                                mathtosend += maininputchar[ii]; 
                            }
                            }
                        else
                            {
                            i = 9999;
                            mathtosend = "error";
                        }
                    }
                }
                if (mathtosend == "")
                    {
                        if (parstart == -1 && parend == -1)
                        {
                            mathtosend = new string(maininputchar);
                            repeat = false;
                        }
                        else
                        {
                            mathtosend = "error";
                        }
                    }
                if (mathtosend != "error" && mathtosend != "Infinity")
                {
                    mathback = MathDo(mathtosend);
                    if (parstart >= 0 && parend >= 0)
                    {
                        maininput = maininput.Remove(parstart, parend-parstart+1);
                        maininput = maininput.Insert(parstart, mathback);
                    }
                    else
                    {
                        maininput = mathback;
                        repeat = false;
                    }
                }
                else  {
                    mathback = mathtosend;
                    repeat = false;
                }
            } while (repeat == true);


            if (mathback == "Infinity") //Give a snarky response if you are mathing too hard.
            { mathback = "Great, you broke it. Think smaller."; }
            else if (mathback == "error")
            {
                mathback = '"' + gwreturn() + '"';
                returnBox.Height = 20 + 40; //make the box a bit bigger too fit his glory.
                this.Size = new Size(218, 258 + 40);
            }
            else {
                returnBox.Height = 20;
                this.Size = new Size(218, 258);
            }
            returnBox.Text = mathback; //Output the only remaining entry
        }
        private string MathDo(string inputstring)
        {
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
                        if (entries.Count != 0 && input[i] == '-') //if there's no numbers ahead of it might be a negative value
                        {
                            if (entries[entries.Count - 1] == "/" || entries[entries.Count - 1] == "*" || entries[entries.Count - 1] == "^") //see if the negative had a * or / ahead of it.
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
                    if (input[i] == ',') //Convert commas to periods
                    { input[i] = '.'; }
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
                    if (Double.TryParse(entries[i], out numbers) == false)
                    { error = true; }
                }
            }

            if (error == false) //if no errors, start counting
            {
                int index;
                //find the operators in the right order. When an operator is detected run both entries to it's sides with the operator
                while (entries.Count > 1) //keep doing this until there is only one number remaining
                {
                    if (entries.Contains("^")) //first look for power off because of the order of operations
                    {
                        index = entries.FindIndex(item => item == "^"); //find the dividors index
                        entries[index - 1] = Convert.ToString(Math.Pow(Convert.ToDouble(entries[index - 1]), Convert.ToDouble(entries[index + 1]))); //perform dividing operation
                        entries.RemoveAt(index); //remove the old entries and replace one of the old with this brand new one that has been calculated
                        entries.RemoveAt(index);
                    }
                    else if (entries.Contains("/")) //do the same for division
                    {
                        index = entries.FindIndex(item => item == "/");
                        entries[index - 1] = Convert.ToString(Convert.ToDouble(entries[index - 1]) / Convert.ToDouble(entries[index + 1]));
                        entries.RemoveAt(index);
                        entries.RemoveAt(index);
                    }
                    else if (entries.Contains("*"))
                    {
                        index = entries.FindIndex(item => item == "*");
                        entries[index - 1] = Convert.ToString(Convert.ToDouble(entries[index - 1]) * Convert.ToDouble(entries[index + 1]));
                        entries.RemoveAt(index);
                        entries.RemoveAt(index);
                    }
                    else if (entries.Contains("+"))
                    {
                        index = entries.FindIndex(item => item == "+");
                        entries[index - 1] = Convert.ToString(Convert.ToDouble(entries[index - 1]) + Convert.ToDouble(entries[index + 1]));
                        entries.RemoveAt(index);
                        entries.RemoveAt(index);
                    }
                    else if (entries.Contains("-"))
                    {
                        index = entries.FindIndex(item => item == "-");
                        entries[index - 1] = Convert.ToString(Convert.ToDouble(entries[index - 1]) - Convert.ToDouble(entries[index + 1]));
                        entries.RemoveAt(index);
                        entries.RemoveAt(index);
                    }
                }
                return Convert.ToString(entries[0]);
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
            int random = rnd1.Next(0, 14);
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
                    citat = "Gustav Fridolin är en orm.";
                    break;
                case 5:
                    citat = "Det är inte bra att ge kommunisterna allt de pekar på.";
                    break;
                case 6:
                    citat = "Jag går på korgen och får massa idéer. Ju fullare desto bättre idéer.";
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
            }
            return citat;
        }

        private void button_Click(Button button) //when pressing a button, add the buttons text too the equation, and select the window again
        {
        inputBox.Text+=button.Text;
        this.ActiveControl = inputBox;
        inputBox.Select(inputBox.Text.Length, inputBox.Text.Length);
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

    }
}
