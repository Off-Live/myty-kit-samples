using System.Collections;
using System.Collections.Generic;
using MYTYKit.AvatarImporter;
using MYTYKit.Components;
using MYTYKit.MotionTemplates;
using UnityEngine;

public class AvatarLoader : MonoBehaviour
{
    public MotionSource motionSource;
    
    IEnumerator Start()
    {
        MYTYAvatarImporter.InitAvatarAsset();
        var asset_name = "ghostsproject_sample";
        var bundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath+"/"+asset_name);
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
