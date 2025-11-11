using Grpc.Core;
using Server.Protos;

namespace Server.Services;

public class GreeterService : Greeter.GreeterBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        HelloReply reply = new HelloReply { Message = $"HeLlO wOrlD - {request.Name}" };
        return Task.FromResult(reply);
    }
}