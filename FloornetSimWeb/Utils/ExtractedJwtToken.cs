using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

namespace IGT.FloorNet.Tools.ServiceSimulator.Utils
{
    public class ExtractedJwtToken
    {
        public ExtractedJwtToken(string cardID = default, long playerID = default, long cardincount = default, int machineNum = default, string machineLoc = default, string originalJwt = default)
        {
            CardID = cardID;
            PlayerID = playerID;
            CardInCount = cardincount;
            MachineNum = machineNum;
            MachineLoc = machineLoc;
            OriginalJwt = originalJwt;
        }

        public const string PlayerClaim = "sub";
        public const string CardInCountClaim = "igt.ses";
        public const string MachineNumberClaim = "igt.num";
        public const string MachineLocationClaim = "igt.loc";
        public const string CardIdClaim = "igt.cid";

        /// <summary>
        /// Gets or Sets CardID
        /// </summary>
        [DataMember(Name = "CardID", EmitDefaultValue = false)]
        public string CardID { get; set; }

        /// <summary>
        /// Gets or Sets PlayerID
        /// </summary>
        [DataMember(Name = "PlayerID", EmitDefaultValue = false)]
        public long PlayerID { get; set; }


        /// <summary>
        /// Gets or Sets CardInCount
        /// </summary>
        [DataMember(Name = "CardInCount", EmitDefaultValue = false)]
        public long CardInCount { get; set; }


        /// <summary>
        /// Gets or Sets MachineNum
        /// </summary>
        [DataMember(Name = "MachineNum", EmitDefaultValue = false)]
        public int MachineNum { get; set; }

        /// <summary>
        /// Gets or Sets MachineLoc
        /// </summary>
        [DataMember(Name = "MachineLoc", EmitDefaultValue = false)]
        public string MachineLoc { get; set; }

        /// <summary>
        /// Gets or Sets Original Jwt
        /// </summary>
        [DataMember(Name = "OriginalJwt", EmitDefaultValue = false)]
        public string OriginalJwt { get; set; }


        public static ExtractedJwtToken ExtractJWT(string jwt, out JwtSecurityToken? token)
        {
            token = null;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                token = handler.ReadJwtToken(jwt);
                string player = token.Claims.First(claim => claim.Type == PlayerClaim)?.Value;
                string cardInCount = token.Claims.First(claim => claim.Type == CardInCountClaim)?.Value;
                string machineNumber = token.Claims.First(claim => claim.Type == MachineNumberClaim)?.Value;

                return new ExtractedJwtToken()
                {
                    CardID = token.Claims.First(claim => claim.Type == CardIdClaim)?.Value,
                    PlayerID = long.Parse(player),
                    CardInCount = long.Parse(cardInCount),
                    MachineNum = int.Parse(machineNumber),
                    MachineLoc = token.Claims.First(claim => claim.Type == MachineLocationClaim)?.Value,
                    OriginalJwt = jwt
                };

            }
            catch(Exception e)
            {
                return null;
            }
        }

        public static ExtractedJwtToken? ExtractJwtToken(string authorization, out JwtSecurityToken? token)
        {
            if (!AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                token = null;
                return null;
            }
            return ExtractedJwtToken.ExtractJWT(headerValue.Parameter, out token);
        }
    }
}
