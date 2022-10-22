using System;
using System.Collections.Generic;
using MYTYKit.MotionTemplates;
using UnityEngine;
using UnityEngine.XR.ARKit;




public class ARKitSimpleFace : MotionTemplateBridge
{
    public float leftEye;
    public float rightEye;

    public float leftEyeBrow;
    public float rightEyeBrow;

    public Vector2 leftPupil = Vector2.zero;
    public Vector2 rightPupil = Vector2.zero;

    public float mouthX;
    public float mouthY;

    
    Dictionary<ARKitBlendShapeLocation, float> m_coeffMap= new();
    public void SetCoefficient(ARKitBlendShapeLocation key, float val)
    {
        m_coeffMap[key] = val;
    }

    public void Flush()
    {
        Process();
        UpdateTemplate();
    }

    protected override void Process()
    {

        leftEye = 1.0f - m_coeffMap[ARKitBlendShapeLocation.EyeBlinkLeft];
        rightEye = 1.0f - m_coeffMap[ARKitBlendShapeLocation.EyeBlinkRight];

        var mouthPouker = m_coeffMap[ARKitBlendShapeLocation.MouthPucker];
        var mouthLeftHalf = Mathf.Max(m_coeffMap[ARKitBlendShapeLocation.MouthStretchLeft], m_coeffMap[ARKitBlendShapeLocation.MouthSmileLeft]);
                            
        var mouthRightHalf =Mathf.Max(m_coeffMap[ARKitBlendShapeLocation.MouthStretchRight], m_coeffMap[ARKitBlendShapeLocation.MouthSmileRight]);
                             
        var mouthNeutralX = 0.4f;

        if (mouthPouker > 0.15f)
        {
            mouthX = (1 - mouthPouker) * mouthNeutralX;
        }
        else
        {
            mouthNeutralX = (1 - mouthPouker) * mouthNeutralX;
            mouthX = mouthNeutralX + (mouthLeftHalf + mouthRightHalf) * (1 - mouthNeutralX) * 0.5f;
        }

        mouthY = m_coeffMap[ARKitBlendShapeLocation.JawOpen] - m_coeffMap[ARKitBlendShapeLocation.MouthClose];

        var eyebrowNeutral = 0.5f;

        leftEyeBrow = (1.0f - m_coeffMap[ARKitBlendShapeLocation.BrowDownLeft]) * eyebrowNeutral +
                      m_coeffMap[ARKitBlendShapeLocation.BrowOuterUpLeft] * (1.0f - eyebrowNeutral);
        rightEyeBrow = (1.0f - m_coeffMap[ARKitBlendShapeLocation.BrowDownRight]) * eyebrowNeutral +
                      m_coeffMap[ARKitBlendShapeLocation.BrowOuterUpRight] * (1.0f - eyebrowNeutral);
        
    }

    protected override void UpdateTemplate()
    {
        if (templateList.Count == 0) return;

        foreach (var motionTemplate in templateList)
        {
            var template = (ParametricTemplate)motionTemplate;
           
            template.SetValue("leftEye",leftEye);
            template.SetValue("rightEye",rightEye);
            template.SetValue("leftEyeBrow", leftEyeBrow);
            template.SetValue("rightEyeBrow", rightEyeBrow);
            template.SetValue("leftPupilX", leftPupil.x);
            template.SetValue("leftPupilY", leftPupil.y);
            template.SetValue("rightPupilX", rightPupil.x);
            template.SetValue("rightPupilY", rightPupil.y);
            template.SetValue("mouthX", mouthX);
            template.SetValue("mouthY", mouthY);
            template.NotifyUpdate();
            
        }

    }
}
