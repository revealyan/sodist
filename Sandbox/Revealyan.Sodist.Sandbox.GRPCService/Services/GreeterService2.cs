using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Revealyan.Sodist.Sandbox.GRPCService
{
    public class GreeterService2 : Greeter2.Greeter2Base
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService2(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public async override Task<HelloReply2> SayHello(HelloRequest2 request, ServerCallContext context)
        {
            await Task.Delay(1);
            return new HelloReply2();
        }
    }
}