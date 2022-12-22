using FireSharp;
using FireSharp.Config;
using FireSharp.Response;
using Microsoft.JScript;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using TemplateEngine.Docx;

namespace InstrumentFactory
{
    /// <summary>
    /// Interaction logic for selectUserPage.xaml
    /// </summary>
    public partial class selectUserPage : Page
    {
        public string doc = "";
        FirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "bOfXjafW1pjYhAfhUscdc1eyQv1EmzETVRw1In3S",
            BasePath = "https://instrumentfactory-1f05e-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        FirebaseClient client;

        List<String> users_list = new List<string>();
        List<User> user_from_list = new List<User>();

        public selectUserPage(string doc)
        {
            InitializeComponent();
            client = new FirebaseClient(config);
            Init(doc);
            this.doc = doc;
        }

        private async void Init(string doc)
        {
            switch (doc)
            {
                case "sample_personal_card.docx":
                    tb_num_doc.Visibility = Visibility.Visible;
                    tb_place.Visibility = Visibility.Hidden;
                    break;

                case "sample_spravka_work_place.docx":
                    tb_place.Visibility = Visibility.Visible;
                    tb_num_doc.Visibility = Visibility.Hidden;
                    break;
            }

            try
            {
                FirebaseResponse getData = await client.GetAsync("users");
                string JsTxt = getData.Body;
                BitmapImage list_image = new BitmapImage();
                if (JsTxt == "null")
                    return;

                dynamic date = JsonConvert.DeserializeObject<dynamic>(JsTxt);
                foreach (var item in date)
                {
                    User user = JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString());
                    
                    users_list.Add(user.fullname.ToString());
                    user_from_list.Add(new User() 
                    {
                    image = user.image,
                        fullname = user.fullname,
                        number_work_treaty = user.number_work_treaty,
                        date_work_treaty = user.date_work_treaty,
                        birth_date = user.birth_date,
                        place_birth = user.place_birth,
                        citizenship = user.citizenship,
                        education = user.education,
                        proffecion = user.proffecion,
                        num_departament = user.num_departament,
                    });
                }


            }
            catch (Exception ex)
            {
                lbl_errors.Visibility = Visibility.Visible;
                var bc = new BrushConverter();
                lbl_errors.Foreground = (Brush)bc.ConvertFrom("#9B2D30");
                lbl_errors.Text = ex.ToString();
                MessageBox.Show(ex.ToString());
            }
            combobox_users.ItemsSource = users_list;
        }

