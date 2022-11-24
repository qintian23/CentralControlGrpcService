using CentralControlGrpcService;
using CentralControlGrpcService.Core;
using Grpc.Core;
using System.Collections.Concurrent;

namespace CentralControlGrpcService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public ConcurrentDictionary<string, TaskInfo> _receiveDataTaskId = new ConcurrentDictionary<string, TaskInfo>();
        public Func<GrpcData, GrpcResult> MessageHandler { get; set; } = null;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<GrpcResult> SendMsg(GrpcData request, ServerCallContext context)
        {
            if (MessageHandler == null)
            {
                return Task.FromResult<GrpcResult>(new GrpcResult() { Result = false });
            }
            Tuple<string, int> clientIP = SplitKeyStrGrpcContextPeer(context.Peer);

            if (string.IsNullOrEmpty(request.Ip))
            {
                request.Ip = clientIP.Item1;
                request.Port = clientIP.Item2;
            }
            GrpcResult result = MessageHandler(request);
            return Task.FromResult<GrpcResult>(result);
        }
        public override Task ReceiveDataFromServer(GrpcData request, IServerStreamWriter<GrpcData> responseStream, ServerCallContext context)
        {
            Tuple<string, int> clientIP = SplitKeyStrGrpcContextPeer(context.Peer);
            string IPkeyStr = GetKeyStr(clientIP.Item1, clientIP.Item2);
            if (_receiveDataTaskId.ContainsKey(IPkeyStr))
            {
                // 线程未完成
                if (_receiveDataTaskId[IPkeyStr].IsStarted)
                {
                    bool res = _receiveDataTaskId[IPkeyStr].Stop();
                }

                TaskInfo tmp;
                _receiveDataTaskId.TryRemove(IPkeyStr, out tmp);
            }

            if (!DataQueues.SendDataQueue.ContainsKey(IPkeyStr))
            {
                DataQueues.SendDataQueue[IPkeyStr] = new QueueInfo<GrpcData>();
            }
            DataQueues.SendDataQueue[IPkeyStr].Clear();
            if (string.IsNullOrEmpty(request.Ip))
            {
                request.Ip = clientIP.Item1;
                request.Port = clientIP.Item2;
            }
            TaskInfo tInfo = new TaskInfo(request, responseStream, context, DataQueues.SendDataQueue[IPkeyStr]);
            bool result = tInfo.Start();
            _receiveDataTaskId[IPkeyStr] = tInfo;
            return tInfo.GetTask();
        }
        private string GetKeyStr(string ip, int port)
        {
            return string.Format("{0}:{1}", ip, port.ToString());
        }
        private Tuple<string, int> SplitKeyStr(string strKey)
        {
            Tuple<string, int> tuple = new Tuple<string, int>("", 0);
            string[] res = strKey.Split(':');
            if (res.Length != 2)
            {
                return tuple;
            }
            int port;
            if (!int.TryParse(res[1], out port))
            {
                return tuple;
            }

            tuple = new Tuple<string, int>(res[0], port);

            return tuple;
        }
        private Tuple<string, int> SplitKeyStrGrpcContextPeer(string strKey)
        {
            Tuple<string, int> tuple = new Tuple<string, int>("", 0);
            string[] res = strKey.Split(':');
            if (res.Length < 2)
            {
                return tuple;
            }
            int lenth = res.Length;
            int port;
            if (!int.TryParse(res[lenth - 1], out port))
            {
                return tuple;
            }

            tuple = new Tuple<string, int>(res[lenth - 2], port);

            return tuple;
        }
    }
}