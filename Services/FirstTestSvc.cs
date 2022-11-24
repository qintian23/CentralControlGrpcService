using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace CentralControlGrpcService.Services
{
    public class FirstTestSvc : FirstTest.FirstTestBase
    {
        private readonly ConcurrentQueue<GrpcResults> queue = new();

        #region 获取主机信息
        /// <summary>
        /// 获取主机信息
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        private static bool TryTakeHost(string peer, out string host)
        {
            //peer地址示例：ipv4:127.0.0.1:52384、ipv6:[::1]:52384
            var ipv4Match = Regex.Match(peer, @"^ipv4:(.+)", RegexOptions.IgnoreCase);
            if (ipv4Match.Success)
            {
                host = ipv4Match.Groups[1].Value;
                return true;
            }
            var ipv6Match = Regex.Match(peer, @"^ipv6:(.+)", RegexOptions.IgnoreCase);
            if (ipv6Match.Success)
            {
                host = ipv6Match.Groups[1].Value;
                return true;
            }
            host = string.Empty;
            return false;
        }
        #endregion

        #region 简单RPC——有参有返回值
        /// <summary>
        /// 简单RPC——有参有返回值
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<GrpcResults> SimpleRpc(GrpcParams request, ServerCallContext context)
        {
            TryTakeHost(context.Peer, out string host);
            Console.WriteLine($"[{host}]：请求SimpleRpc！");
            return await Task.FromResult(new GrpcResults
            {
                Success = true,
                Data = $"数据已被处理--{request.Id}--{request.Name}"
            });
        }
        #endregion

        #region 简单RPC——有参无返回值
        /// <summary>
        /// 简单RPC——有参无返回值
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<Empty> SimpleEmpty(GrpcParams request, ServerCallContext context)
        {
            TryTakeHost(context.Peer, out string host);
            Console.WriteLine($"[{host}]：请求SimpleEmpty！");
            return await Task.FromResult(new Empty());
        }
        #endregion

        #region 客户端流式RPC
        /// <summary>
        /// 客户端流式RPC
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<GrpcResults> ClientStream(IAsyncStreamReader<GrpcParams> requestStream, ServerCallContext context)
        {
            TryTakeHost(context.Peer, out string host);
            while (await requestStream.MoveNext())
            {
                var ret = requestStream.Current;
                Console.WriteLine($"[{host}]：请求ClientStream：id-{ret.Id}！");
                await Task.FromResult(new GrpcResults
                {
                    Success = true,
                    Data = $"数据已被处理--{ret.Id}--{ret.Name}"
                });
            }
            return new GrpcResults { Data = "已完成" };
        }
        #endregion

        #region 服务端流式RPC
        /// <summary>
        /// 服务端流式RPC
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ServerStream(GrpcParams request, IServerStreamWriter<GrpcResults> responseStream, ServerCallContext context)
        {
            TryTakeHost(context.Peer, out string host);
            Console.WriteLine($"[{host}]：客户端已连接ServerStream！");
            await Task.Run(async () =>
            {
                _ = Task.Run(() => //模拟生产数据
                {
                    for (int i = 0; i < 40; i++)
                    {
                        queue.Enqueue(new GrpcResults { Success = true, Data = $"服务端响应数据:{i}" });
                        Thread.Sleep(700);
                    }
                });
                while (true)
                {
                    if (queue.TryDequeue(out GrpcResults? result) && result != null)
                    {
                        await responseStream.WriteAsync(result);
                    }
                    await Task.Delay(700);
                }
            });
            await Task.CompletedTask;
        }
        #endregion

        #region 双向流式RPC
        /// <summary>
        /// 双向流式RPC
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task TwoWayStream(IAsyncStreamReader<GrpcParams> requestStream, IServerStreamWriter<GrpcResults> responseStream, ServerCallContext context)
        {
            TryTakeHost(context.Peer, out string host);
            while (await requestStream.MoveNext())
            {
                var current = requestStream.Current;
                Console.WriteLine($"[{host}]：请求流TwoWayStream数据:{current.Id}！");
                await responseStream.WriteAsync(new GrpcResults
                {
                    Success = true,
                    Data = $"客户端id:{current.Id}--服务端id:{current.Id + new Random().Next(1, 10)}"
                });
            }
            await Task.CompletedTask;
        }
        #endregion
    }
}
