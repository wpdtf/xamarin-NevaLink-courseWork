using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NevaLink
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UpdateRate : ContentPage
	{
		public UpdateRate ()
		{
			InitializeComponent ();
            line1.Stroke = new SolidColorBrush(Color.FromRgb(0, 113, 188));
            titleRate.Text = "Вы выбрали тариф - " + security.selRateName;
			titleRateDiscription.Text = "Тариф включает в себя:\n" + security.selRateDescription;
			titleRatePrica.Text = "Стоимость тарифа (в месяц) - " + security.selRatePrice + "руб.";
            updatePickCity();
            BtnRate.Background = new SolidColorBrush(Color.FromRgb(0, 113, 188));
            BtnRate.TextColor = Color.FromRgb(255, 255, 255);

        }

		private string city;
        private string street;
        private string home;

        private void updatePickCity()
		{
			picCity.Items.Clear();

            string[][] resultComplaint = ServerApi.tableFunc("select distinct city from HomeLink where id_home<>2;");
			for (int i =1; i < resultComplaint.Length; i++)
			{
				picCity.Items.Add(resultComplaint[i][1]);

			}
        }

        private void selCity(object sender, System.EventArgs e)
        {
            city = picCity.Items[picCity.SelectedIndex];
            updatePickStreet();
            picStreet.IsEnabled = true;
        }

        private void selStreet(object sender, System.EventArgs e)
        {
            street = picStreet.Items[picCity.SelectedIndex];
            updatePickHome();
            picHome.IsEnabled = true;
        }

        private void selHome(object sender, System.EventArgs e)
        {
            home = picHome.Items[picCity.SelectedIndex];
            BtnRate.IsEnabled= true;
        }

        private void updatePickStreet()
        {
            picStreet.Items.Clear();

            string[][] resultComplaint = ServerApi.tableFunc("select distinct street from HomeLink where id_home<>2 and city like '"+city+"';");
            for (int i = 1; i < resultComplaint.Length; i++)
            {
                picStreet.Items.Add(resultComplaint[i][1]);

            }
        }

        private void updatePickHome()
        {
            picHome.Items.Clear();

            string[][] resultComplaint = ServerApi.tableFunc("select distinct home from HomeLink where id_home<>2 and city like '" + city + "' and street like '"+street+"';");
            for (int i = 1; i < resultComplaint.Length; i++)
            {
                picHome.Items.Add(resultComplaint[i][1]);

            }
        }

        private async void updateTar(object sender, System.EventArgs e)
        {
            if (telephone.Text != "" && telephone.Text != " ")
            {
                string[][] resultComplaint = ServerApi.tableFunc("insert into Complaint values (" + security.ID + ", -2, 1, GETDATE(), null, 2, 'Первичное подключение, тариф - "+ security.selRateName + "\nНомер телефона для связи - "+telephone.Text+"\nАдрес для подключения - "+city+", "+street+", "+home+"');");
                await this.DisplayAlert("Заявка отправлена!", "Наши тех. специалист свяжется с вами для уточнения времени подключения", "Ок");
                Back();
            }
            else
            {
                Anim();
            }
        }

        private async void Anim()
        {
            await telephone.TranslateTo(-10, 0, 200);
            await telephone.TranslateTo(0, 0, 200);
            await telephone.TranslateTo(+10, 0, 200);
            await telephone.TranslateTo(0, 0, 200);
            await telephone.TranslateTo(-10, 0, 200);
            await telephone.TranslateTo(0, 0, 200);
            await telephone.TranslateTo(+10, 0, 200);
            await telephone.TranslateTo(0, 0, 200);
        }

        private async void Back()
        {
            await Navigation.PopAsync();
        }
    }
}