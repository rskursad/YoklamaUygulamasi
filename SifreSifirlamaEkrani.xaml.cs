namespace yoklamaVsty;

public partial class SifreSifirlamaEkrani : ContentPage
{
	private string Username {  get; set; }
	public SifreSifirlamaEkrani(string username)
	{
		InitializeComponent();
		lblTitle.Text = username + " kullanýcý adý için\nÞifre Deðiþikliði";
		lblTitle.HorizontalTextAlignment = TextAlignment.Center;
		Username = username;
	}

	async void btnOnayla_Clicked(object sender,EventArgs e)
	{
		if (!string.IsNullOrEmpty(txtYeniSifre.Text)&& !string.IsNullOrEmpty(txtYeniSifreTekrar.Text))
		{
			try
			{
				using (var con = new TalebelerModel())
				{
					var hoca = con.Hocalar.FirstOrDefault(h => h.KullaniciAdi == Username);
					if (hoca != null && txtYeniSifre.Text.Trim() == txtYeniSifreTekrar.Text.Trim())
					{
						hoca.Sifre = txtYeniSifre.Text.Trim();
						con.Hocalar.Update(hoca);
						con.SaveChanges();
						await Navigation.PushModalAsync(new MainPage());
						var sayfalar = Navigation.ModalStack;
						for (int i = 0; i < sayfalar.Count-1; i++)
						{
							Navigation.RemovePage(sayfalar[i]);
						}
                    }
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("HATA", ex.Message + "\n" + ex.StackTrace, "Tamam");
			}
        }
		else
		{
			await DisplayAlert("HATA", "HATALI GÝRÝÞ YAPILDI", "TAMAM");
		}
		
	}
}