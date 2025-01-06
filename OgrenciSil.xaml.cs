using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.ObjectModel;
using System.Drawing;

namespace yoklamaVsty;

public partial class OgrenciSil : ContentPage
{
    ObservableCollection<Ogrenci> aramaListesi = new ObservableCollection<Ogrenci>();
    public OgrenciSil()
	{
		InitializeComponent();
        var listOgrenciler = con.Talebeler.ToList();
        OgrencilerOC.Ogrenciler = new ObservableCollection<Ogrenci>(listOgrenciler);
        OgrenciListView.ItemsSource = OgrencilerOC.Ogrenciler;
    }

    private void OgrenciListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var secilenOgrenci = e.Item as Ogrenci;
        if (secilenOgrenci != null)
        {
            DisplayAlert("Öðrenci Bilgisi", $"{secilenOgrenci.Ad} {secilenOgrenci.Soyad}", "Tamam");
        }
    }

    async void SilButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            Button btn = sender as Button;
            int id = (int)btn.CommandParameter;

            using (var con = new TalebelerModel())
            {
                var silinecek = con.Talebeler.FirstOrDefault(t => t.Id == id);
                if (silinecek != null)
                {
                    bool onay = await DisplayAlert("Emin misiniz?",
                                                   $"{silinecek.Ad} {silinecek.Soyad} isimli þahýs silinecek!",
                                                   "Evet",
                                                   "Hayýr");
                    if (onay)
                    {
                        // Veritabanýndan sil
                        con.Talebeler.Remove(silinecek);
                        con.SaveChanges();

                        // ObservableCollection'dan sil
                        OgrencilerOC.Ogrenciler.Remove(OgrencilerOC.Ogrenciler.First(o => o.Id == id));
                    }
                }
            }
        }
        catch (SqliteException slex)
        {
            await DisplayAlert("Veri Tabaný Hatasý", slex.Message, "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Bilinmeyen Hata", ex.Message, "Tamam");
        }
    }


    void searchBarText_Changed(object sender, TextChangedEventArgs e)
    {
        SearchBar sb = sender as SearchBar;

        if (sb != null)
        {
            string Text = sb.Text;
            string[] arananKelimeler = Text.Split(' ');
            List<Ogrenci> arananKisiler;
            if (arananKelimeler.Length == 2)
            {
                arananKisiler = OgrencilerOC.Ogrenciler.Where(a => a.Ad.ToLower().Contains(arananKelimeler[0].ToLower()) || a.Ad.ToLower().Contains(arananKelimeler[1].ToLower()) || a.Soyad.ToLower().Contains(arananKelimeler[0].ToLower()) || a.Soyad.ToLower().Contains(arananKelimeler[1].ToLower())).ToList();
            }
            else
            {
                arananKisiler = OgrencilerOC.Ogrenciler.Where(a => a.Ad.ToLower().Contains(Text) || a.Soyad.ToLower().Contains(Text.ToLower())).ToList();
            }

            if (arananKisiler.Any())
            {
                aramaListesi.Clear();

                foreach (var kisi in arananKisiler)
                {
                    aramaListesi.Add(kisi);
                }
                OgrenciListView.ItemsSource = aramaListesi;
            }

        }

    }

    void btnGeri_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}