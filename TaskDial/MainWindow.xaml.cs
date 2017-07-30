using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Windows.UI.Input;
using TaskDial.RadialController;
using System.Windows.Automation;
using System.Collections.Generic;

namespace TaskDial
{
    enum DialState { NotAcquired, Acquired, Choosing }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Windows.UI.Input.RadialController _controller;
        private List<ProcessItem> _processItems = new List<ProcessItem>();
        private DialState _dialState = DialState.NotAcquired;
        private int _index = 0;

        public MainWindow()
        {
            InitializeComponent();
            StartController();
        }

        private void StartController()
        {
            var interop = (IRadialControllerInterop)System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeMarshal
                .GetActivationFactory(typeof(Windows.UI.Input.RadialController));

            Guid guid = typeof(Windows.UI.Input.RadialController).GetInterface("IRadialController").GUID;

            Window window = GetWindow(this);
            var wih = new WindowInteropHelper(window);
            IntPtr hWnd = wih.Handle;

            _controller = interop.CreateForWindow(hWnd, ref guid);
            _controller.ControlAcquired += Controller_ControlAcquired;
            _controller.ControlLost += Controller_ControlLost;
            _controller.RotationChanged += Controller_RotationChanged;
            _controller.ButtonClicked += Controller_ButtonClicked;
            _controller.Menu.Items.Add(RadialControllerMenuItem.CreateFromKnownIcon(
                "Tasks", RadialControllerMenuKnownIcon.Scroll));
        }

        private void Controller_ButtonClicked(Windows.UI.Input.RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            if (_dialState == DialState.Acquired)
            {
                UpdateProcessItems();
                _dialState = DialState.Choosing;
                DisplayProcessItem();
            }
            else if (_dialState == DialState.Choosing)
            {
                WindowUtil.SwitchToWindow(_processItems[_index].Handle);
                _dialState = DialState.Acquired;
            }
        }

        private void UpdateProcessItems()
        {
            _processItems.Clear();
            var rootElements = AutomationElement.RootElement.FindAll(
                TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
            foreach (AutomationElement el in rootElements)
            {
                if (el.Current.ControlType.ProgrammaticName == "ControlType.Window")
                {
                    _processItems.Add(ProcessItem
                        .Builder()
                        .Name(el.Current.Name)
                        .Handle(el.Current.NativeWindowHandle)
                        .Build());
                }
            }
        }

        private void DisplayProcessItem()
        {
            Console.WriteLine(_processItems[_index].Name);
        }

        private void Controller_RotationChanged(Windows.UI.Input.RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            if (_dialState == DialState.Choosing)
            {
                if (args.RotationDeltaInDegrees > 0)
                {
                    _index++;
                } else
                {
                    _index--;
                }
                if (_index < 0)
                {
                    _index = _processItems.Count - 1;
                }
                if (_index == _processItems.Count)
                {
                    _index = 0;
                }
                DisplayProcessItem();
            }
        }

        private void Controller_ControlLost(Windows.UI.Input.RadialController sender, object args)
        {
            _dialState = DialState.NotAcquired;
        }

        private void Controller_ControlAcquired(Windows.UI.Input.RadialController sender, RadialControllerControlAcquiredEventArgs args)
        {
            _dialState = DialState.Acquired;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //clean up notifyicon (would otherwise stay open until application finishes)
            MyNotifyIcon.Dispose();
            _controller.Menu.Items.Clear();
            base.OnClosing(e);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
