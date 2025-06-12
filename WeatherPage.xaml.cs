using System.Threading.Tasks;
using WeatherApp.Services;

namespace WeatherApp;

public partial class WeatherPage : ContentPage
{
	public List<Models.List> WeatherList;

	private double latitude;
	private double longitude;
    public WeatherPage()
	{
		InitializeComponent();
        WeatherList = new List<Models.List>();
    }

	protected async override void OnAppearing()
	{
		base.OnAppearing();
		await GetLocation();
        await GetWeatherByLocation(latitude, longitude);

    }

	 public async Task GetLocation()
    {
        try
        {
            var location = await Geolocation.GetLocationAsync();

            if (location != null)
            {
                latitude = location.Latitude;
                longitude = location.Longitude;
            }
            else
            {
                await DisplayAlert("Location Error", "Unable to get location.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Location Error", ex.Message, "OK");
        }
    }
	private async void TapLocation_Tapped(object sender, TappedEventArgs e)
    {
        await GetLocation();
        await GetWeatherByLocation(latitude, longitude);
    }
	public async Task GetWeatherByLocation(double latitude, double longitude)
	{
        var result = await ApiService.GetWeather(latitude, longitude);
        if (result != null && result.list?.Count > 0)
        {
            UpdateUI(result);
        }
        else
        {
            await DisplayAlert("Error", "Unable to get weather for your location.", "OK");
        }
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        var response = await DisplayPromptAsync(title: "", message: "", placeholder: "Search weather by city", accept: "Search", cancel: "Cancel");
            if (response != null)
        {
            await GetWeatherDataByCity(response);
        }
    }
    public async Task GetWeatherDataByCity(string city)
    {
        try
        {
            var result = await ApiService.GetWeatherByCity(city);

            if (result != null && result.list?.Count > 0)
            {
                UpdateUI(result);
            }
            else
            {
                await DisplayAlert("Error", "City not found or invalid response.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    public void UpdateUI(dynamic result)
    {
        foreach (var item in result.list)
        {
            WeatherList.Add(item);
        }
        CvWeather.ItemsSource = WeatherList;

        CityLabel.Text = result.city.name;
        WeatherTypeLabel.Text = result.list[0].weather[0].description;
        TemperatureLabel.Text = result.list[0].main.temperature + "℃";
        HumidityLabel.Text = result.list[0].main.humidity + "%";
        WindLabel.Text = result.list[0].wind.speed + " km/h";
        MainWeatherIcon.Source = result.list[0].weather[0].customIcon;
    }
}