using System.Collections;
using System.Collections.Generic;
using MYTYKit.MotionTemplates;
using MYTYKit.MotionTemplates.Mediapipe.Model;
using MYTYKit.ThirdParty.MeFaMo;
using UnityEngine;

public class MotionData
{
    public List<Vector3> face;
    public List<Vector3> pose;
    public int width;
    public int height;
}
public class WebInterface : MonoBehaviour
{
    public MotionSource motionSource;
    
    
    MeFaMoSolver m_solver = new MeFaMoSolver();
    Vector3[] m_solverBuffer;
    
    public void ProcessCapturedData(string json)
    {
        var motionData = JsonUtility.FromJson<MotionData>(json);

        var faceModels = motionSource.GetBridgesInCategory("FaceLandmark");

        foreach (var model in faceModels)
        {
            var basemodel = model as MPBaseModel;
            ProcessNormalized(basemodel, motionData.face);
        }

        var poseModels = motionSource.GetBridgesInCategory("PoseLandmark");
        foreach (var model in poseModels)
        {
            var basemodel = model as MPBaseModel;
            ProcessNormalized(basemodel, motionData.pose);
        }

        if (m_solverBuffer == null || m_solverBuffer.Length != motionData.face.Count)
        {
            m_solverBuffer = new Vector3[motionData.face.Count];
        }

        for (var i = 0; i < motionData.face.Count; i++)
        {
            var elem = motionData.face[i];
            m_solverBuffer[i] = elem;
        }
        m_solver.Solve(m_solverBuffer, motionData.width, motionData.height);
        var solverModels = motionSource.GetBridgesInCategory("FaceSolver");
        foreach (var model in solverModels)
        {
            var solverModel = model as MPSolverModel;
            solverModel.SetSolver(m_solver);
            solverModel.Flush();
        }



    }
    
    
    public void ProcessNormalized(MPBaseModel model, List<Vector3> landmarkList)
    {
        if (model == null || landmarkList == null) return;

        if (model.GetNumPoints() != landmarkList.Count)
        {
            model.Alloc(landmarkList.Count);
        }

        int index = 0;

        foreach (var elem in landmarkList)
        {
            model.SetPoint(index, elem);
            index++;
        }
        model.Flush();

    }
}
