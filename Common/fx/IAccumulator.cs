using System.Collections.Generic;

namespace Common.fx
{
    public interface IAccumulator
    {
        void add(List<double> list);
        void addLast(double value);
        double getLast();
        List<double> getList();
        List<double> getList(int periods);
        bool isEnoughData(int periods);
    }
}