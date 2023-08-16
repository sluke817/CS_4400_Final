using GolfScoreManager;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;
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

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        // data to hold the rounds
        RoundList rounds;
        public HomePage(RoundList rounds)
        {
            this.rounds = rounds;
            InitializeComponent();
            RenderRounds();
        }

        public HomePage()
        {
            
            this.rounds = new RoundList();
            InitializeComponent();
            RenderRounds();
        }

        // navigates to the add round page
        private void AddRoundButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new AddRound(this.rounds));
        }

        // navigates to the generate round page
        private void GenerateRoundButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new SimulateRound(this.rounds));
        }

        // renders the scores of the rounds on the screen
        public void RenderRounds()
        {
            
            roundHolder.Items.Clear();
            if (rounds.Count < 1)
            {
                TextBlock noRounds = new TextBlock();
                noRounds.Text = "There are no rounds to view.";
                roundHolder.Items.Add(noRounds);
                ClearStats();
            }
            else
            {
                foreach (GolfRound round in rounds)
                {
                    TextBlock roundString = new TextBlock();
                    roundHolder.Items.Add(round.CourseName + "\t" + round.Date + "\t" + round.TotalScore.ToString());
                }

                // BestScore is nullable int
                if(rounds.BestScore != null)
                {
                    bestScore.Text = rounds.BestScore.ToString();
                }
                else
                {
                    bestScore.Text = "n/a";
                }
                // puts the scores in the appropriate text boxes
                avgScore.Text = Math.Round(rounds.AverageScore, 1).ToString();
                gir.Text = Math.Round(rounds.GIR_PCT, 0).ToString() + "%";
                pph.Text = Math.Round(rounds.PuttsPerHole, 1).ToString();
                birdiepct.Text = Math.Round(rounds.GetPct(Score.Birdie), 0).ToString() + "%";
                parpct.Text = Math.Round(rounds.GetPct(Score.Par), 0).ToString() + "%";
                bogeypct.Text = Math.Round(rounds.GetPct(Score.Bogey), 0).ToString() + "%";
                btbirdiepct.Text = Math.Round(rounds.GetPct(Score.BtB), 0).ToString() + "%";
                wtbogeypct.Text = Math.Round(rounds.GetPct(Score.WtB), 0).ToString() + "%";

            }
        }
        public void ClearStats()
        {
            bestScore.Text = "n/a";
            avgScore.Text = "n/a";
            gir.Text = "n/a";
            pph.Text = "n/a";
            birdiepct.Text = "n/a";
            parpct.Text = "n/a";
            bogeypct.Text = "n/a";
            btbirdiepct.Text = "n/a";
            wtbogeypct.Text = "n/a";
        }

        // deletes the round from the UI then the data struct then re renders the screen
        private void DeleteRound_Click(object sender, RoutedEventArgs e)
        {
            if (roundHolder.SelectedItem != null)
            {

                foreach (GolfRound round in this.rounds)
                {

                    String[] split = roundHolder.SelectedItem.ToString().Split("\t");
                    if (round.Date == split[1] && round.CourseName == split[0])
                    {
                        this.rounds.Remove(round);
                        break;
                    }
                }
                roundHolder.Items.Remove(roundHolder.SelectedItem);
                RenderRounds();

            }
            else
            {
                MessageBox.Show("No round selected.");
            }
        }

        // saves the rounds stored as a json file
        private void SaveRounds_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog()
            {
                Filter = "Text files (*.json)|*.json",
                Title = "Save as JSON file"
            };

            saveFileDialog1.ShowDialog();

            try
            {
                FileStream fs = File.OpenWrite(saveFileDialog1.FileName);

                string jsonString = JsonSerializer.Serialize(this.rounds);

                byte[] stringToByte = new UTF8Encoding(true).GetBytes(jsonString);
                fs.Write(stringToByte, 0, stringToByte.Length);

                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        // loads the rounds from a json file
        private void LoadRounds_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog()
            {
                FileName = "",
                Filter = "json files (*.json)|*.json",
                Title = "Open JSON file"
            };

            if (openFileDialog1.ShowDialog() == true)
            {
                try
                {
                    var sr = new StreamReader(openFileDialog1.FileName);
                    string jsonString = sr.ReadToEnd();

                    RoundList? jsonPull = JsonSerializer.Deserialize<RoundList>(jsonString);
                    if (jsonPull != null)
                    {
                        this.rounds = jsonPull;
                        this.rounds.calculate();
                    }

                    RenderRounds();

                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("Invalid file format." + "\n" + ex.Message);
                }

                catch (JsonException ex)
                {
                    MessageBox.Show("Invalid file format." + "\n" + ex.Message);
                }
            }
        }
    }
}
