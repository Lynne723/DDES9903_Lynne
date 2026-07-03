using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFadeBlink : MonoBehaviour
{
    [Header("灯光基础设置")]
    [Tooltip("目标灯光，默认取自身Light组件")]
    public Light targetLight;

    [Tooltip("灯光初始最大亮度")]
    public float startIntensity = 8f;

    [Tooltip("倒计时结束后的最低亮度")]
    public float minIntensity = 1f;

    [Header("倒计时时间设置")]
    [Tooltip("总倒计时时长（秒）")]
    public float totalCountdownTime = 10f;

    [Header("闪烁参数")]
    [Tooltip("是否开启闪烁效果")]
    public bool enableBlink = true;

    [Tooltip("闪烁速度，数值越大闪得越快")]
    public float blinkSpeed = 6f;

    [Tooltip("闪烁幅度（0~1），控制单次闪烁明暗差值")]
    [Range(0f, 1f)] public float blinkRange = 0.3f;

    // 私有运行变量
    private float currentTimer;
    private bool countdownFinished = false;

    void Awake()
    {
        // 自动获取自身灯光组件
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        // 初始化灯光亮度
        targetLight.intensity = startIntensity;
        currentTimer = totalCountdownTime;
    }

    void Update()
    {
        if (countdownFinished) return;

        // 倒计时递减
        currentTimer -= Time.deltaTime;

        // 计算整体亮度渐变比例（从初始→最低）
        float progress = 1 - (currentTimer / totalCountdownTime);
        float baseIntensity = Mathf.Lerp(startIntensity, minIntensity, progress);

        // 闪烁偏移计算
        float blinkOffset = 0;
        if (enableBlink)
        {
            // Sin函数实现平滑闪烁 -1~1
            float sinValue = Mathf.Sin(Time.time * blinkSpeed);
            blinkOffset = sinValue * blinkRange * startIntensity;
        }

        // 最终灯光亮度（基础渐变亮度 + 闪烁波动）
        float finalIntensity = baseIntensity + blinkOffset;
        // 防止亮度低于最低值，做下限保护
        finalIntensity = Mathf.Max(finalIntensity, minIntensity);

        targetLight.intensity = finalIntensity;

        // 倒计时结束逻辑
        if (currentTimer <= 0)
        {
            countdownFinished = true;
            targetLight.intensity = minIntensity;
        }
    }

    #region 外部调用方法（可选，脚本外部控制倒计时）
    /// <summary>
    /// 重置倒计时，重新开始闪烁变暗流程
    /// </summary>
    public void RestartCountdown()
    {
        currentTimer = totalCountdownTime;
        countdownFinished = false;
        targetLight.intensity = startIntensity;
    }

    /// <summary>
    /// 立刻终止效果，直接设为最低亮度
    /// </summary>
    public void StopEffectImmediately()
    {
        countdownFinished = true;
        targetLight.intensity = minIntensity;
    }
    #endregion
}