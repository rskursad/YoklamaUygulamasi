namespace yoklamaVsty;

public partial class MainMenu : ContentPage
{
    public MainMenu()
    {
        InitializeComponent();

    }


    async void Button_Clicked(object sender, EventArgs e)
    {
        Button btn = sender as Button;
        if (btn != null)
        {
            switch (btn.Text.ToLower())
            {
                case "yoklama":
                    await Navigation.PushModalAsync(new YoklamaPage(), true);
                    break;
                case "talebe görüntüle":
                    await Navigation.PushModalAsync(new Talebeler(), true);
                    break;
                case "talebe ekle":
                    await Navigation.PushModalAsync(new TalebeEkle(), true);
                    break;
                case "talebe sil":
                    await Navigation.PushModalAsync(new TalebeSil(), true);
                    break;
                case "grafikler":
                    await Navigation.PushModalAsync(new Grafikler());
                    break;
                case "excel dosyasý":
                case "excel dosyasi":
                    using (var stream = new MemoryStream())
                    {
                        await SaveToDownloadsAsync(stream.ToArray());
                    }

                    break;
            }
        }
    }





    private async Task SaveToDownloadsAsync(byte[] fileContent)
    {
        // Dosyayý kaydet
        string fileName = "Yoklama.xlsx";


        string filePath;

#if ANDROID
        // Android indirilenler klasörüne kaydet
        filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
#elif IOS || MACCATALYST
    // iOS/Mac için Belgeler klasörünü kullan
    var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    filePath = Path.Combine(documentsPath, fileName);
#elif WINDOWS
        // Windows için KnownFolders.Downloads kullan
        var downloadsFolder = await Windows.Storage.KnownFolders.GetFolderForUserAsync(null,Windows.Storage.KnownFolderId.AppCaptures);
        filePath = Path.Combine(downloadsFolder.Path, fileName);
#else
    throw new PlatformNotSupportedException("Bu platform desteklenmiyor.");
#endif


        // Dosyayý aç
        await OpenFileAsync(filePath);


    }
    private async Task OpenFileAsync(string filePath)
    {
        try
        {
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Dosya açýlamadý: {ex.Message}", "Tamam");
        }

    }

}