using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace MicaSetup.Design.Controls;

/// <summary>
/// A <see cref="ScrollViewer"/> with smooth, fluid scrolling animations supporting mouse wheel,
/// touchpad (WM_MOUSEHWHEEL), and touch/manipulation input.
/// Ported from FluentWpfCore.Controls.SmoothScrollViewer with the class renamed to avoid
/// conflicts with the existing <see cref="SmoothScrollViewer"/>.
/// <br/>
/// </summary>
/// <remarks>
/// <para>
/// The physics model can be replaced via the <see cref="Physics"/> property.
/// Default is <see cref="DefaultScrollPhysics"/>; swap to <see cref="ExponentialScrollPhysics"/>
/// for a different feel.
/// </para>
/// <para>
/// <b>Important:</b> <see cref="ScrollViewer.Content"/> must be a <see cref="UIElement"/>
/// because the control applies a <see cref="TranslateTransform"/> to produce the visual lag effect.
/// </para>
/// </remarks>
public class SmoothScrollViewer : ScrollViewer
{
    // Minimum accumulated logical movement before syncing the real ScrollViewer offset.
    // Keeps virtualization panels happy by not spamming ScrollToOffset on every frame.
    private const double LogicalOffsetUpdateDistanceThreshold = 20.0;

    private const int WM_MOUSEHWHEEL = 0x020E;

    // Minimum per-frame visual delta change to bother updating the transform/scrollbar thumb.
    private const double VisualUpdateStepThreshold = 0.1;

    // --- Vertical state ---
    private double _logicalOffsetVertical;

    private double _currentVisualOffsetVertical;
    private double _visualDeltaVertical;
    private double _logicalOffsetUpdateAccumulatorVertical;
    private double _lastRenderedOffsetVertical;
    private double _lastLogicalSyncVertical;

    // --- Horizontal state ---
    private double _logicalOffsetHorizontal;

    private double _currentVisualOffsetHorizontal;
    private double _visualDeltaHorizontal;
    private double _logicalOffsetUpdateAccumulatorHorizontal;
    private double _lastRenderedOffsetHorizontal;
    private double _lastLogicalSyncHorizontal;

    // --- Rendering ---
    private long _lastTimestamp;

    private bool _isRendering;

    // --- Visual / template parts ---
    private TranslateTransform? _transform;

    private UIElement? _content;
    private ScrollBar? _PART_VerticalScrollBar;
    private ScrollBar? _PART_HorizontalScrollBar;
    private HwndSource? _hwndSource;

    // --- Physics ---
    private IScrollPhysics _verticalScrollPhysics = new DefaultScrollPhysics();

    private IScrollPhysics _horizontalScrollPhysics = new DefaultScrollPhysics();

    // Cached bindings used to restore ScrollBar.Value binding after animation ends.
    private static readonly Binding VerticalOffsetBinding = new("VerticalOffset")
    {
        RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
        Mode = BindingMode.OneWay,
    };

    private static readonly Binding HorizontalOffsetBinding = new("HorizontalOffset")
    {
        RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
        Mode = BindingMode.OneWay,
    };

    // ------------------------------------------------------------------
    // Constructor
    // ------------------------------------------------------------------

    public SmoothScrollViewer()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    // ------------------------------------------------------------------
    // Public API
    // ------------------------------------------------------------------

