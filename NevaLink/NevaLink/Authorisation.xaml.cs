using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NevaLink
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Authorisation : ContentPage
    {
        public Authorisation()
        {
            InitializeComponent();
            BtnAuth.Background = new SolidColorBrush(Color.FromRgb(0, 113, 188));
            BtnAuth.TextColor = Color.FromRgb(255, 255, 255);
            BtnRegist.Background = new SolidColorBrush(Color.FromRgb(0, 113, 188));
            BtnRegist.TextColor = Color.FromRgb(255, 255, 255);

        }

        private async void btnRegistrat(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }


        private void btnAuth(object sender, System.EventArgs e)
        {
            if (login.Text != "" && login.Text != " " && password.Text != "" && password.Text != " ")
            {
                if (login.Text == "ip")
                {
                    labelErr.IsVisible = true;
                    labelErr.Text = @"Текущий адресс - " + serverCon.address;
                }
                else if (password.Text == "ip")
                {
                    serverCon.address = login.Text;
                }
                else
                {
                    string[][] resultAut = ServerApi.tableFunc("select * from clientAuthorChecked where login = '" + login.Text + "' and password = '" + security.getHash(password.Text) + "';");

                    if (resultAut[0][0] != "No")
                    {
                        security.ID = Convert.ToInt32(resultAut[1][1]);

                        security.Family = resultAut[1][3];
                        security.Name = resultAut[1][2];
                        security.MiddleName = resultAut[1][4];
                        security.Date = resultAut[1][5];

                        security.rate = Convert.ToInt32(resultAut[1][6]);
                        security.rateName = resultAut[1][8];
                        security.rate_price = Convert.ToInt32(resultAut[1][10]);
                        security.rateDescription = resultAut[1][9];

                        security.home = Convert.ToInt32(resultAut[1][7]);
                        security.homeDescription = resultAut[1][11] + " " + resultAut[1][12] + " " + resultAut[1][13];
                        
                        GoPage();
                    }
                    else
                    {
                        labelErr.IsVisible = true;
                        labelErr.Text = @"Логин\пароль не подходят!";
                        Anim();
                    }
                }

                }
            else
            {
                labelErr.IsVisible = true;
                labelErr.Text = "Введите данные!";
                Anim();
            }
        }

        private async void Anim()
        {
            await labelErr.TranslateTo(-10, 0, 200);
            await labelErr.TranslateTo(0, 0, 200);
            await labelErr.TranslateTo(+10, 0, 200);
            await labelErr.TranslateTo(0, 0, 200);
            await labelErr.TranslateTo(-10, 0, 200);
            await labelErr.TranslateTo(0, 0, 200);
            await labelErr.TranslateTo(+10, 0, 200);
            await labelErr.TranslateTo(0, 0, 200);
        }

        private async void GoPage()
        {
            await Navigation.PushAsync(new Menu());
        }

    }
}