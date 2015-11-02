using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kalkulator.NET_v2
{
    public partial class MainWindow
    {
        private void NumericButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button) || resultBox.Text.Length == resultBox.MaxLength)
                return;

            switch (((Button)sender).Content.ToString())
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
                    if (resultBox.Text.Equals("0") || isNextButtonErasingResultBox)
                    {
                        resultBox.Text = "";
                    }
                    resultBox.Text += ((Button)sender).Content.ToString();
                    break;
                case "0":
                    if (resultBox.Text.Equals("0")) return;
                    if (isNextButtonErasingResultBox) resultBox.Text = "0";
                    else
                        resultBox.Text += '0';
                    break;
            }
            isNextButtonErasingResultBox = false;
        }

        private void CommaButton_Click(object sender, RoutedEventArgs e)
        {
            if (resultBox.Text.Contains(',') || resultBox.Text.Length == resultBox.MaxLength)
                return;

            if (isNextButtonErasingResultBox)
                resultBox.Text = "0,";

            resultBox.Text += ',';
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (resultBox.Text.Equals("0"))
                return;

            StringBuilder actualNumber = new StringBuilder(resultBox.Text);

            sbyte digitsToDelete = 1;
            if (actualNumber[actualNumber.Length - 2].ToString() == ",")
                digitsToDelete = 2;
            string newNumber = actualNumber.Remove(actualNumber.Length - digitsToDelete, digitsToDelete).ToString();
            resultBox.Text = newNumber;
        }

        private void CButton_Click(object sender, RoutedEventArgs e)
        {
            isNextButtonErasingResultBox = false;
            memory = null;
            memoryLabel.Content = "";
            resultBox.Text = "0";
            equationType = EquationType.NotChosen;
        }

        private void CEButton_Click(object sender, RoutedEventArgs e)
        {
            resultBox.Text = "0";
        }

        private void PercentButton_Click(object sender, RoutedEventArgs e)
        {
            if (equationType != EquationType.Multiplication)
                return;

            SaveNumberForEquation(1);

            double result = (numbersForEquation[0] / 100) * numbersForEquation[1];

            resultBox.Text = result.ToString();
        }

        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
            {
                MessageBox.Show("Wrong function call.");
                return;
            }

            switch (((Button)sender).Content.ToString())
            {
                case "+":
                    equationType = EquationType.Addition;
                    break;
                case "-":
                    equationType = EquationType.Substraction;
                    break;
                case "*":
                    equationType = EquationType.Multiplication;
                    break;
                case "/":
                    equationType = EquationType.Division;
                    break;
            }

            SaveNumberForEquation(0);

            isNextButtonErasingResultBox = true;
        }

        private void EquationButton_Click(object sender, RoutedEventArgs e)
        {
            SaveNumberForEquation(1);

            double? result = CountResult();
            if (result == null)
                return;

            resultBox.Text = result.ToString();

            isNextButtonErasingResultBox = true;
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
            double result = Math.Pow(numbersForEquation[0], -1);
            resultBox.Text = result.ToString();
        }

        private void SqrtButton_Click(object sender, RoutedEventArgs e)
        {
            SaveNumberForEquation(0);
            if (numbersForEquation[0] < 0) return;
            double result = Math.Sqrt(numbersForEquation[0]);
            resultBox.Text = result.ToString();
        }

        private void MCButton_Click(object sender, RoutedEventArgs e)
        {
            memory = null;
            memoryLabel.Content = "";
        }

        private void MRButton_Click(object sender, RoutedEventArgs e)
        {
            if (memory != null)
                resultBox.Text = memory.ToString();
            isNextButtonErasingResultBox = true;
        }

        private void MSButton_Click(object sender, RoutedEventArgs e)
        {
            double newMemory;
            if (!Double.TryParse(resultBox.Text, out newMemory))
            {
                MessageBox.Show("Parsing failed.");
                return;
            }
            memory = newMemory;
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

            memory = (memory == null) ? (0 - memoryChange) : (memory - memoryChange);
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

            memory = (memory == null) ? (0 + memoryChange) : (memory + memoryChange);
            memoryLabel.Content = "m";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            int keyCode = KeyInterop.VirtualKeyFromKey(e.Key);

            switch (keyCode)
            {
                case 8:
                    BackspaceButton_Click(null, null);
                    break;
                case 13:
                    EquationButton_Click(null, null);
                    break;
                case 27:
                    Application.Current.Shutdown();
                    break;
                case 46:
                    CButton_Click(null, null);
                    break;
                default:
                    return;
            }
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            Button button = new Button();

            switch (e.Text)
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
                case "0":
                    button.Content = e.Text;
                    NumericButton_Click(button, null);
                    break;
                case "%":
                    PercentButton_Click(null, null);
                    break;
                case ",":
                case ".":
                    CommaButton_Click(null, null);
                    break;
                case "+":
                case "-":
                case "*":
                case "/":
                    button.Content = e.Text;
                    OperationButton_Click(button, null);
                    break;
                case "=":
                    EquationButton_Click(null, null);
                    break;
                default:
                    return;
            }
        }

    }
}