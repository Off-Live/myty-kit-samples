using System;
using System.Collections;
using System.Collections.Generic;
using MYTYKit.MotionTemplates;
using UnityEngine;

public class ARHeadRotation : MotionTemplateBridge
{

    Vector3 m_up, m_lookAt;

    public void SetUpAndForward(Vector3 up, Vector3 forward)
    {
        m_up = up;
        m_lookAt = forward;
        m_up.z = -up.z;
        m_lookAt.z = -forward.z;
    }

    public void Flush()
    {
        Process();
        UpdateTemplate();
    }

    protected override void Process()
    {
        Debug.Log("test");
    }

    protected override void UpdateTemplate()
    {
        foreach (var template in templateList)
        {
            var anchor = template as AnchorTemplate;
            anchor.up = m_up;
            anchor.lookAt = m_lookAt;
            anchor.NotifyUpdate();
        }
    }
}
