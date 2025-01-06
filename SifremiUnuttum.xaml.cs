namespace yoklamaVsty;

public partial class SifremiUnuttum : ContentPage
{
	public SifremiUnuttum()
	{
		InitializeComponent();
	}
    void txtUserNameText_Changed(object sender, TextChangedEventArgs e)
    {
        using (var con=new TalebelerModel())
        {
            var hoca = con.Hocalar.FirstOrDefault(h => h.KullaniciAdi == txtUser.Text.Trim());
            if (hoca!=null)
            {
                txtSSSorusu.Text = hoca.SifreKurtarmaSorusu;
                frmSSSorusu.IsVisible = true;
                frmCevap.IsVisible = true;

            }
            else
            {
                frmSSSorusu.IsVisible=false;
                frmCevap.IsVisible=false;
            }
        }   
    }

    async void btnSifreDegistir_Clicked(object sender, EventArgs e)
    {
        using (var con=new TalebelerModel())
        {
            string cevap = txtCevap.Text!=null?txtCevap.Text.Trim():null;
            Hoca hoca = con.Hocalar.FirstOrDefault(h => h.KullaniciAdi == txtUser.Text.Trim());
            if (cevap!=null&&hoca!=null)
            {
                if (hoca.Cevap.ToLower() == cevap.ToLower())
                {
                    await Navigation.PushModalAsync(new SifreSifirlamaEkrani(hoca.KullaniciAdi));
                }
                else
                {
                    await DisplayAlert("HATA", "Hatalý Giriþ Yapýldý!", "Tamam");
                    await Navigation.PopModalAsync();
                }
            }
            
        }
    }
}