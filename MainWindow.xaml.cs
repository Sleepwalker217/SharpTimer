using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;

namespace SharpTimer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int cntMinutes, cntSeconds = 0;
        public double msg_y, msg_x = 0;
        public bool work = true;

        public MainWindow()
        {
            InitializeComponent();
            Topmost = false;
            Min.Focus();
            Height = SystemParameters.PrimaryScreenHeight;
            Width = SystemParameters.PrimaryScreenWidth;
               
            //Инициализация иконки в трее
            NotifyIcon ni = new NotifyIcon();
            ni.Icon = new Icon("icon1.ico");
            ni.Visible = true;
            ni.MouseClick += Ni_MouseClick;

        }

        //Обработка нажатия на иконку в трее
        private void Ni_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Close();
        }

        // Нажатие кнопки Start
        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            prepareTimer();
        }

        // Нажатие кнопки закрытия
        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Нажатие кнопки сворачивания таймера
        private void minimalizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        //Нажатие кнопки остановки таймера
        private void Tstop_Click(object sender, RoutedEventArgs e)
        {
            work = false;

            Topmost = false;
            Top = msg_y;
            Left = msg_x;
            Timer.Visibility = Visibility.Hidden;
            Msg.Visibility = Visibility.Visible;
            Min.Focus();

        }

        //Нажатие кнопки сворачивания таймера
        private void Tmin_Click(object sender, RoutedEventArgs e)
        {
            Timer.Visibility = Visibility.Hidden;
            Tmax.Visibility = Visibility.Visible;
        }

        // Нажатие кнопки разворачивания таймера
        private void Tmax_Click(object sender, RoutedEventArgs e)
        {

            Timer.Visibility = Visibility.Visible;
            Tmax.Visibility = Visibility.Hidden;
        }

        //Обработка ввода текста
        private void Min_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Обновления текста только если нажата цифра
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        //ОБработка нажатия иных клавиш во время ввода минут
        private void Min_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter) prepareTimer(); // Enter - запуск таймера
            if (e.Key == Key.OemMinus) WindowState = WindowState.Minimized; //"-" - сворачивание
            if (e.Key == Key.Escape) Close(); // ESC - закрытие
        }

        // асинхронное выполнения таймера
        public async void startTimer() {
            work = true;
            while(cntMinutes >= 0 && work)
            {
                if (cntMinutes > 9)
                    Minutes.Text = cntMinutes.ToString();
                else
                    Minutes.Text = "0" + cntMinutes.ToString();
                cntSeconds = 60;
                while(cntSeconds > 0 && work)
                {
                    if (cntSeconds > 10)
                        Seconds.Text = (--cntSeconds).ToString();
                    else
                        Seconds.Text = "0" + (--cntSeconds).ToString();
                    await Task.Delay(1000);
                    WindowState = WindowState.Normal;
                }
                cntMinutes--;
            }

            Topmost = false;
            Top = msg_y;
            Left = msg_x;
            Msg.Visibility = Visibility.Visible;
            Min.Focus();
            Timer.Visibility = Visibility.Hidden;
        }

        // Перетаскивание окна
        private void Msg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        //Подготовка таймера и запуск потока отсчета
        public void prepareTimer()
        {
            msg_y = Top;
            msg_x = Left;

            Topmost = true;
            Msg.Visibility = Visibility.Hidden;
            Timer.Visibility = Visibility.Visible;
            Top = 0;
            Left = 0;
            int tmp;
            try { tmp = Convert.ToInt32(Min.Text); }
            catch(FormatException e) { tmp = 0; }

            if (tmp <= 0)
                cntMinutes = 0;
            else
                cntMinutes = --tmp;

            startTimer();
        }

        //------------------------------ДЛЯ СКРЫТИЯ ALT-TAB--------------------------------------------

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr window, int index, int value);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr window, int index);

        private const int GWL_EXSTYLE = -20;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HideFromAltTab(Handle);
        }

        private const int WS_EX_TOOLWINDOW = 0x00000080;

        public static void HideFromAltTab(IntPtr Handle)
        {
            SetWindowLong(Handle, GWL_EXSTYLE, GetWindowLong(Handle,
                GWL_EXSTYLE) | WS_EX_TOOLWINDOW);
        }
        private IntPtr Handle
        {
            get
            {
                return new WindowInteropHelper(this).Handle;
            }
        }


        //------------------------------------------------------------------------------

    }
}
