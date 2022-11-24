using Grpc.Core;

namespace CentralControlGrpcService.Core
{
    public class TaskInfo
    {

        private Task _tk = null;
        private bool _isStarted = false;

        private GrpcData _request = null;
        private IServerStreamWriter<GrpcData> _writerStream = null;
        private ServerCallContext _context = null;
        private QueueInfo<GrpcData> _resDataQueue = null;
        private CancellationTokenSource _cacelTokenSource = new CancellationTokenSource();

        public TaskInfo(GrpcData request, IServerStreamWriter<GrpcData> responseStream, ServerCallContext context, QueueInfo<GrpcData> resDataQueue)
        {
            _request = request;
            _writerStream = responseStream;
            _context = context;
            _resDataQueue = resDataQueue;
        }

        public bool IsStarted
        {
            get
            {
                if (_tk == null)
                {
                    _isStarted = false;
                }
                else if (_tk.IsCompleted)
                {
                    _isStarted = false;
                    _tk.Dispose();
                    _tk = null;
                }
                else
                {
                    _isStarted = true;
                }
                return _isStarted;
            }
        }

        public Task GetTask()
        {
            return _tk;
        }
        public bool Start()
        {
            try
            {
                if (IsStarted)
                {
                    return true;
                }
                _isStarted = true;
                _tk = Task.Factory.StartNew(DataWriteThead);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                if (IsStarted)
                {
                    _isStarted = false;
                    if (_resDataQueue == null)
                    {
                        return false;
                    }
                    _resDataQueue.QueueResetEvent.Set();
                }
                _isStarted = false;
                _cacelTokenSource.Cancel();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void DataWriteThead()
        {
            if (_writerStream == null || _request == null || _context == null || _resDataQueue == null)
            {
                return;
            }
            while (_isStarted && !_context.CancellationToken.IsCancellationRequested && !_cacelTokenSource.IsCancellationRequested)
            {
                try
                {
                    if (_resDataQueue.IsEmpty)
                    {
                        _resDataQueue.QueueResetEvent.WaitOne();
                    }
                    GrpcData data;
                    if (_resDataQueue.TryPeek(out data))
                    {
                        // 向客户端发送数据
                        Task wTask = _writerStream.WriteAsync(data);
                        wTask.Wait(5000, _cacelTokenSource.Token);
                        if (!wTask.IsCanceled && !wTask.IsFaulted)
                        {
                            _resDataQueue.TryDequeue(out data);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
