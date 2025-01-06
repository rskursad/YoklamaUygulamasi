using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.ObjectModel;
namespace yoklamaVsty;

public partial class Ogrenciler : ContentPage
{
    ObservableCollection<Ogrenci> aramaListesi = new ObservableCollection<Ogrenci>();
    public Ogrenciler()
    {
        InitializeComponent();
        using (var con = new TalebelerModel())
        {
            var listOgrenciler = con.Talebeler.ToList();
            OgrencilerOC.Ogrenciler = new ObservableCollection<Ogrenci>(listOgrenciler);
            OgrenciListView.ItemsSource = OgrencilerOC.Ogrenciler;
        }
    }
    private void OgrenciListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var secilenOgrenci = e.Item as Ogrenci;
        if (secilenOgrenci != null)
        {
            DisplayAlert("Öðrenci Bilgisi", $"Id:{secilenOgrenci.Id} Ad: {secilenOgrenci.Ad} {secilenOgrenci.Soyad}", "Tamam");
        }
    }

    void searchBarText_Changed(object sender, TextChangedEventArgs e)
    {
        Entry sb = sender as Entry;

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
                arananKisiler = OgrencilerOC.Ogrenciler.Where(a => a.Ad.ToLower().Contains(Text.ToLower()) || a.Soyad.ToLower().Contains(Text)).ToList();
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
}