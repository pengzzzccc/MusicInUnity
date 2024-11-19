using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioMove : MonoBehaviour
{
    public AudioSource audioSource;
    public Light light;
    public Material material;
    public float scaleMultiplier = 10.0f;
    public int sampleDataLength = 1024;

    private float[] audioSamples;
    private Transform objectTransform;
    private enum AudioStep{low, mid, hight}
    [SerializeField] private AudioStep step;

    void Start()
    {
        audioSamples = new float[sampleDataLength];
        objectTransform = transform;
        
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
            material.EnableKeyword("_EMISSION");  // 启用材质的自发光属性
        }
    }

    void FixedUpdate()
    {
        if (material == null) return;

        // 获取音频频谱数据
        audioSource.GetSpectrumData(audioSamples, 0, FFTWindow.BlackmanHarris);
        Debug.Log(audioSamples.Length);

        int range = audioSamples.Length/3;

        switch(step)
        {
            case AudioStep.low:
                float lowaverageLevel = 0;
                for (int i = 0;i < range ;i++)
                {
                    lowaverageLevel += audioSamples[i];
                }
                lowaverageLevel /= sampleDataLength * 0.2f;

                // 设置对象缩放
                objectTransform.localScale = Vector3.one * Mathf.Lerp((1 + lowaverageLevel * scaleMultiplier), objectTransform.localScale.magnitude, 0.5f);
                light.intensity = lowaverageLevel * scaleMultiplier * 1.2f;
                light.color = Color.red;

                // 设置自发光颜色的强度
                Color emissionColor = Color.red * (1 + lowaverageLevel * scaleMultiplier);
                material.SetColor("_EmissionColor", emissionColor);
                break;
            case AudioStep.mid:
                float midaverageLevel = 0;
                for (int i = range;i < range * 2 ;i++)
                {
                    midaverageLevel += audioSamples[i];
                }
                midaverageLevel /= sampleDataLength * 0.2f;

                // 设置对象缩放
                objectTransform.localScale = Vector3.one * Mathf.Lerp((1 + midaverageLevel * scaleMultiplier), objectTransform.localScale.magnitude, 0.5f);
                light.intensity = midaverageLevel * scaleMultiplier * 1.2f;
                light.color = Color.green;

                // 设置自发光颜色的强度
                Color midemissionColor = Color.green * (1 + midaverageLevel * scaleMultiplier);
                material.SetColor("_EmissionColor", midemissionColor);
            break;
            case AudioStep.hight:
                float hitaverageLevel = 0;
                for (int i = range * 2;i < audioSamples.Length ;i++)
                {
                    hitaverageLevel += audioSamples[i];
                }
                hitaverageLevel /= sampleDataLength * 0.2f;

                // 设置对象缩放
                objectTransform.localScale = Vector3.one * Mathf.Lerp((1 + hitaverageLevel * scaleMultiplier), objectTransform.localScale.magnitude, 0.5f);
                light.intensity = hitaverageLevel * scaleMultiplier * 1.2f;
                light.color = Color.yellow;

                // 设置自发光颜色的强度
                Color hitemissionColor = Color.yellow * (1 + hitaverageLevel * scaleMultiplier);
                material.SetColor("_EmissionColor", hitemissionColor);
            break;
        }



    }
}
