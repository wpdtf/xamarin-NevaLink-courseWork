using Xamarin.Forms;

namespace NevaLink
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BtnRegist.Background = new SolidColorBrush(Color.FromRgb(0, 113, 188));
            BtnRegist.TextColor = Color.FromRgb(255, 255, 255);

            line1.Stroke = new SolidColorBrush(Color.FromRgb(0, 113, 188));
            line2.Stroke = new SolidColorBrush(Color.FromRgb(0, 113, 188));
        }

        private async void btnRegistrat(object sender, System.EventArgs e)
        {
            if (name.Text != "" && name.Text != " " && family.Text != "" && family.Text != " " && login.Text != "" && login.Text != " " && password1.Text != "" && password1.Text != " " && password2.Text != "" && password2.Text != " ")
            {
                if (password1.Text == password2.Text)
                {
                    string[][] resultAut = ServerApi.tableFunc("select id_client from Client where login = '"+login.Text+"';");

                    if (resultAut[0][0]=="No")
                    {
                        if (middlename.Text != "" && middlename.Text != " ")
                        {
                            resultAut = ServerApi.tableFunc("insert into Client values (3, '" + family.Text + "', '" + name.Text + "', '" + middlename.Text + "', GETDATE(), 2, 2, '" + login.Text + "', '" + security.getHash(password1.Text) + "');");
                            await this.DisplayAlert("Аккаунт создан!", "Авторизуйтесь", "Ок");
                            Back();

                        }
                        else
                        {
                            resultAut = ServerApi.tableFunc("insert into Client values (3, '" + family.Text + "', '" + name.Text + "', null, GETDATE(), 2, 2, '" + login.Text + "', '" + security.getHash(password1.Text) + "');");
                            await this.DisplayAlert("Аккаунт создан!", "Авторизуйтесь", "Ок");
                            Back();
                        }
                    }
                    else
                    {
                        labelErrs.IsVisible = true;
                        labelErrs.Text = "Такой логин уже существует!";
                        login.Text = "";
                        password1.Text = "";
                        password2.Text = "";
                    }
                }
                else
                {
                    labelErrs.IsVisible = true;
                    labelErrs.Text = "Пароли отличаются!";
                    password1.Text = "";
                    password2.Text = "";
                }  
            }
            else
            {
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

        private async void Back()
        {
            await Navigation.PopAsync();
        }
    }
}
