using MarcusW.VncClient;
using MarcusW.VncClient.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows;
using Size = MarcusW.VncClient.Size;
using System.Windows.Media.Imaging;
using System.IO;

namespace VncWpfSample
{
    public class WpfRenderTarget : IRenderTarget, IDisposable
    {
        private readonly System.Windows.Controls.Image _image;
        private WriteableBitmap? _bitmap;
        private volatile bool _disposed;
        private readonly object _bitmapReplacementLock = new object();

        public WpfRenderTarget(System.Windows.Controls.Image image)
        {
            _image = image;
        }

       

        // Este método se llama cada vez que la librería necesita un "buffer" para renderizar la imagen.

        public IFramebufferReference GrabFramebufferReference(Size size, IImmutableSet<Screen> layout)
        {
            
                // Si el bitmap es nulo o sus dimensiones no coinciden, lo creamos de nuevo
                    if (_bitmap == null || _bitmap.PixelWidth != size.Width || _bitmap.PixelHeight != size.Height)
                    {
                        _bitmap = new WriteableBitmap(size.Width, size.Height, 96, 96, PixelFormats.Bgra32, null);
                
             
                    }

            // Actualizamos la UI para mostrar el nuevo bitmap
            //GuardarWriteableBitmap(_bitmap, @"C:\logs\InteractiveSession\imagen.png");
            _image.Dispatcher.Invoke(() => {
                _image.Source = _bitmap;


            });
            _bitmap.Lock();
                return new WpfFramebufferReference(_bitmap);
         
        }

        public void GuardarWriteableBitmap(WriteableBitmap bitmap, string rutaArchivo)
        {
            try
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
            catch (Exception ex)
            {
                // Maneja errores (ej: registro, mensaje al usuario)
                throw new InvalidOperationException("Error al guardar el bitmap", ex);
            }
        }


        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                lock (_bitmapReplacementLock)
                {
                    
                    _bitmap = null;
                }
            }

            _disposed = true;
        }
    }
}
