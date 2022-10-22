
using MYTYKit.MotionTemplates;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;



public class MYTYARKitSource : MonoBehaviour
{
    ARFace m_face;
    [SerializeField] ARFaceManager faceManager;
    [SerializeField] MotionSource source;

    ARKitFaceSubsystem m_faceSubsystem;

    void Awake()
    {
        m_face = GetComponent<ARFace>();
    }

    void OnEnable()
    {
        m_faceSubsystem = (ARKitFaceSubsystem)faceManager.subsystem;
        m_face.updated += OnUpdate;
        Debug.Log("enable!");
    }

    void OnUpdate(ARFaceUpdatedEventArgs args)
    {
        
        var headBridges = source.GetBridgesInCategory("Anchors");
        foreach (var item in headBridges)
        {
            var anchorBridge = item as ARHeadRotation;
            anchorBridge.SetUpAndForward(m_face.transform.up, m_face.transform.forward);
            anchorBridge.Flush();
        }

        var parametricBridge = source.GetBridgesInCategory("Parametric");
        using (var blendShapes = m_faceSubsystem.GetBlendShapeCoefficients(m_face.trackableId, Allocator.Temp))
        {
            foreach (var featureCoefficient in blendShapes)
            {

                foreach (var item in parametricBridge)
                {
                    var simpleFace = item as ARKitSimpleFace;
                    simpleFace.SetCoefficient(featureCoefficient.blendShapeLocation,
                        featureCoefficient.coefficient);
                }

            }
        }

        foreach (var item in parametricBridge)
        {
            var simpleFace = item as ARKitSimpleFace;
            simpleFace.Flush();

        }
    }
}

