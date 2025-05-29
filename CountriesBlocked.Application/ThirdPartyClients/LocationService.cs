
using System.Net;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using CountriesBlocked.Application.Dtos;
using CountriesBlocked.Application.Responses;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CountriesBlocked.Application.ThirdPartyClients
{
    public class LocationService(IHttpClientFactory clientFactory,IOptions<IpApiConfig> options):ILocationService
    {
        private readonly HttpClient _httpClient = clientFactory.CreateClient("IpApiConfig");
        private readonly string Access_PKey= options.Value.APIAccessKey;

        public async Task<BlockResponse<IpLocationResponse>> GetLocationByIpAsync(string ip)
        {
            var response = new BlockResponse<IpLocationResponse>();

            if(!IsValidIp(ip)) {
                response.Status=HttpStatusCode.BadRequest;
                response.Message="Format of IP is not correct.";
                return response;
            }

            var url = $"{ip}?access_key={Access_PKey}";

            try {
                var result = await _httpClient.GetAsync(url);

                if(result.IsSuccessStatusCode) {
                    var content = await result.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<IpLocationResponse>(content);

                    if(data!=null) {
                        response.Entity=data;
                        response.Status=HttpStatusCode.OK;
                        response.Message="Successfully retrieved location.";
                        return response;
                    }

                    response.Status=HttpStatusCode.InternalServerError;
                    response.Message="Failed to parse location data.";
                    return response;
                }

                response.Status=result.StatusCode;
                response.Message=$"API error: {result.ReasonPhrase}";
            }
            catch(Exception ex) {
                response.Status=HttpStatusCode.InternalServerError;
                response.Message=$"Exception occurred: {ex.Message}";
            }

            return response;
        }


        private bool IsValidIp(string ip)
        {
            var pattern = @"^(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])(\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])){3}$";
            return Regex.IsMatch(ip,pattern);
        }
    }
}
