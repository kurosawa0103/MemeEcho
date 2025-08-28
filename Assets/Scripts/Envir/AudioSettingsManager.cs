using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider bgmSlider;
    public Slider sfxSlider;

    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Start()
    {
        // ��ʼ������ֵ
        float bgmVolume = PlayerPrefs.GetFloat(BGM_KEY, 0.75f); // Ĭ�� 0.75
        float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 0.75f);

        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float value)
    {
        SetVolume("BGMVolume", value);
        PlayerPrefs.SetFloat(BGM_KEY, value);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume("SFXVolume", value);
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }

    // ͨ�÷������� 0.6 Ϊ 0dB ��׼�㣬���ж�������
    private void SetVolume(string parameterName, float value)
    {
        float minRef = 0.0001f;
        float refPoint = 0.6f;

        // ��ֹ���� 0
        value = Mathf.Clamp(value, minRef, 1f);

        float volumeDb = Mathf.Log10(value / refPoint) * 20f;
        audioMixer.SetFloat(parameterName, volumeDb);
    }

}
