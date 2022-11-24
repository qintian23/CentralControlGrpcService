using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace CentralControlGrpcService.Services
{
    public class CentralControlService : CentralControl.CentralControlBase
    {
        private readonly ILogger<CentralControlService> _logger;

        public CentralControlService(ILogger<CentralControlService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 心跳-中控响应
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Empty> HeartBeatResp(Param1 request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 屏幕（背光）亮灭控制-中控响应
        /// </summary>
        public override Task<Empty> ScreenBrightOutResp(Param2 request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// WIFI启用、禁用控制-中控响应
        /// </summary>
        public override Task<Empty> WIFICtrlResp(Param2 request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 通知应用关闭电脑控制-中控上报
        /// </summary>
        public override Task<Empty> NoticeAppShutdownReport(Param1 request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 功放板音量调节-中控响应
        /// </summary>
        public override Task<Empty> PowerAmplifierVolumeCtrlResp(Param1 request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 功放板模式调节-中控响应
        /// </summary>
        public override Task<Empty> PowerAmplifierModeCtrlResp(Param1 request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 提笔和放笔的上报-中控上报
        /// </summary>
        public override Task<Empty> TakePutPenReport(Param3 request, ServerCallContext context) 
        {
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 光照强度-中控上报
        /// </summary>
        public override Task<Empty> LightIntensityReport(Param3 request, ServerCallContext context) 
        {
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 温湿度-中控上报
        /// </summary>
        public override Task<Empty> HumitureDataReport(Param2 request, ServerCallContext context) 
        { 
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 人体传感器-中控上报
        /// </summary>
        public override Task<Empty> BodySensorReport(Param1 request, ServerCallContext context) 
        { 
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// N640图片显示-中控响应
        /// </summary>
        public override Task<Empty> N640PicShowResp(Param5 request, ServerCallContext context) 
        { 
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// N640截图操作-中控响应
        /// </summary>
        public override Task<Empty> N640ScreenShotResp(Param5 request, ServerCallContext context) 
        { 
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 信号源切换-中控响应
        /// </summary>
        public override Task<Empty> SignalSwitchResp(Param4 request, ServerCallContext context) 
        { 
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 分辨率-中控响应
        /// </summary>
        public override Task<Empty> ResolutionResp(Param2 request, ServerCallContext context) 
        { 
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 是否有信号-中控响应事件
        /// </summary>
        public override Task<Empty> HasSignalResp(Param2 request, ServerCallContext context) 
        { 
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 功放板音量查询-中控响应事件
        /// </summary>
        public override Task<Empty> PowerAmplifierVolumeQueryResp(Param2 request, ServerCallContext context) 
        { 
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 功放板音效查询-中控响应事件
        /// </summary>
        public override Task<Empty> PowerAmplifierModeQueryResp(Param2 request, ServerCallContext context) 
        { 
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 中控状态查询-回复-事件
        /// </summary>
        public override Task<Empty> StatusQueryResp(Param3 request, ServerCallContext context) 
        { 
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 设置中控状态-回复-事件
        /// </summary>
        public override Task<Empty> SetCtCtrlStatusResp(Param2 request, ServerCallContext context) 
        { 
            return Task.FromResult(new Empty());
        }

    }
}
