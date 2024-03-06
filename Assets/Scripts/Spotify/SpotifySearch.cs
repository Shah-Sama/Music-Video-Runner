using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SpotifySearch : MonoBehaviour
{
    public void SearchTracks(string query)
    {
        StartCoroutine(SearchSpotify("track", query));
    }

    private IEnumerator SearchSpotify(string type, string query)
    {
        if (string.IsNullOrEmpty(SpotifyAuthenticationManager.AccessToken))
        {
            Debug.LogError("Not authenticated or access token is missing.");
            yield break;
        }

        string url = $"https://api.spotify.com/v1/search?q={UnityWebRequest.EscapeURL(query)}&type={type}&limit=10";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", "Bearer " + SpotifyAuthenticationManager.AccessToken);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Search results: " + www.downloadHandler.text);
            }
        }
    }
}
