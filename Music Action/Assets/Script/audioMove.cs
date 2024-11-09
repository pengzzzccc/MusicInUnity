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

    void Update()
    {
        if (material == null) return;

        // 获取音频频谱数据
        audioSource.GetSpectrumData(audioSamples, 0, FFTWindow.BlackmanHarris);

        // 计算平均音频幅度，映射为缩放比例
        float averageLevel = 0;
        foreach (float sample in audioSamples)
        {
            averageLevel += sample;
        }
        averageLevel /= sampleDataLength * 0.2f;

        // 设置对象缩放
        objectTransform.localScale = Vector3.one * (1 + averageLevel * scaleMultiplier);
        light.intensity = averageLevel * scaleMultiplier * 1.2f;

        // 设置自发光颜色的强度
        Color emissionColor = Color.white * (averageLevel * scaleMultiplier);  // 这里使用白色，可以更改为其他颜色
        material.SetColor("_EmissionColor", emissionColor);  // 设置自发光颜色
    }
}