    /// <summary>
    /// Gets or sets the scroll physics model. Assigning a new value clones it into independent
    /// vertical and horizontal instances.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    public IScrollPhysics Physics
    {
        get => _verticalScrollPhysics;
        set
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));
            _verticalScrollPhysics = value.Clone();
            _horizontalScrollPhysics = value.Clone();
        }
    }

    /// <summary>Smoothly scrolls to the given vertical offset.</summary>
    public void AnimatedScrollToVerticalOffset(double offset, bool usePreciseMode = false)
    {
        if (!IsEnableSmoothScrolling) { ScrollToVerticalOffset(offset); return; }
        HandleScroll(VerticalOffset - offset, 0, usePreciseMode);
    }

    /// <summary>Smoothly scrolls to the given horizontal offset.</summary>
    public void AnimatedScrollToHorizontalOffset(double offset, bool usePreciseMode = false)
    {
        if (!IsEnableSmoothScrolling) { ScrollToHorizontalOffset(offset); return; }
        HandleScroll(0, HorizontalOffset - offset, usePreciseMode);
    }

    // ------------------------------------------------------------------
    // Template
    // ------------------------------------------------------------------

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _PART_VerticalScrollBar = GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;
        _PART_HorizontalScrollBar = GetTemplateChild("PART_HorizontalScrollBar") as ScrollBar;
    }

    // ------------------------------------------------------------------
    // Load / Unload
    // ------------------------------------------------------------------

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (Content is not UIElement element)
            throw new InvalidOperationException(
                $"{nameof(SmoothScrollViewer)}.{nameof(Content)} must be a UIElement.");

        _content = element;
        _transform = new TranslateTransform();
        element.RenderTransform = _transform;
        element.RenderTransformOrigin = new Point(0, 0);

        // Remove any stale hook before registering a fresh one.
        _hwndSource?.RemoveHook(WndProc);
        var window = Window.GetWindow(this);
        if (window != null)
        {
            _hwndSource = PresentationSource.FromVisual(window) as HwndSource;
            _hwndSource?.AddHook(WndProc);
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        StopRendering();
        _hwndSource?.RemoveHook(WndProc);
        _hwndSource = null;
    }

    // ------------------------------------------------------------------
    // WndProc — horizontal touchpad scroll (WM_MOUSEHWHEEL)
    // ------------------------------------------------------------------

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != WM_MOUSEHWHEEL) return IntPtr.Zero;

        if (!IsVisible || !IsEnabled || !IsEnableSmoothScrolling || !CanScrollHorizontal)
            return IntPtr.Zero;

        var mousePos = Mouse.GetPosition(this);
        if (mousePos.X < 0 || mousePos.X > ActualWidth ||
            mousePos.Y < 0 || mousePos.Y > ActualHeight)
            return IntPtr.Zero;

        if (InputHitTest(mousePos) is DependencyObject hitElement &&
            FindParentFluentScrollViewer(hitElement) == this)
        {
            int delta = (short)((wParam.ToInt64() >> 16) & 0xFFFF);
            bool isPrecise = delta % Mouse.MouseWheelDeltaForOneLine != 0;
            HandleScroll(0, -delta, isPrecise);
            handled = true;
        }

        return IntPtr.Zero;
    }

    // ------------------------------------------------------------------
    // Core scroll dispatcher
    // ------------------------------------------------------------------

    private void HandleScroll(double deltaVertical, double deltaHorizontal, bool isPreciseMode = false)
    {
        if (deltaVertical == 0 && deltaHorizontal == 0) return;

        if (!_isRendering)
        {
            _logicalOffsetVertical = VerticalOffset;
            _currentVisualOffsetVertical = _logicalOffsetVertical;
            _visualDeltaVertical = 0;

            _logicalOffsetHorizontal = HorizontalOffset;
            _currentVisualOffsetHorizontal = _logicalOffsetHorizontal;
            _visualDeltaHorizontal = 0;
        }

        _verticalScrollPhysics.IsPreciseMode = isPreciseMode;
        _horizontalScrollPhysics.IsPreciseMode = isPreciseMode;

        if (deltaVertical != 0) _verticalScrollPhysics.OnScroll(deltaVertical);
        if (deltaHorizontal != 0) _horizontalScrollPhysics.OnScroll(deltaHorizontal);

        StartRendering();
    }

    // ------------------------------------------------------------------
    // Input overrides
    // ------------------------------------------------------------------

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        if (!IsEnableSmoothScrolling) { base.OnMouseWheel(e); return; }

        e.Handled = true;

        bool shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        var orientation = PreferredScrollOrientation;
        if (AllowTogglePreferredScrollOrientationByShiftKey && shift)
            orientation = orientation == Orientation.Vertical ? Orientation.Horizontal : Orientation.Vertical;

        bool isPrecise = e.Delta % Mouse.MouseWheelDeltaForOneLine != 0;

        if (orientation == Orientation.Vertical && CanScrollVertical)
            HandleScroll(e.Delta, 0, isPrecise);
        else if (orientation == Orientation.Horizontal && CanScrollHorizontal)
            HandleScroll(0, e.Delta, isPrecise);
    }

    protected override void OnManipulationStarting(ManipulationStartingEventArgs e)
    {
        base.OnManipulationStarting(e);
        if (!IsEnableSmoothManipulating) return;

        e.Mode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
        e.ManipulationContainer = this;
        e.Handled = true;
    }

    protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
    {
        if (!IsEnableSmoothManipulating) { base.OnManipulationDelta(e); return; }

        var t = e.DeltaManipulation.Translation;
        double dH = CanScrollHorizontal ? t.X : 0;
        double dV = CanScrollVertical ? t.Y : 0;

        if ((dH == 0 && dV == 0)
            || e.DeltaManipulation.Expansion.Length != 0
            || e.DeltaManipulation.Rotation != 0)
        {
            base.OnManipulationDelta(e);
            return;
        }

        HandleScroll(dV, dH, true);
        e.Handled = true;
    }

    protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
    {
        base.OnManipulationInertiaStarting(e);
        if (!IsEnableSmoothManipulating) return;

        if (e.TranslationBehavior != null)
        {
            double speed = e.InitialVelocities.LinearVelocity.Length;
            double decel = speed / 800.0;
            decel = decel < 0.0012 ? 0.0012 : decel > 0.012 ? 0.012 : decel;
            e.TranslationBehavior.DesiredDeceleration = decel;
        }
        e.Handled = true;
    }

    protected override void OnScrollChanged(ScrollChangedEventArgs e)
    {
        base.OnScrollChanged(e);

        if (e.VerticalChange != 0)
        {
            _logicalOffsetVertical = e.VerticalOffset;
            if (_isRendering)
            {
                _visualDeltaVertical = _currentVisualOffsetVertical - _logicalOffsetVertical;
                _transform!.Y = -_visualDeltaVertical;
            }
            else
            {
                _visualDeltaVertical = 0;
                _transform!.Y = 0;
            }
        }

        if (e.HorizontalChange != 0)
        {
            _logicalOffsetHorizontal = e.HorizontalOffset;
            if (_isRendering)
            {
                _visualDeltaHorizontal = _currentVisualOffsetHorizontal - _logicalOffsetHorizontal;
                _transform!.X = -_visualDeltaHorizontal;
            }
            else
            {
                _visualDeltaHorizontal = 0;
                _transform!.X = 0;
            }
        }
    }

    // ------------------------------------------------------------------
    // Rendering loop
    // ------------------------------------------------------------------

    private void StartRendering()
    {
        if (_isRendering) return;

        _lastTimestamp = Stopwatch.GetTimestamp();
        _logicalOffsetUpdateAccumulatorVertical = 0;
        _logicalOffsetUpdateAccumulatorHorizontal = 0;
        _lastLogicalSyncVertical = _currentVisualOffsetVertical;
        _lastLogicalSyncHorizontal = _currentVisualOffsetHorizontal;

        CompositionTarget.Rendering += OnRendering;
        _isRendering = true;
        _content!.IsHitTestVisible = false;
    }

    private void StopRendering()
    {
        if (!_isRendering) return;

        CompositionTarget.Rendering -= OnRendering;
        _isRendering = false;

        double fV = Clamp(_currentVisualOffsetVertical, 0, ScrollableHeight);
        double fH = Clamp(_currentVisualOffsetHorizontal, 0, ScrollableWidth);

        ScrollToVerticalOffset(fV);
        ScrollToHorizontalOffset(fH);

        // Restore the one-way binding so the scrollbar thumb tracks the logical offset again.
        _PART_VerticalScrollBar?.SetBinding(ScrollBar.ValueProperty, VerticalOffsetBinding);
        _PART_HorizontalScrollBar?.SetBinding(ScrollBar.ValueProperty, HorizontalOffsetBinding);

        _visualDeltaVertical = 0;
        _logicalOffsetVertical = fV;
        _transform!.Y = 0;

        _visualDeltaHorizontal = 0;
        _logicalOffsetHorizontal = fH;
        _transform!.X = 0;

        _content!.IsHitTestVisible = true;
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        long now = Stopwatch.GetTimestamp();
        double dt = (double)(now - _lastTimestamp) / Stopwatch.Frequency;
        _lastTimestamp = now;

        _currentVisualOffsetVertical =
            Clamp(_verticalScrollPhysics.Update(_currentVisualOffsetVertical, dt), 0, ScrollableHeight);
        _currentVisualOffsetHorizontal =
            Clamp(_horizontalScrollPhysics.Update(_currentVisualOffsetHorizontal, dt), 0, ScrollableWidth);

        if (_verticalScrollPhysics.IsStable && _horizontalScrollPhysics.IsStable)
        {
            StopRendering();
            return;
        }

        // Accumulate and batch logical offset updates to avoid thrashing virtualization panels.
        _logicalOffsetUpdateAccumulatorVertical +=
            Math.Abs(_currentVisualOffsetVertical - _lastLogicalSyncVertical);
        _logicalOffsetUpdateAccumulatorHorizontal +=
            Math.Abs(_currentVisualOffsetHorizontal - _lastLogicalSyncHorizontal);

        if (_logicalOffsetUpdateAccumulatorVertical >= LogicalOffsetUpdateDistanceThreshold)
        {
            _logicalOffsetUpdateAccumulatorVertical = 0;
            ScrollToVerticalOffset(_currentVisualOffsetVertical);
            _lastLogicalSyncVertical = _currentVisualOffsetVertical;
        }

        if (_logicalOffsetUpdateAccumulatorHorizontal >= LogicalOffsetUpdateDistanceThreshold)
        {
            _logicalOffsetUpdateAccumulatorHorizontal = 0;
            ScrollToHorizontalOffset(_currentVisualOffsetHorizontal);
            _lastLogicalSyncHorizontal = _currentVisualOffsetHorizontal;
        }

        // Update visual transform and scrollbar thumb position.
        _visualDeltaVertical = _logicalOffsetVertical - _currentVisualOffsetVertical;
        if (Math.Abs(_visualDeltaVertical - _lastRenderedOffsetVertical) >= VisualUpdateStepThreshold)
        {
            _transform!.Y = _lastRenderedOffsetVertical = _visualDeltaVertical;
            _PART_VerticalScrollBar?.Value = _currentVisualOffsetVertical;
        }

        _visualDeltaHorizontal = _logicalOffsetHorizontal - _currentVisualOffsetHorizontal;
        if (Math.Abs(_visualDeltaHorizontal - _lastRenderedOffsetHorizontal) >= VisualUpdateStepThreshold)
        {
            _transform!.X = _lastRenderedOffsetHorizontal = _visualDeltaHorizontal;
            _PART_HorizontalScrollBar?.Value = _currentVisualOffsetHorizontal;
        }
    }

    // ------------------------------------------------------------------
    // Dependency Properties
    // ------------------------------------------------------------------

    /// <summary>Whether smooth scrolling is enabled. Default: <see langword="true"/>.</summary>
    public bool IsEnableSmoothScrolling
    {
        get => (bool)GetValue(IsEnableSmoothScrollingProperty);
        set => SetValue(IsEnableSmoothScrollingProperty, value);
    }

    public static readonly DependencyProperty IsEnableSmoothScrollingProperty =
        DependencyProperty.Register(
            nameof(IsEnableSmoothScrolling), typeof(bool), typeof(SmoothScrollViewer),
            new PropertyMetadata(true, OnIsEnableSmoothScrollingChanged));

    private static void OnIsEnableSmoothScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SmoothScrollViewer sv && e.NewValue is false && sv._isRendering)
            sv.StopRendering();
    }

    /// <summary>Whether touch/manipulation smooth scrolling is enabled. Default: <see langword="false"/>.</summary>
    public bool IsEnableSmoothManipulating
    {
        get => (bool)GetValue(IsEnableSmoothManipulatingProperty);
        set => SetValue(IsEnableSmoothManipulatingProperty, value);
    }

    public static readonly DependencyProperty IsEnableSmoothManipulatingProperty =
        DependencyProperty.Register(
            nameof(IsEnableSmoothManipulating), typeof(bool), typeof(SmoothScrollViewer),
            new PropertyMetadata(false, OnIsEnableSmoothManipulatingChanged));

    private static void OnIsEnableSmoothManipulatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SmoothScrollViewer sv)
            sv.IsManipulationEnabled = e.NewValue is true;
    }

    /// <summary>
    /// Primary scroll direction when the mouse wheel is used.
    /// Holding Shift toggles it when <see cref="AllowTogglePreferredScrollOrientationByShiftKey"/> is <see langword="true"/>.
    /// Default: <see cref="Orientation.Vertical"/>.
    /// </summary>
    public Orientation PreferredScrollOrientation
    {
        get => (Orientation)GetValue(PreferredScrollOrientationProperty);
        set => SetValue(PreferredScrollOrientationProperty, value);
    }

    public static readonly DependencyProperty PreferredScrollOrientationProperty =
        DependencyProperty.Register(
            nameof(PreferredScrollOrientation), typeof(Orientation), typeof(SmoothScrollViewer),
            new FrameworkPropertyMetadata(Orientation.Vertical));

    /// <summary>
    /// When <see langword="true"/> (default), holding Shift while scrolling toggles the preferred orientation.
    /// </summary>
    public bool AllowTogglePreferredScrollOrientationByShiftKey
    {
        get => (bool)GetValue(AllowTogglePreferredScrollOrientationByShiftKeyProperty);
        set => SetValue(AllowTogglePreferredScrollOrientationByShiftKeyProperty, value);
    }

    public static readonly DependencyProperty AllowTogglePreferredScrollOrientationByShiftKeyProperty =
        DependencyProperty.Register(
            nameof(AllowTogglePreferredScrollOrientationByShiftKey), typeof(bool), typeof(SmoothScrollViewer),
            new FrameworkPropertyMetadata(true));

    // ------------------------------------------------------------------
    // Read-only helpers
    // ------------------------------------------------------------------

    /// <summary>Gets a value indicating whether the viewer has scrollable vertical content.</summary>
    public bool CanScrollVertical => ExtentHeight > ViewportHeight;

    /// <summary>Gets a value indicating whether the viewer has scrollable horizontal content.</summary>
    public bool CanScrollHorizontal => ExtentWidth > ViewportWidth;

    /// <summary>Gets a value indicating whether the viewer can scroll upward.</summary>
    public bool CanScrollUp => VerticalOffset > 0.5;

    /// <summary>Gets a value indicating whether the viewer can scroll downward.</summary>
    public bool CanScrollDown => VerticalOffset + ViewportHeight < ExtentHeight - 0.5;

    /// <summary>Gets a value indicating whether the viewer can scroll left.</summary>
    public bool CanScrollLeft => HorizontalOffset > 0.5;

    /// <summary>Gets a value indicating whether the viewer can scroll right.</summary>
    public bool CanScrollRight => HorizontalOffset + ViewportWidth < ExtentWidth - 0.5;

    // ------------------------------------------------------------------
    // Private helpers
    // ------------------------------------------------------------------

    private static double Clamp(double value, double min, double max)
        => value < min ? min : value > max ? max : value;

    /// <summary>
    /// Walks the visual tree upward to find the nearest <see cref="SmoothScrollViewer"/>
    /// that supports horizontal scrolling — used to avoid WM_MOUSEHWHEEL mis-routing.
    /// </summary>
    private static SmoothScrollViewer? FindParentFluentScrollViewer(DependencyObject element)
    {
        DependencyObject? current = element;
        while (current != null)
        {
            if (current is SmoothScrollViewer fsv && fsv.CanScrollHorizontal && fsv.IsEnableSmoothScrolling)
                return fsv;
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }
}

