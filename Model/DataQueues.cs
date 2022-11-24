using System.Collections.Concurrent;

namespace CentralControlGrpcService.Model
{
    public class DataQueues
    {
        public static ConcurrentDictionary<string, QueueInfo<GrpcData>> SendDataQueue = new ConcurrentDictionary<string, QueueInfo<GrpcData>>();
        public static ConcurrentDictionary<int, string> ServerDatas = new ConcurrentDictionary<int, string>();
    }
}
