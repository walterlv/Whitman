using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Walterlv.Whitman
{
    public class CircleSlider : FrameworkElement
    {
        public CircleSlider()
        {
            MouseMove += OnMouseMove;
            MouseLeave += OnMouseLeave;
            //_storyboard = new Storyboard();
            //var doubleAnimation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(4)))
            //{
            //    EasingFunction = new ActionEasingFunction(new CubicEase(), UpdateFrame),
            //};
            //Storyboard.SetTarget(doubleAnimation, this);
            //Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(AnimationProgressProperty.Name));
            //_storyboard.Children.Add(doubleAnimation);
            //_storyboard.Completed += (sender, args) =>
            //{
            //    PrepareFrames();
            //    _storyboard.Begin(this, true);
            //};
        }

        private readonly Storyboard _storyboard;
        private Point? _indicatorPosition;

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var size = Math.Min(RenderSize.Width, RenderSize.Height);
            var radius = (size - Thickness) / 2;
            var center = new Point(RenderSize.Width / 2, RenderSize.Height / 2);

            var vector = e.GetPosition(this) - center;
            var angle = Vector.AngleBetween(new Vector(1, 0), vector);

            var matrix = Matrix.Identity;
            matrix.Rotate(angle);
            _indicatorPosition = center + matrix.Transform(new Vector(radius, 0));

            InvalidateVisual();
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _indicatorPosition = null;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            var size = Math.Min(RenderSize.Width, RenderSize.Height);
            var radius = (size - Thickness) / 2;
            var center = new Point(RenderSize.Width / 2, RenderSize.Height / 2);

            if (_indicatorPosition != null)
            {
                dc.DrawLine(new Pen(Brushes.White, 2), center, _indicatorPosition.Value);
                dc.DrawEllipse(Brushes.White, null, center, 4, 4);
            }

            dc.DrawEllipse(Brushes.Transparent, new Pen(Brushes.White, Thickness), center, radius, radius);
        }

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness", typeof(double), typeof(CircleSlider),
            new FrameworkPropertyMetadata(16.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Thickness
        {
            get => (double) GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(int), typeof(CircleSlider), new PropertyMetadata(0));

        public int MaxValue
        {
            get => (int) GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(int), typeof(CircleSlider), new PropertyMetadata(default(int)));

        public int Value
        {
            get => (int) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
