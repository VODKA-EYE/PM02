using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using PM02.Context;
using PM02.Models;

namespace PM02;

public partial class MainWindow : Window
{
  private PostgresContext dbcontext = new();
  private Random random = new();
  public MainWindow()
  {
    InitializeComponent();
    SelectDataDisplayedComboBox.SelectionChanged += SelectDataDisplayedComboBox_OnSelectionChanged;
    LoadMeasurements();
    LoadAdditionalStatistics();
  }

  private void LoadMeasurements()
  {
    // show measurement listbox
    MeasurementsListBox.IsVisible = true;
    StationsListBox.IsVisible = false;
    SensorsListBox.IsVisible = false;
    // load headers
    TB_SensorNumber_StationNumber.Text = "Номер сенсора";
    TB_StationName.Text = "";
    TB_MeasurementValue_WhichStation_StationLongitude.Text = "Измерение";
    TB_MeasurementName_AddedTS_StationLatitude.Text = "Тип измерения";
    TB_MeasurementTs.Text = "Время измерения";
    
    List<Measurement> measurements = dbcontext.Measurements.ToList();
    TBLastMeasurement.Text = measurements.OrderByDescending(m => m.MeasurementTs).FirstOrDefault().MeasurementTs.ToString();
    
    // Average measurement
    decimal measurementValues = 0;
    int amountOfMeasurements = 0;
    foreach (var measurement in measurements)
    {
      measurementValues += measurement.MeasurementValue;
      amountOfMeasurements++;
    }
    TBAverageOfMeasurements.Text = (measurementValues / amountOfMeasurements).ToString();

    // Searching
    string searchString = SearchTextBox.Text ?? "";
    searchString = searchString.ToLower();
    string[] searchStringElements = searchString.Split(' ');

    if (!string.IsNullOrEmpty(searchString))
    {
      foreach (var element in searchStringElements)
      {
        measurements = measurements.Where(c =>
          c.SensorInventoryNumberNavigation.Station.StationName.ToLower().Contains(element) ||
          c.SensorInventoryNumberNavigation.Sensor.SensorName.ToLower().Contains(element) 
        ).ToList();
      }
    }
    
    // Sorting
    switch (SortComboBox.SelectedIndex)
    {
      case 1:
        measurements = measurements.OrderByDescending(m => m.MeasurementValue).ToList();
        break;
      case 2:
        measurements = measurements.OrderBy(m => m.MeasurementValue).ToList();
        break;
      case 3:
        measurements = measurements.OrderByDescending(m => m.MeasurementTs).ToList();
        break;
      case 4:
        measurements = measurements.OrderBy(m => m.MeasurementTs).ToList();
        break;
    }
    
    MeasurementsListBox.ItemsSource = measurements;
  }
  private void LoadMeteostationSensors()
  {
    // Show correct listbox
    MeasurementsListBox.IsVisible = false;
    StationsListBox.IsVisible = false;
    SensorsListBox.IsVisible = true;
    // Load headers
    TB_SensorNumber_StationNumber.Text = "Номер сенсора";
    TB_StationName.Text = "";
    TB_MeasurementValue_WhichStation_StationLongitude.Text = "Модель сенсора";
    TB_MeasurementName_AddedTS_StationLatitude.Text = "Название станции";
    TB_MeasurementTs.Text = "Дата установки";
    
    List<MeteostationsSensor> meteostationsSensors = dbcontext.MeteostationsSensors.ToList();
    
    // Searching
    string searchString = SearchTextBox.Text ?? "";
    searchString = searchString.ToLower();
    string[] searchStringElements = searchString.Split(' ');

    if (!string.IsNullOrEmpty(searchString))
    {
      foreach (var element in searchStringElements)
      {
        meteostationsSensors = meteostationsSensors.Where(c =>
          c.Station.StationName.ToLower().Contains(element) ||
          c.Sensor.SensorName.ToLower().Contains(element) 
        ).ToList();
      }
    }
    
    SensorsListBox.ItemsSource = meteostationsSensors;
  }

  private void LoadMeteostations()
  {
    // Show correct listbox
    MeasurementsListBox.IsVisible = false;
    StationsListBox.IsVisible = true;
    SensorsListBox.IsVisible = false;
    // Load headers
    TB_SensorNumber_StationNumber.Text = "Номер станции";
    TB_StationName.Text = "Имя станции";
    TB_MeasurementValue_WhichStation_StationLongitude.Text = "Долгота";
    TB_MeasurementName_AddedTS_StationLatitude.Text = "Широта";
    TB_MeasurementTs.Text = "";
    
    List<Meteostation> meteostations = dbcontext.Meteostations.ToList();
    
    // Searching
    string searchString = SearchTextBox.Text ?? "";
    searchString = searchString.ToLower();
    string[] searchStringElements = searchString.Split(' ');

    if (!string.IsNullOrEmpty(searchString))
    {
      foreach (var element in searchStringElements)
      {
        meteostations = meteostations.Where(c =>
          c.StationName.ToLower().Contains(element)
        ).ToList();
      }
    }
    
    StationsListBox.ItemsSource = meteostations;
  }
  private void LoadAdditionalStatistics()
  {
    TBAmountOfSensors.Text = dbcontext.MeteostationsSensors.Count().ToString();
    TBAmountOfMeteostations.Text = dbcontext.Meteostations.Count().ToString();
  }

