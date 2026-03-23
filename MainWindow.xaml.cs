using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Windows.Threading;
using Newtonsoft.Json;

namespace WeatherApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly string apiKey = "f0285d3ece134622be8163935262203";
        private string CityName;
        private DateTime _now;
        //сдесь и выше создаем переменную таймера
        public DateTime Now
        {
            get { return _now; }
            set
            {
                _now = value;
                OnPropertyChanged(nameof(Now));
            }
        }
        public MainWindow() 
        {
            //сдесь реализуем получение данных и таймер
            InitializeComponent();
            DataContext = this;
            //сдесь отображение таймеры
            lblDigitalClock.Visibility = Visibility.Hidden;
            //сдесь сам таймер
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1)};
            timer.Tick += (sender, args) =>
            {
                Now = DateTime.Now;
            };
            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        //кнопка закрытия приложения
        private void Button_Click(object sender, RoutedEventArgs e) 
        {
            this.Close();
        }

        private async void btnGetWeather_Click(object sender, RoutedEventArgs e)
        {
            CityName = txtCityName.Text.Trim();
            if (string.IsNullOrEmpty(CityName))
            {
                MessageBox.Show("Введите название города");
                return;
            }

            string apiUri = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={CityName}";
            
            try //в try создаем веб запрос к апи сайта и получаем наши метрики
            {
                HttpWebRequest request = WebRequest.CreateHttp(apiUri);
                request.Method = "GET";
                //создание асинхронного запроса к ресурсу и API
                using (WebResponse response = await request.GetResponseAsync()) //асинхронный запрос
                {
                    using(Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string jsonResponse = reader.ReadToEnd();
                            WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(jsonResponse);
                            DisplayWeatherData(weatherData);
                        }
                    }
                }
            }
            //Обработка ошибки запроса к API
            catch (WebException ex)
            {
                MessageBox.Show("Возникла ошибка определения погоды: " + ex.Message);
            }
        }
        

        private void DisplayWeatherData(WeatherData weatherData)
        {
            //сдесь реализован вывод погоды в метрики .xml файла
            lblCityName.Content = weatherData.Location.Name; 
            lblTemperature.Content = weatherData.Current.TempC + "°C";
            lblCondition.Content = weatherData.Current.Condition.Text;
            lblHumidity.Content = weatherData.Current.Humidity + "%";

            //сдесь получаем иконку через сайт с помощью Bitmap
            BitmapImage weatherIcon = new BitmapImage();
            weatherIcon.BeginInit();
            weatherIcon.UriSource = new Uri("http:" + weatherData.Current.Condition.Icon);
            weatherIcon.EndInit();
            imgWeatherIcon.Source = weatherIcon;
            //ну и тут получаем скорость ветра
            lblWindSpeed.Content = weatherData.Current.WindKph + " км/ч";
        }

        private void txtCityName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
