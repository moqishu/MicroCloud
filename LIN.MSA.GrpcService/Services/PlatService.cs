using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace LIN.MSA.GrpcService
{
    public class PlatService : PlatCenter.PlatCenterBase
    {
        private readonly ILogger<PlatService> _logger;
        public PlatService(ILogger<PlatService> logger)
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

        public override Task<RegisterReply> Register(RegisterRequest request, ServerCallContext context)
        {
            return Task.FromResult(new RegisterReply
            {
                Message = "Hello " + request.Data
            });
        }

        public override Task<FindReply> FindService(FindRequest request, ServerCallContext context)
        {

            var result = string.Empty;

            return Task.FromResult(new FindReply
            {
                Message = result
            });
        }

    }
}
