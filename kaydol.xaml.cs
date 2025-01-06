namespace yoklamaVsty;

public partial class kaydol : ContentPage
{
    List<Entry> kutular;
    public kaydol()
    {
        InitializeComponent();
        kutular = new List<Entry>();
        kutular.AddRange(new List<Entry> { txtAd, txtSoyad, txtKullaniciAdi, txtSifre, txtSifreKurtarmaSorusu, txtCevap });

    }
    async void btnKaydol_Clicked(object sender, EventArgs e)
    {
        foreach (Entry kutu in kutular)
        {
            Entry txtbox = kutu;
            if (string.IsNullOrEmpty(txtbox.Text))
            {
                await DisplayAlert("BOÞ BIRAKILAMAZ!","LÜTFEN KUTUCUKLARI DOLDURUN","Tamam");
                return;
            }
        }
        try
        {
            using (var con = new TalebelerModel())
            {
                String ad = txtAd.Text.Trim(),KAdi=txtKullaniciAdi.Text.Trim();
                var hoca= con.Hocalar.FirstOrDefault(h => h.KullaniciAdi == txtKullaniciAdi.Text.Trim());
                if (hoca==null)
                {
                    
                    con.Hocalar.Add(new Hoca()
                    {
                        Ad = txtAd.Text.Trim(),
                        Soyad = txtSoyad.Text.Trim(),
                        KullaniciAdi = txtKullaniciAdi.Text.Trim(),
                        Sifre = txtSifre.Text.Trim(),
                        SifreKurtarmaSorusu = txtSifreKurtarmaSorusu.Text.Trim(),
                        Cevap = txtCevap.Text.Trim()
                    });
                    con.SaveChanges();
                    await DisplayAlert("BAÞARILI", ad.ToUpper() + " isimli yeni kullanýcý/eðitmen eklendi!", "Tamam");
                    await Navigation.PopModalAsync();
                }
                else
                {
                    await DisplayAlert("BAÞARISIZ", KAdi + " kullanýcý adlý kullanýcý/eðitmen zaten eklenmiþ!", "Tamam");
                    txtKullaniciAdi.Text = "";
                } 
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Beklenmeyen Hata", ex.Message + "\n" + ex.StackTrace, "Tamam");
        }
           
    }
}