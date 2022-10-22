using System.Collections;
using System.Collections.Generic;
using MYTYKit.AvatarImporter;
using MYTYKit.Components;
using MYTYKit.MotionTemplates;
using UnityEngine;
using UnityEngine.Networking;

public class AvatarLoaderWebGL : MonoBehaviour
{
    public MotionSource motionSource;
    
    IEnumerator Start()
    {
        MYTYAvatarImporter.InitAvatarAsset();
        yield return GetAssetBundle();

    }
    IEnumerator GetAssetBundle()
    {
        var url = "http://localhost:3000/webbuild/StreamingAssets/ghostsproject_sample_webgl";
        using (var uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, 0, 0))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Error in get asset bundle from web : " + uwr.error);
                
            }
            else
            {
                Debug.Log("Success to download " + url);
                var bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                
                ImportSigResolver.DetectAndAttachImporter(gameObject, bundle);
                var avatarGo = new GameObject("ghostAvatar");
                var importer = GetComponent<IMYTYAvatarImporter>();
                yield return  importer.LoadMYTYAvatarAsync(motionSource.gameObject, bundle, avatarGo);
                var camera = avatarGo.transform.GetComponentInChildren<Camera>();
        
                camera.clearFlags = CameraClearFlags.Color;
                var mapper = FindObjectOfType<AvatarSelector>();
                mapper.id = 0;
                mapper.Configure();
            }
        }
        
    }



}
