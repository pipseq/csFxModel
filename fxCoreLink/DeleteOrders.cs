using Common;
using fxcore2;
using System;
using System.Collections.Generic;
using System.Threading;

namespace fxCoreLink
{
    public class DeleteOrders : IDisposable
    {
        Logger log = Logger.LogManager("DeleteOrders");
        public DeleteOrders(O2GSession session, Display display)
        {
            this.session = session;
            this.display = display;
        }
        private Display display;
        private O2GSession session;
        public ManualResetEvent manualEvent;

        Dictionary<string, Dictionary<string, string>> orderMap = new Dictionary<string, Dictionary<string, string>>();
        public void deleteOCOs(IFXManager fxManager)
        {
            string tn = "ORDERS";
            orderMap = fxManager.getTable(tn);
            //fxManager.printMap(tn, orderMap);

            // delete where type = SE
            foreach (Dictionary<string, string> map in orderMap.Values)
            {
                string type = map["Type"];
                string oid = map["OrderID"];
                string coid = map["ContingentOrderID"];
                if (type == "SE"
                    && oid != ""
                    //&& coid != "" // the orphaned LSEs don't have a contingency id but need to be deleted, too
                    )
                {
                    deleteOrder(oid);
                }
            }

        }

        // original
        public void deleteOrder(string sOrderID)
        {
            CtrlTimer.getInstance().startTimer("DeleteOrders");

            manualEvent = new ManualResetEvent(false);


            try
            {
                O2GRequestFactory factory = session.getRequestFactory();
                O2GValueMap valuemap = factory.createValueMap();
                valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.DeleteOrder);
                valuemap.setString(O2GRequestParamsEnum.OrderID, sOrderID);
                O2GRequest request = factory.createOrderRequest(valuemap);
                session.sendRequest(request);
                log.debug("Delete order " + sOrderID);
                manualEvent.WaitOne();

            }
            finally
            {
                CtrlTimer.getInstance().stopTimer("DeleteOrders");
            }
        }


        //// original
        //public void deleteOrder(string sOrderID)
        //{
        //    CtrlTimer.getInstance().startTimer("DeleteOrders");

        //    manualEvent = new ManualResetEvent(false);
        //    O2GTableManager tableMgr = session.getTableManager();
        //    O2GTable ordersTable = tableMgr.getTable(O2GTableType.Orders);
        //    O2GTableStatus tableStatus = ordersTable.getStatus();
        //    while (tableStatus != O2GTableStatus.Refreshed && tableStatus != O2GTableStatus.Failed)
        //    {
        //        Thread.Sleep(50);
        //        tableStatus = ordersTable.getStatus();
        //    }
        //    if (tableStatus == O2GTableStatus.Failed)
        //    {
        //        throw new Exception ("Order table refresh failed on delete");
        //    }
        //    ordersTable.subscribeUpdate(O2GTableUpdateType.Delete, this);


        //    try
        //    {
        //        O2GRequestFactory factory = session.getRequestFactory();
        //        O2GValueMap valuemap = factory.createValueMap();
        //        valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.DeleteOrder);
        //        valuemap.setString(O2GRequestParamsEnum.OrderID, sOrderID);
        //        O2GRequest request = factory.createOrderRequest(valuemap);
        //        session.sendRequest(request);
        //        log.debug("Delete order " + sOrderID);
        //        manualEvent.WaitOne();

        //    }
        //    finally
        //    {
        //        CtrlTimer.getInstance().stopTimer("DeleteOrders");
        //        ordersTable.unsubscribeUpdate(O2GTableUpdateType.Delete, this);
        //    }
        //}

        //#region IO2GTableListener Members

        //public void onAdded(string rowID, O2GRow rowData)
        //{
        //}

        //public void onChanged(string rowID, O2GRow rowData)
        //{
        //}

        //public void onDeleted(string rowID, O2GRow rowData)
        //{
        //    manualEvent.Set();
        //}

        //public void onEachRow(string rowID, O2GRow rowData)
        //{
        //}

        //public void onStatusChanged(O2GTableStatus status)
        //{
        //}

        //#endregion


        public void Dispose()
        {
            manualEvent.Dispose();
            manualEvent = null;
        }
    }
}
