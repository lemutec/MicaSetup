using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MicaSetup.Controls;

public class ButtonHelper
{
    static ButtonHelper()
    {
        EventManager.RegisterClassHandler(typeof(Button), Button.MouseEnterEvent, new RoutedEventHandler(OnButtonMouseEnter));
        EventManager.RegisterClassHandler(typeof(Button), Button.MouseLeaveEvent, new RoutedEventHandler(OnButtonMouseLeave));
    }

    public static CornerRadius GetCornerRadius(DependencyObject obj) => (CornerRadius)obj.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(DependencyObject obj, CornerRadius value) => obj.SetValue(CornerRadiusProperty, value);
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ButtonHelper));

    public static Brush GetHoverBrush(DependencyObject obj)
    {
        return (Brush)obj.GetValue(HoverBrushProperty);
    }

    public static void SetHoverBrush(DependencyObject obj, Brush value)
    {
        obj.SetValue(HoverBrushProperty, value);
    }

    public static readonly DependencyProperty HoverBrushProperty =
        DependencyProperty.RegisterAttached("HoverBrush", typeof(Brush), typeof(ButtonHelper));

    internal static void OnButtonMouseEnter(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var hoverBrush = GetHoverBrush(button!);

        if (hoverBrush == null)
            return;

        var dic = new Dictionary<DependencyProperty, Brush>
        {
            [Button.BackgroundProperty] = hoverBrush,
        };
        StoryboardUtils.BeginBrushStoryboard(button!, dic);
    }

    internal static void OnButtonMouseLeave(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var hoverBrush = GetHoverBrush(button!);

        if (hoverBrush == null)
            return;

        var list = new List<DependencyProperty>
        {
            Button.BackgroundProperty,
            Button.ForegroundProperty,
        };
        StoryboardUtils.BeginBrushStoryboard(button!, list);
    }
}

file static class StoryboardUtils
{
    public static void BeginBrushStoryboard(DependencyObject dependencyObj, IDictionary<DependencyProperty, Brush> toDictionary, double durationSeconds = 0.2d)
    {
        var storyboard = new Storyboard();
        foreach (var keyValue in toDictionary)
        {
            BrushAnimation anima = new()
            {
                To = keyValue.Value,
                Duration = TimeSpan.FromSeconds(durationSeconds),
            };
            Storyboard.SetTarget(anima, dependencyObj);
            Storyboard.SetTargetProperty(anima, new PropertyPath(keyValue.Key));
            storyboard.Children.Add(anima);
        }
        storyboard.Begin();
    }

    public static void BeginBrushStoryboard(DependencyObject dependencyObj, IList<DependencyProperty> dpList)
    {
        var storyboard = new Storyboard();
        foreach (var dp in dpList)
        {
            BrushAnimation anima = new()
            {
                Duration = TimeSpan.FromSeconds(0.2),
            };
            Storyboard.SetTarget(anima, dependencyObj);
            Storyboard.SetTargetProperty(anima, new PropertyPath(dp));
            storyboard.Children.Add(anima);
        }
        storyboard.Begin();
    }
}

file sealed class BrushAnimation : AnimationTimeline
{
    private VisualBrush? _visualBrush;

    public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Brush), typeof(BrushAnimation));

    public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Brush), typeof(BrushAnimation));

    public Brush From
    {
        get
        {
            return (Brush)GetValue(FromProperty);
        }
        set
        {
            SetValue(FromProperty, value);
        }
    }

    public Brush To
    {
        get
        {
            return (Brush)GetValue(ToProperty);
        }
        set
        {
            SetValue(ToProperty, value);
        }
    }

    public override Type TargetPropertyType => typeof(Brush);

    public override object GetCurrentValue(object from, object to, AnimationClock clock)
    {
        return GetCurrentValue((from as Brush)!, (to as Brush)!, clock);
    }

    protected override Freezable CreateInstanceCore()
    {
        return new BrushAnimation();
    }

    public object GetCurrentValue(Brush from, Brush to, AnimationClock clock)
    {
        if (!clock.CurrentProgress.HasValue)
        {
            return null!;
        }

        from = From ?? from;
        to = To ?? to;
        if (clock.CurrentProgress.Value == 0d)
        {
            return from;
        }

        if (clock.CurrentProgress.Value == 1d)
        {
            return to;
        }

        bool flag = false;
        double num = 1d;
        double num2 = 1d;
        if (to != null && from != null)
        {
            num = (to is SolidColorBrush solidColorBrush) ? (solidColorBrush.Color.A / 255d * to.Opacity) : to.Opacity;
            num2 = (from is SolidColorBrush solidColorBrush2) ? (solidColorBrush2.Color.A / 255d * from.Opacity) : from.Opacity;
            if (num < 1d && num < num2)
            {
                flag = true;
            }
        }
        else if (to == null && from != null)
        {
            flag = true;
        }

        double num3 = flag ? (1d - clock.CurrentProgress.Value) : clock.CurrentProgress.Value;
        if (num2 < 1d && num < 1d)
        {
            if (_visualBrush == null)
            {
                _visualBrush = CreateGridVisualBrush(flag ? to! : from!, flag ? from! : to!, 1d - num3, num3);
            }
            else
            {
                if (_visualBrush.Visual is not Grid grid || grid.Children.Count <= 1)
                {
                    return null!;
                }

                if (grid.Children[0] is Rectangle rectangle)
                {
                    rectangle.Opacity = 1d - num3;
                }

                if (grid.Children[1] is Rectangle rectangle2)
                {
                    rectangle2.Opacity = num3;
                }
            }
        }
        else if (_visualBrush == null)
        {
            _visualBrush = CreateBorderVisualBrush(flag ? to! : from!, flag ? from! : to!, num3);
        }
        else
        {
            Rectangle rectangle3 = ((_visualBrush.Visual as Border)?.Child as Rectangle)!;
            if (rectangle3 == null)
            {
                return null!;
            }

            rectangle3.Opacity = num3;
        }

        return _visualBrush;
    }

    private VisualBrush CreateBorderVisualBrush(Brush background, Brush foreground, double opacity)
    {
        return new VisualBrush(new Border
        {
            Width = 1d,
            Height = 1d,
            Background = background,
            Child = new Rectangle
            {
                Fill = foreground,
                Opacity = opacity
            }
        });
    }

    private VisualBrush CreateGridVisualBrush(Brush background, Brush foreground, double opacity1, double opacity2)
    {
        return new VisualBrush(new Grid
        {
            Width = 1d,
            Height = 1d,
            Children =
            {
                new Rectangle
                {
                    Fill = background,
                    Opacity = opacity1
                },
                new Rectangle
                {
                    Fill = foreground,
                    Opacity = opacity2
                }
            }
        });
    }
}
