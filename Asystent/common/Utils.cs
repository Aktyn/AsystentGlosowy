using System;
using System.Threading;

namespace Asystent {
    public static class Utils {
        static public void setTimeout(Action TheAction, int Timeout = 0) {
            Thread t = new Thread(
                () => {
                    Thread.Sleep(Timeout);
                    TheAction.Invoke();
                }
            );
            t.Start();
        }
    }
}