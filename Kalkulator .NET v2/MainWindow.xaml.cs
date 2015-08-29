using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Kalkulator.NET_v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private static double[] _numbersForEquation = new double[2];
        private static double? _memory = null;
        private static EquationType _equationType = EquationType.NotChosen;
        private static bool _isNextButtonErasingResultBox = false;
        
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddGestures();
        }

        private void AddGestures()
        {
            _myCommand.InputGestures.Add(new KeyGesture(Key.D5));
        }

        private void NumericButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button) || resultBox.Text.Length == resultBox.MaxLength)
                return;


            switch (((Button) sender).Content.ToString())
            {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    if (resultBox.Text.Equals("0") || _isNextButtonErasingResultBox)
                    {
                        resultBox.Text = "";
                    }
                    resultBox.Text += ((Button) sender).Content.ToString();
                    break;
                case "0":
                    if (resultBox.Text.Equals("0")) return;
                    if (_isNextButtonErasingResultBox) resultBox.Text = "0";
                    else
                        resultBox.Text += '0';
                    break;
            }
            _isNextButtonErasingResultBox = false;
        }

        private void CommaButton_Click(object sender, RoutedEventArgs e)
        {
            if (resultBox.Text.Contains(',') || resultBox.Text.Length == resultBox.MaxLength) return;

            if (_isNextButtonErasingResultBox) resultBox.Text = "0,";

            resultBox.Text += ',';
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (resultBox.Text.Equals("0")) return;

            StringBuilder actualNumber = new StringBuilder(resultBox.Text);

            sbyte digitsToDelete = 1;
            if (actualNumber[actualNumber.Length - 2].ToString() == ",") digitsToDelete = 2;
            string newNumber = actualNumber.Remove(actualNumber.Length - digitsToDelete, digitsToDelete).ToString();
            resultBox.Text = newNumber;
        }

        private void CButton_Click(object sender, RoutedEventArgs e)
        {
            _isNextButtonErasingResultBox = false;
            _memory = null;
            memoryLabel.Content = "";
            resultBox.Text = "0";
            _equationType = EquationType.NotChosen;
        }

        private void CEButton_Click(object sender, RoutedEventArgs e)
        {
            resultBox.Text = "0";
        }


        private void PercentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_equationType != EquationType.Multiplication) return;

            SaveNumberForEquation(1);

            double result = (_numbersForEquation[0]/100)*_numbersForEquation[1];

            resultBox.Text = result.ToString();
        }


        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
            {
                MessageBox.Show("Wrong function call.");
                return;
            }

            switch (((Button) sender).Name)
            {
                case "button_Addition":
                    _equationType = EquationType.Addition;
                    break;
                case "button_Substraction":
                    _equationType = EquationType.Substraction;
                    break;
                case "button_Multiplication":
                    _equationType = EquationType.Multiplication;
                    break;
                case "button_Division":
                    _equationType = EquationType.Division;
                    break;
            }

            SaveNumberForEquation(0);

            _isNextButtonErasingResultBox = true;
        }

        private void EquationButton_Click(object sender, RoutedEventArgs e)
        {
            SaveNumberForEquation(1);

            double? result = CountResult();
            if (result == null)
            {
                return;
            }

            resultBox.Text = result.ToString();

            _isNextButtonErasingResultBox = true;
        }


        private void NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            double result;
            if (!(Double.TryParse(resultBox.Text, out result)))
            {
                MessageBox.Show("Parsing failed.");
                return;
            }
            result *= -1;
            resultBox.Text = result.ToString();
        }

        private void InvertionButton_Click(object sender, RoutedEventArgs e)
        {
            SaveNumberForEquation(0);
            double result = Math.Pow(_numbersForEquation[0], -1);
            resultBox.Text = result.ToString();
        }

        private void SqrtButton_Click(object sender, RoutedEventArgs e)
        {
            SaveNumberForEquation(0);
            double result = Math.Sqrt(_numbersForEquation[0]);
            resultBox.Text = result.ToString();
        }

        #region Memory

        private void MCButton_Click(object sender, RoutedEventArgs e)
        {
            _memory = null;
            memoryLabel.Content = "";
        }

        private void MRButton_Click(object sender, RoutedEventArgs e)
        {
            if (_memory != null)
                resultBox.Text = _memory.ToString();
            _isNextButtonErasingResultBox = true;
        }

        private void MSButton_Click(object sender, RoutedEventArgs e)
        {
            double newMemory;
            if (!Double.TryParse(resultBox.Text, out newMemory))
            {
                MessageBox.Show("Parsing failed.");
                return;
            }
            _memory = newMemory;
            memoryLabel.Content = "M";
        }

        private void MMinusButton_Click(object sender, RoutedEventArgs e)
        {
            double memoryChange;
            if (Double.TryParse(resultBox.Text, out memoryChange))
            {
                MessageBox.Show("Parsing failed.");
                return;
            }

            _memory = (_memory == null) ? (0 - memoryChange) : (_memory - memoryChange);
            memoryLabel.Content = "m";
        }

        private void MPlusButton_Click(object sender, RoutedEventArgs e)
        {
            double memoryChange;
            if (Double.TryParse(resultBox.Text, out memoryChange))
            {
                MessageBox.Show("Parsing failed.");
                return;
            }

            _memory = (_memory == null) ? (0 + memoryChange) : (_memory + memoryChange);
            memoryLabel.Content = "m";
        }

        #endregion

        #region HelpMethods

        private void SaveNumberForEquation(int index)
        {
            if (!double.TryParse(resultBox.Text, out _numbersForEquation[index]))
            {
                MessageBox.Show("Parsing failed.");
                return;
            }
        }

        private double? CountResult()
        {
            double result;
            switch (_equationType)
            {
                case EquationType.Addition:
                    result = _numbersForEquation[0] + _numbersForEquation[1];
                    break;
                case EquationType.Substraction:
                    result = _numbersForEquation[0] - _numbersForEquation[1];
                    break;
                case EquationType.Multiplication:
                    result = _numbersForEquation[0]*_numbersForEquation[1];
                    break;
                case EquationType.Division:
                    result = _numbersForEquation[0]/_numbersForEquation[1];
                    break;
                default:
                    return null;
            }

            return result;
        }

        #endregion

        private enum EquationType
        {
            Addition,
            Substraction,
            Multiplication,
            Division,
            NotChosen
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.LeftShift) return;
            int keyCode = KeyInterop.VirtualKeyFromKey(e.Key);
            MessageBox.Show(keyCode.ToString());
            
            Button button = new Button();
            

            switch (e.Key)
            {
                case Key.D0:
                    button.Content = "0";
                    break;
                case Key.D1:
                    button.Content = "1";
                    break;
                case Key.D2:
                    button.Content = "2";
                    break;
                case Key.D3:
                    button.Content = "3";
                    break;
                case Key.D4:
                    button.Content = "4";
                    break;
                case Key.D5:
                    button.Content = "5";
                    break;
                case Key.D6:
                    button.Content = "6";
                    break;
                case Key.D7:
                    button.Content = "7";
                    break;
                case Key.D8:
                    button.Content = "8";
                    break;
                case Key.D9:
                    button.Content = "9";
                    break;
                case Key.OemComma:
                case Key.OemPeriod:
                    button.Content = ",";
                    break;
                case Key.OemPlus:
                    button.Content = "+";
                    break;
                case Key.OemMinus:
                    button.Content = "-";
                    break;
                case Key.Divide:
                    button.Content = "/";
                    break;
                case Key.Multiply:
                    button.Content = "*";
                    break;
                    //case Key.P

            }

            //NumericButton_Click(button,null);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
      
    }
}
