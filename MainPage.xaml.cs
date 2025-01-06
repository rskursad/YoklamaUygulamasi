namespace yoklamaVsty
{
    public partial class MainPage : ContentPage
    {
        public static MainPage CurrentMainPage;

        public MainPage()
        {
            InitializeComponent();
            using (var con=new TalebelerModel())
            {
                con.Database.EnsureCreated();
                var kayit = con.BeniHatirlaTable.FirstOrDefault(k=>k.Id==1);
                if (kayit!=null)
                {
                    if (kayit.Durum==1)
                    {
                        txtUser.Text = kayit.KullaniciAdi;
                        txtPass.Text = kayit.Sifre;
                    }
                }
            }
        }

        async void btnGirisYap_Clicked(object sender, EventArgs e)
        {
            string username = null, password = null;
            if (!string.IsNullOrEmpty(txtUser.Text))
            {
                username = txtUser.Text.Trim();
            }
            if (!string.IsNullOrEmpty(txtPass.Text))
            {
                password = txtPass.Text.Trim();
            }
            
            using (var con=new TalebelerModel())
            {
                var hoca = con.Hocalar.FirstOrDefault(h=>h.KullaniciAdi==username&&h.Sifre==password);
                if (hoca!=null)
                {
                    await Navigation.PushModalAsync(new MainMenu());
                    var pages = Navigation.ModalStack;
                    for (int i = 0; i < (pages.Count - 1); i++)
                    {
                        Navigation.RemovePage(pages[i]);
                    }
                }
                else
                {
                    lblUyari.IsVisible = true;
                }
            }
           
        }
        async void btnKaydol_Clicked(object sender, EventArgs e)
        {
            CurrentMainPage = this;
           await Navigation.PushModalAsync(new kaydol());
        }

        void textForLogin_Changed(object sender, TextChangedEventArgs e)
        {
            lblUyari.IsVisible = false;
        }

        void BeniHatirlaChecked_Changed(object sender, CheckedChangedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                using (var con = new TalebelerModel())
                {
                    var kayit = con.BeniHatirlaTable.FirstOrDefault(k=>k.Id==1);
                    if (kayit != null)
                    {
                        kayit.Durum=checkBox.IsChecked? 1:0;
                        con.BeniHatirlaTable.Update(kayit);
                    }
                    else
                    {
                        con.BeniHatirlaTable.Add(new BeniHatirla()
                        {
                             KullaniciAdi=txtUser.Text.Trim(),
                             Sifre=txtPass.Text.Trim(),
                             Durum=checkBox.IsChecked? 1:0
                        });
                    }
                    con.SaveChanges();
                }
            }
        }

        async void SifremiUnuttum_Clicked(object sender,EventArgs e)
        {
            await Navigation.PushModalAsync(new SifremiUnuttum(),false);
        }


    }

}
