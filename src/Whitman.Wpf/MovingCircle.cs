using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Walterlv.Whitman
{
    public class MovingCircle : FrameworkElement
    {
        public MovingCircle()
        {
            _storyboard = new Storyboard();
            var doubleAnimation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(4)))
            {
                EasingFunction = new ActionEasingFunction(new CubicEase(), UpdateFrame),
            };
            Storyboard.SetTarget(doubleAnimation, this);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(AnimationProgressProperty.Name));
            _storyboard.Children.Add(doubleAnimation);
            _storyboard.Completed += (sender, args) =>
            {
                PrepareFrames();
                _storyboard.Begin(this, true);
            };
        }

        private readonly Storyboard _storyboard;
        private readonly Random _random = new Random((int) DateTimeOffset.UtcNow.Ticks);
        private readonly Brush _outerCircleBrush = new SolidColorBrush(Color.FromRgb(0x28, 0x9d, 0xd9));

        private bool _isLayouted;
        private Size _lastRenderSize;
        private Point _center;
        private double _radius;
        private Point _fromCenter;
        private double _fromRadius;
        private Point _toCenter;
        private double _toRadius;

        private void PrepareFrames()
        {
            var width = _lastRenderSize.Width;
            var height = _lastRenderSize.Height;

            _fromCenter = _toCenter;
            _toCenter = new Point(
                OffsetRandom(width / 2, width / 2 * CenterRange),
                OffsetRandom(height / 2, height / 2 * CenterRange));
            _fromRadius = _toRadius;
            _toRadius = OffsetRandom((width + height) / 2 * RadiusBaseRange, (width + height) / 2 * RadiusRange);
        }

        private void UpdateFrame(double time)
        {
            _center = new Point(
                Intermediate(_fromCenter.X, _toCenter.X, time),
                Intermediate(_fromCenter.Y, _toCenter.Y, time));
            _radius = Intermediate(_fromRadius, _toRadius, time);

            InvalidateVisual();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!_isLayouted)
            {
                _isLayouted = true;
                _toCenter = new Point(finalSize.Width / 2, finalSize.Height / 2);
                _toRadius = (finalSize.Width + finalSize.Height) / 5;
            }

            if (finalSize != _lastRenderSize)
            {
                _lastRenderSize = finalSize;
                PrepareFrames();
                _storyboard.Stop(this);
                _storyboard.Begin(this, true);
            }

            return base.ArrangeOverride(finalSize);
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            var parent = VisualTreeHelper.GetParent(this);
            if (parent == null)
            {
                _storyboard.Stop(this);
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawEllipse(_outerCircleBrush, null, _center, _radius, _radius);
        }

        public bool IsAnimationEnabled
        {
            get => (bool) GetValue(IsAnimationEnabledProperty);
            set => SetValue(IsAnimationEnabledProperty, value);
        }

        public double BrushOpacity
        {
            get => (double) GetValue(BrushOpacityProperty);
            set => SetValue(BrushOpacityProperty, value);
        }

        public double CenterRange
        {
            get => (double) GetValue(CenterRangeProperty);
            set => SetValue(CenterRangeProperty, value);
        }

        public double RadiusBaseRange
        {
            get => (double) GetValue(RadiusBaseRangeProperty);
            set => SetValue(RadiusBaseRangeProperty, value);
        }

        public double RadiusRange
        {
            get => (double) GetValue(RadiusRangeProperty);
            set => SetValue(RadiusRangeProperty, value);
        }

        public static readonly DependencyProperty AnimationProgressProperty = DependencyProperty.Register(
            "AnimationProgress", typeof(double), typeof(MovingCircle), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty IsAnimationEnabledProperty = DependencyProperty.Register(
            "IsAnimationEnabled", typeof(bool), typeof(MovingCircle),
            new PropertyMetadata(true, OnIsAnimationEnabledChanged));

        public static readonly DependencyProperty BrushOpacityProperty = DependencyProperty.Register(
            "BrushOpacity", typeof(double), typeof(MovingCircle),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, OnBrushOpacityChanged));

        public static readonly DependencyProperty CenterRangeProperty = DependencyProperty.Register(
            "CenterRange", typeof(double), typeof(MovingCircle), new PropertyMetadata(1.0 / 15.0));

        public static readonly DependencyProperty RadiusBaseRangeProperty = DependencyProperty.Register(
            "RadiusBaseRange", typeof(double), typeof(MovingCircle), new PropertyMetadata(1.0 / 2.5));

        public static readonly DependencyProperty RadiusRangeProperty = DependencyProperty.Register(
            "RadiusRange", typeof(double), typeof(MovingCircle), new PropertyMetadata(1.0 / 50.0));

        private static void OnIsAnimationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var local = (MovingCircle) d;
            d.Dispatcher.InvokeAsync(() =>
            {
                if (e.NewValue is true)
                {
                    local._storyboard.Resume(local);
                }
                else
                {
                    local._storyboard.Pause(local);
                }
            });
        }

        private static void OnBrushOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var local = (MovingCircle) d;
            local._outerCircleBrush.Opacity = (double) e.NewValue;
        }

        private static double Intermediate(double from, double to, double value)
        {
            return (to - from) * value + from;
        }

        private double OffsetRandom(double @base, double offset)
        {
            return @base - offset + _random.Next((int) offset * 2);
        }

        private class ActionEasingFunction : Freezable, IEasingFunction
        {
            private readonly IEasingFunction _innerEasingFunction;
            private readonly Action<double> _onEase;

            public ActionEasingFunction(IEasingFunction innerEasingFunction, Action<double> onEase)
            {
                _innerEasingFunction = innerEasingFunction;
                _onEase = onEase;
            }

            protected override Freezable CreateInstanceCore()
            {
                if (_innerEasingFunction is Freezable freezable)
                {
                    return new ActionEasingFunction((IEasingFunction) freezable.Clone(), _onEase);
                }

                return new ActionEasingFunction(_innerEasingFunction, _onEase);
            }

            public double Ease(double normalizedTime)
            {
                var time = _innerEasingFunction.Ease(normalizedTime);
                _onEase?.Invoke(time);
                return time;
            }
        }
    }
}
