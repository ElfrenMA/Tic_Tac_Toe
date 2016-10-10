using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Tic_Tac_Toe
{
    public partial class Form1 : Form
    {
        bool turn = true;// True is player1's turn, False player2's turn.
        int turnCount = 0;// Keeps track of number of turns passed.
        Player player1 = new Player();
        Player player2 = new Player();
        bool enableAI = false;
        bool winnerFound = false;
        List<Button> pnlGameButtons = new List<Button>();
        List<Button> pnlIcons;
        string[][] pointMap;
        StreamReader readFile;
        StreamWriter writeFile;
        /// <summary>
        /// Controls which player chooses 
        /// and icon, if true, player1 chooses 
        /// thier icon.
        /// </summary>
        bool selectPlayerIconController = true;
        string registeredPlayers = "RegisteredPlayers.txt";
        enum Level { Easy, Medium, Hard};
        int AI = 0;
        public Form1()
        {  
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // form starts on pnlPlayerDefaults.
            pnlPlayerDefaults.Visible = true;
            pnlIconSelection.Visible = false;
            pnlGame.Visible = false;
            pnlPlayerStatus.Visible = false;
            if (!File.Exists(registeredPlayers))
            {
                File.Create(registeredPlayers);
            }
            InitOrderedGameButtons();
            InitPointsSequence();
            InitPlayerIconSelection();
        }
        /// <summary>
        /// Sets buttons with player selectable icons.
        /// </summary>
        public void InitPlayerIconSelection()
        {
            Int32 i = 0;
            pnlIcons = pnlIconSelection.Controls.OfType<Button>().ToList();
            foreach (Button button in pnlIcons)
            {
                try
                {
                    button.Image = imageList1.Images[i];
                    ++i;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Elfren Authorlee - Programmer");
            MessageBox.Show("William Beasley");
        }
        /// <summary>
        /// Closes the entire Application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void button_Click(object sender, EventArgs e)
        {
            ++turnCount;// Increment Player turn.
            Button b = (Button)sender;// Cast the sender as a button.
            if (turn)// If its player1's turn.
            {
                b.Image = player1.Btn.Image;
            }
            else // If it's player2's turn.
            {
                b.Image = player2.Btn.Image;
            }
            turn = !turn; // Flips the turn to other player.
            b.Enabled = false; // Disables the button that was just clicked.
            winnerFound = checkForWinner();
            if(winnerFound)
            {
                WinnerFound();
            }
            else if ((turn == false) && (enableAI == true))
            {
                computerMakeMove();    
            }
            else// If there was a draw.
            {
                if (turnCount == 25)
                {
                    MessageBox.Show("It was a Draw");
                    player1.DrawCount += (Int32.Parse(player1.DrawCount) + 1).ToString();
                    player2.DrawCount += (Int32.Parse(player2.DrawCount) + 1).ToString();
                }
            }
        }
        public void WinnerFound()
        {
            List<Button> pnlButtonList = pnlGame.Controls.OfType<Button>().ToList();
            DisableButtons(pnlButtonList); // Disable remaining buttons after winner.
            if (turn)
            {
                player2.WinCount += (Int32.Parse(player2.WinCount) + 1).ToString();
                MessageBox.Show(txtBxPlayer2Name.Text, "Winner!");
            }
            else if (!turn)
            {
                player1.WinCount += (Int32.Parse(player1.WinCount) + 1).ToString();
                MessageBox.Show(txtBxPlayer1Name.Text, "Winner!");
            }
        }
        /// <summary>
        /// Controls the AI.
        /// </summary>
        private void computerMakeMove()
        {
            Button move = null;
            switch (AI)
            {
                case (int)Level.Easy:
                    //MessageBox.Show(Level.Easy.ToString());
                    // Priority 1: Check if ther is a winning play.
                    // Priority 2: Look for random empty space.
                    move = lookForWinOrBlock(player2.Btn.Image); // Looking for win
                    if (move == null)
                    {
                        move = lookForOpenSpace(); 
                    }
                    break;
                case (int)Level.Medium:
                    //MessageBox.Show(Level.Medium.ToString());
                    // Priority 1: Check if there is a winning play.
                    // Priority 2: Take Corner spaces.
                    // Priority 3: Look for random empty space.
                    move = lookForWinOrBlock(player2.Btn.Image); // Looking for win
                    if (move == null)
                    {
                        move = lookForCorner(); // Find a corner play.
                        if (move == null)
                        {
                            move = lookForOpenSpace();
                        }
                    }
                    break;
                case (int)Level.Hard:
                    //MessageBox.Show(Level.Hard.ToString());
                    // priority 1: Check if there is a Winning play.
                    // priority 2: Block the player from winning.
                    // priority 3: Take corner spaces.
                    // priority 4: Look for random empty space.
                    move = lookForWinOrBlock(player2.Btn.Image); // Looking for win
                    if (move == null)
                    {
                        move = lookForWinOrBlock(player1.Btn.Image); // Looking for block.
                        if (move == null)
                        {
                            move = lookForCorner(); // Find a corner play.
                            if (move == null)
                            {
                                move = lookForOpenSpace();
                            }
                        }
                    }
                    break;
            }
            move.PerformClick();
        }
        /// <summary>
        /// Checks if a point has been scored.
        /// </summary>
        private bool checkForWinner()
        {
            ////// CHECK FOR 4 IN A ROW //////
            // Check Horizontal winner.
            if (((A1.Image == A2.Image) && (A2.Image == A3.Image) && (A3.Image == A4.Image) && !A1.Enabled)
             || ((B1.Image == B2.Image) && (B2.Image == B3.Image) && (B3.Image == B4.Image) && !B1.Enabled)
             || ((C1.Image == C2.Image) && (C2.Image == C3.Image) && (C3.Image == C4.Image) && !C1.Enabled)
             || ((D1.Image == D2.Image) && (D2.Image == D3.Image) && (D3.Image == D4.Image) && !D1.Enabled)
             || ((E1.Image == E2.Image) && (E2.Image == E3.Image) && (E3.Image == E4.Image) && !E1.Enabled)
             || ((A5.Image == A4.Image) && (A4.Image == A3.Image) && (A3.Image== A2.Image) && !A5.Enabled)
             || ((B5.Image == B4.Image) && (B4.Image == B3.Image) && (B3.Image == B2.Image) && !B5.Enabled)
             || ((C5.Image == C4.Image) && (C4.Image == C3.Image) && (C3.Image == C2.Image) && !C5.Enabled)
             || ((D5.Image == D4.Image) && (D4.Image == D3.Image) && (D3.Image == D2.Image) && !D5.Enabled)
             || ((E5.Image == E4.Image) && (E4.Image == E3.Image) && (E3.Image == E2.Image) && !E5.Enabled))
             {
                 return true;
             }
            // Check Vertical winner.
            else if (((A1.Image == B1.Image) && (B1.Image == C1.Image) && (C1.Image == D1.Image) && !A1.Enabled)
             || ((A2.Image == B2.Image) && (B2.Image == C2.Image) && (C2.Image == D2.Image) && !A2.Enabled)
             || ((A3.Image == B3.Image) && (B3.Image == C3.Image) && (C3.Image == D3.Image) && !A3.Enabled)
             || ((A4.Image == B4.Image) && (B4.Image == C4.Image) && (C4.Image == D4.Image) && !A4.Enabled)
             || ((A5.Image == B5.Image) && (B5.Image == C5.Image) && (C5.Image == D5.Image) && !A5.Enabled)
             || ((E1.Image == D1.Image) && (D1.Image == C1.Image) && (C1.Image == B1.Image) && !E1.Enabled)
             || ((E2.Image == D2.Image) && (D2.Image == C2.Image) && (C2.Image == B2.Image) && !E2.Enabled)
             || ((E3.Image == D3.Image) && (D3.Image == C3.Image) && (C3.Image == B3.Image) && !E3.Enabled)
             || ((E4.Image == D4.Image) && (D4.Image == C4.Image) && (C4.Image == B4.Image) && !E4.Enabled)
             || ((E5.Image == D5.Image) && (D5.Image == C5.Image) && (C5.Image == B5.Image) && !E5.Enabled))
             {
                 return true;
             }
            // Check Diagonal winner.
            else if (((A1.Image == B2.Image) && (B2.Image == C3.Image) && (C3.Image == D4.Image) && !A1.Enabled)
             || ((A5.Image == B4.Image) && (B4.Image == C3.Image) && (C3.Image == D2.Image) && !A5.Enabled)
             || ((E5.Image == D4.Image) && (D4.Image == C3.Image) && (C3.Image == B2.Image) && !E5.Enabled)
             || ((E1.Image == D2.Image) && (D2.Image == C3.Image) && (C3.Image == B4.Image) && !E1.Enabled)
             || ((B1.Image == C2.Image) && (C2.Image == D3.Image) && (D3.Image == E4.Image) && !B1.Enabled)
             || ((A2.Image == B3.Image) && (B3.Image == C4.Image) && (C4.Image == D5.Image) && !A2.Enabled)
             || ((A4.Image == B3.Image) && (B3.Image == C2.Image) && (C2.Image == D1.Image) && !A4.Enabled)
             || ((B5.Image == C4.Image) && (C4.Image == D3.Image) && (D3.Image == E2.Image) && !B5.Enabled))
             {
                 return true;
             }
            return false;
        }
        /// <summary>
        /// Finds any avaliable space.
        /// </summary>
        /// <returns></returns>
        private Button lookForOpenSpace()
        {
            List<Button> pnlButtonList = pnlGame.Controls.OfType<Button>().ToList();
            foreach(Button b in pnlButtonList)
            {
                try
                {
                    if (b.Image == null)
                    {
                        return b;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
            return null;
        }
        /// <summary>
        ///  Finds an avaliable corner.
        /// </summary>
        /// <returns></returns>
        private Button lookForCorner()
        {
            // Find an empty corner.
            if(A1.Image ==  player2.Btn.Image)
            {
                if(A5.Image == null)
                { return A5; }
                if (E1.Image == null)
                { return E1; }
                if (E5.Image == null)
                { return E5; }
            }
            if (A5.Image == player2.Btn.Image)
            {
                if (A1.Image == null)
                { return A1; }
                if (E1.Image == null)
                { return E1; }
                if (E5.Image == null)
                { return E5; }
            }
            if (E1.Image == player2.Btn.Image)
            {
                if (A5.Image == null)
                { return A5; }
                if (A1.Image == null)
                { return A1; }
                if (E5.Image == null)
                { return E5; }
            }
            if (E5.Image == player2.Btn.Image)
            {
                if (A5.Image == null)
                { return A5; }
                if (A1.Image == null)
                { return A1; }
                if (E1.Image == null)
                { return E1; }
            }
            // If any corner.
            if (A1.Image == null) { return A1; }
            if (A5.Image == null) { return A5; }
            if (E1.Image == null) { return E1; }
            if (E5.Image == null) { return E5; }
            return null;
        }
        private Button lookForWinOrBlock(Image image)
        {
            // A Horizontal Check
            if ((A2.Image == image) && (A3.Image == image) && (A4.Image == image) && (A1.Image == null))
            { return A1; }
            if ((A1.Image == image) && (A3.Image == image) && (A4.Image == image) && (A2.Image == null))
            { return A2; }
            if ((A1.Image == image) && (A2.Image == image) && (A4.Image == image) && (A3.Image == null))
            { return A3; }
            if ((A1.Image == image) && (A2.Image == image) && (A3.Image == image) && (A4.Image == null))
            { return A4; }
            if ((A2.Image == image) && (A3.Image == image) && (A5.Image == image) && (A4.Image == null))
            { return A4; }
            if ((A2.Image == image) && (A4.Image == image) && (A4.Image == image) && (A3.Image == null))
            { return A3; }
            if ((A3.Image == image) && (A4.Image == image) && (A5.Image == image) && (A2.Image == null))
            { return A2; }
            if ((A2.Image == image) && (A3.Image == image) && (A4.Image == image) && (A5.Image == null))
            { return A5; }
            // B Horizontal Check
            if ((B2.Image == image) && (B3.Image == image) && (B4.Image == image) && (B1.Image == null))
            { return B1; }
            if ((B1.Image == image) && (B3.Image == image) && (B4.Image == image) && (B2.Image == null))
            { return B2; }
            if ((B1.Image == image) && (B2.Image == image) && (B4.Image == image) && (B3.Image == null))
            { return B3; }
            if ((B1.Image == image) && (B2.Image == image) && (B3.Image == image) && (B4.Image == null))
            { return B4; }
            if ((B2.Image == image) && (B3.Image == image) && (B5.Image == image) && (B4.Image == null))
            { return B4; }
            if ((B2.Image == image) && (B4.Image == image) && (B4.Image == image) && (B3.Image == null))
            { return B3; }
            if ((B3.Image == image) && (B4.Image == image) && (B5.Image == image) && (B2.Image == null))
            { return B2; }
            if ((B2.Image == image) && (B3.Image == image) && (B4.Image == image) && (B5.Image == null))
            { return B5; }
            // C Horizontal Check
            if ((C2.Image == image) && (C3.Image == image) && (C4.Image == image) && (C1.Image == null))
            { return C1; }
            if ((C1.Image == image) && (C3.Image == image) && (C4.Image == image) && (C2.Image == null))
            { return C2; }
            if ((C1.Image == image) && (C2.Image == image) && (C4.Image == image) && (C3.Image == null))
            { return C3; }
            if ((C1.Image == image) && (C2.Image == image) && (C3.Image == image) && (C4.Image == null))
            { return C4; }
            if ((C2.Image == image) && (C3.Image == image) && (C5.Image == image) && (C4.Image == null))
            { return C4; }
            if ((C2.Image == image) && (C4.Image == image) && (C4.Image == image) && (C3.Image == null))
            { return C3; }
            if ((C3.Image == image) && (C4.Image == image) && (C5.Image == image) && (C2.Image == null))
            { return C2; }
            if ((C2.Image == image) && (C3.Image == image) && (C4.Image == image) && (C5.Image == null))
            { return C5; }
            // D Horizontal Check
            if ((D2.Image == image) && (D3.Image == image) && (D4.Image == image) && (D1.Image == null))
            { return D1; }
            if ((D1.Image == image) && (D3.Image == image) && (D4.Image == image) && (D2.Image == null))
            { return D2; }
            if ((D1.Image == image) && (D2.Image == image) && (D4.Image == image) && (D3.Image == null))
            { return D3; }
            if ((D1.Image == image) && (D2.Image == image) && (D3.Image == image) && (D4.Image == null))
            { return D4; }
            if ((D2.Image == image) && (D3.Image == image) && (D5.Image == image) && (D4.Image == null))
            { return D4; }
            if ((D2.Image == image) && (D4.Image == image) && (D4.Image == image) && (D3.Image == null))
            { return D3; }
            if ((D3.Image == image) && (D4.Image == image) && (D5.Image == image) && (D2.Image == null))
            { return D2; }
            if ((D2.Image == image) && (D3.Image == image) && (D4.Image == image) && (D5.Image == null))
            { return D5; }
            // E Horizontal Check
            if ((E2.Image == image) && (E3.Image == image) && (E4.Image == image) && (E1.Image == null))
            { return E1; }
            if ((E1.Image == image) && (E3.Image == image) && (E4.Image == image) && (E2.Image == null))
            { return E2; }
            if ((E1.Image == image) && (E2.Image == image) && (E4.Image == image) && (E3.Image == null))
            { return E3; }
            if ((E1.Image == image) && (E2.Image == image) && (E3.Image == image) && (E4.Image == null))
            { return E4; }
            if ((E2.Image == image) && (E3.Image == image) && (E5.Image == image) && (E4.Image == null))
            { return E4; }
            if ((E2.Image == image) && (E4.Image == image) && (E4.Image == image) && (E3.Image == null))
            { return E3; }
            if ((E3.Image == image) && (E4.Image == image) && (E5.Image == image) && (E2.Image == null))
            { return E2; }
            if ((E2.Image == image) && (E3.Image == image) && (E4.Image == image) && (E5.Image == null))
            { return E5; }
            // 1 Vertical Check
            if ((B1.Image == image) && (C1.Image == image) && (D1.Image == image) && (A1.Image == null))
            { return A1; }
            if ((A1.Image == image) && (C1.Image == image) && (D1.Image == image) && (B1.Image == null))
            { return B1; }
            if ((A1.Image == image) && (B1.Image == image) && (D1.Image == image) && (C1.Image == null))
            { return C1; }
            if ((A1.Image == image) && (B1.Image == image) && (C1.Image == image) && (D1.Image == null))
            { return D1; }
            if ((C1.Image == image) && (D1.Image == image) && (E1.Image == image) && (B1.Image == null))
            { return B1; }
            if ((B1.Image == image) && (D1.Image == image) && (E1.Image == image) && (C1.Image == null))
            { return C1; }
            if ((B1.Image == image) && (C1.Image == image) && (E1.Image == image) && (D1.Image == null))
            { return D1; }
            if ((B1.Image == image) && (C1.Image == image) && (D1.Image == image) && (E1.Image == null))
            { return E1; }
            // 2 Vertical Check
            if ((B2.Image == image) && (C2.Image == image) && (D2.Image == image) && (A2.Image == null))
            { return A2; }
            if ((A2.Image == image) && (C2.Image == image) && (D2.Image == image) && (B2.Image == null))
            { return B2; }
            if ((A2.Image == image) && (B2.Image == image) && (D2.Image == image) && (C2.Image == null))
            { return C2; }
            if ((A2.Image == image) && (B2.Image == image) && (C2.Image == image) && (D2.Image == null))
            { return D2; }
            if ((C2.Image == image) && (D2.Image == image) && (E2.Image == image) && (B2.Image == null))
            { return B2; }
            if ((B2.Image == image) && (D2.Image == image) && (E2.Image == image) && (C2.Image == null))
            { return C2; }
            if ((B2.Image == image) && (C2.Image == image) && (E2.Image == image) && (D2.Image == null))
            { return D2; }
            if ((B2.Image == image) && (C2.Image == image) && (D2.Image == image) && (E2.Image == null))
            { return E2; }
            // 3 Vertical Check
            if ((B3.Image == image) && (C3.Image == image) && (D3.Image == image) && (A3.Image == null))
            { return A3; }
            if ((A3.Image == image) && (C3.Image == image) && (D3.Image == image) && (B3.Image == null))
            { return B3; }
            if ((A3.Image == image) && (B3.Image == image) && (D3.Image == image) && (C3.Image == null))
            { return C3; }
            if ((A3.Image == image) && (B3.Image == image) && (C3.Image == image) && (D3.Image == null))
            { return D3; }
            if ((C3.Image == image) && (D3.Image == image) && (E3.Image == image) && (B3.Image == null))
            { return B3; }
            if ((B3.Image == image) && (D3.Image == image) && (E3.Image == image) && (C3.Image == null))
            { return C3; }
            if ((B3.Image == image) && (C3.Image == image) && (E3.Image == image) && (D3.Image == null))
            { return D3; }
            if ((B3.Image == image) && (C3.Image == image) && (D3.Image == image) && (E3.Image == null))
            { return E3; }
            // 4 Vertical Check
            if ((B4.Image == image) && (C4.Image == image) && (D4.Image == image) && (A4.Image == null))
            { return A4; }
            if ((A4.Image == image) && (C4.Image == image) && (D4.Image == image) && (B4.Image == null))
            { return B4; }
            if ((A4.Image == image) && (B4.Image == image) && (D4.Image == image) && (C4.Image == null))
            { return C4; }
            if ((A4.Image == image) && (B4.Image == image) && (C4.Image == image) && (D4.Image == null))
            { return D4; }
            if ((C4.Image == image) && (D4.Image == image) && (E4.Image == image) && (B4.Image == null))
            { return B4; }
            if ((B4.Image == image) && (D4.Image == image) && (E4.Image == image) && (C4.Image == null))
            { return C4; }
            if ((B4.Image == image) && (C4.Image == image) && (E4.Image == image) && (D4.Image == null))
            { return D4; }
            if ((B4.Image == image) && (C4.Image == image) && (D4.Image == image) && (E4.Image == null))
            { return E4; }
            // 5 Vertical Check
            if ((B5.Image == image) && (C5.Image == image) && (D5.Image == image) && (A5.Image == null))
            { return A5; }
            if ((A5.Image == image) && (C5.Image == image) && (D5.Image == image) && (B5.Image == null))
            { return B5; }
            if ((A5.Image == image) && (B5.Image == image) && (D5.Image == image) && (C5.Image == null))
            { return C5; }
            if ((A5.Image == image) && (B5.Image == image) && (C5.Image == image) && (D5.Image == null))
            { return D5; }
            if ((C5.Image == image) && (D5.Image == image) && (E5.Image == image) && (B5.Image == null))
            { return B5; }
            if ((B5.Image == image) && (D5.Image == image) && (E5.Image == image) && (C5.Image == null))
            { return C5; }
            if ((B5.Image == image) && (C5.Image == image) && (E5.Image == image) && (D5.Image == null))
            { return D5; }
            if ((B5.Image == image) && (C5.Image == image) && (D5.Image == image) && (E5.Image == null))
            { return E5; }
            // Left to Right Diagonal Check
            if ((B2.Image == image) && (C3.Image == image) && (D4.Image == image) && (A1.Text == null))
            { return A1; }
            if ((A1.Image == image) && (C3.Image == image) && (D4.Image == image) && (B2.Text == null))
            { return B2; }
            if ((A1.Image == image) && (B2.Image == image) && (D4.Image == image) && (C3.Text == null))
            { return C3; }
            if ((A1.Image == image) && (B2.Image == image) && (C3.Image == image) && (D4.Text == null))
            { return D4; }
            if ((C3.Image == image) && (D4.Image == image) && (E5.Image == image) && (B2.Text == null))
            { return B2; }
            if ((B2.Image == image) && (D4.Image == image) && (E5.Image == image) && (C3.Text == null))
            { return C3; }
            if ((B2.Image == image) && (C3.Image == image) && (E5.Image == image) && (D4.Text == null))
            { return D4; }
            if ((B2.Image == image) && (C3.Image == image) && (D4.Image == image) && (E5.Text == null))
            { return E5; }
            // Right to Left Diagonal Check
            if ((A5.Image == image) && (B4.Image == image) && (C3.Image == image) && (D2.Text == null))
            { return D2; }
            if ((A5.Image == image) && (B4.Image == image) && (D2.Image == image) && (C3.Text == null))
            { return C3; }
            if ((A5.Image == image) && (C3.Image == image) && (D2.Image == image) && (B4.Text == null))
            { return B4; }
            if ((B4.Image == image) && (C3.Image == image) && (D2.Image == image) && (A5.Text == null))
            { return A5; }
            if ((B4.Image == image) && (C3.Image == image) && (D2.Image == image) && (E1.Text == null))
            { return E1; }
            if ((E1.Image == image) && (C3.Image == image) && (B4.Image == image) && (D2.Text == null))
            { return D2; }
            if ((E1.Image == image) && (D2.Image == image) && (B4.Image == image) && (C3.Text == null))
            { return C3; }
            if ((E1.Image == image) && (D2.Image == image) && (C3.Image == image) && (B4.Text == null))
            { return B4; }
            // Right to Left Bottom.
            if ((E2.Image == image) && (D3.Image == image) && (C4.Image == image) && (B5.Text == null))
            { return B5; }
            if ((E2.Image == image) && (D3.Image == image) && (B5.Image == image) && (C4.Text == null))
            { return C4; }
            if ((E2.Image == image) && (C4.Image == image) && (B5.Image == image) && (D3.Text == null))
            { return D3; }
            if ((D3.Image == image) && (C4.Image == image) && (B5.Image == image) && (E2.Text == null))
            { return E2; }
            // Right to Left Top.
            if ((D1.Image == image) && (C2.Image == image) && (B3.Image == image) && (A4.Text == null))
            { return A4; }
            if ((D1.Image == image) && (C2.Image == image) && (A4.Image == image) && (B3.Text == null))
            { return B3; }
            if ((D1.Image == image) && (B3.Image == image) && (A4.Image == image) && (C2.Text == null))
            { return C2; }
            if ((C2.Image == image) && (B3.Image == image) && (A4.Image == image) && (D1.Text == null))
            { return D1; }
            // Left to Right Bottom.
            if ((B1.Image == image) && (C2.Image == image) && (D3.Image == image) && (E4.Text == null))
            { return E4; }
            if ((B1.Image == image) && (C2.Image == image) && (E4.Image == image) && (D3.Text == null))
            { return D3; }
            if ((B1.Image == image) && (D3.Image == image) && (E4.Image == image) && (C2.Text == null))
            { return C2; }
            if ((C2.Image == image) && (D3.Image == image) && (E4.Image == image) && (B1.Text == null))
            { return B1; }
            // Left to Right Top.
            if ((A2.Image == image) && (B3.Image == image) && (C4.Image == image) && (D5.Text == null))
            { return D5; }
            if ((A2.Image == image) && (B3.Image == image) && (D5.Image == image) && (C4.Text == null))
            { return C4; }
            if ((A2.Image == image) && (C4.Image == image) && (D5.Image == image) && (B3.Text == null))
            { return B3; }
            if ((B3.Image == image) && (C4.Image == image) && (D5.Image == image) && (A2.Text == null))
            { return A2; }
            return null;
        }
        /// <summary>
        /// Disables remaining buttons after a winner.
        /// </summary>
        private void DisableButtons(List<Button> list)
        {
            foreach (Button button in list)
            {
                try
                { 
                    button.Enabled = false;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }
        /// <summary>
        /// Displays Icon of current Player on button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Enter(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (b.Enabled)
            {
                if (turn) // Player1's turn
                {
                    b.Image = player1.Btn.Image;
                }
                else // Player2's turn
                {
                    b.Image = player2.Btn.Image;
                }
            }
        }
        /// <summary>
        /// Clears button image after mouse over.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Leave(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (b.Enabled)
            {
                b.Image = null;
            }
        }
        /// <summary>
        /// Creates and ordered list of all buttons in grid position.
        /// </summary>
        private void InitOrderedGameButtons()
        {
            pnlGameButtons.Add(A1);
            pnlGameButtons.Add(B1);
            pnlGameButtons.Add(C1);
            pnlGameButtons.Add(D1);
            pnlGameButtons.Add(E1);
            pnlGameButtons.Add(A2);
            pnlGameButtons.Add(B2);
            pnlGameButtons.Add(C2);
            pnlGameButtons.Add(D2);
            pnlGameButtons.Add(E2);
            pnlGameButtons.Add(A3);
            pnlGameButtons.Add(B3);
            pnlGameButtons.Add(C3);
            pnlGameButtons.Add(D3);
            pnlGameButtons.Add(E3);
            pnlGameButtons.Add(A4);
            pnlGameButtons.Add(B4);
            pnlGameButtons.Add(C4);
            pnlGameButtons.Add(D4);
            pnlGameButtons.Add(E4);
            pnlGameButtons.Add(A5);
            pnlGameButtons.Add(B5);
            pnlGameButtons.Add(C5);
            pnlGameButtons.Add(D5);
            pnlGameButtons.Add(E5);
        }
        /// <summary>
        /// Creates a jagged array sequence of all possible scoring conditions.
        /// </summary>
        private void InitPointsSequence()
        {
            pointMap = new string[][]
            {
                // Vertical.
                new string[] { "A1", "B1", "C1", "D1", "E1" },
                new string[] { "A2", "B2", "C2", "D2", "E2" },
                new string[] { "A3", "B3", "C3", "D3", "E3" },
                new string[] { "A4", "B4", "C4", "D4", "E4" },
                new string[] { "A5", "B5", "C5", "D5", "E5" },
                // Vertical A-D.
                new string[] { "A1", "B1", "C1", "D1" },
                new string[] { "A2", "B2", "C2", "D2" },
                new string[] { "A3", "B3", "C3", "D3" },
                new string[] { "A4", "B4", "C4", "D4" },
                new string[] { "A5", "B5", "C5", "D5" },
                // Vertical E-B.
                new string[] { "B1", "C1", "D1", "E1" },
                new string[] { "B2", "C2", "D2", "E2" },
                new string[] { "B3", "C3", "D3", "E3" },
                new string[] { "B4", "C4", "D4", "E4" },
                new string[] { "B5", "C5", "D5", "E5" },
                // Horizontal.
                new string[] { "A1", "A2", "A3", "A4", "A5" },
                new string[] { "B1", "B2", "B3", "B4", "B5" },
                new string[] { "C1", "C2", "C3", "C4", "C5" },
                new string[] { "D1", "D2", "D3", "D4", "D5" },
                new string[] { "E1", "E2", "E3", "E4", "E5" },
                // Horizontal 1-4.
                new string[] { "A1", "A2", "A3", "A4" },
                new string[] { "B1", "B2", "B3", "B4" },
                new string[] { "C1", "C2", "C3", "C4" },
                new string[] { "D1", "D2", "D3", "D4" },
                new string[] { "E1", "E2", "E3", "E4" },
                // Horizontal 5-2.
                new string[] { "A2", "A3", "A4", "A5" },
                new string[] { "B2", "B3", "B4", "B5" },
                new string[] { "C2", "C3", "C4", "C5" },
                new string[] { "D2", "D3", "D4", "D5" },
                new string[] { "E2", "E3", "E4", "E5" },
                // Diagnoal.
                new string[] { "A1", "B2", "C3", "D4", "E5" },
                new string[] { "A5", "B4", "C3", "D2", "E1" },
                new string[] { "A2", "B3", "C4", "D5", },
                new string[] { "B1", "C2", "D3", "D4", },
                new string[] { "A4", "B3", "C2", "D1", },
                new string[] { "B5", "C4", "D3", "E2", }
            };
        }
        /// <summary>
        /// Transfers visibility to Player Status panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playerStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pnlPlayerStatus.Visible = true;
            pnlGame.Visible = false;
            pnlPlayerDefaults.Visible = false;
            pnlPlayerStatus.Visible = false;
        }
        /// <summary>
        /// Confirms game settings once start button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartGame_Click(object sender, EventArgs e)
        {
            if (txtBxPlayer1Name.Text == String.Empty)
            {
                MessageBox.Show("Enter a name for player1");
            }
            else if (txtBxPlayer2Name.Text == String.Empty)
            {
                MessageBox.Show("Enter a name for player2");
            }

            else if (txtBxPlayer1Name.Text == txtBxPlayer2Name.Text)
            {
                MessageBox.Show("Players can't have the same name");
            }
            else if(radRegisterPlayer1.Checked == false && radPlayer1AsGuest.Checked == false)
            {
                MessageBox.Show("1.Register player1\n2.Play as a guest");
            }
            else if (radRegisterPlayer2.Checked == false && radPlayer2AsGuest.Checked == false && radPlayAsAI.Checked == false)
            {
                MessageBox.Show("1.Register player2\n2.Play as a guest\n3.Play against AI");
            }
            else if (radPlayer2GoesFirst.Checked == false && radPlayer1GoesFirst.Checked == false)
            {
                MessageBox.Show("Decide who goes first.");
            }
            else
            {
                InitPlayerValues();
                // Player Icon selection screen becomes visible.
                pnlIconSelection.Visible = true;
                pnlGame.Visible = false;
                pnlPlayerDefaults.Visible = false;
                pnlPlayerStatus.Visible = false;
            }
        }
        /// <summary>
        /// Initializes player and game settings from setting panel
        /// </summary>
        private void InitPlayerValues()
        {
            player1.Name = txtBxPlayer1Name.Text;
            player1.Opponent = txtBxPlayer2Name.Text;
            player1.Turn = radPlayer1GoesFirst.Checked;
            player1.Registered = radRegisterPlayer1.Checked;
            player1.FileName =  txtBxPlayer1Name.Text + "History.txt";
            player2.Name = txtBxPlayer2Name.Text;
            player2.Opponent = txtBxPlayer1Name.Text;
            player2.Turn = radPlayer2GoesFirst.Checked;
            player2.Registered = radRegisterPlayer2.Checked;
            player2.FileName = txtBxPlayer2Name.Text + "History.txt";
            if(player1.Turn)
            {
                turn = true;
            }
            else
            {
                turn = false;
            }
            // Registers the player name if true.
            if(player1.Registered)
            {
                RegisterPlayer(player1.Name);
            }
            if(player2.Registered)
            {
                RegisterPlayer(player2.Name);
            }
            if(radPlayAsAI.Checked)
            {
                enableAI = true;
                if(radBtnEasy.Checked)
                {
                   AI =  (int)Level.Easy;
                }
                else if(radBtnMedium.Checked)
                {
                    AI = (int)Level.Medium;
                }
                else if(radBtnHard.Checked)
                {
                    AI = (int)Level.Hard;
                }
            }
        }
        /// <summary>
        /// Registers a new player.
        /// </summary>
        /// <param name="playerName"></param>
        public void RegisterPlayer(string playerName)
        {
            // Check if the name is already registered.
            if(nameIsRegisteredCheck(playerName) == false)
            {
                try
                {
                    string write = String.Empty;
                    // If RegisteredPlayer.txt exist, then append.   
                    writeFile = new StreamWriter(registeredPlayers, true);
                    // Write name to file.
                    writeFile.WriteLine(playerName);
                    // End the write object.
                    writeFile.Close();
                    writeFile = null;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        /// <summary>
        /// Checks if the name has been previously registered.
        /// return true if name is registered, false otherwise.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool nameIsRegisteredCheck(string name)
        {
            string read = String.Empty;
            try
            {
                readFile = new StreamReader(registeredPlayers);
                // If name exists in the file then exit.
                while ((read = readFile.ReadLine()) != null)
                {
                    if (name.Equals(read))
                    {
                        // End the read object.
                        readFile.Close();
                        readFile = null;
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                // End the read object.
                readFile.Close();
                readFile = null;
            }
            return false;
        }
        /// <summary>
        /// Starts a new game with new settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startNewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetGame(); // Reset all settings and buttons before switching panels.
            pnlPlayerDefaults.Visible = true;
            pnlIconSelection.Visible = false;
            pnlGame.Visible = false;
            pnlPlayerStatus.Visible = false;
        }
        /// <summary>
        /// Start a new Match with same settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void restartGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetGame(); // Reset all setting and buttons before switching panels.
            pnlGame.Visible = true;
            pnlPlayerDefaults.Visible = false;
            pnlIconSelection.Visible = false;
            pnlPlayerStatus.Visible = false;
        }
        /// <summary>
        /// Resets the grid and all values.
        /// </summary>
        private void resetGame()
        {
            player1.WinCount = "0";
            player2.WinCount = "0";
            player1.DrawCount = "0";
            player2.DrawCount = "0";
            turnCount = 0;
            turn = true; // Sets the turn back to player 1.
            winnerFound = false;
            List<Button> pnlButtonList = pnlGame.Controls.OfType<Button>().ToList();
            List<Button> pnlPlayerIcon = pnlIconSelection.Controls.OfType<Button>().ToList();
            enableButtons(pnlButtonList);
            enableButtons(pnlPlayerIcon);
            removeButtonImages(pnlButtonList);
        }
        /// <summary>
        /// Resets all buttons in the game.
        /// </summary>
        /// <param name="list"></param>
        public void enableButtons(List<Button> list)
        {
            foreach (Button button in list)
            {
                try
                {
                    if (button.Enabled == false)
                    {
                        button.Enabled = true;
                    }
                    button.Text = String.Empty;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        /// <summary>
        /// Remove all Icons from all buttons.
        /// </summary>
        /// <param name="list"></param>
        public void removeButtonImages(List<Button> list)
        {
            foreach(Button button in list)
            {
                try
                {
                    if(button.Enabled == true)
                    {
                        button.Image = null;
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        /// <summary>
        /// Saves the history of Player1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void player1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(radRegisterPlayer1.Checked == true)
            {
                player1.SaveHistory();
            }
            else
            {
                MessageBox.Show("Player1 is not registered.");
            }
        }
        /// <summary>
        /// Save the history of Player2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void player2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (radRegisterPlayer2.Checked == true)
            {
                player2.SaveHistory();
            }
            else
            {
                MessageBox.Show("Player2 is not registered.");
            }
        }
        /// <summary>
        /// Save the history of Both Players.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bothToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (radRegisterPlayer1.Checked == true && radRegisterPlayer2.Checked == true)
            {
                player1.SaveHistory();
                player2.SaveHistory();
            }
            else
            {
                MessageBox.Show("Both player are not registered.");
            }
        }
        private void pnlPlayerStatus_Paint(object sender, PaintEventArgs e)
        {
            if(player1.Registered)
            {
                player1.ReadHistory();
            }
            if(player2.Registered)
            {
                player2.ReadHistory();
            }
        }
        /// <summary>
        /// Disables or Enables the AI Difficulty Box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radPlayAsAI_CheckedChanged(object sender, EventArgs e)
        {
            if (radPlayAsAI.Checked)
            {
                txtBxPlayer2Name.Text = "AI";
                grpBxAI.Enabled = true;
                radBtnEasy.Checked = true;
            }
            else
            {
                txtBxPlayer2Name.Text = String.Empty;
                grpBxAI.Enabled = false;
                radBtnEasy.Checked = false;
                radBtnMedium.Checked = false;
                radBtnHard.Checked = false;
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string line = String.Empty;
            string searchPlayer = String.Empty;
            searchPlayer = txtSearch.Text + "History.txt";
            if(Player.FindPlayerHistory(searchPlayer))
            {
                readFile = new StreamReader(searchPlayer);
                line = readFile.ReadToEnd();
                txtResult.Text = line;
                readFile.Close();
                readFile = null;
            }
            else
            {
                MessageBox.Show("Player history not found");
            }
        }
        private void pnlPlayerDefaults_Enter(object sender, EventArgs e)
        {
            radPlayer2AsGuest.Checked = true;
            radPlayer1AsGuest.Checked = true;
        }
        private void pnlIconSelection_Paint(object sender, PaintEventArgs e)
        {
            if (radPlayer1GoesFirst.Checked)
            {
                lblPlayerToSelectIcon.Text = player1.Name + " select your Icon";
            }
            else
            {
                lblPlayerToSelectIcon.Text = player2.Name + " select your Icon";
            }
        }
        private void addIcon(object sender, EventArgs e)
        {
            if(radPlayer1GoesFirst.Checked)
            {
                lblPlayerToSelectIcon.Text = player2.Name + " select your Icon";
            }
            else
            {
                lblPlayerToSelectIcon.Text = player1.Name + " select your Icon";
            }
            Button button = (Button)sender;
            if (button.Enabled)
            {
                if (selectPlayerIconController)
                {
                    player1.Btn.Image = button.Image;
                    selectPlayerIconController = false;
                    button.Enabled = false;
                }
                else
                {
                    player2.Btn.Image = button.Image;
                    selectPlayerIconController = true; // Next button press will be for other player.
                    button.Enabled = false;
                    List<Button> pnlPlayerIcon = pnlIconSelection.Controls.OfType<Button>().ToList();   
                    DisableButtons(pnlPlayerIcon);
                    pnlGame.Visible = true;
                    pnlIconSelection.Visible = false;
                    pnlPlayerDefaults.Visible = false;
                    pnlPlayerStatus.Visible = false;
                }
            }
        }
        private void readRegisteredPlayerNames(object sender, EventArgs e)
        {
            cBxRegisteredPlayerNames1.Items.Clear();
            cBxRegisteredPlayerNames2.Items.Clear();
            string line = String.Empty;
            readFile = new StreamReader("RegisteredPlayers.txt");
            while((line = readFile.ReadLine()) != null)
            {
                cBxRegisteredPlayerNames1.Items.Add(line);
                cBxRegisteredPlayerNames2.Items.Add(line);
            }
            readFile.Close();
            readFile = null;
        }
        private void cBxRegisteredPlayerNames1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = (string)cBxRegisteredPlayerNames1.SelectedItem;
            txtBxPlayer1Name.Text = str;
        }
        private void cBxRegisteredPlayerNames2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = (string)cBxRegisteredPlayerNames2.SelectedItem;
            txtBxPlayer2Name.Text = str;
        }
    }
}