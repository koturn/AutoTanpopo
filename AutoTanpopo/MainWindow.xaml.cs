using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using AutoTanpopo.Inputs;


namespace AutoTanpopo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Interoperation helper for handling "WndProc" and getting window handle of this window class.
        /// </summary>
        private readonly WindowInteropHelper _interopHelper;
        /// <summary>
        /// ID of <see cref="HotKeyManager.Register(IntPtr, ModifierKeys, System.Windows.Forms.Keys)"/>.
        /// </summary>
        private int _hotKeyId = -1;
        /// <summary>
        /// Click burst task.
        /// </summary>
        private Task? _task;
        /// <summary>
        /// <see cref="CancellationTokenSource"/> for <see cref="_task"/>.
        /// </summary>
        private CancellationTokenSource? _cts;


        /// <summary>
        /// Initialize <see cref="_interopHelper"/>, setup WndProc and components.
        /// </summary>
        public MainWindow()
        {
            _interopHelper = new WindowInteropHelper(this);
            var hWnd = _interopHelper.EnsureHandle();
            HwndSource.FromHwnd(hWnd).AddHook(new HwndSourceHook(WndProc));

            InitializeComponent();
        }

        /// <summary>
        /// Windows message reciever to start/stop <see cref="_task"/>.
        /// </summary>
        /// <param name="m">Windows message.</param>
        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int)WindowMessages.HotKey)
            {
                if ((int)wParam == _hotKeyId)
                {
                    ToggleTaskAsync();
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Start/Stop <see cref="_task"/>.
        /// </summary>
        private async void ToggleTaskAsync()
        {
            if (_task != null)
            {
                _labelStatus.Content = "Stopping...";
                _labelStatus.Foreground = System.Windows.Media.Brushes.Black;
                Title = "AutoTanpopo: Stopping...";
                if (_cts != null)
                {
                    _cts.Cancel();
                }
                await _task;
                _labelStatus.Content = "Inactive";
                _labelStatus.Foreground = System.Windows.Media.Brushes.Black;
                Title = "AutoTanpopo: Inactive";
                _task = null;
            }
            else
            {
                var isClientBasedResize = _rbClientBased.IsChecked.GetValueOrDefault();
                var windowWidth = (int)_nudWindowWidth.Value;
                var windowHeight = (int)_nudWindowHeight.Value;
                var clientWidth = (int)_nudClientWidth.Value;
                var clientHeight = (int)_nudClientHeight.Value;
                var offsetX = (int)_nudOffsetX.Value;
                var offsetY = (int)_nudOffsetY.Value;
                var framerate = (int)_nudFramerate.Value;
                var process = _cbResize.IsChecked.GetValueOrDefault() ? (Process)_cbVRChatProcess.SelectedItem : null;
                _task = Task.Factory.StartNew(
                    () =>
                    {
                        const int SashimiInterval = 1500;

                        var cts = _cts;
                        if (cts == null)
                        {
                            return;
                        }

                        var prevWinRect = default(WindowRect);
                        if (process != null)
                        {
                            var hWnd = process.MainWindowHandle;
                            prevWinRect = WindowUtil.GetWindowRect(hWnd);
                            if (isClientBasedResize)
                            {
                                WindowUtil.SetClientSize(hWnd, clientWidth, clientHeight);
                            }
                            else
                            {
                                WindowUtil.SetWindowSize(hWnd, windowWidth, windowHeight);
                            }
                            WindowUtil.NativeMethods.SetForegroundWindow(hWnd);
                            Thread.Sleep(16 * 8);
                        }

                        var mouseInputs = new[]
                        {
                            new []
                            {
                                Input.CreateMouseInput(MouseEventFlags.LeftDown),
                                Input.CreateMouseInput(MouseEventFlags.Move, 0, offsetY)
                            },
                            new []
                            {
                                Input.CreateMouseInput(MouseEventFlags.Move, offsetX, 0)
                            },
                            new []
                            {
                                Input.CreateMouseInput(MouseEventFlags.Move, 0, -offsetY)
                            },
                            new []
                            {
                                Input.CreateMouseInput(MouseEventFlags.LeftUp),
                                Input.CreateMouseInput(MouseEventFlags.Move, -offsetX)
                            }
                        };

                        var frameInterval = (int)Math.Round(1000.0 / framerate);
                        var grabInterval = Math.Min(100, frameInterval * 2);

                        var sleepTimes = new[]
                        {
                            grabInterval,
                            grabInterval,
                            SashimiInterval - grabInterval * 4,
                            grabInterval * 2
                        };

                        while (!cts.IsCancellationRequested)
                        {
                            for (int i = 0; i < mouseInputs.Length; i++)
                            {
                                InputUtil.SendInput(mouseInputs[i]);
                                Thread.Sleep(sleepTimes[i]);
                            }
                        }

                        if (process != null)
                        {
                            process.Refresh();
                            if (!process.HasExited && process.MainWindowHandle != IntPtr.Zero)
                            {
                                WindowUtil.SetWindowSize(process.MainWindowHandle, prevWinRect.Width, prevWinRect.Height);
                            }
                        }
                    },
                    (_cts = new CancellationTokenSource()).Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
                _labelStatus.Content = "Active";
                _labelStatus.Foreground = System.Windows.Media.Brushes.Red;
                Title = "AutoTanpopo: Active";
                Topmost = true;
                Topmost = false;
            }
        }

        /// <summary>
        /// Refresh VRChat process ID combobox, <see cref="_cbVRChatProcess"/>.
        /// </summary>
        /// <param name="sender">The source of the event (this).</param>
        /// <param name="e"><see cref="RoutedEventArgs"/> that contains the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshProcessComboBox();
        }

        /// <summary>
        /// <para>This method is called just before the window is closed.</para>
        /// <para>Unregister all registered hot keys.</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"><see cref="CancelEventArgs"/> that contains the event data.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            HotKeyManager.UnregisterAll();
        }

        /// <summary>
        /// <para>This method is called when selection of <see cref="_cbHotKey"/> is changed.</para>
        /// <para>Unregister all registered hot keys.</para>
        /// </summary>
        /// <param name="sender">The source of the event (<see cref="_cbHotKey"/>).</param>
        /// <param name="e"><see cref="SelectionChangedEventArgs"/> that contains the event data.</param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedText = (string)((ComboBoxItem)((ComboBox)sender).SelectedItem).Content;
            var hotKey = Enum.Parse<System.Windows.Forms.Keys>(selectedText);
            var hWnd = _interopHelper.EnsureHandle();

            HotKeyManager.Unregister(hWnd, _hotKeyId);
            var id = HotKeyManager.Register(hWnd, hotKey);
            if (id == -1)
            {
                MessageBox.Show(
                    this,
                    "Failed to register hot key " + selectedText,
                    nameof(MessageBoxImage.Warning),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            _hotKeyId = id;
        }

        /// <summary>
        /// Refresh VRChat process ID combobox, <see cref="_cbVRChatProcess"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"><see cref="RoutedEventArgs"/> that contains the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RefreshProcessComboBox();
        }

        /// <summary>
        /// Refresh items of <see cref="_cbVRChatProcess"/>.
        /// </summary>
        private void RefreshProcessComboBox()
        {
            var items = _cbVRChatProcess.Items;
            items.Clear();
            foreach (var p in Process.GetProcessesByName("VRChat"))
            {
                items.Add(p);
            }
            if (items.Count > 0)
            {
                _cbVRChatProcess.SelectedIndex = 0;
            }
        }
    }
}
