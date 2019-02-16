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

namespace ADFGVXGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ADFGVX ADFGVXConverter = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void EncodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlphabetKeywordBox.Text == "" || ShiftKeywordBox.Text == "" || InputBox.Text == "")
            {
                OutputBox.Text = "Both keywords and an input must be provided.";
            }
            else
            {
                if (ADFGVXConverter == null)
                {
                    ADFGVXConverter = new ADFGVX(AlphabetKeywordBox.Text, ShiftKeywordBox.Text);
                }
                else if (ADFGVXConverter.RawKeyword != AlphabetKeywordBox.Text || ADFGVXConverter.RawShiftKeyword != ShiftKeywordBox.Text)
                {
                    ADFGVXConverter.ModifyKeywords(AlphabetKeywordBox.Text, ShiftKeywordBox.Text);
                }

                OutputBox.Text = ADFGVXConverter.EncodeMessage(InputBox.Text);
            }
        }

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlphabetKeywordBox.Text == "" || ShiftKeywordBox.Text == "" || InputBox.Text == "")
            {
                OutputBox.Text = "Both keywords and an input must be provided.";
            }
            else
            {
                if (ADFGVXConverter == null)
                {
                    ADFGVXConverter = new ADFGVX(AlphabetKeywordBox.Text, ShiftKeywordBox.Text);
                }
                else if (ADFGVXConverter.RawKeyword != AlphabetKeywordBox.Text || ADFGVXConverter.RawShiftKeyword != ShiftKeywordBox.Text)
                {
                    ADFGVXConverter.ModifyKeywords(AlphabetKeywordBox.Text, ShiftKeywordBox.Text);
                }

                OutputBox.Text = ADFGVXConverter.DecodeMessage(InputBox.Text);
            }
        }
    }
}
