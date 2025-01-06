using System.Collections.ObjectModel;

namespace yoklamaVsty;

public partial class GenelGrafik : ContentPage
{
    ObservableCollection<YoklamaModel> OCyoklamaListesi;

    public GenelGrafik()
	{
		InitializeComponent();
        using (var con=new TalebelerModel())
        {
            double toplamYoklama = 0,toplamKatilim=0,GenelSonuc=0,
                    sabahYoklama=0,sabahKatilim=0,SabahSonuc=0,
                    yatsiYoklama = 0, yatsiKatilim = 0, YatsiSonuc = 0;
            if (con.YoklamaTable.Any())
            {
                toplamYoklama= con.YoklamaTable.Count();
                toplamKatilim = con.YoklamaTable.Where(y=>y.Durum=="Var").Count(); 
                GenelSonuc = toplamKatilim / toplamYoklama;
                barGenelProgress.Progress = GenelSonuc;
                if (GenelSonuc >= 0)
                {
                    lblGenelBar.Text = "%" + GenelSonuc * 100;
                }
                else
                {
                    lblGenelBar.Text = "Hesaplama için veri yok!";
                }
                
            }

            var listYoklamalar = con.YoklamaTable.ToList();
            OCyoklamaListesi = new ObservableCollection<YoklamaModel>(listYoklamalar);
            OgrenciListView.ItemsSource = OCyoklamaListesi;

        }
	}

    private void OgrenciListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var secilenKayit = e.Item as YoklamaModel;
        if (secilenKayit != null)
        {
            DisplayAlert("Kayýt Bilgisi", $"Tarih:{secilenKayit.Tarih.ToShortDateString()}\nZaman: {secilenKayit.zaman}\nAd: {secilenKayit.Ad}\nSoyad: {secilenKayit.Soyad}\nDurum:  {secilenKayit.Durum}", "Tamam");
        }
    }

    void btnGeri_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}