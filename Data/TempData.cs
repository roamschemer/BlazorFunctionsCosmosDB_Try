using System;

namespace Data {
    public class TempData {
        /// <summary>機器名</summary>
        public string Name { get; set; }
        /// <summary>温度(℃)</summary>
        public double Temp { get; set; }
        /// <summary>燻煙が上がっている</summary>
        public bool HasSmoke { get; set; }
        /// <summary>電熱器がON</summary>
        public bool HasSwitchOn { get; set; }
        /// <summary>日時</summary>
        public DateTime DateTime { get; set; }
    }
}