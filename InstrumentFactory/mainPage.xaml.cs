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

namespace InstrumentFactory
{
    /// <summary>
    /// Interaction logic for mainPage.xaml
    /// </summary>
    public partial class mainPage : Page
    {
        List<String> cb_items = new List<string>();

        string sample = "";

        public mainPage()
        {
            InitializeComponent();
            init_combobox();
        }

        private void init_combobox()
        {
            cb_items.Add("Личное дело");
            cb_items.Add("Справка о месте работы");
            combobox_documents.ItemsSource = cb_items;
        }

        private void combobox_documents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int item = combobox_documents.SelectedIndex;
            switch (item) 
            {
                case 0:
                    {
                        img_documents_preview.Source = new BitmapImage(new Uri("pack://application:,,,/samples/previews/preview_personal_card.jpg"));
                        sample = "sample_personal_card.docx";
                        btn_next.Visibility = Visibility.Visible;
                    }
                    break;
                case 1:
                    {
                        img_documents_preview.Source = new BitmapImage(new Uri("pack://application:,,,/samples/previews/preview_spravka_work_place.jpg"));
                        sample = "sample_spravka_work_place.docx";
                        btn_next.Visibility = Visibility.Visible;
                    }
                    break;
                    
            }
        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new selectUserPage(sample));
        }

        private void btn_settings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new userControlPage());
        }
    }
}
