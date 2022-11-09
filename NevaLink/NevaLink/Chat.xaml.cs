using System;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace NevaLink
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Chat : ContentPage
    {

        private bool _isChat = false;
        private bool active = false;
        private int _isPost = 0;
        private int _isPost2 = 0;

        private string Complaint;
        private string idHelper;
        private string rateName;

        public Chat(int wtfHelp, string Rate)
        {
            InitializeComponent();

            if (wtfHelp == 1)
            {
                titleHelp.Text = "Тех. поддержка";
                _isPost = -1;
                _isPost2 = -2;
            }
            else
            {
                titleHelp.Text = "Помощь по тарифу";
                _isPost = -3;
                rateName = Rate;
            }

            frame1.BorderColor = Color.FromRgb(0, 113, 188);
            frame2.BorderColor = Color.FromRgb(0, 113, 188);
            frame3.BorderColor = Color.FromRgb(0, 113, 188);
            BtnChat.Background = new SolidColorBrush(Color.FromRgb(0, 113, 188));
            BtnChat.TextColor = Color.FromRgb(255, 255, 255);
            _isChat = false;
            string[][] resultComplaint = ServerApi.tableFunc("select * from Complaint where Client=" + security.ID + " and Active=1 and Level = "+-_isPost+";");
            if (resultComplaint[0][0] == "No")
            {
                middleHelp.Text = "Обращений нет";
                //textChat.Text = "";
                
            }
            else
            {
                //открытие второго потока
                ThreadStart writeSecon = new ThreadStart(chatUpdate);
                Thread thread = new Thread(writeSecon);
                thread.Start();
            }
        }

        //Функция работующая во втором потоке
        private void chatUpdate()
        {
            string[][] resultComplaint = ServerApi.tableFunc("select * from Complaint where Client=" + security.ID + " and Active=1 and Level = " + -_isPost + ";");
            Complaint = resultComplaint[1][1];
            while (true)
            {
                if (_isChat)
                {
                    resultComplaint = ServerApi.tableFunc("select * from chatUpdateCheckedSotr where Who = " + idHelper + " and ToWhom= -" + security.ID + " and complaint=" + Complaint + " union select * from chatUpdateCheckedClient where Who= -" + security.ID + " and ToWhom = " + idHelper + " and complaint=" + Complaint + " ORDER BY WhenTime;");
                    Console.WriteLine(resultComplaint);
                    string result = "";
                    bool isActive = false;
                    StackLayout st = new StackLayout();
                    for (int i = 1; i < resultComplaint.Length; i++)
                    {
                        
                        StackLayout st2 = new StackLayout();

                        Editor ed = new Editor();
                        ed.AutoSize = EditorAutoSizeOption.TextChanges;
                        ed.IsReadOnly = true;
                        st2.WidthRequest = 150;
                        ed.WidthRequest = 150;
                        ed.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                        Label lb = new Label();
                        lb.FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label));
                        lb.WidthRequest = 150;

                        string hours = resultComplaint[i][4];
                        string minute = resultComplaint[i][5];
                        if (hours.Length == 1)
                            hours = "0" + hours;
                        if (minute.Length == 1)
                            minute = "0" + minute;

                        if (resultComplaint[i][6] == idHelper)
                        {
                            if (resultComplaint[i][3] != "IMAGESSEND")
                            {
                                ed.Text = resultComplaint[i][3];
                                lb.Text = resultComplaint[i][1] + " " + resultComplaint[i][2] + " " + hours + ":" + minute;

                                st2.Children.Add(ed);
                                st2.Children.Add(lb);
                                st2.HorizontalOptions = LayoutOptions.Start;
                            }
                            else
                            {
                                Image im = new Image();
                                im.WidthRequest = 150;
                                string ImageS = resultComplaint[i][10];
                                byte[] imageByte = ImageS.Split(';').Select(a => byte.Parse(a)).ToArray();
                                im.Source = ImageSource.FromStream(() =>
                                {
                                    return new MemoryStream(imageByte);
                                });
                                lb.Text = resultComplaint[i][1] + " " + resultComplaint[i][2] + " " + hours + ":" + minute;
                                st2.Children.Add(im);
                                st2.Children.Add(lb);
                                st2.HorizontalOptions = LayoutOptions.Start;
                            }
                        }
                        else
                        {
                            if (resultComplaint[i][3] != "IMAGESSEND")
                            {
                                ed.Text = resultComplaint[i][3];
                                lb.Text = "вы " + hours + ":" + minute;

                                st2.Children.Add(ed);
                                st2.Children.Add(lb);
                                st2.HorizontalOptions = LayoutOptions.End;
                            }
                            else
                            {
                                Image im = new Image();
                                im.WidthRequest = 150;
                                string ImageS = resultComplaint[i][10];
                                byte[] imageByte = ImageS.Split(';').Select(a => byte.Parse(a)).ToArray();
                                im.Source = ImageSource.FromStream(() =>
                                {
                                    return new MemoryStream(imageByte);
                                });
                                lb.Text = "вы " + hours + ":" + minute;
                                st2.Children.Add(im);
                                st2.Children.Add(lb);
                                st2.HorizontalOptions = LayoutOptions.End;
                            }
                        }
                        st.Children.Add(st2);
                        if (resultComplaint[i][3] == "Ваше обращение переведено на старшего сотрудника!")
                        {
                            _isChat = false;
                        }
                        if (resultComplaint[i][3] == "Ваше обращение было закрыто!")
                        {
                            _isChat = false;
                            active = true;
                            isActive = true;
                        }
                    }
                    if (isActive)
                        //обращение к элементу формы находящимся в другом потоке
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            BtnChat.IsEnabled = false;
                        });
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        chats.Children.Clear();
                        chats.Children.Add(st);
                    });
                }
                else
                {
                    resultComplaint = ServerApi.tableFunc("select * from Complaint where id_complaint = "+ Complaint + ";"); 
                    if (resultComplaint[1][3] != _isPost.ToString() && resultComplaint[1][3] != _isPost2.ToString())
                    {
                        resultComplaint = ServerApi.tableFunc("select id_sotr, Name from Sotr where id_sotr = " + resultComplaint[1][3] + ";");
                        idHelper = resultComplaint[1][1];

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            middleHelp.Text = "специалист " + resultComplaint[1][2];
                        });

                        
                        _isChat = true;
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            middleHelp.Text = "Ожидание ответа";
                        });
                    }
                }
                if (active)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        middleHelp.Text = "Обращений нет";
                    });
                    break;
                }
                    
                Thread.Sleep(3000);
            }
            
        }


        private void btnChat(object sender, System.EventArgs e)
        {
            if (textToChat.Text != "" && textToChat.Text != " ")
            {
                if (_isChat)
                {
                    string[][] resultComplaint = ServerApi.tableFunc("insert into Chat values (" + -security.ID + ", " + idHelper + ", GETDATE(), '" + textToChat.Text + "', "+ Complaint + ", null);");
                    textToChat.Text = "";
                }
                else
                {
                    if (_isPost == -3)
                    {
                        string[][] resultComplaint = ServerApi.tableFunc("insert into Complaint values (" + security.ID + ", " + _isPost + ", 1, GETDATE(), null, " + -_isPost + ", '" + textToChat.Text +" %%%% "+ rateName + "');");

                    }
                    else
                    {
                        string[][] resultComplaint = ServerApi.tableFunc("insert into Complaint values (" + security.ID + ", " + _isPost + ", 1, GETDATE(), null, " + -_isPost + ", '" + textToChat.Text + "');");

                    }
                    //открытие второго потока
                    ThreadStart writeSecon = new ThreadStart(chatUpdate);
                    Thread thread = new Thread(writeSecon);
                    textToChat.Text = "";
                    thread.Start();
                }
            }
            else
            {
                Anim();
            }
        }

        private async void Anim()
        {
            await textToChat.TranslateTo(-10, 0, 200);
            await textToChat.TranslateTo(0, 0, 200);
            await textToChat.TranslateTo(+10, 0, 200);
            await textToChat.TranslateTo(0, 0, 200);
            await textToChat.TranslateTo(-10, 0, 200);
            await textToChat.TranslateTo(0, 0, 200);
            await textToChat.TranslateTo(+10, 0, 200);
            await textToChat.TranslateTo(0, 0, 200);
        }
    }
}