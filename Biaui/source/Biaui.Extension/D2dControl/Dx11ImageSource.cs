using SharpDX.Direct3D9;
using System;
using System.Windows.Interop;

namespace D2dControl
{
    class Dx11ImageSource : D3DImage, IDisposable
    {
        private Direct3DEx d3DContext;
        private DeviceEx d3DDevice;
        private Texture renderTarget;

        public Dx11ImageSource()
        {
            StartD3D();
        }

        public void Dispose()
        {
            SetRenderTarget(null);

            Disposer.SafeDispose(ref renderTarget);

            EndD3D();
        }

        public void InvalidateD3DImage()
        {
            if (renderTarget != null)
                AddDirtyRect(new System.Windows.Int32Rect(0, 0, PixelWidth, PixelHeight));
        }

        public void SetRenderTarget(SharpDX.Direct3D11.Texture2D target)
        {
            if (renderTarget != null)
            {
                Lock();
                SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                Unlock();

                Disposer.SafeDispose(ref renderTarget);
            }

            if (target == null)
                return;

            var format = TranslateFormat(target);
            var handle = GetSharedHandle(target);

            if (IsShareable(target) == false)
                throw new ArgumentException("Texture must be created with ResourceOptionFlags.Shared");

            if (format == Format.Unknown)
                throw new ArgumentException("Texture format is not compatible with OpenSharedResource");

            if (handle == IntPtr.Zero)
                throw new ArgumentException("Invalid handle");

            renderTarget = new Texture(d3DDevice, target.Description.Width, target.Description.Height, 1,
                Usage.RenderTarget, format, Pool.Default, ref handle);

            using (var surface = renderTarget.GetSurfaceLevel(0))
            {
                Lock();
                SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
                Unlock();
            }
        }

        private void StartD3D()
        {
            var presentParams = GetPresentParameters();
            var createFlags = CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded |
                              CreateFlags.FpuPreserve;

            d3DContext = new Direct3DEx();
            d3DDevice = new DeviceEx(d3DContext, 0, DeviceType.Hardware, IntPtr.Zero, createFlags, presentParams);
        }

        private void EndD3D()
        {
            Disposer.SafeDispose(ref renderTarget);
            Disposer.SafeDispose(ref d3DDevice);
            Disposer.SafeDispose(ref d3DContext);
        }

        private static PresentParameters GetPresentParameters()
        {
            var presentParams = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                DeviceWindowHandle = NativeMethods.GetDesktopWindow(),
                PresentationInterval = PresentInterval.Default
            };

            return presentParams;
        }

        private IntPtr GetSharedHandle(SharpDX.Direct3D11.Texture2D texture)
        {
            using (var resource = texture.QueryInterface<SharpDX.DXGI.Resource>())
            {
                return resource.SharedHandle;
            }
        }

        private static Format TranslateFormat(SharpDX.Direct3D11.Texture2D texture)
        {
            switch (texture.Description.Format)
            {
                case SharpDX.DXGI.Format.R10G10B10A2_UNorm: return Format.A2B10G10R10;
                case SharpDX.DXGI.Format.R16G16B16A16_Float: return Format.A16B16G16R16F;
                case SharpDX.DXGI.Format.B8G8R8A8_UNorm: return Format.A8R8G8B8;
                default: return Format.Unknown;
            }
        }

        private static bool IsShareable(SharpDX.Direct3D11.Texture2D texture)
        {
            return (texture.Description.OptionFlags & SharpDX.Direct3D11.ResourceOptionFlags.Shared) != 0;
        }
    }
}