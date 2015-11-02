using System;
using System.Windows;

namespace Kalkulator.NET_v2
{
    public partial class MainWindow : Window
    {
        private static double[] numbersForEquation = new double[2];
        private static double? memory = null;
        private static EquationType equationType = EquationType.NotChosen;
        private static bool isNextButtonErasingResultBox = false;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void SaveNumberForEquation(int index)
        {
            if (!double.TryParse(resultBox.Text, out numbersForEquation[index]))
            {
                MessageBox.Show("Parsing failed.");
                return;
            }
        }

        private double? CountResult()
        {
            double result;
            switch (equationType)
            {
                case EquationType.Addition:
                    result = numbersForEquation[0] + numbersForEquation[1];
                    break;
                case EquationType.Substraction:
                    result = numbersForEquation[0] - numbersForEquation[1];
                    break;
                case EquationType.Multiplication:
                    result = numbersForEquation[0]*numbersForEquation[1];
                    break;
                case EquationType.Division:
                    result = numbersForEquation[0]/numbersForEquation[1];
                    break;
                default:
                    return null;
            }

            return result;
        }

        private enum EquationType
        {
            Addition,
            Substraction,
            Multiplication,
            Division,
            NotChosen
        }
    }
}