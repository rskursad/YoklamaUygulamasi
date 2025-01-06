using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace yoklamaVsty;

public partial class OgrenciEkle : ContentPage
{
	public OgrenciEkle()
	{
		InitializeComponent();
	}

    void btnGeri_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    async void btnTalebeEkle_Clicked(object sender, EventArgs e)
    {
        string ad = null, soyad = null;
        if ((txtAd.Text != null && !string.IsNullOrEmpty(txtAd.Text.Trim())) && (txtSoyad.Text != null && !string.IsNullOrEmpty(txtSoyad.Text.Trim())))
        {
            using (var con = new TalebelerModel())
            {
                var talebe = con.Talebeler.FirstOrDefault(t => t.Ad.ToLower() == txtAd.Text.Trim().ToLower() && t.Soyad.ToLower() == txtSoyad.Text.Trim().ToLower());
                if (talebe == null)
                {
                    ad = txtAd.Text.Trim();
                    soyad = txtSoyad.Text.Trim();
                    try
                    {
                        con.Talebeler.Add(new Talebe() { Ad = ad, Soyad = soyad });
                        con.SaveChanges();
                        await DisplayAlert("BAÞARILI", "Öðrenci ekleme iþlemi baþarýlý bir þekilde tammalandý!", "TAMAM");
                    }
                    catch (SqliteException slex)
                    {
                        await DisplayAlert("BAÞARISIZ", slex.Message + "\n" + slex.StackTrace + "\n" + slex.Source, "TAMAM");
                    }
                }
                else
                {
                    await DisplayAlert("BAÞARISIZ", "Bu isme sahip bir öðrenci zaten mevcut!", "TAMAM");
                }
            }

        }
        else
        {
            await DisplayAlert("BAÞARISIZ", "Boþ kutucuklarý uygun þekilde doldurunuz!", "TAMAM");
        }
        kutucuklariTemizle();
    }
    void kutucuklariTemizle()
    {
        txtAd.Text = "";
        txtSoyad.Text = "";
    }
}