using System.Threading;

namespace Thorium.Threading
{
    public abstract class RestartableThreadClass
    {
        protected object runThreadLock = new object();
        protected Thread runThread = null;
        bool isBackground;

        public RestartableThreadClass(bool isBackground)
        {
            this.isBackground = isBackground;
        }

        public virtual void Start()
        {
            lock(runThreadLock)
            {
                if(runThread == null)
                {
                    runThread = new Thread(Run) { IsBackground = isBackground };
                }
                if((runThread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted)
                {
                    runThread.Start();
                }
            }
        }

        public virtual void Stop(int joinTimeoutms = -1)
        {
            Thread stopThread;
            lock(runThreadLock)
            {
                stopThread = runThread;
                runThread = null;
            }

            if(stopThread != null)
            {
                stopThread.Interrupt();
                if(Thread.CurrentThread != stopThread)
                {
                    if(!stopThread.Join(joinTimeoutms))
                    {
                        stopThread.Abort(); //abort if it doesnt exit in time
                    }
                }
                stopThread = null;
            }
        }

        public virtual void Join(int timeoutms = -1)
        {
            if(runThread != null)
            {
                runThread.Join(timeoutms);
            }
        }

        protected abstract void Run();
    }
}
