using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using static System.Formats.Asn1.AsnWriter;

namespace FinalProject
{ 


    // You may see "GIR" throughout the program files, this stands for "Greens in Regulation"
    // A green in regulation is when you are "on the green" in the appropriate number of strokes (par - 2)

    // Scores for golf
    public enum Score
    {
        BtB = -2, // better than birdie
        Birdie = -1,
        Par = 0,
        Bogey = 1,
        WtB = 2 // Worse than bogey
    }

    // Overridng of the List class to add functionality to it (calculations)
    public class RoundList : List<GolfRound>
    {

        private int totalScore;
        private int totalHoles;
        private int totalPutts;
        private int totalGIR;

        private int? bestScore;
        IDictionary<Score, int> scoreMap;

        [JsonConstructor]
        public RoundList() : base()
        {
            this.scoreMap= new Dictionary<Score, int>();
            scoreMap[Score.BtB]= 0;
            scoreMap[Score.Birdie] = 0;
            scoreMap[Score.Par] = 0;
            scoreMap[Score.Bogey] = 0;
            scoreMap[Score.WtB] = 0;

            this.totalScore = 0;
            this.totalPutts = 0;
            this.totalHoles = 0;
            this.totalGIR = 0;
        }


        public new void Add(GolfRound round)
        {
            base.Add(round);
            calculate();
        }

        public new void Remove(GolfRound round)
        {
            base.Remove(round);
            calculate();
        }

        // calculates and stores stats for later use
        public void calculate()
        {
            scoreMap[Score.BtB] = 0;
            scoreMap[Score.Birdie] = 0;
            scoreMap[Score.Par] = 0;
            scoreMap[Score.Bogey] = 0;
            scoreMap[Score.WtB] = 0;

            totalScore = 0;
            totalPutts = 0;
            totalHoles = 0;
            totalGIR = 0;

            bestScore = 1000;
            bool foundBestScore = false;
            
            // Add best score on 9
            // add best overall
            foreach (GolfRound round in this)
            {
                totalScore += round.TotalScore;
                if (round.TotalScore < bestScore && round.Holes.Count == 18)
                {
                    bestScore = round.TotalScore;
                    foundBestScore = true;
                }

                foreach (GolfHole hole in round.Holes)
                {
                    totalPutts += hole.Putts;
                    totalHoles++;

                    if(hole.capturedGIR())
                    {
                        totalGIR++;
                    }

                    int score = hole.Score - hole.Par;
                    if (score < -2) score = -2;
                    if (score > 2) score = 2;
                    scoreMap[(Score)score] += 1;
                }
            }
            Sort();

            if(!foundBestScore)
            {
                bestScore = null;
            }

        }

        public double AverageScore
        {
            get 
            { 
                if(Count < 1) return 0;
                else return (float)(totalScore * 18) / (float)(totalHoles);
            }
        }
        public int? BestScore
        {
            get { return bestScore; }
        }

        public double GetPct(Score score)
        {
            if (Count < 1)
            {
                return 0;
            }
            else
            {
                return ((float)scoreMap[score] / (float)totalHoles) * 100;
            }
        }

        public double PuttsPerHole
        {
            get
            {
                if(Count < 1)
                {
                    return 0;
                }
                else
                {
                    return (float)totalPutts / (float)totalHoles;
                }
            }
        }
        public double GIR_PCT
        {
            get 
            {
                

                if(Count < 1)
                {
                    return 0;
                }
                else
                {
                    return (double)totalGIR / (double)totalHoles * 100;
                }
            }
        }
    }

    public class GolfRound : IComparable<GolfRound>
    {
        public String Date { get; set; }
        public String CourseName { get; set; }
        public List<GolfHole> Holes { get; set; }

        public int TotalPar { get; set; }
        public int TotalScore { get; set; }


        [JsonConstructor]
        public GolfRound(String Date, String CourseName, List<GolfHole> Holes) { 
            this.Date = Date;
            this.CourseName = CourseName;
            this.Holes = Holes;

            this.TotalPar = 0;
            this.TotalScore = 0;

            foreach(GolfHole hole in Holes)
            {
                TotalPar += hole.Par;
                TotalScore += hole.Score;
            }
        }

        public int CompareTo(GolfRound other)
        {
            if(other.TotalScore > this.TotalScore) return -1;
            if (other.TotalScore < this.TotalScore) return 1;
            return 0;
        }

        public double GIR_PCT()
        {
            int greensInReg = 0;
            foreach (GolfHole hole in Holes)
            {
                if (hole.capturedGIR())
                {
                    greensInReg++;
                }
            }
            return (double)greensInReg / Holes.Count * 100;
        }
    }

    public class GolfHole
    {
        [JsonPropertyName("Par")]
        public int Par { get; set; }

        [JsonPropertyName("Score")]
        public int Score { get; set; }

        [JsonPropertyName("Putts")]
        public int Putts { get; set; }


        [JsonConstructor]
        public GolfHole(int Par, int Score, int Putts) { 
            this.Par = Par;
            this.Score = Score;
            this.Putts = Putts;
        }

        public bool capturedGIR()
        {
            if(Score - 2 <= Par - 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
