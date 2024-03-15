using Azure.Communication.CallingServer;

namespace Azure.Communication.CallingServers.Models
{
    public class CreateCallResult
    {
        internal CreateCallResult(CallConnection callConnection, CallConnectionProperties callConnectionProperties)
        {
            CallConnection = callConnection;
            CallConnectionProperties = callConnectionProperties;
        }

        /// <summary> CallConnection instance. </summary>
        public CallConnection CallConnection { get; }

        /// <summary> Properties of the call. </summary>
        public CallConnectionProperties CallConnectionProperties { get; }
    }
}
