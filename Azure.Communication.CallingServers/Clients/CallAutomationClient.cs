using Azure.Communication.CallAutomation;
using Azure.Communication.CallingServer;
using Azure.Communication.CallingServers.Models;
using Azure.Core.Pipeline;
using AnswerCallResult = Azure.Communication.CallAutomation.AnswerCallResult;
using CallConnection = Azure.Communication.CallingServer.CallConnection;
using CallConnectionProperties = Azure.Communication.CallAutomation.CallConnectionProperties;

namespace Azure.Communication.CallingServers.Clients
{
    public interface ICallAutomationClient
    {
        //public Task<Response<CreateCallResult>> CreateCall();
    }
    public class CallAutomationClient : ICallAutomationClient
    {
        internal readonly string _resourceEndpoint;
        internal readonly ClientDiagnostics _clientDiagnostics;
        internal readonly HttpPipeline _pipeline;
        public virtual async Task<Response<AnswerCallResult>> AnswerCallAsync(string incomingCallContext, Uri callbackUri, CancellationToken cancellationToken = default)
        {
            AnswerCallOptions options = new AnswerCallOptions(incomingCallContext, callbackUri);

            return await AnswerCallAsync(options, cancellationToken).ConfigureAwait(false);
        }
        public virtual async Task<Response<AnswerCallResult>> AnswerCallAsync(AnswerCallOptions options, CancellationToken cancellationToken = default)
        {
            try
            {
                if (options == null) throw new ArgumentNullException(nameof(options));

                var request = CreateAnswerCallRequest(options);

                var answerResponse = await AzureCommunicationServicesRestClient.AnswerCallAsync(request, cancellationToken).ConfigureAwait(false);

                var result = new AnswerCallResult(GetCallConnection(answerResponse.Value.CallConnectionId), new CallConnectionProperties(answerResponse.Value));
                result.SetEventProcessor(EventProcessor, answerResponse.Value.CallConnectionId, null);

                return Response.FromValue(result,
                    answerResponse.GetRawResponse());
            }
            catch (Exception ex)
            {
                scope.Failed(ex);
                throw;
            }
        }
         public virtual Response<AnswerCallResult> AnswerCall(string incomingCallContext, Uri callbackUri, CancellationToken cancellationToken = default)
        {
            AnswerCallOptions options = new AnswerCallOptions(incomingCallContext, callbackUri);

            return AnswerCall(options, cancellationToken);
        }

       public virtual Response<AnswerCallResult> AnswerCall(AnswerCallOptions options, CancellationToken cancellationToken = default)
        {
            using DiagnosticScope scope = _clientDiagnostics.CreateScope($"{nameof(CallAutomationClient)}.{nameof(AnswerCall)}");
            scope.Start();
            try
            {
                if (options == null) throw new ArgumentNullException(nameof(options));

                AnswerCallRequestInternal request = CreateAnswerCallRequest(options);

                var answerResponse = AzureCommunicationServicesRestClient.AnswerCall(request, cancellationToken);

                var result = new AnswerCallResult(GetCallConnection(answerResponse.Value.CallConnectionId), new CallConnectionProperties(answerResponse.Value));
                result.SetEventProcessor(EventProcessor, answerResponse.Value.CallConnectionId, null);

                return Response.FromValue(result,
                    answerResponse.GetRawResponse());
            }
            catch (Exception ex)
            {
                scope.Failed(ex);
                throw;
            }
        }

        private AnswerCallRequestInternal CreateAnswerCallRequest(AnswerCallOptions options)
        {
            AnswerCallRequestInternal request = new AnswerCallRequestInternal(options.IncomingCallContext, options.CallbackUri.AbsoluteUri);

            // Add CallIntelligenceOptions such as custom cognitive service domain name
            string cognitiveServicesEndpoint = options.CallIntelligenceOptions?.CognitiveServicesEndpoint?.AbsoluteUri;
            if (cognitiveServicesEndpoint != null)
            {
                request.CallIntelligenceOptions = new()
                {
                    CognitiveServicesEndpoint = cognitiveServicesEndpoint
                };
            }

            request.MediaStreamingConfiguration = CreateMediaStreamingOptionsInternal(options.MediaStreamingOptions);
            request.TranscriptionConfiguration = CreateTranscriptionOptionsInternal(options.TranscriptionOptions);
            request.AnsweredBy = Source == null ? null : new CommunicationUserIdentifierModel(Source.Id);
            request.OperationContext = options.OperationContext;
            request.SourceCallerIdNumber = options.SourceCallerIdNumber == null ? null : new PhoneNumberIdentifierModel(options.SourceCallerIdNumber.PhoneNumber);

            return request;
        }
        public virtual CallConnection GetCallConnection(string callConnectionId)
        {
            using DiagnosticScope scope = _clientDiagnostics.CreateScope($"{nameof(CallAutomationClient)}.{nameof(GetCallConnection)}");
            scope.Start();
            try
            {
                return new CallConnection(callConnectionId, CallConnectionsRestClient, ContentRestClient, _clientDiagnostics);
            }
            catch (Exception ex)
            {
                scope.Failed(ex);
                throw;
            }
        }
    }
}
