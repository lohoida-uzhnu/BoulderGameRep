using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace BoulderGame.Rendering
{
    public class SpriteSheet
    {
        private readonly Bitmap bitmap;

        public int FrameWidth { get; }
        public int FrameHeight { get; }
        public int FrameCount { get; }

        public SpriteSheet(string assetUri, int frameWidth, int frameHeight, int frameCount)
        {
            bitmap = new Bitmap(AssetLoader.Open(new Uri(assetUri)));
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            FrameCount = frameCount;
        }

        public IImage GetFrame(int frameIndex)
        {
            int safeFrameIndex = Math.Clamp(frameIndex, 0, FrameCount - 1);
            var sourceRect = new PixelRect(
                safeFrameIndex * FrameWidth,
                0,
                FrameWidth,
                FrameHeight);

            return new CroppedBitmap(bitmap, sourceRect);
        }
    }
}
