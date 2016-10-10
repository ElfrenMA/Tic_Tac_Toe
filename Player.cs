using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
namespace Tic_Tac_Toe
{
    public class Player
    {
        #region member variables
        private string _name;
        private string _winCount;
        private string _loseCount;
        private string _drawCount;
        private string _date; // For file read.
        private bool _registered;
        private bool _turn;
        string _fileName;
        DateTime date = DateTime.Now;
        private string _opponentName;
        StreamWriter wFile;
        StreamReader rFile;
        Button button;
        private List<Button> marks = new List<Button>();
        #endregion
        #region Properties
        public string Name
        {
            get
            {
                string soft = _name;
                return soft;
            }
            set
            {
                _name = value;
            }
        }
        public string WinCount
        {
            get
            {
                string soft = _winCount;
                return soft;
            }
            set
            {
                _winCount = value;    
            }
        }
        public string LoseCount
        {
            get
            {
                string soft = _loseCount;
                return soft;
            }
            set
            {
              _loseCount = value; 
            }
        }
        public string DrawCount
        {
            get
            {
                string soft = _drawCount;
                return soft;
            }
            set
            {
                _drawCount = value;
            }
        }
        public string Opponent
        {
            get
            {
                string soft = _opponentName;
                return soft;
            }
            set
            {
                _opponentName = value;
            }
        }
        public Button AddMark
        {
            set
            {
                try
                {
                    marks.Add(value as Button);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        public bool Registered
        {
            get
            {
                bool soft = _registered;
                return soft;
            }
            set
            {
                _registered = value;
            }
        }
        public bool Turn
        {
            get
            {
                bool soft = _turn;
                return soft;
            }
            set
            {
                _turn = value;
            }
        }
        public string FileName
        {
            get
            {
                string soft = _fileName;
                return soft;
            }
            set
            {
                _fileName = value;
            }
        }
        public Button Btn
        {
            get
            {
                Button soft = button;
                return soft;
            }
            set
            {
                button = value;
            }
        }
        #endregion
        #region Members
        public Player()
        {
            _winCount = "0";
            _loseCount = "0";
            _drawCount = "0";
            _fileName = String.Empty;
            _date = String.Empty;
            _registered = false;
            _turn = false;
            wFile = null;
            rFile = null;
            button = new Button();
        }
        static public bool FindPlayerHistory(string name)
        {
            try
            {
                if (File.Exists(name))
                {
                    return true;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return false;
        }
        public void ReadHistory()
        {
            if (File.Exists(_fileName)) 
            {
                try
                {
                    rFile = new StreamReader(_fileName);
                    _date = rFile.ReadLine(); // Read Date
                    _name = rFile.ReadLine(); // Name
                    _opponentName = rFile.ReadLine(); // Opponent Name
                    _winCount = rFile.ReadLine(); // Win Count
                    _loseCount = rFile.ReadLine(); // Lose Count
                    _drawCount = rFile.ReadLine(); // Draw Count
                    rFile.Close();
                    rFile = null;
                }
                catch(IOException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                MessageBox.Show("No player history found");
                MessageBox.Show("Create ");
            }
        }
        public void SaveHistory()
        {
            //string fileName = _name.Insert(_name.Length - 1, "History.txt");
            File.Create(_fileName);
            try
            {
                MessageBox.Show(_fileName, "Opened File");
                wFile = new StreamWriter(_fileName, true);
                wFile.WriteLine("Date: {0}", date.ToString("MM/dd/yyy HH:mm:ss.fff", CultureInfo.InvariantCulture));
                wFile.WriteLine("Player Name: {0}", _name);
                wFile.WriteLine("Opponent: {0}", _opponentName);
                wFile.WriteLine("Wins: {0}", _winCount);
                wFile.WriteLine("Loses: {0}", _loseCount);
                wFile.WriteLine("Draws: {0}", _drawCount);
                MessageBox.Show(_fileName + " was saved");
                wFile.Close();
                wFile = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion
    }
}