  private void SelectDataDisplayedComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
  {
    switch (SelectDataDisplayedComboBox.SelectedIndex)
    {
      case 0:
        LoadMeasurements();
        break;
      case 1:
        LoadMeteostationSensors();
        break;
      case 2:
        LoadMeteostations();
        break;
    }
  }

  private void GenerateButton_OnClick(object? sender, RoutedEventArgs e)
  {
    int howManyGenerate;
    try
    {
      HowMuchGenerate.Background = Brushes.White;
      howManyGenerate = Int32.Parse(HowMuchGenerate.Text);
      switch (SelectDataDisplayedComboBox.SelectedIndex)
      {
        case 0:
          for (int i = 0; i < howManyGenerate; i++)
          {
            GenerateMeasurements();
          }
          break;
        case 1:
          for (int i = 0; i < howManyGenerate; i++)
          {
            GenerateSensors();
          }
          break;
        case 2:
          for (int i = 0; i < howManyGenerate; i++)
          {
            GenerateMeteostations();
          }
          break;
      }
    }
    catch (Exception exception)
    {
      HowMuchGenerate.Background = Brushes.Red;
    }
  }

  private void MeasurementsListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
  {
    var listboxObject = MeasurementsListBox.SelectedItem;
    if (listboxObject != null)
    {
      Guid id = (Guid)(listboxObject.GetType().GetProperty("SensorInventoryNumber").GetValue(listboxObject, null));
      MeteostationsSensor sensor = dbcontext.MeteostationsSensors.Where(ms => ms.SensorInventoryNumber == id)
        .FirstOrDefault();
      TB_Additional_StationName.Text = sensor.Station.StationName;
      TB_Additional_SensorName.Text = sensor.Sensor.SensorName;
      TB_Additional_AddedTS.Text = sensor.AddedTs.ToString();
    }
    MeasurementsListBox.UnselectAll();
  }

  private void GenerateMeasurements()
  {
    Measurement measurement = new();
    List<MeteostationsSensor> sensors = dbcontext.MeteostationsSensors.ToList();
    
    // Get uuid of sensors
    Guid[] sensorsIds = new Guid[sensors.Count];
    for (int i = 0; i < sensors.Count; i++)
    {
      sensorsIds[i] = sensors[i].SensorInventoryNumber;
    }
    measurement.SensorInventoryNumber = sensorsIds[random.Next(0, sensorsIds.Length)];
    // Get random measurement value from -100 to 100
    measurement.MeasurementValue = Convert.ToDecimal(random.NextDouble() * (random.Next(0,101)-random.Next(0,101)));
    measurement.MeasurementType = random.Next(1,dbcontext.MeasurementsTypes.Count());
    // Save measurement
    dbcontext.Add(measurement);
    dbcontext.SaveChanges();
    dbcontext = new();
    LoadAdditionalStatistics();
    LoadMeasurements();
  }

  private void GenerateMeteostations()
  {
    Meteostation meteostation = new();
    // Given names of meteostations, and set random one
    string[] meteostationNames = {"Jan Mayen", "Grahuken", "Hornsund", "Ny-Alesund Ii", "Edgeoya", "Ny Alesund", "Svalbard Lufthavn", "Phippsoya" };
    string randomName = meteostationNames[random.Next(0, meteostationNames.Length)];
    meteostation.StationName = randomName;
    // Random longitude and latitude
    meteostation.StationLongitude = Convert.ToDecimal(random.NextDouble()*180);
    meteostation.StationLatitude = Convert.ToDecimal(random.NextDouble()*90);
    // Save new meteostation
    dbcontext.Add(meteostation);
    dbcontext.SaveChanges();
    dbcontext = new();
    LoadAdditionalStatistics();
    LoadMeteostations();
  }

  private void GenerateSensors()
  {
    MeteostationsSensor meteostationsSensor = new();
    meteostationsSensor.SensorInventoryNumber = Guid.NewGuid();

    List<Meteostation> meteostations = dbcontext.Meteostations.ToList();
    // get meteostation ID
    int[] meteostationsIds = new int[meteostations.Count];
    for (int i = 0; i < meteostations.Count; i++)
    {
      meteostationsIds[i] = meteostations[i].StationId;
    }
    meteostationsSensor.StationId = meteostationsIds[random.Next(0,meteostationsIds.Length)];
    meteostationsSensor.SensorId = random.Next(1, dbcontext.Sensors.Count());
    // Set random date from 2000 year
    DateTime initialDate = new(2000,1,1);
    int range = (DateTime.Today - initialDate).Days;
    meteostationsSensor.AddedTs = initialDate.AddDays(random.Next(range));
    // Save new sensor
    dbcontext.Add(meteostationsSensor);
    dbcontext.SaveChanges();
    dbcontext = new();
    LoadAdditionalStatistics();
    LoadMeteostationSensors();
  }
  
}