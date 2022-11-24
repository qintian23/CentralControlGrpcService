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

        public override void HeartBeatResp(Param1 request, ServerCallContext context)
        {

        }
    }
}
