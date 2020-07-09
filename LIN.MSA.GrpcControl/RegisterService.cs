using Grpc.Core;
using LIN.MSA.GrpcService;
using System;

namespace LIN.MSA.GrpcControl
{
    public class RegisterService
    {
        public static string Register(string serAddress,string req)
        {
            var channel = new Channel(serAddress, ChannelCredentials.Insecure);

            try
            {
                var client = new PlatCenter.PlatCenterClient(channel);

                var response = client.Register(new RegisterRequest { Data = req });

                return response.Message;

            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
