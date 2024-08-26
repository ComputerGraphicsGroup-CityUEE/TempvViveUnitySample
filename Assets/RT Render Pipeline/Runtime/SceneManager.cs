using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
public class SceneManager : MonoBehaviour
{
    const int maxNumSubMeshes = 32;
    
    private bool[] subMeshFlagArray = new bool[maxNumSubMeshes];
    private bool[] subMeshCutoffArray = new bool[maxNumSubMeshes];

    private static SceneManager s_Instance;

    public static SceneManager Instance
    {
        get
        {
            if (s_Instance != null) return s_Instance;

            s_Instance = GameObject.FindObjectOfType<SceneManager>();
            s_Instance?.Init();
            return s_Instance;
        }
    }

    public GameObject[] tempVecObjs;        // the objects that is used to calculate the visbility
    public static bool isPlayAnimation = false;
    public static bool onClickAddFrame = false;
    public static int checkFrame = 0;
    [SerializeField] RayTracingRenderPipelineAsset rayTracingRenderPipelineAsset;

  [System.NonSerialized]
    public bool isDirty = true;

    public void Awake()
    {
        if (Application.isPlaying)
            DontDestroyOnLoad(this);

        isDirty = true;
        RayTracingResources.Instance.IsProgramRunning = true;

        isPlayAnimation = true;
        rayTracingRenderPipelineAsset.RenderMode = RayTracingRenderPipelineAsset.RTRenderSetting.RTRenderMode.VSAT;
        rayTracingRenderPipelineAsset.EnableIndirect = true;
        RayTracingRenderPipelineAsset.enableDenoise = false;
    }

    public void OnDisable()
    {
        isPlayAnimation = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (isPlayAnimation)
            {
                isPlayAnimation = false;
                checkFrame = -1;
            }
            else
                isPlayAnimation = true;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            onClickAddFrame = true;
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    public void FillAccelerationStructure(ref RayTracingAccelerationStructure accelerationStructure, ref GameObject mergeGO)
    {
        Renderer m_renderer = mergeGO.GetComponent<Renderer>();
        if (m_renderer)
        {
            accelerationStructure.AddInstance(m_renderer, subMeshFlagArray, subMeshCutoffArray);
        }
    }

    public void UpdateAccelerationStructure(ref RayTracingAccelerationStructure accelerationStructure, ref GameObject mergeGO)
    {
        Renderer m_renderer = mergeGO.GetComponent<Renderer>();
        if (m_renderer)
        {
            accelerationStructure.UpdateInstanceTransform(m_renderer);
        }
    }

    private void Init()
    {
        for (var i = 0; i < maxNumSubMeshes; ++i)
        {
            subMeshFlagArray[i] = true;
            subMeshCutoffArray[i] = false;
        }
    }
}