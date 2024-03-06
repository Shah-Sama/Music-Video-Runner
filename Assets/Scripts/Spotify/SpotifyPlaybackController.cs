using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SpotifyPlaybackController : MonoBehaviour
{
    public void StartPlayback(string device_id = null)
    {
        string url = "https://api.spotify.com/v1/me/player/play";
        if (!string.IsNullOrEmpty(device_id))
        {
            url += $"?device_id={device_id}";
        }
        StartCoroutine(SendPlaybackCommand(url, "PUT"));
    }

    public void PausePlayback()
    {
        string url = "https://api.spotify.com/v1/me/player/pause";
        StartCoroutine(SendPlaybackCommand(url, "PUT"));
    }

    public void SkipToNextTrack()
    {
        string url = "https://api.spotify.com/v1/me/player/next";
        StartCoroutine(SendPlaybackCommand(url, "POST"));
    }

    public void SkipToPreviousTrack()
    {
        string url = "https://api.spotify.com/v1/me/player/previous";
        StartCoroutine(SendPlaybackCommand(url, "POST"));
    }
    private IEnumerator SendPlaybackCommand(string url, string method)
    {
        using (UnityWebRequest www = new UnityWebRequest(url, method))
        {
            www.SetRequestHeader("Authorization", "Bearer " + SpotifyAuthenticationManager.AccessToken);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Playback command successful: " + www.downloadHandler.text);
            }
        }
    }

     public void GetAvailableDevices()
    {
        StartCoroutine(FetchAvailableDevices());
    }

    private IEnumerator FetchAvailableDevices()
    {
        string url = "https://api.spotify.com/v1/me/player/devices";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", "Bearer " + SpotifyAuthenticationManager.AccessToken);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching devices: " + www.error);
            }
            else
            {
                Debug.Log("Available devices: " + www.downloadHandler.text);
                ProcessDevicesJson(www.downloadHandler.text);
            }
        }
    }

    private void ProcessDevicesJson(string json)
    {
        DevicesResponse devicesResponse = JsonUtility.FromJson<DevicesResponse>(json);
        foreach (var device in devicesResponse.devices)
        {
            if (device.is_active)
            {
                Debug.Log($"Device Name: {device.name}, Type: {device.type}, Volume: {device.volume_percent}");
            }
        }
    }

[Serializable]
public class DevicesResponse
{
    public DeviceObject[] devices;
}

[Serializable]
public class DeviceObject
{
    public string id;
    public bool is_active;
    public bool is_private_session;
    public bool is_restricted;
    public string name;
    public string type;
    public int volume_percent;
    public bool supports_volume;
}