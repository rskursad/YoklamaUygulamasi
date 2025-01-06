using OfficeOpenXml;
using System.Collections.ObjectModel;

namespace yoklamaVsty;

public partial class YoklamaPage : ContentPage
{
    ObservableCollection<YoklamaModel> yoklamaListesi;
    bool kontrol = true;
    public YoklamaPage()
    {
        InitializeComponent();
        yoklamaListesi = new ObservableCollection<YoklamaModel>();
        using (var con = new TalebelerModel())
        {
            var listTalebeler = con.Talebeler.ToList();
            OgrencilerOC.Ogrenciler = new ObservableCollection<Ogrenci>(listTalebeler);
            OgrenciListView.ItemsSource = OgrencilerOC.Ogrenciler;
        }
    }
    private async void OgrenciListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var secilenOgrenci = e.Item as Ogrenci;
        if (secilenOgrenci != null)
        {
            await DisplayAlert("Öðrenci Bilgisi", $"{secilenOgrenci.Ad} {secilenOgrenci.Soyad}", "Tamam");
        }
    }



    async void checked_changed(object sender, CheckedChangedEventArgs e)
    {
        try
        {
            CheckBox check = sender as CheckBox;
            if (int.TryParse(check.AutomationId, out int talebeId))
            {

                var mevcutKayit = yoklamaListesi.FirstOrDefault(o => o.TalebeId == talebeId);

                if (mevcutKayit != null)
                {

                    yoklamaListesi.Remove(mevcutKayit);
                }


                var talebe = OgrencilerOC.Ogrenciler.FirstOrDefault(o => o.Id == talebeId);
                if (talebe != null)
                {
                    var mevcutSaat = DateTime.Now.Hour;
                    string zmn = mevcutSaat < 12 ? "Sabah" : "Akþam";
                    var yeniKayit = new YoklamaModel
                    {
                        Tarih = DateTime.Now,
                        TalebeId = talebe.Id,
                        Ad = talebe.Ad,
                        Soyad = talebe.Soyad,
                        Durum = check.IsChecked ? "Var" : "Yok",
                        zaman = zmn
                    };

                    yoklamaListesi.Add(yeniKayit);
                }
            }
            else
            {
                await DisplayAlert("Hata", "Geçersiz AutomationId", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }


    void yoklamayiBitir_Clicked(object sender, EventArgs e)
    {

        yokYazilacaklar();
        veritabaniniGuncelle();
        ExportToExcelAsync();
    }

    void yokYazilacaklar()
    {
        using (var con = new TalebelerModel())
        {
            List<int> olanlar = new List<int>();
            foreach (var yoklama in yoklamaListesi)
            {
                olanlar.Add(yoklama.TalebeId);
                olanlar.Capacity = olanlar.Count;
            }
            var olmayanlar = con.Talebeler.Where(o => !olanlar.Contains(o.Id)).ToList();
            if (olmayanlar != null)
            {
                var mevcutSaat = DateTime.Now.Hour;
                string zmn = mevcutSaat < 12 ? "Sabah" : "Akþam";
                foreach (var olmayan in olmayanlar)
                {
                    YoklamaModel model = new YoklamaModel() { Tarih = DateTime.Now, TalebeId = olmayan.Id, Ad = olmayan.Ad, Soyad = olmayan.Soyad, Durum = "Yok", zaman = zmn };
                    yoklamaListesi.Add(model);
                }
            }

        }
    }

    async void veritabaniniGuncelle()
    {
        try
        {
            using (var con = new TalebelerModel())
            {
                var mevcutSaat = DateTime.Now.Hour;
                string ogun = mevcutSaat < 12 ? "Sabah" : "Akþam";

                // Ayný gün ve öðün için kontrol
                bool ogunAlindi = con.geciciTarihler.Any(y => y.Tarih.Date == DateTime.Now.Date && y.Ogun == ogun);

                if (!ogunAlindi)
                {
                    // Yoklama kayýtlarýný ekle
                    foreach (var yoklama in yoklamaListesi)
                    {
                        con.YoklamaTable.Add(yoklama);
                    }

                    // Öðün bilgisini kaydet
                    con.geciciTarihler.Add(new geciciTarih
                    {
                        Tarih = DateTime.Now,
                        Ogun = ogun
                    });

                    con.SaveChanges();
                    kontrol = false;
                }
                else
                {
                    await DisplayAlert("Eþleþme", $"{ogun} yoklamasý bugün zaten alýnmýþ!", "Tamam");
                    kontrol = true;
                }
            }
        }
        catch (Microsoft.Data.Sqlite.SqliteException slex)
        {
            await DisplayAlert("Veri Tabaný Hatasý", slex.Message, "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Bilinmeyen Hata", ex.Message, "Tamam");
        }
    }

    private async Task ExportToExcelAsync()
    {
        if (!kontrol)
        {
            List<Ogrenci> talebeler;
            List<YoklamaModel> yoklamalar;
            List<geciciTarih> Tarihler;

            using (var con = new TalebelerModel())
            {
                talebeler = con.Talebeler.ToList();
                yoklamalar = con.YoklamaTable.ToList();
                Tarihler = con.geciciTarihler.ToList();
            }

            try
            {
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Yoklama");

                    // Tarihler sabah/akþam ayrýmýyla düzenleniyor
                    var tarihler = Tarihler
                        .OrderBy(t => t.Tarih)
                        .Select(t => new { t.Tarih, t.Ogun })
                        .ToList();

                    worksheet.Cells[1, 1].Value = "Öðrenci";
                    for (int i = 0; i < tarihler.Count; i++)
                    {
                        var tarih = tarihler[i];
                        string ogunLabel = tarih.Ogun == "Sabah" ? "(1.)" : "(2.)";
                        worksheet.Cells[1, i + 2].Value = $"{ogunLabel} {tarih.Tarih.ToShortDateString()}";
                    }

                    int row = 2;
                    foreach (var talebe in talebeler)
                    {
                        worksheet.Cells[row, 1].Value = $"{talebe.Ad} {talebe.Soyad}";

                        for (int col = 0; col < tarihler.Count; col++)
                        {
                            var tarih = tarihler[col].Tarih;
                            //var zmn = DateTime.Now.Hour < 12 ? "Sabah" : "Akþam";


                            //var yoklama = yoklamalar.FirstOrDefault(y =>
                            //    y.TalebeId == talebe.Id && y.Tarih.Date == tarih.Date);


                            var arananYoklamalar = yoklamalar.Where(y => y.TalebeId == talebe.Id && y.Tarih.Date == tarih.Date).ToList();
                            if (arananYoklamalar.Any()) {
                                foreach (var yoklama in arananYoklamalar)
                                {
                                    var zmn = yoklama.Tarih.Hour<12?"Sabah":"Akþam";
                                    if (yoklama.zaman==zmn)
                                    {
                                        worksheet.Cells[row, col + 2].Value = yoklama != null ? yoklama.Durum : "Yok";
                                        break;
                                    }
                                    
                                }
                                
                            }
                            
                        }

                        row++;
                    }

                    // Dosyayý kaydet
                    var fileName = "Yoklama.xlsx";
                    using (var stream = new MemoryStream())
                    {
                        package.SaveAs(stream);
                        await SaveToDownloadsAsync(fileName, stream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
    }


    private async Task SaveToDownloadsAsync(string fileName, byte[] fileContent)
    {
        if (kontrol == false)
        {
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
            try
            {
                await File.WriteAllBytesAsync(filePath, fileContent);
                await DisplayAlert("Baþarýlý", $"Dosya kaydedildi: {filePath}", "Tamam");
            }
            catch (UnauthorizedAccessException ex)
            {
                await DisplayAlert("Eriþim Hatasý", $"Dosyaya yazma izni yok: {ex.Message}", "Tamam");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Bilinmeyen Hata", $"Bir hata oluþtu: {ex.Message}", "Tamam");

                // Dosyayý aç
                
            }
            await OpenFileAsync(filePath);
        }

    }
    private async Task OpenFileAsync(string filePath)
    {
        if (kontrol == false)
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

    void btnGeri_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

}