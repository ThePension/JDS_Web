using Timer = System.Timers.Timer;

namespace JDSCommon.Services
{
    public static class JSTimer
    {
        public static void ClearInterval(Timer timer)
        {
            timer.Stop();
            timer.Dispose();
        }

        public static Timer SetInterval(Action action, TimeSpan timeout)
        {
            Timer timer = new Timer(timeout.TotalMilliseconds);

            timer.Elapsed += (s, e) =>
            {
                timer.Enabled = false;
                action();
                timer.Enabled = true;
            };

            timer.Enabled = true;
            return timer;
        }

        public static async Task SetTimeout(Action action, TimeSpan timeout)
        {
            await Task.Delay(timeout).ConfigureAwait(false);

            action();
        }
    }
}
