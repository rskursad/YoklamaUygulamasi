using System.Collections.ObjectModel;

namespace yoklamaVsty;

public partial class KisiselGrafik1 : ContentPage
{
    ObservableCollection<YoklamaModel> OCyoklamaListesi ;
    public KisiselGrafik1( int talebeID,string talebeAD, string talebeSOYAD)
	{
		InitializeComponent();

        //using (var con = new TalebelerModel())
        //{
        //    var listTalebeler = con.Talebeler.ToList();
        //    OgrencilerOC.Ogrenciler = new ObservableCollection<Talebe>(listTalebeler);
        //    OgrenciListView.ItemsSource = OgrencilerOC.Ogrenciler;
        //}
        lblKisiselBaslik.Text = talebeAD.ToUpper() + " "+ talebeSOYAD.ToUpper()+"\nKATILIM GRAFÝÐÝ";
        using (var con = new TalebelerModel())
        {
            double toplamYoklama = 0, toplamKatilim = 0, GenelSonuc = 0;
                   
            if (con.YoklamaTable.Any())
            {
                toplamYoklama = con.YoklamaTable.Where(y=> y.TalebeId == talebeID).Count();
                toplamKatilim = con.YoklamaTable.Where(y => y.Durum == "Var"&&y.TalebeId==talebeID).Count();
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

            var listYoklamalar = con.YoklamaTable.Where(y => y.TalebeId == talebeID).ToList();
            OCyoklamaListesi = new ObservableCollection<YoklamaModel>(listYoklamalar);
            OgrenciListView.ItemsSource= OCyoklamaListesi;
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