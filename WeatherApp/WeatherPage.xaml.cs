using System.Diagnostics;
using WeatherApp.Services;
using System.Timers;

using static System.Net.WebRequestMethods;
using Microsoft.Maui.Animations;
using System.Net;
using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace WeatherApp;

public partial class WeatherPage : ContentPage
{

/* Unmerged change from project 'WeatherApp (net6.0-android)'
Before:
    public List<Models.List> WeatherList;
After:
    public List<List> WeatherList;
*/

/* Unmerged change from project 'WeatherApp (net7.0-ios)'
Before:
    public List<Models.OpenWeather.List> WeatherList;
After:
    public List<List> WeatherList;
*/
    public List<Models.List> WeatherList;
    private double latitude;
    private double longitude;
  
	public WeatherPage()
	{
        
        InitializeComponent();
        
        bgGrid.BackgroundColor = GlobalServices.GetbgColor();
        BGImage.Source = null;
        BGImage.Background = GlobalServices.GetGradient();
        
        /* Unmerged change from project 'WeatherApp (net6.0-android)'
        Before:
                WeatherList = new List<Models.List>();
        After:
                WeatherList = new List<List>();
        */

        /* Unmerged change from project 'WeatherApp (net7.0-ios)'
        Before:
                WeatherList = new List<Models.OpenWeather.List>();
        After:
                WeatherList = new List<List>();
        */
        WeatherList = new List<Models.List>();
	}
    protected async override void OnAppearing()
    {
        await MainGrid.FadeTo(1, 1000, Easing.Linear);
        base.OnAppearing();
        await GetLocation();
  }

    public async Task GetLocation()
    {
        var location = await Geolocation.GetLocationAsync();
        latitude = location.Latitude;
        longitude = location.Longitude;
    }
    private async void ClickLocation_Tapped(object sender, EventArgs e)
    {

        entrySearch.PlaceholderColor = Colors.Black;
        entrySearch.Placeholder = "Searching for location...";
        entrySearch.Text = "";
        await GetLocation();
        await GetWeatherDataByLocation(latitude, longitude);

    }
    public async Task GetWeatherDataByLocation(double latitude, double longitude)
    {
     
        var forecast = await ApiService.GetForecastByGps(latitude, longitude);
        var weather = await ApiService.GetWeatherByGPS(latitude, longitude);
        UpdateUI(weather,forecast);
    }
    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        string response = entrySearch.Text;
        if (response != null)
        {
            try
            {
                entrySearch.PlaceholderColor = Colors.Black;
                entrySearch.Placeholder = "Searching for location...";
                entrySearch.Text = "";
                await GetWeatherDataByCity(response);
            }catch (Exception ex)
            {
                entrySearch.Text = "";
                entrySearch.PlaceholderColor = Colors.Red;
                entrySearch.Placeholder= "Error try again";
            }
        }
   }
    public async Task GetWeatherDataByCity(string city)
    {
        var weather = await ApiService.GetWeatherByCity(city);
        var forecast = await ApiService.GetForecastByCity(city);
        UpdateUI(weather,forecast);
    }
    public async Task GetLocationImage(string location)
    {
        var result = await ApiService.GetLocation(location);
        var image_src = await ApiService.GetPhoto(result.predictions[0].place_id);
        UpdateBG(image_src);
    }
    public async void UpdateUI(dynamic weather , dynamic forecast)
    {
        await MainGrid.FadeTo(0, 1000, Easing.Linear);
        imgHumidity.IsVisible = true;
        imgWind.IsVisible = true;
        foreach (var item in forecast.list)
        {
            WeatherList.Add(item);

        }
        CvWeather.ItemsSource = WeatherList;

        LblCity.Text = forecast.city.name;
    
        LblWeatherDescription.Text = weather.weather[0].description;
        LblTemperature.Text = GlobalServices.GetTempScaleTemp(weather.main.temp.ToString());//result.list[0].main.temperature + "�C";
        LblHumidity.Text = weather.main.humidity + "%";
        LblWind.Text = weather.wind.speed + "km/h";
        LblCurrentDay.Text = GlobalServices.ConvertUnixToTimeZone(weather.dt, "h:mm tt");
        entrySearch.Text = LblCity.Text;
        ImgWeatherIcon.Source = weather.weather[0].customIcon;
        if (SettingsServices.Get("BackgroundToggleOff", false) == false)
        {
            Debug.WriteLine("yo");
            GetLocationImage(forecast.city.name);
        }
        else
        {


            Debug.WriteLine("hey");
            await MainGrid.FadeTo(1, 1000, Easing.Linear);
        
        }
    }
    private async void ToSettings(object sender, EventArgs e)
    {
        await MainGrid.FadeTo(0, 1000, Easing.Linear);
       
      await Navigation.PushAsync(new SettingsPage(),false);
    }
    public async void UpdateBG(string src)
    {
        
       Console.WriteLine(src);

            var image = src;

            var imagesource = await ImageLoader.LoadImageFromUriAsync(image);
        
    

            if (imagesource != null)
            {
            
            BGImage.Source = imagesource;
                await MainGrid.FadeTo(1, 1000, Easing.Linear);
            
        }

    }
}  