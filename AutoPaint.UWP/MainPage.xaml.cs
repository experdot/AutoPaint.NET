using AutoPaint.Core;
using AutoPaint.Utilities;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AutoPaint.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Machine Machine { get; set; }
        public CanvasDrawingSession DrawingSession { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            Machine = new Machine();
        }

        private void CanvasAnimatedControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            if (Machine.Painter.Canvas != null)
            {
                args.DrawingSession.Clear(Windows.UI.Colors.Transparent);
                Machine.Painter.Draw(args.DrawingSession);
            }
            this.DrawingSession = args.DrawingSession;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileOpenPicker.FileTypeFilter.Add(".jpg");
            fileOpenPicker.FileTypeFilter.Add(".bmp");
            fileOpenPicker.FileTypeFilter.Add(".png");
            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;

            var inputFile = await fileOpenPicker.PickSingleFileAsync();

            if (inputFile == null)
            {
                // The user cancelled the picking operation
                return;
            }

            using (IRandomAccessStream stream = await inputFile.OpenAsync(FileAccessMode.Read))
            {
                // Create the decoder from the stream
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                var data = await decoder.GetPixelDataAsync();
                var bytes = data.DetachPixelData();
                var pixel = GetPixel(bytes, 1, 1, decoder.PixelWidth, decoder.PixelHeight);

                var pixelData = new PixelData((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                for (int i = 0; i < pixelData.Width; i++)
                {
                    for (int j = 0; j < pixelData.Height; j++)
                    {
                        pixelData.Colors[i, j] = GetPixel(bytes, i, j, decoder.PixelWidth, decoder.PixelHeight);
                    }
                }

                CanvasControl.Width = pixelData.Width;
                CanvasControl.Height = pixelData.Height;
                Machine.Painter.Canvas = new Painter.Win2DAnimation.LayerCanvas(CanvasControl, new Size(pixelData.Width, pixelData.Height), 10);
                await Task.Run(() =>
                {
                    Machine.Run(pixelData);
                });
            }
        }

        public System.Drawing.Color GetPixel(byte[] pixels, int x, int y, uint width, uint height)
        {
            int k = (y * (int)width + x) * 4;
            var b = pixels[k + 0];
            var g = pixels[k + 1];
            var r = pixels[k + 2];
            var a = pixels[k + 3];
            return System.Drawing.Color.FromArgb(a, r, g, b);
        }
    }
}