/// <summary>
/// Interface for scroll physics models that control smooth scrolling animation behavior.
/// </summary>
public interface IScrollPhysics
{
    /// <summary>Called when a scroll input is received.</summary>
    /// <param name="delta">The scroll amount.</param>
    public void OnScroll(double delta);

    /// <summary>
    /// Updates the scroll offset based on the current state and elapsed time.
    /// </summary>
    /// <param name="currentOffset">The current scroll offset.</param>
    /// <param name="dt">Delta time since last update in seconds.</param>
    /// <returns>The new scroll offset.</returns>
    public double Update(double currentOffset, double dt);

    /// <summary>Gets a value indicating whether the scrolling animation has stabilized.</summary>
    public bool IsStable { get; }

    /// <summary>Gets or sets a value indicating whether precise mode is enabled (e.g., for touchpad input).</summary>
    public bool IsPreciseMode { get; set; }
}

file static class ScrollPhysicsExtensions
{
    /// <summary>Creates a deep clone of the physics instance by copying all public read/write properties.</summary>
    internal static IScrollPhysics Clone(this IScrollPhysics source)
    {
        Type type = source.GetType();
        if (Activator.CreateInstance(type) is not IScrollPhysics clone)
            throw new InvalidOperationException($"Cannot create instance of physics type {type.FullName}.");

        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!prop.CanRead || !prop.CanWrite) continue;
            if (prop.GetIndexParameters().Length > 0) continue;
            prop.SetValue(clone, prop.GetValue(source, null), null);
        }

        return clone;
    }
}

