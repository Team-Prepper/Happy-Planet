using System;

namespace EHTool.UIKit {
    public interface IGUI : IComparable<IGUI> {

        public uint Priority { get; }

        public void SetOn();
        public void SetOff();
        public void Open();
        public void Open(CallbackMethod callback);
        public void Close();

    }
}