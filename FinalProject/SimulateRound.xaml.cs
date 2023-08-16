using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Threading;

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for GenerateRound.xaml
    /// </summary>
    public partial class SimulateRound : UserControl
    {
        RoundList rounds;

        System.Timers.Timer imageTimer;
        int imageNum;
        int ticksLeft;
        bool inUse;
        public SimulateRound(RoundList rounds)
        {
            this.rounds = rounds;
            InitializeComponent();
            swingImage.Source = LoadBitmap(@"swing1.png");

            imageTimer = new System.Timers.Timer();
            imageTimer.Interval = 750;
            imageTimer.Elapsed += ChangeImage;
            inUse = false;

        }

        protected BitmapImage LoadBitmap(String swingRelativePath, double decodeWidth = 500)
        {
            BitmapImage theBitmap = new BitmapImage();
            theBitmap.BeginInit();
            String basePath = System.IO.Path.Combine(Environment.CurrentDirectory, @"assets\swingPhotos\");
            String path = System.IO.Path.Combine(basePath, swingRelativePath);
            theBitmap.UriSource = new Uri(path, UriKind.Absolute);
            theBitmap.DecodePixelWidth = (int)decodeWidth;
            theBitmap.EndInit();

            return theBitmap;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

            if(inUse)
            {
                imageTimer.Stop(); 
            }
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new HomePage(this.rounds));

        }

        
        // starts the timer, when the timer is up it will spit out the round 
        private void GenerateRound_Click(object sender, RoutedEventArgs e)
        {

            if (!inUse)
            {

                try
                {
                    int handicap = Convert.ToInt32(handicapEntry.Text);

                    if(handicap < -10 || handicap > 30)
                    {
                        MessageBoxResult enterRound = MessageBox.Show("Strange handicap detected. Would you like to continue with a handicap of " 
                                                                        + handicap.ToString() + "?", "Are you sure?", MessageBoxButton.YesNo);
                        switch (enterRound)
                        {
                            case MessageBoxResult.Yes:
                                break;
                            case MessageBoxResult.No:
                                return;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid handicap format.");
                    return;
                }

                imageNum = 0;
                ticksLeft = 8;

                imageTimer.Start();

                generatingText.Text = "Generating round...";

                inUse = true;
            }
        }

        private void ChangeImage(object sender, EventArgs e)
        {
            // do not need to be on a truly seperate thread, only changing the UI and want to wait on further execution
            Dispatcher.Invoke(() =>
            {
                imageNum += 1;
                if (imageNum > 4)
                {
                    imageNum = 1;
                }
                ticksLeft--;
                String newSwingImage = "swing" + imageNum.ToString() + ".png";
                swingImage.Source = LoadBitmap(newSwingImage);

                if (ticksLeft <= 0)
                {
                    imageTimer.Stop();
                    inUse = false;
                    generatingText.Text = "Complete!";
                    CalculateGeneratedRound();
                    
                }
            });
            

            
        }

        // calcuates the states from a desired handicap
        private void CalculateGeneratedRound()
        {
            if (handicapEntry.Text.Length <= 0)
            {
                MessageBox.Show("No Handicap Entered. Please enter a handicap!");
                return;
            }

            int handicap = Convert.ToInt32(handicapEntry.Text); ;

            Random rand = new Random();

            int overallScore = handicap + 72 + 4; // adds 4 to offset handicap advantage
            
            // determines the score for each hole randomly from a randomly generated overall score
            // (the result overall score of the hole will change)
            double avgScorePerHole = (double)overallScore / 18.00;
            double lowEnd = avgScorePerHole - 1.7;
            double highEnd = avgScorePerHole + 1.7;
            List<int> scores = new List<int>();


            for (int i = 0; i < 18; i++)
            {
                double randScore = rand.NextDouble() * (highEnd - lowEnd) + lowEnd;
                if (randScore < lowEnd + 0.3)
                {
                    scores.Add((int)Math.Floor(randScore));
                }
                else if (randScore >= lowEnd + 0.3 && randScore < avgScorePerHole)
                {
                    scores.Add((int)Math.Ceiling(randScore));
                }
                else if (randScore >= avgScorePerHole && randScore < highEnd - 0.3)
                {
                    scores.Add((int)Math.Floor(randScore));
                }
                else
                {
                    scores.Add((int)Math.Ceiling(randScore));
                }
            }


            // determines the putts for each hole randomly
            double avgPuttsPerHole = (double)overallScore / 36.00;
            lowEnd = avgPuttsPerHole - 1;
            highEnd = avgPuttsPerHole + 1;
            List<int> putts = new List<int>();


            for (int i = 0; i < 18; i++)
            {
                double randPutts = rand.NextDouble() * (highEnd - lowEnd) + lowEnd;
                if (randPutts < lowEnd + 0.2)
                {
                    putts.Add((int)Math.Floor(randPutts));
                }
                else if (randPutts >= lowEnd + 0.2 && randPutts < avgPuttsPerHole)
                {
                    putts.Add((int)Math.Ceiling(randPutts));
                }
                else if (randPutts >= avgPuttsPerHole && randPutts < highEnd - 0.2)
                {
                    putts.Add((int)Math.Floor(randPutts));
                }
                else
                {
                    if(Math.Ceiling(randPutts) >= 4)
                    {
                        putts.Add(3);
                    }
                    else
                    {
                        putts.Add((int)Math.Ceiling(randPutts));
                    }
                    
                }
            }

            // create a new round based off the randomly generated data
            List<GolfHole> holes = new List<GolfHole>();
            for (int i = 0; i < 18; i++)
            {
                holes.Add(new GolfHole(4, scores[i], putts[i]));
            }

            GolfRound simulatedRound = new GolfRound(DateTime.Now.ToShortDateString(), "SimRound", holes);

            // determine if they want to add their score to their rounds
            MessageBoxResult enterRound = MessageBox.Show("Round Generated!\n\nScore: " + simulatedRound.TotalScore.ToString()
                                                        + "\nGIR: " + ((int)simulatedRound.GIR_PCT()).ToString() + "%", "Add round to your scores?", MessageBoxButton.YesNo);
            switch (enterRound)
            {
                case MessageBoxResult.Yes:
                    rounds.Add(simulatedRound); break;
                case MessageBoxResult.No:
                    break;
            }
        }
    }
}
