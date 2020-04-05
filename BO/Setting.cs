using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuckatorService.BO
{
    public class Setting
    {
        public int Id { get; set; }
        public int KeyName { get; set; }
        public string KeyValue { get; set; }        

    }

    public class SettingList
    {
        public List<Setting> Settings { get; set; }           
    }    

}

