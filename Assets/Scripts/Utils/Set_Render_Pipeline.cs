using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;

public class Set_Render_Pipeline : MonoBehaviour
{
    public LightweightRenderPipelineAsset low, medium, high;

    private void Start()
    {
        if(QualitySettings.GetQualityLevel() == 0)
        {
            GraphicsSettings.renderPipelineAsset = low;
            return;
        }
        if (QualitySettings.GetQualityLevel() == 1)
        {
            GraphicsSettings.renderPipelineAsset = medium;
            return;
        }
        if (QualitySettings.GetQualityLevel() >= 2)
        {
            GraphicsSettings.renderPipelineAsset = high;
            return;
        }
    }
}
