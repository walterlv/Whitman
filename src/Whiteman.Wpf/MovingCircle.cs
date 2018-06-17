using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Walterlv.Whiteman
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

        private bool _isLayouted;
        private Size _lastRenderSize;
        private readonly Storyboard _storyboard;
        private readonly Random _random = new Random((int) DateTimeOffset.UtcNow.Ticks);
        private readonly Brush _outerCircleBrush = new SolidColorBrush(Colors.White) { Opacity = 0.33 };
        private readonly Brush _middleCircleBrush = new SolidColorBrush(Colors.White) { Opacity = 0.5 };
        private readonly Brush _innerCircleBrush = new SolidColorBrush(Colors.White) { Opacity = 0.67 };
        private readonly CircleAnimationData _outerData = new CircleAnimationData();
        private readonly CircleAnimationData _middleData = new CircleAnimationData();
        private readonly CircleAnimationData _innerData = new CircleAnimationData();

        private void PrepareFrames()
        {
            var width = _lastRenderSize.Width;
            var height = _lastRenderSize.Height;

            _outerData.FromCenter = _outerData.ToCenter;
            _outerData.ToCenter = new Point(
                OffsetRandom(width / 2, width / 30),
                OffsetRandom(height / 2, height / 30));
            _outerData.FromRadius = _outerData.ToRadius;
            _outerData.ToRadius = OffsetRandom((width + height) / 5, (width + height) / 100);

            _middleData.FromCenter = _middleData.ToCenter;
            _middleData.ToCenter = new Point(
                OffsetRandom(width / 2, width / 40),
                OffsetRandom(height / 2, height / 40));
            _middleData.FromRadius = _middleData.ToRadius;
            _middleData.ToRadius = OffsetRandom((width + height) / 6, (width + height) / 100);

            _innerData.FromCenter = _innerData.ToCenter;
            _innerData.ToCenter = new Point(
                OffsetRandom(width / 2, width / 40),
                OffsetRandom(height / 2, height / 40));
            _innerData.FromRadius = _innerData.ToRadius;
            _innerData.ToRadius = OffsetRandom((width + height) / 9, (width + height) / 100);
        }

        private void UpdateFrame(double time)
        {
            _outerData.Center = new Point(
                Intermediate(_outerData.FromCenter.X, _outerData.ToCenter.X, time),
                Intermediate(_outerData.FromCenter.Y, _outerData.ToCenter.Y, time));
            _outerData.Radius = Intermediate(_outerData.FromRadius, _outerData.ToRadius, time);

            _middleData.Center = new Point(
                Intermediate(_middleData.FromCenter.X, _middleData.ToCenter.X, time),
                Intermediate(_middleData.FromCenter.Y, _middleData.ToCenter.Y, time));
            _middleData.Radius = Intermediate(_middleData.FromRadius, _middleData.ToRadius, time);

            _innerData.Center = new Point(
                Intermediate(_innerData.FromCenter.X, _innerData.ToCenter.X, time),
                Intermediate(_innerData.FromCenter.Y, _innerData.ToCenter.Y, time));
            _innerData.Radius = Intermediate(_innerData.FromRadius, _innerData.ToRadius, time);

            InvalidateVisual();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!_isLayouted)
            {
                _isLayouted = true;
                _outerData.ToCenter = new Point(finalSize.Width / 2, finalSize.Height / 2);
                _middleData.ToCenter = new Point(finalSize.Width / 2, finalSize.Height / 2);
                _innerData.ToCenter = new Point(finalSize.Width / 2, finalSize.Height / 2);
                _outerData.ToRadius = (finalSize.Width + finalSize.Height) / 5;
                _middleData.ToRadius = (finalSize.Width + finalSize.Height) / 6;
                _innerData.ToRadius = (finalSize.Width + finalSize.Height) / 9;
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

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawEllipse(_outerCircleBrush, null, _outerData.Center, _outerData.Radius, _outerData.Radius);
            dc.DrawEllipse(_middleCircleBrush, null, _middleData.Center, _middleData.Radius, _middleData.Radius);
            dc.DrawEllipse(_innerCircleBrush, null, _innerData.Center, _innerData.Radius, _innerData.Radius);
        }

        private static double Intermediate(double from, double to, double value)
        {
            return (to - from) * value + from;
        }

        private double OffsetRandom(double @base, double offset)
        {
            return @base - offset + _random.Next((int) offset * 2);
        }

        public static readonly DependencyProperty AnimationProgressProperty = DependencyProperty.Register(
            "AnimationProgress", typeof(double), typeof(MovingCircle), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty IsAnimationEnabledProperty = DependencyProperty.Register(
            "IsAnimationEnabled", typeof(bool), typeof(MovingCircle), new PropertyMetadata(false, OnIsAnimationEnabledChanged));

        public bool IsAnimationEnabled
        {
            get => (bool) GetValue(IsAnimationEnabledProperty);
            set => SetValue(IsAnimationEnabledProperty, value);
        }

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

        private class CircleAnimationData
        {
            internal Point Center;
            internal double Radius;
            internal Point FromCenter;
            internal double FromRadius;
            internal Point ToCenter;
            internal double ToRadius;
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
