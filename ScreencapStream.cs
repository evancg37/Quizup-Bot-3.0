using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using AForge.Imaging;

namespace QuizupBot_3._0
{
    class ScreencapStream
    {
        private const int DEFAULT_TICK = 100;
        private Bitmap currentSnapshot;
        private bool running = true;
        private int tickTime;
        public Rectangle sourceArea;
        public Size outputSize = new Size(1080, 1920);

        public Bitmap getCurrentSnapshot()
        {
            return new Bitmap(currentSnapshot);
        }

        public void stopSnapshotStream()
        {
            running = false;
        }

        public void beginSnapshotStream(){
            new Thread(run).Start();
        }

        private void run()
        {
            while (running)
            {
                updateSnapshot();
                Thread.Sleep(tickTime);
            }
        }

        private void updateSnapshot()
        {
            Bitmap bm = new Bitmap(sourceArea.Size.Width, sourceArea.Size.Height);

            using (Graphics g = Graphics.FromImage(bm))
            {
                g.CopyFromScreen(sourceArea.Location, new Point(0, 0), sourceArea.Size);
            }
            new AForge.Imaging.Filters.ResizeNearestNeighbor(1080, 1920).Apply(bm);

            currentSnapshot = bm;
        }

        public ScreencapStream(Rectangle source, Size size, int tick = DEFAULT_TICK)
        {
            sourceArea = source;
            outputSize = size;
            tickTime = tick;
        }

        public ScreencapStream(Rectangle source, int tick = DEFAULT_TICK)
        {
            sourceArea = source;
            outputSize = new Size(1080, 1920);
            tickTime = tick;
        }

    }
}
