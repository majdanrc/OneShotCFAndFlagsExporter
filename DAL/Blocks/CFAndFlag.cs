using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Blocks
{
    public class CFAndFlag
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string CF1s
        {
            get
            {
                return string.Join("#;#", _CF1List);
            }
            set
            {
                _CF1List = !string.IsNullOrWhiteSpace(value) ? value.Split(new string[] { "#;#" }, StringSplitOptions.None).ToList() : new List<string>();
            }
        }
        public string CF2s {
            get {
                return string.Join("#;#", _CF2List);
            }
            set {
                _CF2List = !string.IsNullOrWhiteSpace(value) ? value.Split(new string[] { "#;#" }, StringSplitOptions.None).ToList() : new List<string>();
            }
        }
        public string Flags
        {
            get
            {
                return string.Join("#;#", _flagList);
            }
            set
            {
                _flagList = !string.IsNullOrWhiteSpace(value) ? value.Split(new string[] { "#;#" }, StringSplitOptions.None).ToList() : new List<string>();
            }
        }

        private List<string> _CF1List = new List<string>();
        private List<string> _CF2List = new List<string>();
        private List<string> _flagList = new List<string>();

        public List<string> GetCF1List()
        {
            return _CF1List;
        }

        public List<string> GetCF2List()
        {
            return _CF2List;
        }

        public List<string> GetFlags()
        {
            return _flagList;
        }
    }
}
