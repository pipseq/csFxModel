using System.Collections.Generic;

namespace fxCoreLink
{
    #region interface definition
    public interface Control
    {
        bool ordersArmed();
        bool isExcludedPair(string pair);
        bool isProcessingAllowed();
        bool isErrorState();
        bool isValidPair(string pair);
        bool isJournalRead();
        bool isJournalWrite();
        void journalReadDone();
        string getProperty(string name, string dflt);
        bool getBoolProperty(string name, bool dflt);
        int getIntProperty(string name, int dflt);
        double getDoubleProperty(string name, double dflt);
        Dictionary<string, object> getPairParams(string pair);
    }
    public interface Display
    {
        void listUpdate(List<string> list);
        void display(string s);
        void append(string s);
        void appendLine(string s);
        void appendLine(string s, params object[] values);
        //void appendLine(string s, object o);
        void setStatus(string s);
        //void setStatus2(string s, Color color);
        void logger(string s);
    }
    #endregion
}
