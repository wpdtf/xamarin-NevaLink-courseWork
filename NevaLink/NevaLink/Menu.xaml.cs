using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NevaLink
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu : ContentPage
    {
        public Menu()
        {
            InitializeComponent();
            frameTitle.BorderColor = Color.FromRgb(0, 113, 188);
            frameRate.BorderColor = Color.FromRgb(0, 113, 188);
            frameRate2.BorderColor = Color.FromRgb(0, 113, 188);
            BtnHelpSys.Background = new SolidColorBrush(Color.FromRgb(0, 113, 188));
            BtnHelpSys.TextColor = Color.FromRgb(255, 255, 255);
            upload();
        }

        private void upload()
        {
            titleName.Text = security.Family + " " + security.Name + " " + security.MiddleName;
            titleDate.Text = security.Date;
            titleHome.Text = security.homeDescription;


            if (security.rate != 2)
            {
                imageRate.Source = ImageSource.FromFile("ethernet.png");
                frameRate.IsVisible = true;
                titleRateName.Text = security.rateName;
                titleRateDescription.Text = security.rateDescription;
                titleRatePrice.Text = security.rate_price.ToString() + " руб";
            }
            stackRate();
        }

        private void stackRate()
        {
            string[][] resultRate = ServerApi.tableFunc("select * from rate where id_rate<>2 and id_rate<>"+security.rate+";");
            for (int i = 1; i<resultRate.Length; i++)
            {
                Label tN = new Label() { Text = "Название тарифа:" };
                tN.FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label));
                Label eN = new Label() { Text = resultRate[i][2] };
                eN.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                Label tO = new Label() { Text = "Описание тарифа:" };
                tO.FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label));
                Label eO = new Label() { Text = resultRate[i][3] };
                eO.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                Label tP = new Label() { Text = "Стоимость (месяц):" };
                tP.FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label));
                Label eP = new Label() { Text = resultRate[i][4]+" руб" };
                eP.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                Button bt_of = new Button() { Text = "Оформить", WidthRequest = 130, CornerRadius = 10, Background = new SolidColorBrush(Color.FromRgb(0, 113, 188)), Margin = 3, TextColor = Color.FromRgb(255, 255, 255), CommandParameter = resultRate[i][1]+"_"+ resultRate[i][2] + "_"+ resultRate[i][3] + "_"+ resultRate[i][4] };
                Button bt_hp = new Button() { Text = "Помощь", WidthRequest = 130, CornerRadius = 10, Background = new SolidColorBrush(Color.FromRgb(0, 113, 188)), Margin = 3, TextColor = Color.FromRgb(255, 255, 255), CommandParameter = resultRate[i][2] };
                bt_of.Clicked += (e, s) => btnHelpRate(e, s);
                bt_hp.Clicked += (e, s) => btnHelpMen(e, s);
                Frame rateF = new Frame()
                {
                    CornerRadius = 10,
                    Padding = 15,
                    Margin = 5,
                    BorderColor = Color.FromRgb(0, 113, 188),

                    Content = new StackLayout
                    {
                        Children = {tN, eN, tO, eO, tP, eP, bt_of, bt_hp }
                    },
                    
                };
                childrensMenu.Children.Add(rateF);
            }
        }

        private void btnHelpSys(object sender, System.EventArgs e)
        {
            GoPage(1, "");
        }

        private async void btnHelpRate(object sender, System.EventArgs e)
        {
            if (security.rate != 2)
            {
                if (await this.DisplayAlert("Подтвердите действие!", "Вы хотите поменять тариф?", "Да", "Нет"))
                {
                    string[] resultBtn = (sender as Button).CommandParameter.ToString().Split('_');
                    security.selRateID = resultBtn[0];
                    string[][] resultComplaint = ServerApi.tableFunc("update Client set rate = "+ security.selRateID+" where id_client = " +security.ID+";");
                    resultComplaint = ServerApi.tableFunc("select * from clientAuthorChecked where id_client = " + security.ID + ";");
                    security.rate = Convert.ToInt32(resultComplaint[1][6]);
                    security.rateName = resultComplaint[1][8];
                    security.rate_price = Convert.ToInt32(resultComplaint[1][10]);
                    security.rateDescription = resultComplaint[1][9];
                    await this.DisplayAlert("Тариф изменен!", "Теперь ваш новый тариф - "+ resultComplaint[1][8], "Ок");
                    upload();
                }
            }
            else
            {
                string[] resultBtn = (sender as Button).CommandParameter.ToString().Split('_');
                security.selRateID = resultBtn[0];
                security.selRateName = resultBtn[1];
                security.selRateDescription = resultBtn[2];
                security.selRatePrice = resultBtn[3];
                GoRate();
            }
        }

        private void btnHelpMen(object sender, System.EventArgs e)
        {
            GoPage(-1, (sender as Button).CommandParameter.ToString());
        }

        private async void GoPage(int i, string s)
        {
            await Navigation.PushAsync(new Chat(i, s));
        }

        private async void GoRate()
        {
            await Navigation.PushAsync(new UpdateRate());
        }
    }
}