using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Walterlv.Whitman.Shell
{
    public class DwmAttribute : Freezable
    {
        public static readonly DependencyProperty DwmAttributeProperty = DependencyProperty.RegisterAttached(
            "DwmAttribute", typeof(DwmAttribute), typeof(DwmAttribute),
            new PropertyMetadata(null, OnDwmAttributeChanged));

        public static DwmAttribute GetDwmAttribute(DependencyObject element)
        {
            return (DwmAttribute) element.GetValue(DwmAttributeProperty);
        }

        public static void SetDwmAttribute(DependencyObject element, DwmAttribute value)
        {
            element.SetValue(DwmAttributeProperty, value);
        }

        private static void OnDwmAttributeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            var window = (Window) d;
            var oldAttribute = (DwmAttribute) e.OldValue;
            var newAttribute = (DwmAttribute) e.NewValue;

            oldAttribute?.Detach();
            newAttribute?.Attach(window);

            //var attributeWorker = DwmAttributeWorker.GetDwmAttributeWorker(window);
            //if (attributeWorker == null)
            //{
            //    attributeWorker = new DwmAttributeWorker();
            //    DwmAttributeWorker.SetDwmAttributeWorker(window, attributeWorker);
            //}

            //attributeWorker.SetDwmAttribute(newAttribute);
        }

        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
            "BackgroundColor", typeof(Color?), typeof(DwmAttribute),
            new PropertyMetadata(null));

        public Color? BackgroundColor
        {
            get => (Color?) GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        private void Attach(Window window)
        {
            Append(window);
        }

        private void Detach()
        {
        }

        private void Append(Window window)
        {
            var helper = new WindowInteropHelper(window);
            if (IntPtr.Zero.Equals(helper.Handle))
            {
                window.SourceInitialized += OnSourceInitialized;
            }
            else
            {
                ApplyAttributes(window);
            }
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            var window = (Window)sender;
            window.SourceInitialized -= OnSourceInitialized;
            ApplyAttributes(window);
        }

        private void ApplyAttributes(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            int attrValue = (int) DWMNCRENDERINGPOLICY.DWMNCRP_ENABLED;
            //DwmSetWindowAttribute(hwnd, DWMWINDOWATTRIBUTE., ref attrValue, sizeof(DWMNCRENDERINGPOLICY));
        }

        protected override Freezable CreateInstanceCore()
        {
            return new DwmAttribute();
        }
        
        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

        enum DWMWINDOWATTRIBUTE : uint
        {
            NCRenderingEnabled = 1,
            NCRenderingPolicy,
            TransitionsForceDisabled,
            AllowNCPaint,
            CaptionButtonBounds,
            NonClientRtlLayout,
            ForceIconicRepresentation,
            Flip3DPolicy,
            ExtendedFrameBounds,
            HasIconicBitmap,
            DisallowPeek,
            ExcludedFromPeek,
            Cloak,
            Cloaked,
            FreezeRepresentation
        }
        enum DWMNCRENDERINGPOLICY
        {
            DWMNCRP_USEWINDOWSTYLE,
            DWMNCRP_DISABLED,
            DWMNCRP_ENABLED,
            DWMNCRP_LAST
        };
    }
}
