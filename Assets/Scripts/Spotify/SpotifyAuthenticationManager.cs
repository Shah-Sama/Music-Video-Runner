using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SpotifyAuthenticationManager : MonoBehaviour
{
    public static string AccessToken { get; private set; }
    private const string ClientId = "YOUR_CLIENT_ID";
    private const string RedirectUri = "YOUR_REDIRECT_URI";
    private readonly string[] Scopes = { "user-read-playback-state", "user-modify-playback-state", "user-read-currently-playing", "streaming" };
    private string codeVerifier;
    private string codeChallenge;

    void Start()
    {
        GenerateCodeVerifierAndChallenge();
        RequestUserAuthorization();
    }

    private void GenerateCodeVerifierAndChallenge()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            codeVerifier = Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                codeChallenge = Convert.ToBase64String(challengeBytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
        }
    }

    private void RequestUserAuthorization()
    {
        var scopeString = string.Join(" ", Scopes);
        var authorizationUrl = $"https://accounts.spotify.com/authorize?response_type=code&client_id={ClientId}&scope={Uri.EscapeDataString(scopeString)}&redirect_uri={RedirectUri}&code_challenge_method=S256&code_challenge={codeChallenge}";

        Application.OpenURL(authorizationUrl);
    }

    public void ExchangeCodeForToken(string authorizationCode)
    {
        StartCoroutine(ExchangeCode(authorizationCode));
    }

    private IEnumerator ExchangeCode(string code)
    {
        var tokenUrl = "https://accounts.spotify.com/api/token";
        var postData = $"client_id={ClientId}&grant_type=authorization_code&code={code}&redirect_uri={RedirectUri}&code_verifier={codeVerifier}";
        var data = Encoding.UTF8.GetBytes(postData);

        using (UnityWebRequest www = UnityWebRequest.Post(tokenUrl, UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(data);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var jsonResponse = JsonUtility.FromJson<AuthorizationResponse>(www.downloadHandler.text);
                AccessToken = jsonResponse.access_token;
                Debug.Log("Access Token: " + AccessToken);
            }

            else if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.LogError("Unknown error");
            }

        }
    }

    [Serializable]
    private class AuthorizationResponse
    {
        public string access_token;
        public string token_type;
        public string scope;
        public int expires_in;
        public string refresh_token;
    }
}
