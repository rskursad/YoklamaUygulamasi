using System.Collections.ObjectModel;
namespace yoklamaVsty;

public partial class Grafikler : ContentPage
{
    ObservableCollection<Talebe> aramaListesi = new ObservableCollection<Talebe>();
    public Grafikler()
    {
        InitializeComponent();
        using (var con = new TalebelerModel())
        {
            var listTalebeler = con.Talebeler.ToList();
            OgrencilerOC.Ogrenciler = new ObservableCollection<Talebe>(listTalebeler);
            OgrenciListView.ItemsSource = OgrencilerOC.Ogrenciler;
        }
    }
    private async void OgrenciListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var secilenOgrenci = e.Item as Talebe;
        if (secilenOgrenci != null)
        {
            await Navigation.PushModalAsync(new KisiselGrafik1(secilenOgrenci.Id,secilenOgrenci.Ad,secilenOgrenci.Soyad));
        }
    }

    void btnGGrafik_Clicked(object sender, EventArgs e) => Navigation.PushModalAsync(new GenelGrafik());

    void searchBarText_Changed(object sender, TextChangedEventArgs e)
    {
        Entry sb = sender as Entry;

        if (sb != null)
        {
            string Text = sb.Text;
            string[] arananKelimeler = Text.Split(' ');
            List<Talebe> arananKisiler;
            if (arananKelimeler.Length == 2)
            {
                arananKisiler = OgrencilerOC.Ogrenciler.Where(a => a.Ad.Contains(arananKelimeler[0]) || a.Ad.Contains(arananKelimeler[1]) || a.Soyad.Contains(arananKelimeler[0]) || a.Soyad.Contains(arananKelimeler[1])).ToList();
            }
            else
            {
                arananKisiler = OgrencilerOC.Ogrenciler.Where(a => a.Ad.Contains(Text) || a.Soyad.Contains(Text)).ToList();
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