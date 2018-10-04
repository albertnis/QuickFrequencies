
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MechanicalMusings.QuickFrequencies
{
    public class VtxBand {
        public string name;
        public int frequencyStart;
        public int frequencyInterval;

        VtxBand(string name, int frequencyStart, int frequencyInterval) {
            this.name = name;
            this.frequencyStart = frequencyStart;
            this.frequencyInterval = frequencyInterval;
        }

        public static List<VtxBand> bands = new List<VtxBand>(){
            new VtxBand("A", 5865, -20),
            new VtxBand("B", 5733, 19),
            new VtxBand("E", 5705, -20),
            new VtxBand("F", 5740, 20),
            new VtxBand("C", 5658, 37),
            new VtxBand("D", 5362, 37),
            new VtxBand("U", 5325, 23),
            new VtxBand("O", 5474, 18),
            new VtxBand("L", 5333, 40),
            new VtxBand("H", 5653, 40),
        };
    }

    public class VtxChannel {
        public VtxBand band;
        public int number;
        public int frequency {
            get {
                return band.frequencyStart + band.frequencyInterval * (number - 1);
            }
        }
    }

    public class Intent {
        public string name;
        public string displayName;
    }

    public class ChannelToFrequencyParameters {
        public string vtxband;
        public string vtxchannelnumber;
    }

    public static class Convert
    {
        [FunctionName("Convert")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            Intent intent  = data.queryResult.intent;

            if (intent.name == "projects/quickfrequencies/agent/intents/b9813f3f-9fd5-41ef-aae5-04f1535ef9ea") {
                ChannelToFrequencyParameters vtxChannelParams = data.queryResult.parameters;
                return (ActionResult)new OkObjectResult($"You're looking for channel {vtxChannelParams.vtxband} {vtxChannelParams.vtxchannelnumber}");
            }

            return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
