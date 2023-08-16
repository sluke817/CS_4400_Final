using GolfScoreManager;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json.Serialization;

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 
    public class InvalidInputException : Exception
    {
        public InvalidInputException()
        {
        }

        public InvalidInputException(string message)
            : base(message)
        {
        }
    }
    public partial class AddRound : UserControl
    {
        RoundList rounds;
        public AddRound(RoundList rounds)
        {
            this.rounds = rounds;
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new HomePage(this.rounds));

        }

        private void SaveStats_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (datePick.SelectedDate == null)
                {
                    throw new InvalidInputException("Date is not selected.");
                }
                if(datePick.SelectedDate > DateTime.Now)
                {
                    MessageBoxResult futureDate = MessageBox.Show("Selected date is later than today's date. Would you like to continue with a postdated scorecard? "
                                                                      , "Are you sure?", MessageBoxButton.YesNo);
                    switch (futureDate)
                    {
                        case MessageBoxResult.Yes:
                            break;
                        case MessageBoxResult.No:
                            return;
                    }
                }
                if (courseName.Text == "")
                {
                    throw new InvalidInputException("Course name is empty.");
                }
                if(courseName.Text.Length > 60)
                {
                    throw new InvalidInputException("Course name is too long.");
                }

                List<GolfHole> holes = new List<GolfHole>();
                holes.Add(new GolfHole(Convert.ToInt32(par1.Text), Convert.ToInt32(score1.Text), Convert.ToInt32(putts1.Text)));
                holes.Add(new GolfHole(Convert.ToInt32(par2.Text), Convert.ToInt32(score2.Text), Convert.ToInt32(putts2.Text)));
                holes.Add(new GolfHole(Convert.ToInt32(par3.Text), Convert.ToInt32(score3.Text), Convert.ToInt32(putts3.Text)));
                holes.Add(new GolfHole(Convert.ToInt32(par4.Text), Convert.ToInt32(score4.Text), Convert.ToInt32(putts4.Text)));
                holes.Add(new GolfHole(Convert.ToInt32(par5.Text), Convert.ToInt32(score5.Text), Convert.ToInt32(putts5.Text)));
                holes.Add(new GolfHole(Convert.ToInt32(par6.Text), Convert.ToInt32(score6.Text), Convert.ToInt32(putts6.Text)));
                holes.Add(new GolfHole(Convert.ToInt32(par7.Text), Convert.ToInt32(score7.Text), Convert.ToInt32(putts7.Text)));
                holes.Add(new GolfHole(Convert.ToInt32(par8.Text), Convert.ToInt32(score8.Text), Convert.ToInt32(putts8.Text)));
                holes.Add(new GolfHole(Convert.ToInt32(par9.Text), Convert.ToInt32(score9.Text), Convert.ToInt32(putts9.Text)));

                if (eighteen.IsChecked == true)
                {
                    holes.Add(new GolfHole(Convert.ToInt32(par10.Text), Convert.ToInt32(score10.Text), Convert.ToInt32(putts10.Text)));
                    holes.Add(new GolfHole(Convert.ToInt32(par11.Text), Convert.ToInt32(score11.Text), Convert.ToInt32(putts11.Text)));
                    holes.Add(new GolfHole(Convert.ToInt32(par12.Text), Convert.ToInt32(score12.Text), Convert.ToInt32(putts12.Text)));
                    holes.Add(new GolfHole(Convert.ToInt32(par13.Text), Convert.ToInt32(score13.Text), Convert.ToInt32(putts13.Text)));
                    holes.Add(new GolfHole(Convert.ToInt32(par14.Text), Convert.ToInt32(score14.Text), Convert.ToInt32(putts14.Text)));
                    holes.Add(new GolfHole(Convert.ToInt32(par15.Text), Convert.ToInt32(score15.Text), Convert.ToInt32(putts15.Text)));
                    holes.Add(new GolfHole(Convert.ToInt32(par16.Text), Convert.ToInt32(score16.Text), Convert.ToInt32(putts16.Text)));
                    holes.Add(new GolfHole(Convert.ToInt32(par17.Text), Convert.ToInt32(score17.Text), Convert.ToInt32(putts17.Text)));
                    holes.Add(new GolfHole(Convert.ToInt32(par18.Text), Convert.ToInt32(score18.Text), Convert.ToInt32(putts18.Text)));
                }


                // validating input
                foreach(GolfHole hole in holes)
                {
                    if(hole.Par <= 0)
                    {
                        throw new InvalidInputException("Par must be at least 1.");
                    }
                    if(hole.Putts < 0)
                    {
                        throw new InvalidInputException("Putts cannot be negative.");
                    }
                    if(hole.Score <= 0)
                    {
                        throw new InvalidInputException("Score must be at least 1.");
                    }

                    if (hole.Putts > hole.Score)
                    {
                        throw new InvalidInputException("Putts cannot be more than the score.");
                    }


                    if (hole.Score > 8)
                    {
                        MessageBoxResult enterRound = MessageBox.Show("Strange score detected. Would you like to continue with a hole score of "
                                                                        + hole.Score.ToString() + "?", "Are you sure?", MessageBoxButton.YesNo);
                        switch (enterRound)
                        {
                            case MessageBoxResult.Yes:
                                break;
                            case MessageBoxResult.No:
                                return;
                        }
                    }

                    if (hole.Par > 5 || hole.Par < 3)
                    {
                        MessageBoxResult enterRound = MessageBox.Show("Strange par score detected. Would you like to continue with a hole par of "
                                                                        + hole.Par.ToString() + "?", "Are you sure?", MessageBoxButton.YesNo);
                        switch (enterRound)
                        {
                            case MessageBoxResult.Yes:
                                break;
                            case MessageBoxResult.No:
                                return;
                        }
                    }

                }

                // adds the rounds to the data set
                GolfRound round = new GolfRound(datePick.SelectedDate.Value.ToShortDateString() ?? DateTime.Now.ToShortDateString(), courseName.Text, holes);

                rounds.Add(round);

                MessageBox.Show("Your round has been saved.");
                ClearForm();
            }
            // if any errors encountered, inform the user and do not continue
            catch(InvalidInputException ex)
            {
                MessageBox.Show(ex.Message + " Please try again.");
            }
            catch(Exception)
            {
                MessageBox.Show("Not all holes were filled out properly. Please make sure par/score/putt for all holes are filled out properly!");
            }
        }

        public void ClearForm()
        {

            courseName.Text = null;
            datePick.SelectedDate = null;

            par1.Text = null;
            par2.Text = null;
            par3.Text = null;
            par4.Text = null;
            par5.Text = null;
            par6.Text = null;
            par7.Text = null; 
            par8.Text = null;
            par9.Text = null;
            par10.Text = null;
            par11.Text = null;
            par12.Text = null; 
            par13.Text = null;
            par14.Text = null; 
            par15.Text = null; 
            par16.Text = null;
            par17.Text = null;
            par18.Text = null;

            score1.Text = null;
            score2.Text = null;
            score3.Text = null;
            score4.Text = null;
            score5.Text = null;
            score6.Text = null;
            score7.Text = null;
            score8.Text = null;
            score9.Text = null;
            score10.Text = null;
            score11.Text = null;
            score12.Text = null;
            score13.Text = null;
            score14.Text = null;
            score15.Text = null;
            score16.Text = null;
            score17.Text = null;
            score18.Text = null;

            putts1.Text = null;
            putts2.Text = null;
            putts3.Text = null;
            putts4.Text = null;
            putts5.Text = null;
            putts6.Text = null;
            putts7.Text = null;
            putts8.Text = null;
            putts9.Text = null;
            putts10.Text = null;
            putts11.Text = null;
            putts12.Text = null;
            putts13.Text = null;
            putts14.Text = null;
            putts15.Text = null;
            putts16.Text = null;
            putts17.Text = null;
            putts18.Text = null;

        }
    }
}
