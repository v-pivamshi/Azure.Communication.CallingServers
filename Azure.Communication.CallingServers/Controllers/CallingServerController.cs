using Azure.Communication.CallingServer;
using Microsoft.AspNetCore.Mvc;
using CreateCallOptions = Azure.Communication.CallAutomation.CreateCallOptions;

namespace Azure.Communication.CallingServers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallingServerController : ControllerBase
    {
        private readonly CallingServerClient _client;
        private readonly IConfiguration _configuration;
        private readonly CallAutomationClient _callAutomationClient;
        public CallingServerController(IConfiguration configuration, CallAutomationClient callAutomationClient)
        {
            _configuration = configuration;
            _callAutomationClient = callAutomationClient;
            _client = new CallingServerClient(_configuration["ACSResourceConnectionString"]);
        }

        [HttpGet("createCall")]
        public async Task<IActionResult> CreateCallAsync()
        {
            try
            {
                var caller = new CommunicationUserIdentifier("8:acs:19ae37ff-1a44-4e19-aade-198eedddbdf2_0000001e-dc59-733e-f883-08482200a171");
                var target = new PhoneNumberIdentifier("+918688023395");

                var createCallOption = new CreateCallOptions(new Uri(_configuration["CallbackUriHost"]),
                    new[] { MediaType.Audio },
                    new[]
                   {
                       EventSubscriptionType.ParticipantsUpdated,
                       EventSubscriptionType.DtmfReceived
                   });

                var callConnection = await _client.CreateCallConnectionAsync(caller, new List<CommunicationIdentifier>() { target }, createCallOption);
                return Ok(callConnection);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getCallConnection")]
        public async Task<IActionResult> GetCallConnection()
        {
            try
            {
                var caller = new CommunicationUserIdentifier("8:acs:19ae37ff-1a44-4e19-aade-198eedddbdf2_0000001e-dc59-733e-f883-08482200a171");
                var target = new PhoneNumberIdentifier("+918688023395");

                var createCallOption = new CreateCallOptions(new Uri(_configuration["CallbackUriHost"]),
                    new[] { MediaType.Audio },
                    new[]
                   {
                       EventSubscriptionType.ParticipantsUpdated,
                       EventSubscriptionType.DtmfReceived
                   });

                var callConnection = await _client.CreateCallConnectionAsync(caller, new List<CommunicationIdentifier>() { target }, createCallOption);
                var getCallConnection = _client.GetCallConnection(callConnection.Value.CallConnectionId);
                var a = await _callAutomationClient.CreateCallAsync

                return Ok(getCallConnection);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
