using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Condorcet_Winners
{
    class VM : INotifyPropertyChanged
    {
        #region Constants
        const int CLEAR_SCORE = 0;
        const int INITIAL_ID = 0;
        const int CANDIDATE_ID_ADJUSTMENT = 1;
        const int TIMES_OF_WINNER_ADJUSTMENT = 1;
        const int WINNER_CONDITION_FACTOR = 2;
        const string NO_WINNER = "There is no winner!";
        #endregion

        #region Properties
        string displayMessage;
        public string DisplayMessage
        {
            get { return displayMessage; }
            set { displayMessage = value; NotifyChange(); }
        }

        ObservableCollection<LineText> lineTexts = new ObservableCollection<LineText>();
        public ObservableCollection<LineText> LineTexts
        {
            get { return lineTexts; }
            set { lineTexts = value; NotifyChange(); }
        }

        ObservableCollection<Ballot> ballots = new ObservableCollection<Ballot>();
        public ObservableCollection<Ballot> Ballots
        {
            get { return ballots; }
            set { ballots = value; NotifyChange(); }
        }

        List<WinnerNumber> winnerNumbers = new List<WinnerNumber>();
        public List<WinnerNumber> WinnerNumbers
        {
            get { return winnerNumbers; }
            set { winnerNumbers = value; NotifyChange(); }
        }
        #endregion

        #region Method
        public void Compare()
        {
            int candidateId;
            int b = ballots[INITIAL_ID].SingleBallot.Length;
            int c = ballots.Count();
            //because candidates will need to compare with others on an one-on-one contest, we need to have a loop for the candidate and a loop for the competitors.
            //switching the current candidate. The current candidate changes only when he finishes competing all others.
            for (candidateId = INITIAL_ID; candidateId < c; candidateId++)
            {
                //if one candidate wants to win, he needs to beat all others, so compare the current candidate with all competitors
                for (int competitorId = INITIAL_ID; competitorId < c; competitorId++)
                {
                    int candidateScore = CLEAR_SCORE;
                    //can not compete themselves
                    if (candidateId != competitorId)
                    {
                        //compare the current candidate with a certain competitor through each ballot
                        for (int ballotId = INITIAL_ID; ballotId < b; ballotId++)
                        {
                            //in each ballot, if the position of candidateId is head of the competitorId, the candidateId gets one point
                            if (Array.IndexOf(ballots[ballotId].SingleBallot, candidateId) < Array.IndexOf(ballots[ballotId].SingleBallot, competitorId))
                                //candidateId gets one point
                                candidateScore++;
                        }
                        //the current candidate beats competitor when he get over b/2 points.                                 
                        if (candidateScore > b / WINNER_CONDITION_FACTOR)
                        {
                            WinnerNumber winnerNumber = new WinnerNumber()
                            {
                                //add candidateId to the list
                                WinnerId = candidateId
                            };
                            winnerNumbers.Add(winnerNumber);
                        }
                    }
                }
                //Count the number of rows in the list. if it is equal to c-1, this candidate beats all competitors.
                if (winnerNumbers.Count() == c - TIMES_OF_WINNER_ADJUSTMENT)
                {
                    DisplayMessage = string.Format("The winner is {0}", candidateId);
                    break;
                }
                //no one beats all other candidates
                if (candidateId == c - CANDIDATE_ID_ADJUSTMENT)
                {
                    DisplayMessage = NO_WINNER;
                }
                winnerNumbers.Clear();
            }
            winnerNumbers.Clear();
        }

        public void LoadVotes()
        {
            lineTexts.Clear();
            ballots.Clear();
            //read txt through lines
            string[] lines = File.ReadAllLines("vote.txt");
            //lines = lines.Skip(1).ToArray(); use this line if b, c are entered in the first line;
            //go through each line of data
            foreach (string line in lines)
            {
                //Because listbox cannot show array in each line, this list is used to show the data from the file in the listbox
                LineText lineText = new LineText
                {
                    VoteResultPerLine = line
                };
                lineTexts.Add(lineText);
                //split each line of data as array
                string[] lineContent = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Ballot ballot = new Ballot
                {
                    //convert string[] to int[]
                    SingleBallot = Array.ConvertAll(lineContent, int.Parse)
                };
                ballots.Add(ballot);
            }
            DisplayMessage = "";
        }

        public VM()
        {
            LoadVotes();
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyChange([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
