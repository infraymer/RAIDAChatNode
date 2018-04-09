using System.Collections.Generic;

namespace RAIDAChatNode.WebAPI.DTO
{
    public class InputSyncData
    {
        public string Key { get; set; }
        public string Code { get; set; }
    }

    public class InputInfo
    {
        public string ServerURL { get; set; }
        public DataInfo<SyncMemberInfo> MemberInfo { get; set; }
        public DataInfo<SyncGroupInfo> GroupInfo { get; set; }
        public DataInfo<SyncMembInGroupInfo> MinGInfo { get; set; }
    }
    
    public class DataInfo<T>
    {
        public List<T> NewRows { get; set; }
        public List<string> Actual { get; set; }

        public DataInfo()
        {
            NewRows = new List<T>();
            Actual = new List<string>();
        }
    }
}