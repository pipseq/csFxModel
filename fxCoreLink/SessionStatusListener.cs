using System;
using System.Threading;
using Common;
using fxcore2;

namespace fxCoreLink
{
    public class SessionStatusListener : IO2GSessionStatus, IDisposable
    {
        // Connection , session and status variables
        Logger log = Logger.LogManager("SessionStatusListener");

        public ManualResetEvent manualEvent;
        private Display display;
		public SessionStatusListener(Display display)
		{
			this.display = display;
            mTradingClosed = false;
            manualEvent = new ManualResetEvent(false);
        }
        public bool Connected
        {
            get { return mConnected; }
        }
        private bool mConnected = false;

        public bool Disconnected
        {
            get { return mDisconnected; }
        }
        private bool mDisconnected = false;

        public bool Error
        {
            get { return mError; }
        }
        private bool mError = false;

        public bool TradingClosed
        {
            get { return mTradingClosed; }
        }
        private bool mTradingClosed = false;

        private string mDBName = string.Empty;
        private string mPin = string.Empty;

        private O2GSession mSession = null;

        public O2GSessionStatusCode Status
        {
            get { return mStatus; }
        }
        private O2GSessionStatusCode mStatus;

        // Constructor
        public SessionStatusListener(O2GSession session, String dbName, String pin)
        {
            mSession = session;
            mDBName = dbName;
            mPin = pin;
        }

        // Implementation of IO2GSessionStatus interface public method onSessionStatusChanged
        public void onSessionStatusChanged(O2GSessionStatusCode status)
        {
            mStatus = status;
            switch (status)
            {
                case O2GSessionStatusCode.Connecting:
                    break;

                case O2GSessionStatusCode.Connected:
                    mConnected = (mStatus == O2GSessionStatusCode.Connected);
                    manualEvent.Set();
                    CtrlTimer.getInstance().stopTimer("Session");
                    break;

                case O2GSessionStatusCode.Disconnecting:
                    break;

                case O2GSessionStatusCode.Disconnected:
                    mDisconnected = (mStatus == O2GSessionStatusCode.Disconnected);
                    manualEvent.Set();
                    CtrlTimer.getInstance().stopTimer("Session");
                    break;

                case O2GSessionStatusCode.Reconnecting:
                    break;

                case O2GSessionStatusCode.SessionLost:
                    manualEvent.Set();
                    CtrlTimer.getInstance().stopTimer("Session");
                    break;
            }
            //log.info("Status: " + mStatus.ToString());

        }
        // 05/29/15 19:48:16.959:SessionStatusListener:DEBUG:Login error: Trading is closed for the weekend.  We are currently performing our regular weekly maintenance. You will be able to log in to your account by the time trading opens on Sunday.  Normal trading hours are 5:15 p.m. Eastern Time (New York) on Sunday to 5:00 p.m. Eastern Time (New York) on Friday.
        // Implementation of IO2GSessionStatus interface public method onLoginFailed
        public void onLoginFailed(String error)
        {
            mError = true;
            CtrlTimer.getInstance().stopTimer("login");
            manualEvent.Set();
            log.debug("Login error: " + error);
            if (error.Contains("Trading is closed"))
            {
                mTradingClosed = true;
            }
        }

        public void Dispose()
        {
            manualEvent.Dispose();
        }
    }
}