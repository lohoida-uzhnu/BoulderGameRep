using System;
using Avalonia.Media;

namespace BoulderGame.Rendering
{
    public class SpriteAnimator
    {
        private readonly SpriteSheet spriteSheet;
        private readonly int ticksPerFrame;

        private int currentFrame;
        private int tickCounter;

        public SpriteAnimator(SpriteSheet spriteSheet, int framesPerSecond, int gameTicksPerSecond = 60)
        {
            this.spriteSheet = spriteSheet;
            ticksPerFrame = Math.Max(1, gameTicksPerSecond / framesPerSecond);
        }

        public IImage CurrentFrame => spriteSheet.GetFrame(currentFrame);

        public void Reset()
        {
            currentFrame = 0;
            tickCounter = 0;
        }

        public void Update(bool isMoving)
        {
            if (!isMoving)
            {
                currentFrame = 0;
                tickCounter = 0;
                return;
            }

            tickCounter++;

            if (tickCounter < ticksPerFrame)
            {
                return;
            }

            tickCounter = 0;
            currentFrame = (currentFrame + 1) % spriteSheet.FrameCount;
        }
    }
}
