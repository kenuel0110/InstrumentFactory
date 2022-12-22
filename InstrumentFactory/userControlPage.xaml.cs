using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using FireSharp.EventStreaming;
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
using FireSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using Microsoft.Win32;
using Google.Apis.Drive.v2;
using Google.Apis.Auth;
using Nemiro.OAuth.LoginForms;
using System.IO;

namespace InstrumentFactory
{
    /// <summary>
    /// Interaction logic for userControlPage.xaml
    /// </summary>
    public partial class userControlPage : Page
    {

        List<User_item> users_list = new List<User_item>();

        FirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "bOfXjafW1pjYhAfhUscdc1eyQv1EmzETVRw1In3S",
            BasePath = "https://instrumentfactory-1f05e-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        FirebaseClient client;
        string img_path = "";
        byte[] buffer = Array.Empty<byte>();

        public struct User_item
        {
            public string fullname { get; set; }
            public BitmapImage image { get; set; }
        }

        public userControlPage()
        {
            InitializeComponent();
            client = new FirebaseClient(config);
            Init();
        }

        private async void Init()
        {
            btn_changePhoto.Visibility = Visibility.Visible;
            img_photo.Visibility = Visibility.Hidden;
            users_list.Clear();
            listView.ItemsSource = null;
            try
            {
                FirebaseResponse getData = await client.GetAsync("users");
                string JsTxt = getData.Body;
                BitmapImage list_image = new BitmapImage();
                if (JsTxt == "null")
                    return ;

                dynamic date = JsonConvert.DeserializeObject<dynamic>(JsTxt);
                foreach (var item in date)
                {
                    User user = JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString());
                    if (System.Text.Encoding.UTF8.GetString(user.image) != "")
                    {
                        //   WebClient webClient = new WebClient();
                        if (System.IO.Directory.Exists("images") == false)
                            System.IO.Directory.CreateDirectory("images");

                        string path = $"{System.IO.Path.GetFullPath("images")}/{user.number_work_treaty}.jpg".Replace('\\', '/');
                        if (System.IO.File.Exists(path) == false)
                        {
                            //webClient.DownloadFile(user.image.ToString(), path);
                            File.WriteAllBytes(path, user.image);
                            list_image = new BitmapImage(new Uri(path));
                        }
                        else if (System.IO.File.Exists(path) == true)
                        {
                            list_image = new BitmapImage(new Uri(path));
                        }
                    }
                    else
                    {
                        list_image = new BitmapImage(new Uri("pack://application:,,,/icons/image_404.jpg"));
                    }

                    users_list.Add(new User_item() {image = list_image, fullname = user.fullname });
                }
                /*User[] items = getData.ResultAs<User[]>();
                
                MessageBox.Show(getData.ResultAs<User>().ToString());
                */
                
            }
            catch (Exception ex) 
            {
                lbl_errors.Visibility = Visibility.Visible;
                lbl_errors.Text = ex.ToString();
            }
            listView.ItemsSource = users_list;
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack == true)
                NavigationService.GoBack();
        }
            
        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            if (
                tb_birth_date.Text == "" ||
                tb_citizenship.Text == "" ||
                tb_date_work_treaty.Text == "" ||
                tb_education.Text == "" ||
                tb_firstname.Text == "" ||
                tb_lastname.Text == "" ||
                tb_name.Text == "" ||
                tb_number_work_treaty.Text == "" ||
                tb_num_departament.Text == "" ||
                tb_place_birth.Text == "" ||
                tb_proffecion.Text == ""
                )
            {
                lbl_error_add.Visibility = Visibility.Visible;
                var bc = new BrushConverter();
                lbl_error_add.Foreground = (Brush)bc.ConvertFrom("#DDBEC3");
                lbl_error_add.Text = "Заполните все поля";
                scroll.ScrollToVerticalOffset(scroll.ExtentHeight);
            }
            else 
            {
                lbl_error_add.Visibility = Visibility.Hidden;
                lbl_error_add.Text = "";

                

                if (img_path != "")
                {
                    buffer = File.ReadAllBytes(img_path);
                }
                else 
                {
                    buffer = Array.Empty<byte>();
                }

                string string_birth_date = tb_birth_date.Text.ToString();
                string string_citizenship = tb_citizenship.Text.ToString();
                string string_date_work_treaty = tb_date_work_treaty.Text.ToString();
                string string_education = tb_education.Text.ToString();
                string string_firstname = tb_firstname.Text.ToString();
                string string_lastname = tb_lastname.Text.ToString();
                string string_name = tb_name.Text.ToString();
                string string_number_work_treaty = tb_number_work_treaty.Text.ToString();
                string string_num_departament = tb_num_departament.Text.ToString();
                string string_place_birth = tb_place_birth.Text.ToString();
                string string_proffecion = tb_proffecion.Text.ToString();

                pushData(string_birth_date, string_citizenship, string_date_work_treaty, string_education, string_firstname, string_lastname, string_number_work_treaty, string_name, string_num_departament, string_place_birth, string_proffecion);

            }
        }

        private async void pushData (string string_birth_date, string string_citizenship, string string_date_work_treaty,
            string string_education, string string_firstname, string string_lastname, string string_number_work_treaty,
            string string_name, string string_num_departament, string string_place_birth, string string_proffecion)
        {
            try
            {
                User user = new User() {image = buffer,
                    fullname = $"{string_firstname} {string_name} {string_lastname}",
                    number_work_treaty = string_number_work_treaty,
                    date_work_treaty = string_date_work_treaty,
                    birth_date = string_birth_date,
                    place_birth = string_place_birth,
                    citizenship = string_citizenship,
                    education = string_education,
                    proffecion = string_proffecion,
                    num_departament = string_num_departament
                };
                //var user_json = JsonConvert.SerializeObject(user);
                await client.SetAsync($"users/{string_number_work_treaty}", user);

                tb_birth_date.Text = "";
                tb_citizenship.Text = "";
                tb_date_work_treaty.Text = "";
                tb_education.Text = "";
                tb_firstname.Text = "";
                tb_lastname.Text = "";
                tb_name.Text = "";
                tb_number_work_treaty.Text = "";
                tb_num_departament.Text = "";
                tb_place_birth.Text = "";
                tb_proffecion.Text = "";

                lbl_error_add.Visibility = Visibility.Visible;
                lbl_error_add.Text = "Данные добавлены";
                var bc = new BrushConverter();
                lbl_error_add.Foreground = (Brush)bc.ConvertFrom("#A0F8A0");
                scroll.ScrollToVerticalOffset(scroll.ExtentHeight);
            }
            catch (Exception ex)
            {
                lbl_error_add.Visibility = Visibility.Visible;
                var bc = new BrushConverter();
                lbl_error_add.Foreground = (Brush)bc.ConvertFrom("#DDBEC3");
                lbl_error_add.Text = ex.ToString();
                scroll.ScrollToVerticalOffset(scroll.ExtentHeight);
            }
            Init();
        }

        private void btn_changePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Картинка (*.jpg)|*.jpg";
            if (dialog.ShowDialog() == true)
            {
                btn_changePhoto.Visibility = Visibility.Hidden;
                img_photo.Visibility = Visibility.Visible;
                img_photo.Source = new BitmapImage(new Uri(dialog.FileName.ToString()));
                img_path = dialog.FileName.ToString();
            }
        }

        private void img_photo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Картинка (*.jpg)|*.jpg";
            if (dialog.ShowDialog() == true)
            {
                btn_changePhoto.Visibility = Visibility.Hidden;
                img_photo.Visibility = Visibility.Visible;
                img_photo.Source = new BitmapImage(new Uri(dialog.FileName.ToString()));
                img_path = dialog.FileName.ToString();
            }
        }
    }
}