/// <summary>
/// Default scroll physics using velocity-based friction decay.
/// Provides natural momentum scrolling where the total scroll distance equals the input delta.
/// </summary>
file class DefaultScrollPhysics : IScrollPhysics
{
    // Friction coefficient range: 0.85 (stops quickly) ~ 0.96 (smooth deceleration)
    private const double MinFriction = 0.85d;

    private const double MaxFriction = 0.96d;
    private const double PreciseModeFriction = 0.88d;

    // Reference frame time for frame-rate–independent physics (normalised to 144 Hz)
    private const double ReferenceFrameTime = 1d / 144d;

    private double _friction;
    private double _smoothness = 0.78d;
    private double _velocity;
    private bool _isStable = true;
    private bool _isPreciseMode;

    public DefaultScrollPhysics()
    {
        _friction = MinFriction + (MaxFriction - MinFriction) * _smoothness;
    }

    /// <summary>
    /// Scroll smoothness in [0, 1]. 0 = stops quickly; 1 = very smooth long deceleration.
    /// </summary>
    [Category("Scroll Physics")]
    [Description("The larger the value, the smoother and longer the rolling; The smaller the value, the faster it stops. Take the value 0~1")]
    public double Smoothness
    {
        get => _smoothness;
        set
        {
            _smoothness = value < 0.0 ? 0.0 : value > 1.0 ? 1.0 : value;
            _friction = MinFriction + (MaxFriction - MinFriction) * _smoothness;
        }
    }

    /// <summary>
    /// Stop threshold in DIPs. Scrolling stops when remaining velocity drops below this.
    /// </summary>
    [Category("Scroll Physics")]
    [Description("Stop threshold, stop scrolling when the remaining distance is less than this value. Take the value of 0.1~5")]
    public double StopThreshold { get; set; } = 0.5;

    /// <inheritdoc/>
    public bool IsStable => _isStable;

    /// <inheritdoc/>
    public bool IsPreciseMode
    {
        get => _isPreciseMode;
        set => _isPreciseMode = value;
    }

    /// <inheritdoc/>
    public void OnScroll(double delta)
    {
        _isStable = false;
        // Accumulate velocity; total displacement = velocity when k = (1 - f).
        _velocity -= delta;
    }

    /// <inheritdoc/>
    public double Update(double currentOffset, double dt)
    {
        if (_isStable) return currentOffset;

        if (Math.Abs(_velocity) < StopThreshold)
        {
            _velocity = 0;
            _isStable = true;
            return currentOffset;
        }

        // Frame-rate–independent: scale friction exponent by (dt / referenceFrameTime).
        double timeFactor = dt / ReferenceFrameTime;
        double f = Math.Pow(_isPreciseMode ? PreciseModeFriction : _friction, timeFactor);
        double displacement = _velocity * (1.0 - f);
        _velocity -= displacement;
        return currentOffset + displacement;
    }
}