        private void combobox_users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btn_next.Visibility = Visibility.Visible;
            lbl_errors.Visibility = Visibility.Hidden;

        }

        private async void btn_next_Click(object sender, RoutedEventArgs e)
        {
            int item = combobox_users.SelectedIndex;
            string docx_path;
            FileInfo fileInf = new FileInfo($"samples/{doc}");
            switch (doc)
            {
                case "sample_personal_card.docx":
                    { if (tb_num_doc.Text == "")
                        {
                            lbl_errors.Text = "Заполните поля";
                            lbl_errors.Visibility = Visibility.Visible;
                            var bc = new BrushConverter();
                            lbl_errors.Foreground = (Brush)bc.ConvertFrom("#9B2D30");
                        }
                        else {
                            try {
                                User user = new User();

                                user = user_from_list[item];
                                byte[] img_photo;

                                if (System.Text.Encoding.UTF8.GetString(user.image) != "")
                                {
                                    string path = $"{System.IO.Path.GetFullPath("images")}/{user.number_work_treaty}.jpg".Replace('\\', '/');
                                    if (System.IO.File.Exists(path) == false)
                                    {
                                        File.WriteAllBytes(path, user.image);
                                    }
                                    FileInfo filephoto = new FileInfo(path);
                                }
                                else
                                {
                                    FileInfo file404 = new FileInfo("404notegsist.jpg");
                                    using (FileStream fs = File.OpenRead(file404.FullName))
                                    {
                                        img_photo = File.ReadAllBytes(file404.FullName);
                                    }
                                }

                                SaveFileDialog dialog = new SaveFileDialog();
                                dialog.Filter = "Документ (*.docx)|*.docx";
                                if (dialog.ShowDialog() == true)
                                {
                                    docx_path = dialog.FileName.ToString();

                                    string[] partofFullname = user.fullname.Split(' '); 
                                    File.Copy(fileInf.FullName.ToString(), docx_path);

                                    var valuesToFill = new Content(
                                        new TableContent("table_treaty")
                                            .AddRow(new FieldContent("id_treaty", tb_num_doc.Text.ToString()), new FieldContent("number_work_treaty", user.number_work_treaty.ToString()), new FieldContent("date_work_treaty", user.date_work_treaty.ToString())),
                                        new TableContent("table_bio")
                                        .AddRow(
                                        new FieldContent("surname", partofFullname[0].ToString()),
                                        new FieldContent("name", partofFullname[1].ToString()),
                                        new FieldContent("patronymic", partofFullname[2].ToString())),
                                        new TableContent("table_other")
                                        .AddRow(new FieldContent("birth_date", user.birth_date.ToString()),
                                        new FieldContent("place_birth", user.place_birth.ToString()),
                                        new FieldContent("citizenship", user.citizenship.ToString()),
                                        new FieldContent("education", user.education.ToString()),
                                        new FieldContent("proffesion", user.proffecion.ToString()),
                                        new FieldContent("num_departament", user.num_departament.ToString())),
                                        new TableContent("table_date")
                                        .AddRow(new FieldContent("date", DateTime.Now.ToShortDateString().ToString()))
                                        );
                                    using (var outputDocument = new TemplateProcessor(docx_path)
                                        .SetRemoveContentControls(true)) {
                                        outputDocument.FillContent(valuesToFill);
                                        outputDocument.SaveChanges();
                                    }
                                    tb_num_doc.Text = "";
                                    lbl_errors.Visibility = Visibility.Visible;
                                    lbl_errors.Text = "Файл сохранён";
                                    var bc = new BrushConverter();
                                    lbl_errors.Foreground = (Brush)bc.ConvertFrom("#34C924");
                                }
                            }
                            catch (Exception ex){
                                MessageBox.Show(ex.ToString());
                                lbl_errors.Visibility = Visibility.Visible;
                                lbl_errors.Text = ex.ToString();
                                var bc = new BrushConverter();
                                lbl_errors.Foreground = (Brush)bc.ConvertFrom("#9B2D30");
                            }
                        }
                    }
                    break;

                case "sample_spravka_work_place.docx": 
                    {
                        if (tb_place.Text == "")
                        {
                            lbl_errors.Text = "Заполните поля";
                            lbl_errors.Visibility = Visibility.Visible;
                            var bc = new BrushConverter();
                            lbl_errors.Foreground = (Brush)bc.ConvertFrom("#9B2D30");
                        }
                        else
                        {
                            try
                            {
                                User user = new User();

                                user = user_from_list[item];

                                SaveFileDialog dialog = new SaveFileDialog();
                                dialog.Filter = "Документ (*.docx)|*.docx";
                                if (dialog.ShowDialog() == true)
                                {
                                    docx_path = dialog.FileName.ToString();

                                    File.Copy(fileInf.FullName.ToString(), docx_path);

                                    var valuesToFill = new Content(
                                        new TableContent("table_giveTo")
                                        .AddRow(new FieldContent("fullname", user.fullname)),
                                        new TableContent("table_to_place")
                                        .AddRow(new FieldContent("place", tb_place.Text.ToString())),
                                        new TableContent("table_date")
                                        .AddRow(new FieldContent("date", DateTime.Now.ToShortDateString().ToString()))
                                    );
                                    using (var outputDocument = new TemplateProcessor(docx_path)
                                        .SetRemoveContentControls(true))
                                    {
                                        outputDocument.FillContent(valuesToFill);
                                        outputDocument.SaveChanges();
                                    }
                                    tb_place.Text = "";
                                    lbl_errors.Visibility = Visibility.Visible;
                                    lbl_errors.Text = "Файл сохранён";
                                    var bc = new BrushConverter();
                                    lbl_errors.Foreground = (Brush)bc.ConvertFrom("#34C924");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                                lbl_errors.Visibility = Visibility.Visible;
                                lbl_errors.Text = ex.ToString();
                                var bc = new BrushConverter();
                                lbl_errors.Foreground = (Brush)bc.ConvertFrom("#9B2D30");
                            }
                        }
                    }
                    break;
            }
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
    
}
