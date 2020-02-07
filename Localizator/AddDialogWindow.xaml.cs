using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Localizator
{
    public partial class AddDialogWindow : Window
    {
        public string Key;
        public string Value;
        public AddDialogWindow()
        {
            InitializeComponent();
        }
        private void CancelButtonHandler(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OkButtonHandler(object sender, RoutedEventArgs e)
        {
            //Owner.AddWindowData = { Key: "Хуй", Value: "Моржоыый"}
            Key = KeyBox.Text;
            Value = ValueBox.Text;
            DialogResult = true;
        }

        void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (KeyBox.Text.Length > 0)
            {
                e.Handled = !e.Text.All(IsGood);
                if (!e.Text.All(IsGood))
                {
                    MessageBox.Show("Возможны только символы латиницы и цифры. Также ключ не может начинаться с цифры.", "Введён некорректный символ!", MessageBoxButton.OK);
                }
            }
            else
            {
                e.Handled = !e.Text.All(IsGoodFirst);
                if (!e.Text.All(IsGoodFirst))
                {
                    MessageBox.Show("Возможны только символы латиницы и цифры. Также ключ не может начинаться с цифры.", "Введён некорректный символ!", MessageBoxButton.OK);
                }
            }
        }
        
        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var stringData = (string)e.DataObject.GetData(typeof(string));
            if (stringData == null || !stringData.All(IsGood))
                e.CancelCommand();
        }
        
        bool IsGoodFirst(char c)
        {
            if (c >= 'a' && c <= 'z')
                return true;
            if (c >= 'A' && c <= 'Z')
                return true;
            return false;
        }
        bool IsGood(char c)
        {
            if (c == ' ')
                return false;
            if (c >= '0' && c <= '9')
                return true;
            return IsGoodFirst(c);
        }
    }
}