/// <summary>
/// Exponential decay scroll physics: offset = target + (start - target) * e^(-k*t).
/// Characteristics: fast start, slow end, natural feel.
/// </summary>
file class ExponentialScrollPhysics : IScrollPhysics
{
    private double _decayRate = 8.0;
    private double _remainingDistance;
    private bool _isStable = true;

    /// <summary>
    /// Decay rate in [1, 20]. Higher values reach the target faster.
    /// </summary>
    [Category("Scroll Physics")]
    [Description("The higher the decay rate, the faster the roll reaches the target position. Take the value of 1~20")]
    public double DecayRate
    {
        get => _decayRate;
        set => _decayRate = value < 1.0 ? 1.0 : value > 20.0 ? 20.0 : value;
    }

    /// <summary>
    /// Stop threshold in DIPs. Scrolling stops when the remaining distance drops below this.
    /// </summary>
    [Category("Scroll Physics")]
    [Description("Stop threshold, stop scrolling when the remaining distance is less than this value. Take the value of 0.1~5")]
    public double StopThreshold { get; set; } = 0.5;

    /// <inheritdoc/>
    public bool IsStable => _isStable;

    /// <inheritdoc/>
    public bool IsPreciseMode { get; set; }

    /// <inheritdoc/>
    public void OnScroll(double delta)
    {
        _isStable = false;
        _remainingDistance -= delta;
    }

    /// <inheritdoc/>
    public double Update(double currentOffset, double dt)
    {
        if (_isStable) return currentOffset;

        if (Math.Abs(_remainingDistance) < StopThreshold)
        {
            double last = _remainingDistance;
            _remainingDistance = 0;
            _isStable = true;
            return currentOffset + last;
        }

        // factor = 1 - e^(-k*dt): fraction of remaining distance consumed this frame.
        double factor = 1.0 - Math.Exp(-_decayRate * dt);
        double displacement = _remainingDistance * factor;
        _remainingDistance -= displacement;
        return currentOffset + displacement;
    }
}
