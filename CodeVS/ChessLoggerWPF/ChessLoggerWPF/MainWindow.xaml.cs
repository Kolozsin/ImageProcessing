using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace ChessLoggerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string filePath = "default";
        string filePath2 = "default";
        string stepsString = "";
        Game game;
        public MainWindow()
        {
            InitializeComponent();
            game = new Game();
            CurrentGameState.Text = game.createStringOfGame();
        }

        private void Select_first_image_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                filePath = openFileDialog.FileName;
            First_image_path.Text = filePath.Split("\\")[filePath.Split("\\").Length-1];
        }

        private void Select_second_image_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                filePath2 = openFileDialog.FileName;
            Second_image_path.Text = filePath2.Split("\\")[filePath2.Split("\\").Length - 1];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("C:\\Users\\Kolozsi\\PycharmProjects\\pythonProject\\dist\\main\\main.exe",filePath + " " + filePath2)
                .WaitForExit();
            //Process.Start("C:\\Users\\Kolozsi\\PycharmProjects\\pythonProject\\dist\\main\\main.exe").WaitForExit();
            string content = File.ReadAllText("C:\\Users\\Kolozsi\\Documents\\MSc\\Kepfeldolgozas\\beadando\\output.txt");
            Console.WriteLine(content);
            if (content == "ERROR") {
                MessageBox.Show("There was an error in the python script");
                return;
            }
            string affectedFirst = content.Split(";")[0];
            string affectedSecond = content.Split(";")[1];
            Position position1 = new Position(affectedFirst);
            Position position2 = new Position(affectedSecond);
            filePath = filePath2;
            filePath2 = "";
            First_image_path.Text = Second_image_path.Text;
            Second_image_path.Text = "Select new path";
            string nextStep = "ERROR";
            try
            {
                 nextStep = game.CalculateNextStep(position1, position2);

            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return;
            }
            if (nextStep == "ERROR")
                MessageBox.Show("ERROR in the game logic");
            else
            {
                stepsString += nextStep;
                Steps.Text = stepsString + "\t";
                CurrentGameState.Text = game.createStringOfGame();
            }


        }
    }
}
