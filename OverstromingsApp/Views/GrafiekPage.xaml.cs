using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using OverstromingsApp.Controllers;
using OverstromingsApp.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
#if WINDOWS
using Microsoft.UI.Xaml.Input;
#endif

namespace OverstromingsApp.Views
{
    public partial class GrafiekPage : ContentPage
    {
        /* ctor zonder parameters – vereist door XAML/Shell */
        public GrafiekPage()
            : this(App.Current!.Handler!.MauiContext!.Services
                       .GetRequiredService<NeerslagController>())
        { }

        private readonly NeerslagController _ctrl;
        private readonly SimpleLineChartDrawable _draw = new();

        private List<YearMonthValue> _yearData = new();   // 1 punt per jaar
        private List<YearMonthValue> _monthData = new();   // elke maand
        private bool _showingMonths = false;
        private double _lastPanX = 0;

        private const float ZoomYears = 1f;
        private const float ZoomMonths = 10f;

        public GrafiekPage(NeerslagController ctrl)
        {
            InitializeComponent();
            _ctrl = ctrl;
            ChartView.Drawable = _draw;

            /* Pinch – enkel wisselen tussen jaar- en maandweergave */
            var pinch = new PinchGestureRecognizer();
            pinch.PinchUpdated += (s, e) =>
            {
                if (e.Status == GestureStatus.Running)
                    Zoom(e.Scale);
            };
            ChartView.GestureRecognizers.Add(pinch);

            /* Pan */
            var pan = new PanGestureRecognizer();
            pan.PanUpdated += OnPan;
            ChartView.GestureRecognizers.Add(pan);

#if WINDOWS
            ChartView.HandlerChanged += (_, _) =>
            {
                if (ChartView.Handler?.PlatformView is Microsoft.UI.Xaml.FrameworkElement fe)
                    fe.PointerWheelChanged += OnWheel;
            };
#endif
        }

        /* ── lifecycle ─────────────────────────────── */
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _yearData = await _ctrl.GetYearTotalsAsync();
            _monthData = await _ctrl.GetAllMonthsAsync();
            ShowYears();
        }

        /* ── dataset helpers ───────────────────────── */
        private void Apply(List<YearMonthValue> list, bool months)
        {
            _draw.Values = list.Select(v => v.Value).ToList();

            if (!months)                            // jaarlabels
            {
                _draw.Labels = list.Select(v => v.Year.ToString()).ToList();
                _draw.SecondLabels = null;
            }
            else                                     // maandlabels + jaar onder januari
            {
                _draw.Labels = list.Select(v =>
                    CultureInfo.CurrentCulture.DateTimeFormat
                             .AbbreviatedMonthNames[v.Month!.Value - 1]).ToList();

                _draw.SecondLabels = list.Select(v =>
                    v.Month == 1 ? v.Year.ToString() : "").ToList();
            }
            ChartView.Invalidate();
        }

        private void ShowYears()
        {
            Apply(_yearData, months: false);
            _showingMonths = false;
            _draw.PanOffset = 0;
            _draw.ZoomFactor = ZoomYears;
            _draw.YearSeparators = new();           // geen hulplijnen in jaar-view
        }

        private void ShowMonths()
        {
            Apply(_monthData, months: true);
            _showingMonths = true;
            _draw.ZoomFactor = ZoomMonths;

            /* indices waar januari start (index > 0) */
            _draw.YearSeparators = _monthData
                                   .Select((v, idx) => (v.Month == 1) ? idx : -1)
                                   .Where(idx => idx > 0)
                                   .ToList();
        }

        /* ── zoom ─────────────────────────────────── */
        private void Zoom(double scale)
        {
            if (!_showingMonths && scale > 1)
                ShowMonths();
            else if (_showingMonths && scale < 1)
                ShowYears();
            // Geen tussenliggende zoomniveaus.
        }

        /* ── pan ──────────────────────────────────── */
        private void OnPan(object s, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Started)
                _lastPanX = e.TotalX;
            else if (e.StatusType == GestureStatus.Running)
            {
                var dx = e.TotalX - _lastPanX;
                _lastPanX = e.TotalX;
                _draw.PanOffset = Math.Max(0, _draw.PanOffset - (float)dx);
                ChartView.Invalidate();
            }
        }

#if WINDOWS
        private void OnWheel(object s, PointerRoutedEventArgs e)
        {
            const float step = 1.15f;
            Zoom(e.GetCurrentPoint(null).Properties.MouseWheelDelta > 0 ? step : 1 / step);
            e.Handled = true;
        }
#endif
    }
}
