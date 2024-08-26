using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class RTPipelineInputManager : MonoBehaviour
{
    // Oculus
    [Header("XR Origin")]
    [SerializeField] GameObject XROriginGO;
    [SerializeField] RayTracingRenderPipelineAsset rayTracingRenderPipelineAsset;

    [Header("Vive Right Controller")]
    [SerializeField] private InputActionReference vive_rightTriggerPressed;
    [SerializeField] private InputActionReference vive_rightMenuPressed;
    [SerializeField] private InputActionReference vive_rightTrackPadClicked;
    [SerializeField] private InputActionReference vive_rightGripPressed;

    [Header("Vive Left Controller")]
    [SerializeField] private InputActionReference vive_leftTriggerPressed;
    [SerializeField] private InputActionReference vive_leftMenuPressed;
    [SerializeField] private InputActionReference vive_leftTrackPadClicked;
    [SerializeField] private InputActionReference vive_leftGripPressed;

    [Header("Material")]
    public Material foxMat;
    public Material planeMat;

    float fox_metallic;
    float fox_roughness;
    float plane_metallic;
    float plane_roughness;

    float perTimeMatChange;

    bool isPadAddPressed_R;
    bool isPadReducePressed_R;

    bool isPadAddPressed_L;
    bool isPadReducePressed_L;

    int rtXRControlMode;

    ActionBasedSnapTurnProvider snapTurnProvider;
    DynamicMoveProvider moveProvider;

    void Start()
    {
        fox_metallic = 0f;
        fox_roughness = 1f;
        plane_roughness = 0.05f;
        plane_metallic = 0.35f;

        foxMat.SetFloat("_Metallic", fox_metallic);
        foxMat.SetFloat("_Roughness", fox_roughness);
        planeMat.SetFloat("_Metallic", plane_metallic);
        planeMat.SetFloat("_Roughness", plane_roughness);

        perTimeMatChange = 0.05f;
        
        snapTurnProvider = XROriginGO.GetComponent<ActionBasedSnapTurnProvider>();
        moveProvider = XROriginGO.GetComponent<DynamicMoveProvider>();

        rtXRControlMode = 0;
    }



    // Update is called once per frame
    void Update()
    {
        // Lisener of input pad of vive
        isPadAddPressed_R = (vive_rightTrackPadClicked.action.ReadValue<Vector2>().y > 0);
        isPadReducePressed_R = (vive_rightTrackPadClicked.action.ReadValue<Vector2>().y < 0);

        isPadAddPressed_L = (vive_leftTrackPadClicked.action.ReadValue<Vector2>().y > 0 ? true : false);
        isPadReducePressed_L = (vive_leftTrackPadClicked.action.ReadValue<Vector2>().y < 0 ? true : false);


        // Press trigger to switch different control mode.
        if (vive_rightTriggerPressed.action.triggered)
        {
            if (rtXRControlMode < 1)
            {
                rtXRControlMode++;
            }
        }
        if (vive_leftTriggerPressed.action.triggered)
        {
            if (rtXRControlMode > 0)
            {
                rtXRControlMode--;
            }
        }

        switch (rtXRControlMode) 
        {
            // control mode == 0, use pad to move
            case 0:
                snapTurnProvider.enabled = true;
                moveProvider.enabled = true;
                break;

            // control mode == 1, use pad to adjust material
            case 1:
                snapTurnProvider.enabled = false;
                moveProvider.enabled = false;

                if (isPadAddPressed_L)
                    ChangeFoxMetallic();

                if (isPadReducePressed_L)
                    ChangeFoxRoughness();

                if (isPadAddPressed_R)
                    ChangePlaneMetallic();

                if (isPadReducePressed_R)
                    ChangePlaneRoughness();
                break;
        }


        // Switch animation
        if (vive_leftGripPressed.action.triggered)
            SceneManager.isPlayAnimation = SceneManager.isPlayAnimation ? false : true;

        // Enable denoise
        //if ((vive_rightGripPressed.action.triggered || Input.GetKeyDown(KeyCode.F8))
        //    && rayTracingRenderPipelineAsset.RenderMode == RayTracingRenderPipelineAsset.RTRenderSetting.RTRenderMode.MIS)
        //{
        //    RayTracingRenderPipelineAsset.EnableDenoise = RayTracingRenderPipelineAsset.EnableDenoise ? false : true;
        //    plane_metallic = 0.8f;
        //    planeMat.SetFloat("_Metallic", plane_metallic);
        //}

        // Switch direct / indirect
        if (vive_leftMenuPressed.action.triggered)
        {
            if (rayTracingRenderPipelineAsset.RtRenderSetting.EnableIndirect == true)
                rayTracingRenderPipelineAsset.EnableIndirect = false;
            else
                rayTracingRenderPipelineAsset.EnableIndirect = true;
        }

        // Swich rendermode
        if (vive_rightMenuPressed.action.triggered)
        {
            if (rayTracingRenderPipelineAsset.RtRenderSetting.renderMode == RayTracingRenderPipelineAsset.RTRenderSetting.RTRenderMode.MIS)
                rayTracingRenderPipelineAsset.RenderMode = RayTracingRenderPipelineAsset.RTRenderSetting.RTRenderMode.VSAT;
            else
                rayTracingRenderPipelineAsset.RenderMode = RayTracingRenderPipelineAsset.RTRenderSetting.RTRenderMode.MIS;
        }
    }

    void ChangeFoxMetallic()
    {
        if (fox_metallic < 1 - perTimeMatChange)
        {
            fox_metallic += perTimeMatChange;
            foxMat.SetFloat("_Metallic", fox_metallic);
        }
        else
        {
            fox_metallic = 0.0f;
            foxMat.SetFloat("_Metallic", fox_metallic);
        }
    }

    void ChangePlaneMetallic()
    {
        if (plane_metallic < 1 - perTimeMatChange)
        {
            plane_metallic += perTimeMatChange;
            planeMat.SetFloat("_Metallic", plane_metallic);
        }
        else
        {
            plane_metallic = 0.0f;
            planeMat.SetFloat("_Metallic", plane_metallic);
        }
    }

    void ChangeFoxRoughness()
    {
        //Debug.Log(fox_roughness);
        if (fox_roughness < 1 - perTimeMatChange)
        {
            fox_roughness += perTimeMatChange;
            foxMat.SetFloat("_Roughness", fox_roughness);
        }
        else
        {
            fox_roughness = 0.0f;
            foxMat.SetFloat("_Roughness", fox_roughness);
        }
    }

    void ChangePlaneRoughness()
    {
        if (plane_roughness < 1 - perTimeMatChange)
        {
            plane_roughness += perTimeMatChange;
            planeMat.SetFloat("_Roughness", plane_roughness);
        }
        else
        {
            plane_roughness = 0.0f;
            planeMat.SetFloat("_Roughness", plane_roughness);
        }
    }
}
