using MarcusW.VncClient;
using MarcusW.VncClient.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Size = MarcusW.VncClient.Size;

namespace VncWpfSample
{
    public class WpfFramebufferReference : IFramebufferReference
    {
        private WriteableBitmap _bitmap;
        private bool _disposed;

        public WpfFramebufferReference(WriteableBitmap bitmap)
        {
            _bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
            _bitmap.Lock();
        }

        /// <summary>
        /// Devuelve la dirección del primer píxel.
        /// </summary>
        public IntPtr Address => _bitmap.BackBuffer;

        /// <summary>
        /// Devuelve el tamaño del framebuffer en píxeles del dispositivo.
        /// </summary>
        public Size Size => new Size(_bitmap.PixelWidth, _bitmap.PixelHeight);

        /// <summary>
        /// Devuelve el formato de cómo se representan los píxeles en memoria.
        /// </summary>
        public MarcusW.VncClient.PixelFormat Format
        {
            get
            {
                // Ejemplo para Bgra32
                if (_bitmap.Format == System.Windows.Media.PixelFormats.Bgra32)
                {
                    return new MarcusW.VncClient.PixelFormat(
                        name: "Bgra32",
                        bitsPerPixel: 32,
                        depth: 24,
                        bigEndian: false,
                        trueColor: true,
                        hasAlpha: true,
                        redMax: 255,
                        greenMax: 255,
                        blueMax: 255,
                        alphaMax: 255,
                        redShift: 16,
                        greenShift: 8,
                        blueShift: 0,
                        alphaShift: 24
                    );
                }
                throw new NotSupportedException("El formato de píxel no está soportado.");
            }
        }
         
        

        /// <summary>
        /// Devuelve la resolución horizontal (DPI) de la pantalla subyacente.
        /// </summary>
        public double HorizontalDpi => _bitmap.DpiX;

        /// <summary>
        /// Devuelve la resolución vertical (DPI) de la pantalla subyacente.
        /// </summary>
        public double VerticalDpi => _bitmap.DpiY;

        /// <summary>
        /// Al finalizar la actualización, marca el área completa como modificada y desbloquea el bitmap.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                // Se marca toda la región del bitmap como "dirty" para actualizar la imagen en pantalla.
                _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
                _bitmap.Unlock();
                _disposed = true;
            }
        }

    }
}
