using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using SharpDX;
using Microsoft.Win32;

namespace D2dControl
{
    public abstract class D2dControl : System.Windows.Controls.Image
    {
        protected readonly ResourceCache ResourceCache = new ResourceCache();

        private static SharpDX.Direct3D11.Device device;

        private Texture2D sharedTarget;
        private Texture2D dx11Target;
        private Dx11ImageSource d3DSurface;
        private SharpDX.Direct2D1.DeviceContext d2DRenderTarget;

        private bool IsInDesignMode
        {
            get
            {
                if (_IsInDesignMode.HasValue == false)
                    _IsInDesignMode = DesignerProperties.GetIsInDesignMode(this);

                return _IsInDesignMode.Value;
            }
        }

        private bool? _IsInDesignMode;

        private bool _isRequestUpdate = true;

        #region IsAutoFrameUpdate

        public bool IsAutoFrameUpdate
        {
            get => _IsAutoFrameUpdate;
            set
            {
                if (value != _IsAutoFrameUpdate)
                    SetValue(IsAutoFrameUpdateProperty, value);
            }
        }

        private bool _IsAutoFrameUpdate = true;

        public static readonly DependencyProperty IsAutoFrameUpdateProperty =
            DependencyProperty.Register(
                nameof(IsAutoFrameUpdate),
                typeof(bool),
                typeof(D2dControl),
                new PropertyMetadata(
                    true,
                    (s, e) =>
                    {
                        var self = (D2dControl) s;
                        self._IsAutoFrameUpdate = (bool) e.NewValue;
                    }));

        #endregion

        public static void Initialize()
        {
            device = new SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);

            Dx11ImageSource.Initialize();
        }

        public static void Destroy()
        {
            Dx11ImageSource.Destroy();

            Disposer.SafeDispose(ref device);
        }

        public void Invalidate()
        {
            if (IsAutoFrameUpdate)
                return;

            _isRequestUpdate = true;
        }

        protected D2dControl()
        {
            Loaded += OnLoaded;

            Stretch = System.Windows.Media.Stretch.Fill;
        }

        public abstract void Render(SharpDX.Direct2D1.DeviceContext target);



        private bool _isInitialized;
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
                return;

            if (device == null)
                throw new NullReferenceException("Not yet initialized. You need to call D2dControl.Initialize().");

            if (_isInitialized)
                return;

            _isInitialized = true;

            StartD3D();
            StartRendering();

            // event
            {
                SystemEvents.SessionSwitch += SystemEventsOnSessionSwitch;
                Unloaded += OnUnloaded;

                _parentWindow = GetParent<Window>();
                if (_parentWindow != null)
                    _parentWindow.Closed += OnUnloaded;

                _parentPopup = GetParent<Popup>();
                if (_parentPopup != null)
                    _parentPopup.Closed += OnUnloaded;
            }
        }

        private Window _parentWindow;
        private Popup _parentPopup;

        private void OnUnloaded(object sender, EventArgs e)
        {
            if (IsInDesignMode)
                return;

            if (device == null)
                throw new NullReferenceException("Not yet initialized. You need to call D2dControl.Initialize().");

            if (_isInitialized == false)
                return;

            _isInitialized = false;

            Shutdown();

            // event
            {
                SystemEvents.SessionSwitch -= SystemEventsOnSessionSwitch;
                Unloaded -= OnUnloaded;

                if (_parentWindow != null)
                    _parentWindow.Closed -= OnUnloaded;

                if (_parentPopup != null)
                    _parentPopup.Closed -= OnUnloaded;
            }
        }
    
        private void SystemEventsOnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            // アンロック以降、描画されない。
            // デバイスロスト時の振る舞いに似ているが、デバイスロストとしては検知されないため明示的に再描画している
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                var timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
                timer.Interval = TimeSpan.FromMilliseconds(1000);
                timer.Tick += (_, __) =>
                {
                    CreateAndBindTargets();
                    Invalidate();

                    timer.Stop();
                    timer = null;
                };

                timer.Start();
            }
        }

        protected void Shutdown()
        {
            StopRendering();
            EndD3D();
        }

        private void InvalidateInternal()
        {
            if (device == null)
                throw new NullReferenceException("Not yet initialized. You need to call D2dControl.Initialize().");

            try
            {
                PrepareAndCallRender();

                d3DSurface.Lock();

                device.ImmediateContext.ResolveSubresource(dx11Target, 0, sharedTarget, 0, Format.B8G8R8A8_UNorm);
                d3DSurface.InvalidateD3DImage();

                d3DSurface.Unlock();

                device.ImmediateContext.Flush();
            }
            catch (SharpDXException ex)
            {
                if (ex.ResultCode == SharpDX.DXGI.ResultCode.DeviceRemoved ||
                    ex.ResultCode == SharpDX.DXGI.ResultCode.DeviceReset)
                {
                    CreateAndBindTargets();
                    Invalidate();
                }
            }
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (IsAutoFrameUpdate == false &&
                _isRequestUpdate == false)
                return;

            _isRequestUpdate = false;

            InvalidateInternal();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            CreateAndBindTargets();

            Invalidate();

            base.OnRenderSizeChanged(sizeInfo);
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (d3DSurface.IsFrontBufferAvailable)
                StartRendering();
            else
                StopRendering();
        }

        private void StartD3D()
        {
            d3DSurface = new Dx11ImageSource();
            d3DSurface.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            CreateAndBindTargets();

            Source = d3DSurface;
        }

        private void EndD3D()
        {
            d3DSurface.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
            Source = null;

            Disposer.SafeDispose(ref d2DRenderTarget);
            Disposer.SafeDispose(ref d3DSurface);
            Disposer.SafeDispose(ref sharedTarget);
            Disposer.SafeDispose(ref dx11Target);
        }

        private void CreateAndBindTargets()
        {
            if (d3DSurface == null)
                return;

            d3DSurface.SetRenderTarget(null);

            Disposer.SafeDispose(ref d2DRenderTarget);
            Disposer.SafeDispose(ref sharedTarget);
            Disposer.SafeDispose(ref dx11Target);

            var width = Math.Max((int) ActualWidth, 100);
            var height = Math.Max((int) ActualHeight, 100);

            var frontDesc = new Texture2DDescription
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.Shared,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            };

            var backDesc = new Texture2DDescription
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            };

            sharedTarget = new Texture2D(device, frontDesc);
            dx11Target = new Texture2D(device, backDesc);

            using (var surface = dx11Target.QueryInterface<Surface>())
            {
                d2DRenderTarget = new SharpDX.Direct2D1.DeviceContext(surface, new CreationProperties()
                {
                    Options = DeviceContextOptions.EnableMultithreadedOptimizations,
                    ThreadingMode = ThreadingMode.SingleThreaded
                });
            }

            ResourceCache.RenderTarget = d2DRenderTarget;

            d3DSurface.SetRenderTarget(sharedTarget);

            device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height);
        }

        private void StartRendering()
        {
            System.Windows.Media.CompositionTarget.Rendering += OnRendering;
        }

        private void StopRendering()
        {
            System.Windows.Media.CompositionTarget.Rendering -= OnRendering;
        }

        private void PrepareAndCallRender()
        {
            d2DRenderTarget.BeginDraw();

            Render(d2DRenderTarget);

            d2DRenderTarget.EndDraw();
        }

        // codebase : Biaui
        private T GetParent<T>() where T : class
        {
            var parent = this as DependencyObject;

            do
            {
                if (parent is T tp)
                    return tp;

                parent = VisualTreeHelper.GetParent(parent);
            } while (parent != null);

            return null;
        }
    }
}