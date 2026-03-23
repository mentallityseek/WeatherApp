Font Folder -> Folder with fonts
Image Folder -> Folder with weather images
Main Window.xaml(.cs) -> own project logic files with api responses code
Styles Folder -> button, back and foreground app styles
App.xaml -> Resuorce Dictionaries
WeatherData.cs (class) -> Json reader class with host weather info

If you have some trouble with project try rebuild project and:
For yourself using get the new API for MainWindow.cs on this page https://www.weatherapi.com/
Change api in this code strting: private readonly string apiKey = "YOUR API KEY";
